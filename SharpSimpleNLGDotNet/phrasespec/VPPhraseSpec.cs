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
    /**
     * <p>
     * This class defines a verb phrase.  It is essentially
     * a wrapper around the <code>PhraseElement</code> class, with methods
     * for setting common constituents such as Objects.
     * For example, the <code>setVerb</code> method in this class sets
     * the head of the element to be the specified verb
     *
     * From an API perspective, this class is a simplified version of the SPhraseSpec
     * class in simplenlg V3.  It provides an alternative way for creating syntactic
     * structures, compared to directly manipulating a V4 <code>PhraseElement</code>.
     * 
     * Methods are provided for setting and getting the following constituents: 
     * <UL>
     * <LI>PreModifier		(eg, "reluctantly")
     * <LI>Verb				(eg, "gave")
     * <LI>IndirectObject	(eg, "Mary")
     * <LI>object	        (eg, "an apple")
     * <LI>PostModifier     (eg, "before school")
     * </UL>
     * 
     * NOTE: If there is a complex verb group, a preModifer set at the VP level appears before
     * the verb, while a preModifier set at the clause level appears before the verb group.  Eg
     *   "Mary unfortunately will eat the apple"  ("unfortunately" is clause preModifier)
     *   "Mary will happily eat the apple"  ("happily" is VP preModifier)
     *   
     * NOTE: The setModifier method will attempt to automatically determine whether
     * a modifier should be expressed as a PreModifier or PostModifier
     * 
     * Features (such as negated) must be accessed via the <code>setFeature</code> and
     * <code>getFeature</code> methods (inherited from <code>NLGElement</code>).
     * Features which are often set on VPPhraseSpec include
     * <UL>
     * <LI>Modal    (eg, "John eats an apple" vs "John can eat an apple")
     * <LI>Negated  (eg, "John eats an apple" vs "John does not eat an apple")
     * <LI>Passive  (eg, "John eats an apple" vs "An apple is eaten by John")
     * <LI>Perfect  (eg, "John ate an apple" vs "John has eaten an apple")
     * <LI>Progressive  (eg, "John eats an apple" vs "John is eating an apple")
     * <LI>Tense    (eg, "John ate" vs "John eats" vs "John will eat")
     * </UL>
     * Note that most VP features can be set on an SPhraseSpec, they will automatically
     * be propogated to the VP
     * 
     * <code>VPPhraseSpec</code> are produced by the <code>createVerbPhrase</code>
     * method of a <code>PhraseFactory</code>
     * </p>
     * 
     * @author E. Reiter, University of Aberdeen.
     * @version 4.1
     * 
     */

    public class VPPhraseSpec : PhraseElement
    {


        /** create an empty clause
         */

        public VPPhraseSpec(NLGFactory phraseFactory) : base(new PhraseCategory_VERB_PHRASE())
        {
            this.setFactory(phraseFactory);
            // set default feature values
            this.setFeature(Feature.PERFECT.ToString(), false);
            this.setFeature(Feature.PROGRESSIVE.ToString(), false);
            this.setFeature(Feature.PASSIVE.ToString(), false);
            this.setFeature(Feature.NEGATED.ToString(), false);
            this.setFeature(Feature.TENSE.ToString(), Tense.PRESENT);
            this.setFeature(Feature.PERSON.ToString(), Person.THIRD);
            this.setPlural(false);
            this.setFeature(Feature.FORM.ToString(), Form.NORMAL);
            this.setFeature(InternalFeature.REALISE_AUXILIARY.ToString(), true);
        }

        /** sets the verb (head) of a verb phrase.
         * Extract particle from verb if necessary
         * @param verb
         */

        public void setVerb(object verb)
        {
            INLGElement verbElement;

            if (verb is string)
            {
                // if string given, check for particle
                var space = ((string) verb).indexOf(' ');

                if (space == -1)
                {
                    // no space, so no particle
                    verbElement = getFactory().createWord(verb, new LexicalCategory_VERB());

                }
                else
                {
                    // space, so break up into verb and particle
                    verbElement = getFactory().createWord(((string) verb)
                        .substring(0, space), new LexicalCategory_VERB());
                    setFeature(Feature.PARTICLE.ToString(), ((string) verb)
                        .substring(space + 1, ((string) verb).length()));
                }
            }
            else
            {
                // object is not a string
                verbElement = getFactory().createNLGElement(verb, new LexicalCategory_VERB());
            }
            setHead(verbElement);
        }


        /**
         * @return verb (head) of verb phrase
         */

        public INLGElement getVerb()
        {
            return getHead();
        }

        /** Sets the direct object of a clause  (assumes this is the only direct object)
         *
         * @param object
         */

        public void setObject(object objectd)
        {
            INLGElement objectPhrase;
            if (objectd is PhraseElement || objectd is CoordinatedPhraseElement)
                objectPhrase = (INLGElement) objectd;
            else
                objectPhrase = getFactory().createNounPhrase(objectd);

            objectPhrase.setFeature(InternalFeature.DISCOURSE_FUNCTION.ToString(), DiscourseFunction.OBJECT);
            setComplement(objectPhrase);
        }


        /** Returns the direct object of a clause (assumes there is only one)
         * 
         * @return subject of clause (assume only one)
         */

        public INLGElement getObject()
        {
            List<INLGElement> complements = getFeatureAsElementList(InternalFeature.COMPLEMENTS.ToString());
            foreach (var complement in complements)
            {
                var c = complement.getFeature(InternalFeature.DISCOURSE_FUNCTION.ToString()).ToString();
                if (c == "OBJECT")
                {
                    return complement;
                }
            }
            return null;
        }

        /** Set the indirect object of a clause (assumes this is the only direct indirect object)
         *
         * @param indirectObject
         */

        public void setIndirectObject(object indirectObject)
        {
            INLGElement indirectObjectPhrase;
            if (indirectObject is PhraseElement || indirectObject is CoordinatedPhraseElement)
                indirectObjectPhrase = (INLGElement) indirectObject;
            else
                indirectObjectPhrase = getFactory().createNounPhrase(indirectObject);

            indirectObjectPhrase.setFeature(InternalFeature.DISCOURSE_FUNCTION.ToString(),
                DiscourseFunction.INDIRECT_OBJECT);
            setComplement(indirectObjectPhrase);
        }

        /** Returns the indirect object of a clause (assumes there is only one)
         * 
         * @return subject of clause (assume only one)
         */

        public INLGElement getIndirectObject()
        {
            List<INLGElement> complements = getFeatureAsElementList(InternalFeature.COMPLEMENTS.ToString());
            foreach (var complement in complements)
            {
                var c = complement.getFeature(InternalFeature.DISCOURSE_FUNCTION.ToString()).ToString();
                if (c == DiscourseFunction.INDIRECT_OBJECT.ToString())
                {
                    return complement;
                }
            }
            return null;
        }

        // note that addFrontModifier, addPostModifier, addPreModifier are inherited from PhraseElement
        // likewise getFrontModifiers, getPostModifiers, getPreModifiers


        /** Add a modifier to a verb phrase
         * Use heuristics to decide where it goes
         * @param modifier
         */

        public override void addModifier(object modifier)
        {
            // adverb is preModifier
            // string which is one lexicographic word is looked up in lexicon,
            // if it is an adverb than it becomes a preModifier
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
                    modifierElement = getFactory().createWord(modifier, new LexicalCategory_ANY());
            }

            // if no modifier element, must be a complex string
            if (modifierElement == null)
            {
                addPostModifier((string) modifier);
                return;
            }

            // extract WordElement if modifier is a single word
            WordElement modifierWord = null;
            if (modifierElement != null && modifierElement is WordElement)
                modifierWord = (WordElement) modifierElement;
            else if (modifierElement != null && modifierElement is InflectedWordElement)
                modifierWord = ((InflectedWordElement) modifierElement).getBaseWord();

            if (modifierWord != null && modifierWord.getCategory().enumType == (int)LexicalCategoryEnum.ADVERB)
            {
                addPreModifier(modifierWord);
                return;
            }

            // default case
            addPostModifier(modifierElement);
        }
    }
}