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

using SimpleNLG.Extensions;

namespace SimpleNLG
{
    /**
     * <p>
     * This class defines a noun phrase. It is essentially a wrapper around the
     * <code>PhraseElement</code> class, with methods for setting common
     * constituents such as specifier. For example, the <code>setNoun</code> method
     * in this class sets the head of the element to be the specified noun
     * 
     * From an API perspective, this class is a simplified version of the
     * NPPhraseSpec class in simplenlg V3. It provides an alternative way for
     * creating syntactic structures, compared to directly manipulating a V4
     * <code>PhraseElement</code>.
     * 
     * Methods are provided for setting and getting the following constituents:
     * <UL>
     * <li>Specifier (eg, "the")
     * <LI>PreModifier (eg, "green")
     * <LI>Noun (eg, "apple")
     * <LI>PostModifier (eg, "in the shop")
     * </UL>
     * 
     * NOTE: The setModifier method will attempt to automatically determine whether
     * a modifier should be expressed as a PreModifier, or PostModifier
     * 
     * NOTE: Specifiers are currently pretty basic, this needs more development
     * 
     * Features (such as number) must be accessed via the <code>setFeature</code>
     * and <code>getFeature</code> methods (inherited from <code>NLGElement</code>).
     * Features which are often set on NPPhraseSpec include
     * <UL>
     * <LI>Number (eg, "the apple" vs "the apples")
     * <LI>Possessive (eg, "John" vs "John's")
     * <LI>Pronominal (eg, "the apple" vs "it")
     * </UL>
     * 
     * <code>NPPhraseSpec</code> are produced by the <code>createNounPhrase</code>
     * method of a <code>PhraseFactory</code>
     * </p>
     * @author E. Reiter, University of Aberdeen.
     * @version 4.1
     * 
     */

    public class NPPhraseSpec : PhraseElement
    {

        public NPPhraseSpec(NLGFactory phraseFactory) : base(new PhraseCategory_NOUN_PHRASE())
        {
             setFeature(Feature.ELIDED.ToString(), false);
            this.setFactory(phraseFactory);
        }

        /*
         * (non-Javadoc)
         * 
         * @see simplenlg.framework.PhraseElement#setHead(java.lang.object) This
         * version sets NP default features from the head
         */

        public override void setHead(object newHead)
        {
            base.setHead(newHead);
            setNounPhraseFeatures(getFeatureAsElement(InternalFeature.HEAD.ToString()));
        }

        /**
         * A helper method to set the features required for noun phrases, from the
         * head noun
         * 
         * @param phraseElement
         *            the phrase element.
         * @param nounElement
         *            the element representing the noun.
         */

        private void setNounPhraseFeatures(INLGElement nounElement)
        {
            if (nounElement == null)
                return;

            setFeature(Feature.POSSESSIVE.ToString(), nounElement != null
                ? nounElement
                    .getFeatureAsBoolean(Feature.POSSESSIVE.ToString())
                : false);
            setFeature(InternalFeature.RAISED.ToString(), false);
            setFeature(InternalFeature.ACRONYM.ToString(), false);

            if (nounElement != null && nounElement.hasFeature(Feature.NUMBER.ToString()))
            {

                setFeature(Feature.NUMBER.ToString(), nounElement.getFeature(Feature.NUMBER.ToString()));
            }
            else
            {
                setPlural(false);
            }
            if (nounElement != null && nounElement.hasFeature(Feature.PERSON.ToString()))
            {

                setFeature(Feature.PERSON.ToString(), nounElement.getFeature(Feature.PERSON.ToString()));
            }
            else
            {
                setFeature(Feature.PERSON.ToString(), Person.THIRD.ToString());
            }
            if (nounElement != null
                && nounElement.hasFeature(LexicalFeature.GENDER))
            {

                setFeature(LexicalFeature.GENDER, nounElement
                    .getFeature(LexicalFeature.GENDER));
            }
            else
            {
                setFeature(LexicalFeature.GENDER, Gender.NEUTER);
            }

            if (nounElement != null
                && nounElement.hasFeature(LexicalFeature.EXPLETIVE_SUBJECT))
            {

                setFeature(LexicalFeature.EXPLETIVE_SUBJECT, nounElement
                    .getFeature(LexicalFeature.EXPLETIVE_SUBJECT));
            }

            setFeature(Feature.ADJECTIVE_ORDERING.ToString(), true);
        }

        /**
         * sets the noun (head) of a noun phrase
         * 
         * @param noun
         */

        public void setNoun(object noun)
        {
            var nounElement = getFactory().createNLGElement(noun, new LexicalCategory_NOUN());
            setHead(nounElement);
        }

        /**
         * @return noun (head) of noun phrase
         */

        public INLGElement getNoun()
        {
            return getHead();
        }


        /**
         * setDeterminer - Convenience method for when a person tries to set 
         *                 a determiner (e.g. "the") to a NPPhraseSpec.
         */

        public override void setDeterminer(object determiner)
        {
            setSpecifier(determiner);
        }

        /**
         * getDeterminer - Convenience method for when a person tries to get a
         *                 determiner (e.g. "the") from a NPPhraseSpec.
         */

        public INLGElement getDeterminer()
        {
            return getSpecifier();
        }

        /**
         * sets the specifier of a noun phrase. Can be determiner (eg "the"),
         * possessive (eg, "John's")
         * 
         * @param specifier
         */

        public void setSpecifier(object specifier)
        {
            if (specifier is INLGElement)
            {
                setFeature(InternalFeature.SPECIFIER.ToString(), specifier);
                ((INLGElement) specifier).setFeature(
                    InternalFeature.DISCOURSE_FUNCTION.ToString(),
                    DiscourseFunction.SPECIFIER.ToString());
            }
            else
            {
                // create specifier as word (assume determiner)
                var specifierElement = getFactory().createWord(specifier,
                    new LexicalCategory_DETERMINER());

                // set specifier feature
                if (specifierElement != null)
                {
                    setFeature(InternalFeature.SPECIFIER.ToString(), specifierElement);
                    specifierElement.setFeature(InternalFeature.DISCOURSE_FUNCTION.ToString(),
                        DiscourseFunction.SPECIFIER.ToString());
                }
            }
        }

        /**
         * @return specifier (eg, determiner) of noun phrase
         */

        public INLGElement getSpecifier()
        {
            return getFeatureAsElement(InternalFeature.SPECIFIER.ToString());
        }

        /**
         * Add a modifier to an NP Use heuristics to decide where it goes
         * 
         * @param modifier
         */

        public override void addModifier(object modifier)
        {
            // string which is one lexicographic word is looked up in lexicon,
            // adjective is preModifier
            // Everything else is postModifier
            if (modifier == null)
                return;

            // get modifier as NLGElement if possible
            INLGElement modifierElement = null;
            if (modifier is INLGElement)
                modifierElement = (INLGElement) modifier;
            else if (modifier is string)
            {
                var modifierString = (string) modifier;
                if (modifierString.length() > 0 && !modifierString.contains(" "))
                    modifierElement = getFactory().createWord(modifier,
                        new LexicalCategory_ANY());
            }

            // if no modifier element, must be a complex string, add as postModifier
            if (modifierElement == null)
            {
                addPostModifier((string) modifier);
                return;
            }

            // AdjP is premodifer
            if (modifierElement is AdjPhraseSpec)
            {
                addPreModifier(modifierElement);
                return;
            }

            // else extract WordElement if modifier is a single word
            WordElement modifierWord = null;
            if (modifierElement != null && modifierElement is WordElement)
                modifierWord = (WordElement) modifierElement;
            else if (modifierElement != null
                     && modifierElement is InflectedWordElement)
                modifierWord = ((InflectedWordElement) modifierElement)
                    .getBaseWord();

            // check if modifier is an adjective
            if (modifierWord != null
                && modifierWord.getCategory().enumType == (int)LexicalCategoryEnum.ADJECTIVE)
            {
                addPreModifier(modifierWord);
                return;
            }

            // default case
            addPostModifier(modifierElement);
        }
    }
}