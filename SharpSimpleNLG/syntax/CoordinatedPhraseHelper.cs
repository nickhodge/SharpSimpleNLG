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
using SimpleNLG.Extensions;

namespace SimpleNLG
{
    public abstract class CoordinatedPhraseHelper
    {

        /**
         * The main method for realising coordinated phrases.
         * 
         * @param parent
         *            the <code>SyntaxProcessor</code> that called this method.
         * @param phrase
         *            the <code>CoordinatedPhrase</code> to be realised.
         * @return the realised <code>NLGElement</code>.
         */

        public static INLGElement realise(SyntaxProcessor parent,
            CoordinatedPhraseElement phrase)
        {
            ListElement realisedElement = null;

            if (phrase != null)
            {
                realisedElement = new ListElement();
                PhraseHelper.realiseList(parent, realisedElement, phrase
                    .getPreModifiers(), DiscourseFunction.PRE_MODIFIER);

                var coordinated = new CoordinatedPhraseElement();

                List<INLGElement> children = phrase.getChildren();
                var conjunction = phrase.getFeatureAsString(Feature.CONJUNCTION.ToString());
                coordinated.setFeature(Feature.CONJUNCTION.ToString(), conjunction);
                coordinated.setFeature(Feature.CONJUNCTION_TYPE.ToString(), phrase
                    .getFeature(Feature.CONJUNCTION_TYPE.ToString()));

                InflectedWordElement conjunctionElement = null;

                if (children != null && children.size() > 0)
                {

                    if (phrase.getFeatureAsBoolean(Feature.RAISE_SPECIFIER.ToString())
                        )
                    {
                        raiseSpecifier(children);
                    }

                    var child = phrase.getLastCoordinate();
                    if (child is SPhraseSpec)
                    {
                        ((SPhraseSpec) child).setFeature(Feature.POSSESSIVE.ToString(), phrase
                            .getFeature(Feature.POSSESSIVE.ToString()));
                    }
                    else
                    {
                        child.setFeature(Feature.POSSESSIVE.ToString(), phrase
                            .getFeature(Feature.POSSESSIVE.ToString()));
                    }

                    child = children.get(0);

                    setChildFeatures(phrase, child);

                    coordinated.addCoordinate(parent.realise(child));
                    for (var index = 1; index < children.size(); index++)
                    {
                        child = children.get(index);
                        setChildFeatures(phrase, child);
                        if (child is SPhraseSpec)
                        {
                            if (phrase.getFeatureAsBoolean(Feature.AGGREGATE_AUXILIARY.ToString())
                            )
                            {
                                ((SPhraseSpec) child).setFeature(InternalFeature.REALISE_AUXILIARY.ToString(),
                                    false);
                            }
                        } else
                        {
                            if (phrase.getFeatureAsBoolean(Feature.AGGREGATE_AUXILIARY.ToString())
)
                            {
                                child.setFeature(InternalFeature.REALISE_AUXILIARY.ToString(),
                                    false);
                            }
                        }

                        if (child.isA(PhraseCategoryEnum.CLAUSE))
                        {
                            ((SPhraseSpec)child)
                                .setFeature(
                                    Feature.SUPRESSED_COMPLEMENTISER.ToString(),
                                    phrase
                                        .getFeature(Feature.SUPRESSED_COMPLEMENTISER.ToString()));
                        }

                        //skip conjunction if it's null or empty string
                        if (conjunction != null && conjunction.length() > 0)
                        {
                            conjunctionElement = new InflectedWordElement(
                                conjunction, new LexicalCategory_CONJUNCTION());
                            conjunctionElement.setFeature(
                                InternalFeature.DISCOURSE_FUNCTION.ToString(),
                                DiscourseFunction.CONJUNCTION);
                            coordinated.addCoordinate(conjunctionElement);
                        }

                        coordinated.addCoordinate(parent.realise(child));
                    }
                    realisedElement.addComponent(coordinated);
                }

                PhraseHelper.realiseList(parent, realisedElement, phrase
                    .getPostModifiers(), DiscourseFunction.POST_MODIFIER);
                PhraseHelper.realiseList(parent, realisedElement, phrase
                    .getComplements(), DiscourseFunction.COMPLEMENT);
            }
            return realisedElement;
        }

        /**
         * Sets the common features from the phrase to the child element.
         * 
         * @param phrase
         *            the <code>CoordinatedPhraseElement</code>
         * @param child
         *            a single coordinated <code>NLGElement</code> within the
         *            coordination.
         */

        private static void setChildFeatures(CoordinatedPhraseElement phrase,
            INLGElement child)
        {
            SPhraseSpec twinchild;

            if (child is SPhraseSpec)
            {
                if (phrase.hasFeature(Feature.PROGRESSIVE.ToString()))
                {
                    ((SPhraseSpec)child).setFeature(Feature.PROGRESSIVE.ToString(), phrase
                        .getFeature(Feature.PROGRESSIVE.ToString()));
                }
                if (phrase.hasFeature(Feature.PERFECT.ToString()))
                {
                    ((SPhraseSpec)child).setFeature(Feature.PERFECT.ToString(), phrase
                        .getFeature(Feature.PERFECT.ToString()));
                }
                if (phrase.hasFeature(InternalFeature.SPECIFIER.ToString()))
                {
                    ((SPhraseSpec)child).setFeature(InternalFeature.SPECIFIER.ToString(), phrase
                        .getFeature(InternalFeature.SPECIFIER.ToString()));
                }
                if (phrase.hasFeature(LexicalFeature.GENDER))
                {
                    ((SPhraseSpec)child).setFeature(LexicalFeature.GENDER, phrase
                        .getFeature(LexicalFeature.GENDER));
                }
                if (phrase.hasFeature(Feature.NUMBER.ToString()))
                {
                    ((SPhraseSpec)child).setFeature(Feature.NUMBER.ToString(), phrase.getFeature(Feature.NUMBER.ToString()));
                }
                if (phrase.hasFeature(Feature.TENSE.ToString()))
                {
                    ((SPhraseSpec)child).setFeature(Feature.TENSE.ToString(), phrase.getFeatureTense(Feature.TENSE.ToString()));
                }
                if (phrase.hasFeature(Feature.PERSON.ToString()))
                {
                    ((SPhraseSpec)child).setFeature(Feature.PERSON.ToString(), phrase.getFeature(Feature.PERSON.ToString()));
                }
                if (phrase.hasFeature(Feature.NEGATED.ToString()))
                {
                    ((SPhraseSpec)child).setFeature(Feature.NEGATED.ToString(), phrase.getFeature(Feature.NEGATED.ToString()));
                }
                if (phrase.hasFeature(Feature.MODAL.ToString()))
                {
                    ((SPhraseSpec)child).setFeature(Feature.MODAL.ToString(), phrase.getFeature(Feature.MODAL.ToString()));
                }
                if (phrase.hasFeature(InternalFeature.DISCOURSE_FUNCTION.ToString()))
                {
                    ((SPhraseSpec)child).setFeature(InternalFeature.DISCOURSE_FUNCTION.ToString(), phrase
                        .getFeature(InternalFeature.DISCOURSE_FUNCTION.ToString()));
                }
                if (phrase.hasFeature(Feature.FORM.ToString()))
                {
                    ((SPhraseSpec)child).setFeature(Feature.FORM.ToString(), phrase.getFeature(Feature.FORM.ToString()));
                }
                if (phrase.hasFeature(InternalFeature.CLAUSE_STATUS.ToString()))
                {
                    ((SPhraseSpec)child).setFeature(InternalFeature.CLAUSE_STATUS.ToString(), phrase
                        .getFeature(InternalFeature.CLAUSE_STATUS.ToString()));
                }
                if (phrase.hasFeature(Feature.INTERROGATIVE_TYPE.ToString()))
                {
                    ((SPhraseSpec)child).setFeature(InternalFeature.IGNORE_MODAL.ToString(), true);
                }
            }
            else
            {
                if (phrase.hasFeature(Feature.PROGRESSIVE.ToString()))
                {
                    child.setFeature(Feature.PROGRESSIVE.ToString(), phrase
                        .getFeature(Feature.PROGRESSIVE.ToString()));
                }
                if (phrase.hasFeature(Feature.PERFECT.ToString()))
                {
                    child.setFeature(Feature.PERFECT.ToString(), phrase
                        .getFeature(Feature.PERFECT.ToString()));
                }
                if (phrase.hasFeature(InternalFeature.SPECIFIER.ToString()))
                {
                    child.setFeature(InternalFeature.SPECIFIER.ToString(), phrase
                        .getFeature(InternalFeature.SPECIFIER.ToString()));
                }
                if (phrase.hasFeature(LexicalFeature.GENDER))
                {
                    child.setFeature(LexicalFeature.GENDER, phrase
                        .getFeature(LexicalFeature.GENDER));
                }
                if (phrase.hasFeature(Feature.NUMBER.ToString()))
                {
                    child.setFeature(Feature.NUMBER.ToString(), phrase.getFeature(Feature.NUMBER.ToString()));
                }
                if (phrase.hasFeature(Feature.TENSE.ToString()))
                {
                    child.setFeature(Feature.TENSE.ToString(), phrase.getFeatureTense(Feature.TENSE.ToString()));
                }
                if (phrase.hasFeature(Feature.PERSON.ToString()))
                {
                    child.setFeature(Feature.PERSON.ToString(), phrase.getFeature(Feature.PERSON.ToString()));
                }
                if (phrase.hasFeature(Feature.NEGATED.ToString()))
                {
                    child.setFeature(Feature.NEGATED.ToString(), phrase.getFeature(Feature.NEGATED.ToString()));
                }
                if (phrase.hasFeature(Feature.MODAL.ToString()))
                {
                    child.setFeature(Feature.MODAL.ToString(), phrase.getFeature(Feature.MODAL.ToString()));
                }
                if (phrase.hasFeature(InternalFeature.DISCOURSE_FUNCTION.ToString()))
                {
                    child.setFeature(InternalFeature.DISCOURSE_FUNCTION.ToString(), phrase
                        .getFeature(InternalFeature.DISCOURSE_FUNCTION.ToString()));
                }
                if (phrase.hasFeature(Feature.FORM.ToString()))
                {
                    child.setFeature(Feature.FORM.ToString(), phrase.getFeature(Feature.FORM.ToString()));
                }
                if (phrase.hasFeature(InternalFeature.CLAUSE_STATUS.ToString()))
                {
                    child.setFeature(InternalFeature.CLAUSE_STATUS.ToString(), phrase
                        .getFeature(InternalFeature.CLAUSE_STATUS.ToString()));
                }
                if (phrase.hasFeature(Feature.INTERROGATIVE_TYPE.ToString()))
                {
                    child.setFeature(InternalFeature.IGNORE_MODAL.ToString(), true);
                }
            }
        }

        /**
         * Checks to see if the specifier can be raised and then raises it. In order
         * to be raised the specifier must be the same on all coordinates. For
         * example, <em>the cat and the dog</em> will be realised as
         * <em>the cat and dog</em> while <em>the cat and any dog</em> will remain
         * <em>the cat and any dog</em>.
         * 
         * @param children
         *            the <code>List</code> of coordinates in the
         *            <code>CoordinatedPhraseElement</code>
         */

        private static void raiseSpecifier(List<INLGElement> children)
        {
            var allMatch = true;
            var child = children.get(0);
            INLGElement specifier = null;
            string test = null;

            if (child != null)
            {
                specifier = child.getFeatureAsElement(InternalFeature.SPECIFIER.ToString());

                if (specifier != null)
                {
                    // AG: this assumes the specifier is an InflectedWordElement or
                    // phrase.
                    // it could be a Wordelement, in which case, we want the
                    // baseform
                    test = (specifier is WordElement)
                        ? ((WordElement) specifier)
                            .getBaseForm()
                        : specifier
                            .getFeatureAsString(LexicalFeature.BASE_FORM);
                }

                if (test != null)
                {
                    var index = 1;

                    while (index < children.size() && allMatch)
                    {
                        child = children.get(index);

                        if (child == null)
                        {
                            allMatch = false;

                        }
                        else
                        {
                            specifier = child
                                .getFeatureAsElement(InternalFeature.SPECIFIER.ToString());
                            var childForm = (specifier is WordElement)
                                ? ((WordElement) specifier)
                                    .getBaseForm()
                                : specifier
                                    .getFeatureAsString(LexicalFeature.BASE_FORM);

                            if (!test.Equals(childForm))
                            {
                                allMatch = false;
                            }
                        }
                        index++;
                    }
                    if (allMatch)
                    {
                        for (var eachChild = 1; eachChild < children.size(); eachChild++)
                        {
                            child = children.get(eachChild);
                            if (child is SPhraseSpec)
                                ((SPhraseSpec)child).setFeature(InternalFeature.RAISED.ToString(), true);
                            else
                                child.setFeature(InternalFeature.RAISED.ToString(), true);
                        }
                    }
                }
            }
        }
    }
}
