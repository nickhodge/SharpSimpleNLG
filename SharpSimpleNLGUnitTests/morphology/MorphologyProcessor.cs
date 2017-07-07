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
using SharpNLG.Extensions;
using SimpleNLG.Extensions;

namespace SimpleNLG
{
    /**
     * <p>
     * This is the processor for handling morphology within the SimpleNLG. The
     * processor inflects words form the base form depending on the features applied
     * to the word. For example, <em>kiss</em> is inflected to <em>kissed</em> for
     * past tense, <em>dog</em> is inflected to <em>dogs</em> for pluralisation.
     * </p>
     *
     * <p>
     * As a matter of course, the processor will first use any user-defined
     * inflection for the world. If no inflection is provided then the lexicon, if
     * it exists, will be examined for the correct inflection. Failing this a set of
     * very basic rules will be examined to inflect the word.
     * </p>
     *
     * <p>
     * All processing modules perform realisation on a tree of
     * <code>NLGElement</code>s. The modules can alter the tree in whichever way
     * they wish. For example, the syntax processor replaces phrase elements with
     * list elements consisting of inflected words while the morphology processor
     * replaces inflected words with string elements.
     * </p>
     *
     * <p>
     * <b>N.B.</b> the use of <em>module</em>, <em>processing module</em> and
     * <em>processor</em> is interchangeable. They all mean an instance of this
     * class.
     * </p>
     *
     *
     * @author D. Westwater, University of Aberdeen.
     * @version 4.0
     */

    public class MorphologyProcessor : NLGModule
    {


        public override void initialise()
        {
            // Do nothing
        }


        public override INLGElement realise(INLGElement element)
        {
            INLGElement realisedElement = null;

            if (element is InflectedWordElement)
            {
                realisedElement = doMorphology((InflectedWordElement) element);

            }
            else if (element is StringElement)
            {
                realisedElement = element;

            }
            else if (element is WordElement)
            {
                // AG: now retrieves the default spelling variant, not the baseform
                // string baseForm = ((WordElement) element).getBaseForm();
                var defaultSpell = ((WordElement) element).getDefaultSpellingVariant();

                if (defaultSpell != null)
                {
                    realisedElement = new StringElement(defaultSpell);
                }

            }
            else if (element is DocumentElement)
            {
                List<INLGElement> children = element.getChildren();
                ((DocumentElement) element).setComponents(realise(children));
                realisedElement = element;

            }
            else if (element is ListElement)
            {
                realisedElement = new ListElement();
                ((ListElement) realisedElement).addComponents(realise(element.getChildren()));

            }
            else if (element is CoordinatedPhraseElement)
            {
                List<INLGElement> children = element.getChildren();
                ((CoordinatedPhraseElement) element).clearCoordinates();

                if (children != null && children.size() > 0)
                {
                    ((CoordinatedPhraseElement) element).addCoordinate(realise(children.get(0)));

                    for (var index = 1; index < children.size(); index++)
                    {
                        ((CoordinatedPhraseElement) element).addCoordinate(realise(children.get(index)));
                    }

                    realisedElement = element;
                }

            }
            else if (element != null)
            {
                realisedElement = element;
            }

            return realisedElement;
        }

        /**
         * This is the main method for performing the morphology. It effectively
         * examines the lexical category of the element and calls the relevant set
         * of rules from <code>MorphologyRules</em>.
         *
         * @param element
         *            the <code>InflectedWordElement</code>
         * @return an <code>NLGElement</code> reflecting the correct inflection for
         *         the word.
         */

        private INLGElement doMorphology(InflectedWordElement element)
        {
            INLGElement realisedElement = null;
            if (element.getFeatureAsBoolean(InternalFeature.NON_MORPH.ToString()))
            {
                realisedElement = new StringElement(element.getBaseForm());
                realisedElement.setFeature(InternalFeature.DISCOURSE_FUNCTION.ToString(),
                    element.getFeature(InternalFeature.DISCOURSE_FUNCTION.ToString()));

            }
            else
            {
                INLGElement baseWord = element.getFeatureAsElement(InternalFeature.BASE_WORD.ToString());

                if (baseWord == null && this.lexicon != null)
                {
                    baseWord = this.lexicon.lookupWord(element.getBaseForm());
                }

                var category = element.getCategory();

                if (category is ILexicalCategory)
                {
                    switch ((LexicalCategoryEnum)category.enumType)
                    {
                        case LexicalCategoryEnum.PRONOUN:
                            realisedElement = MorphologyRules.doPronounMorphology(element);
                            break;

                        case LexicalCategoryEnum.NOUN:
                            realisedElement = MorphologyRules.doNounMorphology(element, (WordElement) baseWord);
                            break;

                        case LexicalCategoryEnum.VERB:
                            realisedElement = MorphologyRules.doVerbMorphology(element, (WordElement) baseWord);
                            break;

                        case LexicalCategoryEnum.ADJECTIVE:
                            realisedElement = MorphologyRules.doAdjectiveMorphology(element, (WordElement) baseWord);
                            break;

                        case LexicalCategoryEnum.ADVERB:
                            realisedElement = MorphologyRules.doAdverbMorphology(element, (WordElement) baseWord);
                            break;

                        default:
                            realisedElement = new StringElement(element.getBaseForm());
                            realisedElement.setFeature(InternalFeature.DISCOURSE_FUNCTION.ToString(),
                                element.getFeature(InternalFeature.DISCOURSE_FUNCTION.ToString()));
                            break;
                    }
                }
            }
            return realisedElement;
        }


        public override List<INLGElement> realise(List<INLGElement> elements)
        {
            var realisedElements = new List<INLGElement>();
            INLGElement currentElement = null;
            INLGElement determiner = null;
            INLGElement prevElement = null;

            if (elements != null)
            {
                foreach (var eachElement in elements)
                {
                    currentElement = realise(eachElement);

                    if (currentElement != null)
                    {
                        //pass the discourse function and appositive features -- important for orth processor
                        currentElement.setFeature(Feature.APPOSITIVE.ToString(),
                            eachElement.getFeature(Feature.APPOSITIVE.ToString()));
                        var function = eachElement.getFeature(InternalFeature.DISCOURSE_FUNCTION.ToString());

                        if (function != null)
                        {
                            currentElement.setFeature(InternalFeature.DISCOURSE_FUNCTION.ToString(), function);
                        }

                        if (prevElement != null && prevElement is StringElement
                            && eachElement is InflectedWordElement
                            && (eachElement.getCategory().enumType == (int)LexicalCategoryEnum.NOUN))
                        {

                            var prevString = prevElement.getRealisation();

                            //realisedElements.get(realisedElements.size() - 1)

                            prevElement.setRealisation(DeterminerAgrHelper.checkEndsWithIndefiniteArticle(prevString,
                                currentElement.getRealisation()));

                        }

                        // realisedElements.add(realise(currentElement));
                        realisedElements.add(currentElement);

                        if (determiner == null && DiscourseFunction.SPECIFIER.Equals(currentElement.getFeature(
                                InternalFeature.DISCOURSE_FUNCTION.ToString())))
                        {
                            determiner = currentElement;
                            determiner.setFeature(Feature.NUMBER.ToString(),
                                eachElement.getFeature(Feature.NUMBER.ToString()));
                            // MorphologyRules.doDeterminerMorphology(determiner,
                            // currentElement.getRealisation());

                        }
                        else if (determiner != null)
                        {

                            if (currentElement is ListElement)
                            {
                                // list elements: ensure det matches first element
                                INLGElement firstChild = ((ListElement) currentElement).getChildren().get(0);

                                if (firstChild != null)
                                {
                                    //AG: need to check if child is a coordinate
                                    if (firstChild is CoordinatedPhraseElement)
                                    {
                                        MorphologyRules.doDeterminerMorphology(determiner,
                                            firstChild.getChildren().get(0).getRealisation());
                                    }
                                    else
                                    {
                                        MorphologyRules.doDeterminerMorphology(determiner, firstChild.getRealisation());
                                    }
                                }

                            }
                            else
                            {
                                // everything else: ensure det matches realisation
                                MorphologyRules.doDeterminerMorphology(determiner, currentElement.getRealisation());
                            }

                            determiner = null;
                        }
                    }
                    prevElement = eachElement;
                }
            }

            return realisedElements;
        }

    }
}