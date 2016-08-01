/*
 * The contents of this file are subject to the Mozilla Public License
 * Version 1.1 (the "License"); you may not use this file except in
 * compliance with the License. You may obtain a copy of the License at
 * http://www.mozilla.org/MPL/
 *
 * Software distributed under the License is distributed on an "AS IS"
 * basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. See the
 * License for the specific language governing rights and limitations
 * under the License.
 *
 * The Original Code is "SharpSimpleNLG".
 *
 * The Initial Developer of the Original Code is Ehud Reiter, Albert Gatt and Dave Westwater.
 * Portions created by Ehud Reiter, Albert Gatt and Dave Westwater are Copyright (C) 2010-11 The University of Aberdeen. All Rights Reserved.
 *
 * Contributor(s): Ehud Reiter, Albert Gatt, Dave Wewstwater, Roman Kutlak, Margaret Mitchell, Saad Mahamood, Nick Hodge
 */

/* Additional Notes:
 *    - Original Java source is SimpleNLG from 12-Jun-2016 https://github.com/simplenlg/simplenlg
 *    - This is a port of the Java version to C# with no additional features
 *    - I have left the "Initial Developer" section to reflect this fact
 *    - Any questions, comments, feedback on this port can be sent to Nick Hodge <nhodge@mungr.com>
 */

using System.Collections.Generic;
using System.Text;
using SimpleNLG.Extensions;

namespace SimpleNLG
{
    /**
     * <p>
     * This processing module deals with punctuation when applied to
     * <code>DocumentElement</code>s. The punctuation currently handled by this
     * processor includes the following (as of version 4.0):
     * <ul>
     * <li>Capitalisation of the first letter in sentences.</li>
     * <li>Termination of sentences with a period if not interrogative.</li>
     * <li>Termination of sentences with a question mark if they are interrogative.</li>
     * <li>Replacement of multiple conjunctions with a comma. For example,
     * <em>John and Peter and Simon</em> becomes <em>John, Peter and Simon</em>.</li>
     * </ul>
     * </p>
     * 
     * 
     * @author D. Westwater, University of Aberdeen.
     * @version 4.0
     * 
     */

    public class OrthographyProcessor : NLGModule
    {

        private bool commaSepPremodifiers; // set whether to separate
        // premodifiers using commas

        private bool commaSepCuephrase; // set whether to include a comma after a
        // cue phrase (if marked by the
        // CUE_PHRASE=true) feature.

        public override void initialise()
        {
            this.commaSepPremodifiers = true;
            this.commaSepCuephrase = false;
        }

        /**
         * Check whether this processor separates premodifiers using a comma.
         * 
         * @return <code>true</code> if premodifiers in the noun phrase are
         *         comma-separated.
         */

        public bool isCommaSepPremodifiers()
        {
            return commaSepPremodifiers;
        }

        /**
         * Set whether to separate premodifiers using a comma. If <code>true</code>,
         * premodifiers will be comma-separated, as in <i>the long, dark road</i>.
         * If <code>false</code>, they won't.
         * 
         * @param commaSepPremodifiers
         *            the commaSepPremodifiers to set
         */

        public void setCommaSepPremodifiers(bool commaSepPremodifiers)
        {
            this.commaSepPremodifiers = commaSepPremodifiers;
        }

        /**
         * Check whether this processor separates cue phrases from a matrix phrase
         * using a comma.
         * 
         * @return <code>true</code> if this parameter is set.
         */

        public bool isCommaSepCuephrase()
        {
            return commaSepCuephrase;
        }

        /**
         * If set to <code>true</code>, separates a cue phrase from the matrix
         * phrase using a comma. Cue phrases are typically at the start of a
         * sentence (e.g. <i><u>However</u>, John left early</i>). This will only
         * apply to phrases with the feature
         * {@link simplenlg.features.DiscourseFunction#CUE_PHRASE} or {@link simplenlg.features.DiscourseFunction#FRONT_MODIFIER}.
         * 
         * @param commaSepCuephrase
         *            whether to separate cue phrases using a comma
         */

        public void setCommaSepCuephrase(bool commaSepCuephrase)
        {
            this.commaSepCuephrase = commaSepCuephrase;
        }

        public override INLGElement realise(INLGElement element)
        {
            INLGElement realisedElement = null;
            object function = null; //the element's discourse function

            //get the element's function first
            if (element is ListElement)
            {
                List<INLGElement> children = element.getChildren();
                if (!children.isEmpty())
                {
                    var firstChild = children.get(0);
                    function = firstChild.getFeature(InternalFeature.DISCOURSE_FUNCTION.ToString());
                }
            }
            else
            {
                if (element != null)
                {
                    function = element.getFeature(InternalFeature.DISCOURSE_FUNCTION.ToString());
                }
            }

            if (element != null)
            {
                var category = element.getCategory();

                if (category is IDocumentCategory && element is DocumentElement)
                {
                    var components = ((DocumentElement) element).getComponents();

                    switch ((DocumentCategoryEnum) category.enumType)
                    {

                        case DocumentCategoryEnum.SENTENCE:
                            realisedElement = realiseSentence(components, element);
                            break;

                        case DocumentCategoryEnum.LIST_ITEM:
                            if (components != null && components.size() > 0)
                            {
                                // recursively realise whatever is in the list item
                                // NB: this will realise embedded lists within list
                                // items
                                realisedElement = new ListElement(realise(components));
                                realisedElement.setParent(element.getParent());
                            }
                            break;

                        default:
                            ((DocumentElement) element).setComponents(realise(components));
                            realisedElement = element;
                            break;
                    }

                }
                else if (element is ListElement)
                {
                    // AG: changes here: if we have a premodifier, then we ask the
                    // realiseList method to separate with a comma.
                    // if it's a postmod, we need commas at the start and end only
                    // if it's appositive
                    var buffer = new StringBuilder();

                    if (DiscourseFunction.PRE_MODIFIER.Equals(function))
                    {

                        var all_appositives = true;
                        foreach (NLGElement child in element.getChildren())
                        {
                            all_appositives = all_appositives && child.getFeatureAsBoolean(Feature.APPOSITIVE.ToString());
                        }

                        // TODO: unless this is the end of the sentence
                        if (all_appositives)
                        {
                            buffer.append(", ");
                        }
                        realiseList(buffer, element.getChildren(), this.commaSepPremodifiers ? "," : "");
                        if (all_appositives)
                        {
                            buffer.append(", ");
                        }
                    }
                    else if (DiscourseFunction.POST_MODIFIER.Equals(function))
                    {
// &&
                        // appositive)
                        // {
                        List<INLGElement> postmods = element.getChildren();
                        // bug fix due to Owen Bennett
                        var len = postmods.size();

                        for (var i = 0; i < len; i++)
                        {
                            // for(NLGElement postmod: element.getChildren()) {
                            var postmod = postmods.get(i);

                            // if the postmod is appositive, it's sandwiched in
                            // commas
                            if (postmod.getFeatureAsBoolean(Feature.APPOSITIVE.ToString()))
                            {
                                buffer.append(", ");
                                buffer.append(realise(postmod).ToString());

                                if (i < len - 1)
                                {
                                    buffer.append(", ");
                                }
                            }
                            else
                            {
                                buffer.append(realise(postmod).ToString());
                                if (postmod is ListElement
                                    || (postmod.getRealisation() != null && !postmod.getRealisation().Equals("")))
                                {
                                    buffer.append(" ");
                                }
                            }
                        }

                    }
                    else if ((DiscourseFunction.CUE_PHRASE.Equals(function) ||
                              DiscourseFunction.FRONT_MODIFIER.Equals(function))
                             && this.commaSepCuephrase)
                    {
                        realiseList(buffer, element.getChildren(), this.commaSepCuephrase ? "," : "");

                    }
                    else
                    {
                        realiseList(buffer, element.getChildren(), "");
                    }

                    // realiseList(buffer, element.getChildren(), "");
                    realisedElement = new StringElement(buffer.ToString());

                }
                else if (element is CoordinatedPhraseElement)
                {
                    realisedElement = realiseCoordinatedPhrase(element.getChildren());
                }
                else
                {
                    realisedElement = element;
                }

                // make the realised element inherit the original category
                // essential if list items are to be properly formatted later
                if (realisedElement != null)
                {
                    realisedElement.setCategory(category);
                }

                //check if this is a cue phrase; if param is set, postfix a comma
                if ((DiscourseFunction.CUE_PHRASE.Equals(function) || DiscourseFunction.FRONT_MODIFIER.Equals(function))
                    && this.commaSepCuephrase)
                {
                    var realisation = realisedElement.getRealisation();

                    if (!realisation.endsWith(","))
                    {
                        realisation = realisation + ",";
                    }

                    realisedElement.setRealisation(realisation);
                }
            }

            //remove preceding and trailing whitespace from internal punctuation
            removePunctSpace(realisedElement);
            return realisedElement;
        }

        /**
         * removes extra spaces preceding punctuation from a realised element
         * 
         * @param realisedElement
         */

        private void removePunctSpace(INLGElement realisedElement)
        {

            if (realisedElement != null)
            {

                var realisation = realisedElement.getRealisation();

                if (realisation != null)
                {
                    realisation = realisation.replaceAll(" ,", ",");
                    realisation = realisation.replaceAll(",,+", ",");
                    realisedElement.setRealisation(realisation);
                }

            }
        }

        /**
         * Performs the realisation on a sentence. This includes adding the
         * terminator and capitalising the first letter.
         * 
         * @param components
         *            the <code>List</code> of <code>NLGElement</code>s representing
         *            the components that make up the sentence.
         * @param element
         *            the <code>NLGElement</code> representing the sentence.
         * @return the realised element as an <code>NLGElement</code>.
         */

        private INLGElement realiseSentence(List<INLGElement> components, INLGElement element)
        {

            INLGElement realisedElement = null;
            if (components != null && components.size() > 0)
            {
                var realisation = new StringBuilder();
                realiseList(realisation, components, "");

                stripLeadingCommas(realisation);
                capitaliseFirstLetter(realisation);
                terminateSentence(realisation, element.getFeatureAsBoolean(InternalFeature.INTERROGATIVE.ToString()));

                ((DocumentElement) element).clearComponents();
                // realisation.append(' ');
                element.setRealisation(realisation.ToString());
                realisedElement = element;
            }

            return realisedElement;
        }

        /**
         * Adds the sentence terminator to the sentence. This is a period ('.') for
         * normal sentences or a question mark ('?') for interrogatives.
         * 
         * @param realisation
         *            the <code>StringBuilder<code> containing the current 
         * realisation of the sentence.
         * @param interrogative
         *            a <code>bool</code> flag showing <code>true</code> if the
         *            sentence is an interrogative, <code>false</code> otherwise.
         */

        private void terminateSentence(StringBuilder realisation, bool interrogative)
        {
            var character = realisation.charAt(realisation.length() - 1);
            if (character != '.' && character != '?')
            {
                if (interrogative)
                {
                    realisation.append('?');
                }
                else
                {
                    realisation.append('.');
                }
            }
        }

        /**
         * Remove recursively any leading spaces or commas at the start 
         * of a sentence.
         * 
         * @param realisation
         *            the <code>StringBuilder<code> containing the current 
         * realisation of the sentence.
         */

        private void stripLeadingCommas(StringBuilder realisation)
        {
            var character = realisation.charAt(0);
            if (character == ' ' || character == ',')
            {
                realisation.deleteCharAt(0);
                stripLeadingCommas(realisation);
            }
        }


        /**
         * Capitalises the first character of a sentence if it is a lower case
         * letter.
         * 
         * @param realisation
         *            the <code>StringBuilder<code> containing the current 
         * realisation of the sentence.
         */

        private void capitaliseFirstLetter(StringBuilder realisation)
        {
            var character = realisation.charAt(0);
            if (character >= 'a' && character <= 'z')
            {
                character = (char) ('A' + (character - 'a'));
                realisation[0] = character;
            }
        }

        public override List<INLGElement> realise(List<INLGElement> elements)
        {
            var realisedList = new List<INLGElement>();

            if (elements != null && elements.size() > 0)
            {
                foreach (var eachElement in elements)
                {
                    if (eachElement is DocumentElement)
                    {
                        realisedList.add(realise(eachElement));
                    }
                    else
                    {
                        realisedList.add(eachElement);
                    }
                }
            }
            return realisedList;
        }

        /**
         * Realises a list of elements appending the result to the on-going
         * realisation.
         * 
         * @param realisation
         *            the <code>StringBuilder<code> containing the current 
         * 			  realisation of the sentence.
         * @param components
         *            the <code>List</code> of <code>NLGElement</code>s representing
         *            the components that make up the sentence.
         * @param listSeparator
         *            the string to use to separate elements of the list, empty if
         *            no separator needed
         */

        private void realiseList(StringBuilder realisation, List<INLGElement> components, string listSeparator)
        {

            INLGElement realisedChild = null;

            for (var i = 0; i < components.size(); i++)
            {
                var thisElement = components.get(i);
                realisedChild = realise(thisElement);
                var childRealisation = realisedChild.getRealisation();

                // check that the child realisation is non-empty
                if (childRealisation != null && childRealisation.length() > 0 &&
                    !childRealisation.matches(@"^[\s\n]+$"))
                {
                    realisation.append(realisedChild.getRealisation());

                    if (components.size() > 1 && i < components.size() - 1)
                    {
                        realisation.append(listSeparator);
                    }

                    realisation.append(' ');
                }
            }

            if (realisation.length() > 0)
            {
                realisation.setLength(realisation.length() - 1);
            }
        }

        /**
         * Realises coordinated phrases. Where there are more than two coordinates,
         * then a comma replaces the conjunction word between all the coordinates
         * save the last two. For example, <em>John and Peter and Simon</em> becomes
         * <em>John, Peter and Simon</em>.
         * 
         * @param components
         *            the <code>List</code> of <code>NLGElement</code>s representing
         *            the components that make up the sentence.
         * @return the realised element as an <code>NLGElement</code>.
         */

        private INLGElement realiseCoordinatedPhrase(List<INLGElement> components)
        {
            var realisation = new StringBuilder();
            INLGElement realisedChild;

            var length = components.size();

            for (var index = 0; index < length; index++)
            {
                realisedChild = components.get(index);
                if (index < length - 2
                    &&
                    DiscourseFunction.CONJUNCTION.Equals(realisedChild.getFeature(InternalFeature.DISCOURSE_FUNCTION.ToString())))
                {

                    realisation.append(", "); 
                }
                else
                {
                    realisedChild = realise(realisedChild);
                    realisation.append(realisedChild.getRealisation()).append(" ");
                }
            }
            realisation.setLength(realisation.length() - 1);
            return new StringElement(realisation.ToString().replaceAll(" ,", ","));  
        }
    }
}
