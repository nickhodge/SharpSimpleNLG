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
    public abstract class NounPhraseHelper
    {

        /** The qualitative position for ordering premodifiers. */
        private static int QUALITATIVE_POSITION = 1;

        /** The colour position for ordering premodifiers. */
        private static int COLOUR_POSITION = 2;

        /** The classifying position for ordering premodifiers. */
        private static int CLASSIFYING_POSITION = 3;

        /** The noun position for ordering premodifiers. */
        private static int NOUN_POSITION = 4;

        /**
         * The main method for realising noun phrases.
         * 
         * @param parent
         *            the <code>SyntaxProcessor</code> that called this method.
         * @param phrase
         *            the <code>PhraseElement</code> to be realised.
         * @return the realised <code>NLGElement</code>.
         */

        public static INLGElement realise(SyntaxProcessor parent, PhraseElement phrase)
        {
            ListElement realisedElement = null;

            if (phrase != null
                && !phrase.getFeatureAsBoolean(Feature.ELIDED.ToString()))
            {
                realisedElement = new ListElement();

                if (phrase.getFeatureAsBoolean(Feature.PRONOMINAL.ToString()))
                {
                    realisedElement.addComponent(createPronoun(parent, phrase));

                }
                else
                {
                    realiseSpecifier(phrase, parent, realisedElement);
                    realisePreModifiers(phrase, parent, realisedElement);
                    realiseHeadNoun(phrase, parent, realisedElement);
                    PhraseHelper.realiseList(parent, realisedElement, phrase
                            .getFeatureAsElementList(InternalFeature.COMPLEMENTS.ToString()),
                        DiscourseFunction.COMPLEMENT);

                    PhraseHelper.realiseList(parent, realisedElement, phrase
                        .getPostModifiers(), DiscourseFunction.POST_MODIFIER);
                }
            }

            return realisedElement;
        }

        /**
         * Realises the head noun of the noun phrase.
         * 
         * @param phrase
         *            the <code>PhraseElement</code> representing this noun phrase.
         * @param parent
         *            the parent <code>SyntaxProcessor</code> that will do the
         *            realisation of the complementiser.
         * @param realisedElement
         *            the current realisation of the noun phrase.
         */

        private static void realiseHeadNoun(PhraseElement phrase,
            SyntaxProcessor parent, ListElement realisedElement)
        {
            INLGElement headElement = phrase.getHead();

            if (headElement != null)
            {
                headElement.setFeature(Feature.ELIDED.ToString(), phrase
                    .getFeature(Feature.ELIDED.ToString()));
                headElement.setFeature(LexicalFeature.GENDER, phrase
                    .getFeature(LexicalFeature.GENDER));
                headElement.setFeature(InternalFeature.ACRONYM.ToString(), phrase
                    .getFeature(InternalFeature.ACRONYM.ToString()));
                headElement.setFeature(Feature.NUMBER.ToString(), phrase
                    .getFeature(Feature.NUMBER.ToString()));
                headElement.setFeature(Feature.PERSON.ToString(), phrase
                    .getFeature(Feature.PERSON.ToString()));
                headElement.setFeature(Feature.POSSESSIVE.ToString(), phrase
                    .getFeature(Feature.POSSESSIVE.ToString()));
                headElement.setFeature(Feature.PASSIVE.ToString(), phrase
                    .getFeature(Feature.PASSIVE.ToString()));
                var currentElement = parent.realise(headElement);
                currentElement.setFeature(InternalFeature.DISCOURSE_FUNCTION.ToString(),
                    DiscourseFunction.SUBJECT);
                realisedElement.addComponent(currentElement);
            }
        }

        /**
         * Realises the pre-modifiers of the noun phrase. Before being realised,
         * pre-modifiers undergo some basic sorting based on adjective ordering.
         * 
         * @param phrase
         *            the <code>PhraseElement</code> representing this noun phrase.
         * @param parent
         *            the parent <code>SyntaxProcessor</code> that will do the
         *            realisation of the complementiser.
         * @param realisedElement
         *            the current realisation of the noun phrase.
         */

        private static void realisePreModifiers(PhraseElement phrase,
            SyntaxProcessor parent, ListElement realisedElement)
        {

            var preModifiers = phrase.getPreModifiers();
            if (phrase.getFeatureAsBoolean(Feature.ADJECTIVE_ORDERING.ToString())
                )
            {
                preModifiers = sortNPPreModifiers(preModifiers);
            }
            PhraseHelper.realiseList(parent, realisedElement, preModifiers,
                DiscourseFunction.PRE_MODIFIER);
        }

        /**
         * Realises the specifier of the noun phrase.
         * 
         * @param phrase
         *            the <code>PhraseElement</code> representing this noun phrase.
         * @param parent
         *            the parent <code>SyntaxProcessor</code> that will do the
         *            realisation of the complementiser.
         * @param realisedElement
         *            the current realisation of the noun phrase.
         */

        private static void realiseSpecifier(PhraseElement phrase,
            SyntaxProcessor parent, ListElement realisedElement)
        {
            INLGElement specifierElement = phrase
                .getFeatureAsElement(InternalFeature.SPECIFIER.ToString());

            if (specifierElement != null
                && !phrase.getFeatureAsBoolean(InternalFeature.RAISED.ToString())
                     && !phrase.getFeatureAsBoolean(Feature.ELIDED.ToString()))
            {
                if (!specifierElement.isA(LexicalCategoryEnum.PRONOUN) &&
                    specifierElement.getCategory().enumType != (int) PhraseCategoryEnum.NOUN_PHRASE)
                {
                    specifierElement.setFeature(Feature.NUMBER.ToString(), phrase
                        .getFeature(Feature.NUMBER.ToString()));
                }

                var currentElement = parent.realise(specifierElement);

                if (currentElement != null)
                {
                    currentElement.setFeature(InternalFeature.DISCOURSE_FUNCTION.ToString(),
                        DiscourseFunction.SPECIFIER);
                    realisedElement.addComponent(currentElement);
                }
            }
        }

        /**
         * Sort the list of premodifiers for this noun phrase using adjective
         * ordering (ie, "big" comes before "red")
         * 
         * @param originalModifiers
         *            the original listing of the premodifiers.
         * @return the sorted <code>List</code> of premodifiers.
         */

        private static List<INLGElement> sortNPPreModifiers(
            List<INLGElement> originalModifiers)
        {

            List<INLGElement> orderedModifiers = null;

            if (originalModifiers == null || originalModifiers.size() <= 1)
            {
                orderedModifiers = originalModifiers;
            }
            else
            {
                orderedModifiers = new List<INLGElement>(originalModifiers);
                var changesMade = false;
                do
                {
                    changesMade = false;
                    for (var i = 0; i < orderedModifiers.size() - 1; i++)
                    {
                        if (getMinPos(orderedModifiers.get(i)) > getMaxPos(orderedModifiers
                                .get(i + 1)))
                        {
                            var temp = orderedModifiers.get(i);
                            orderedModifiers.set(i, orderedModifiers.get(i + 1));
                            orderedModifiers.set(i + 1, temp);
                            changesMade = true;
                        }
                    }
                } while (changesMade == true);
            }
            return orderedModifiers;
        }

        /**
         * Determines the minimim position at which this modifier can occur.
         * 
         * @param modifier
         *            the modifier to be checked.
         * @return the minimum position for this modifier.
         */

        private static int getMinPos(INLGElement modifier)
        {
            var position = QUALITATIVE_POSITION;

            if (modifier.isA(LexicalCategoryEnum.NOUN)
                || modifier.isA(PhraseCategoryEnum.NOUN_PHRASE))
            {

                position = NOUN_POSITION;
            }
            else if (modifier.isA(LexicalCategoryEnum.ADJECTIVE)
                     || modifier.isA(PhraseCategoryEnum.ADJECTIVE_PHRASE))
            {
                var adjective = getHeadWordElement(modifier);

                if (adjective.getFeatureAsBoolean(LexicalFeature.QUALITATIVE)
                    )
                {
                    position = QUALITATIVE_POSITION;
                }
                else if (adjective.getFeatureAsBoolean(LexicalFeature.COLOUR)
                    )
                {
                    position = COLOUR_POSITION;
                }
                else if (adjective
                    .getFeatureAsBoolean(LexicalFeature.CLASSIFYING)
                    )
                {
                    position = CLASSIFYING_POSITION;
                }
            }
            return position;
        }

        /**
         * Determines the maximim position at which this modifier can occur.
         * 
         * @param modifier
         *            the modifier to be checked.
         * @return the maximum position for this modifier.
         */

        private static int getMaxPos(INLGElement modifier)
        {
            var position = NOUN_POSITION;

            if (modifier.isA(LexicalCategoryEnum.ADJECTIVE)
                || modifier.isA(PhraseCategoryEnum.ADJECTIVE_PHRASE))
            {
                var adjective = getHeadWordElement(modifier);

                if (adjective.getFeatureAsBoolean(LexicalFeature.CLASSIFYING)
                    )
                {
                    position = CLASSIFYING_POSITION;
                }
                else if (adjective.getFeatureAsBoolean(LexicalFeature.COLOUR)
                    )
                {
                    position = COLOUR_POSITION;
                }
                else if (adjective
                    .getFeatureAsBoolean(LexicalFeature.QUALITATIVE)
                    )
                {
                    position = QUALITATIVE_POSITION;
                }
                else
                {
                    position = CLASSIFYING_POSITION;
                }
            }
            return position;
        }

        /**
         * Retrieves the correct representation of the word from the element. This
         * method will find the <code>WordElement</code>, if it exists, for the
         * given phrase or inflected word.
         * 
         * @param element
         *            the <code>NLGElement</code> from which the head is required.
         * @return the <code>WordElement</code>
         */

        private static WordElement getHeadWordElement(INLGElement element)
        {
            WordElement head = null;

            if (element is WordElement)
                head = (WordElement) element;
            else if (element is InflectedWordElement)
            {
                head = (WordElement) element.getFeature(InternalFeature.BASE_WORD.ToString());
            }
            else if (element is PhraseElement)
            {
                head = getHeadWordElement(((PhraseElement) element).getHead());
            }

            return head;
        }

        /**
         * Creates the appropriate pronoun if the subject of the noun phrase is
         * pronominal.
         * 
         * @param parent
         *            the parent <code>SyntaxProcessor</code> that will do the
         *            realisation of the complementiser.
         * @param phrase
         *            the <code>PhraseElement</code> representing this noun phrase.
         * @return the <code>NLGElement</code> representing the pronominal.
         */

        private static INLGElement createPronoun(SyntaxProcessor parent,
            PhraseElement phrase)
        {

            var pronoun = "it"; 
            var phraseFactory = phrase.getFactory();
            var personValue = phrase.getFeature(Feature.PERSON.ToString());

            if (Person.FIRST.Equals(personValue))
            {
                pronoun = "I"; 
            }
            else if (Person.SECOND.Equals(personValue))
            {
                pronoun = "you"; 
            }
            else
            {
                var genderValue = phrase.getFeature(LexicalFeature.GENDER);
                if (Gender.FEMININE.Equals(genderValue))
                {
                    pronoun = "she"; 
                }
                else if (Gender.MASCULINE.Equals(genderValue))
                {
                    pronoun = "he"; 
                }
            }
            // AG: createWord now returns WordElement; so we embed it in an
            // inflected word element here
            INLGElement element;
            var proElement = phraseFactory.createWord(pronoun,
                new LexicalCategory_PRONOUN());

            if (proElement is WordElement)
            {
                element = new InflectedWordElement((WordElement) proElement);
                element.setFeature(LexicalFeature.GENDER, ((WordElement) proElement).getFeature(LexicalFeature.GENDER));
                // Ehud - also copy over person
                element.setFeature(Feature.PERSON.ToString(), ((WordElement) proElement).getFeature(Feature.PERSON.ToString()));
            }
            else
            {
                element = proElement;
            }

            element.setFeature(InternalFeature.DISCOURSE_FUNCTION.ToString(),
                DiscourseFunction.SPECIFIER);
            element.setFeature(Feature.POSSESSIVE.ToString(), phrase
                .getFeature(Feature.POSSESSIVE.ToString()));
            element
                .setFeature(Feature.NUMBER.ToString(), phrase.getFeature(Feature.NUMBER.ToString()));


            if (phrase.hasFeature(InternalFeature.DISCOURSE_FUNCTION.ToString()))
            {
                element.setFeature(InternalFeature.DISCOURSE_FUNCTION.ToString(), phrase
                    .getFeature(InternalFeature.DISCOURSE_FUNCTION.ToString()));
            }

            return element;
        }
    }
}
