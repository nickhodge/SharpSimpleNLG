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
     * This class defines a clause (sentence-like phrase). It is essentially a
     * wrapper around the <code>PhraseElement</code> class, with methods for setting
     * common constituents such as Subject. For example, the <code>setVerb</code>
     * method in this class sets the head of the element to be the specified verb
     * 
     * From an API perspective, this class is a simplified version of the
     * SPhraseSpec class in simplenlg V3. It provides an alternative way for
     * creating syntactic structures, compared to directly manipulating a V4
     * <code>PhraseElement</code>.
     * 
     * Methods are provided for setting and getting the following constituents:
     * <UL>
     * <li>FrontModifier (eg, "Yesterday")
     * <LI>Subject (eg, "John")
     * <LI>PreModifier (eg, "reluctantly")
     * <LI>Verb (eg, "gave")
     * <LI>IndirectObject (eg, "Mary")
     * <LI>object (eg, "an apple")
     * <LI>PostModifier (eg, "before school")
     * </UL>
     * Note that verb, indirect object, and object are propagated to the underlying
     * verb phrase
     * 
     * NOTE: The setModifier method will attempt to automatically determine whether
     * a modifier should be expressed as a FrontModifier, PreModifier, or
     * PostModifier
     * 
     * Features (such as negated) must be accessed via the <code>setFeature</code>
     * and <code>getFeature</code> methods (inherited from <code>NLGElement</code>).
     * Features which are often set on SPhraseSpec include
     * <UL>
     * <LI>Form (eg, "John eats an apple" vs "John eating an apple")
     * <LI>InterrogativeType (eg, "John eats an apple" vs "Is John eating an apple"
     * vs "What is John eating")
     * <LI>Modal (eg, "John eats an apple" vs "John can eat an apple")
     * <LI>Negated (eg, "John eats an apple" vs "John does not eat an apple")
     * <LI>Passive (eg, "John eats an apple" vs "An apple is eaten by John")
     * <LI>Perfect (eg, "John ate an apple" vs "John has eaten an apple")
     * <LI>Progressive (eg, "John eats an apple" vs "John is eating an apple")
     * <LI>Tense (eg, "John ate" vs "John eats" vs "John will eat")
     * </UL>
     * Note that most features are propagated to the underlying verb phrase
     * Premodifers are also propogated to the underlying VP
     * 
     * <code>SPhraseSpec</code> are produced by the <code>createClause</code> method
     * of a <code>PhraseFactory</code>
     * </p>
     * 
     * @author E. Reiter, University of Aberdeen.
     * @version 4.1
     * 
     */

    public class SPhraseSpec : PhraseElement
    {

        // the following features are copied to the VPPhraseSpec
        static List<string> vpFeatures = new List<string>
        {
            Feature.MODAL.ToString(),
            Feature.TENSE.ToString(),
            Feature.NEGATED.ToString(),
            Feature.NUMBER.ToString(),
            Feature.PASSIVE.ToString(),
            Feature.PERFECT.ToString(),
            Feature.PARTICLE.ToString(),
            Feature.PERSON.ToString(),
            Feature.PROGRESSIVE.ToString(),
            InternalFeature.REALISE_AUXILIARY.ToString(),
            Feature.FORM.ToString(),
            Feature.INTERROGATIVE_TYPE.ToString()
        };

        /**
         * create an empty clause
         */

        public SPhraseSpec(NLGFactory phraseFactory) : base (new PhraseCategory_CLAUSE())
        {
            base.setFactory(phraseFactory);
            // create VP
            setVerbPhrase(phraseFactory.createVerbPhrase());

            // set default values
            setFeature(Feature.ELIDED.ToString(), false);
            setFeature(InternalFeature.CLAUSE_STATUS.ToString(), ClauseStatus.MATRIX);
            setFeature(Feature.SUPRESSED_COMPLEMENTISER.ToString(), false);
            setFeature(LexicalFeature.EXPLETIVE_SUBJECT, false);
            setFeature(Feature.COMPLEMENTISER.ToString(), phraseFactory.createWord(
                "that", new LexicalCategory_COMPLEMENTISER())); 

        }

        // intercept and override setFeature, to set VP features as needed

        /**
         * adds a feature, possibly to the underlying VP as well as the SPhraseSpec
         * itself
         * 
         * @see simplenlg.framework.NLGElement#setFeature(java.lang.string,
         * java.lang.object)
         */

        public new void setFeature(string featureName, object featureValue)
        {
            base.setFeature(featureName, featureValue);

            if (vpFeatures.Contains(featureName))
            {
                var verbPhrase = (INLGElement) getFeatureAsElement(InternalFeature.VERB_PHRASE.ToString());
                if (verbPhrase != null || verbPhrase is VPPhraseSpec)
                    ((VPPhraseSpec)verbPhrase).setFeature(featureName, featureValue);
            }
        }

        /*
         * adds a premodifier, if possible to the underlying VP
         * 
         * @see
         * simplenlg.framework.PhraseElement#addPreModifier(simplenlg.framework.
         * NLGElement)
         */

        public override void addPreModifier(INLGElement newPreModifier)
        {
            var verbPhrase = (INLGElement) getFeatureAsElement(InternalFeature.VERB_PHRASE.ToString());

            if (verbPhrase != null)
            {

                if (verbPhrase is PhraseElement)
                {
                    ((PhraseElement) verbPhrase).addPreModifier(newPreModifier);
                }
                else if (verbPhrase is CoordinatedPhraseElement)
                {
                    ((CoordinatedPhraseElement) verbPhrase)
                        .addPreModifier(newPreModifier);
                }
                else
                {
                    base.addPreModifier(newPreModifier);
                }
            }
        }

        /*
         * adds a complement, if possible to the underlying VP
         * 
         * @seesimplenlg.framework.PhraseElement#addComplement(simplenlg.framework.
         * NLGElement)
         */

        public override void addComplement(INLGElement complement)
        {
            var verbPhrase = (PhraseElement) getFeatureAsElement(InternalFeature.VERB_PHRASE.ToString());
            if (verbPhrase != null || verbPhrase is VPPhraseSpec)
                ((VPPhraseSpec)verbPhrase).addComplement(complement);
            else
                base.addComplement(complement);
        }

        /*
         * adds a complement, if possible to the underlying VP
         * 
         * @see simplenlg.framework.PhraseElement#addComplement(java.lang.string)
         */

        public override void addComplement(string newComplement)
        {
            var verbPhrase = (PhraseElement) getFeatureAsElement(InternalFeature.VERB_PHRASE.ToString());
            if (verbPhrase != null || verbPhrase is VPPhraseSpec)
                ((VPPhraseSpec)verbPhrase).addComplement(newComplement);
            else
                base.addComplement(newComplement);
        }

        /*
         * (non-Javadoc)
         * 
         * @see simplenlg.framework.NLGElement#setFeature(java.lang.string, bool)
         */

        public override void setFeature(string featureName, bool featureValue)
        {
            base.setFeature(featureName, featureValue);
            if (vpFeatures.Contains(featureName))
            {
                //PhraseElement verbPhrase = (PhraseElement) getFeatureAsElement(InternalFeature.VERB_PHRASE);
                //AG: bug fix: VP could be coordinate phrase, so cast to NLGElement not PhraseElement
                var verbPhrase = (INLGElement) getFeatureAsElement(InternalFeature.VERB_PHRASE.ToString());
                if (verbPhrase != null || verbPhrase is VPPhraseSpec)
                    ((VPPhraseSpec)verbPhrase).setFeature(featureName, featureValue);
            }
        }

        /* (non-Javadoc)
         * @see simplenlg.framework.NLGElement#getFeature(java.lang.string)
         */

        public override object getFeature(string featureName)
        {
            if (base.getFeature(featureName) != null)
                return base.getFeature(featureName);
            if (vpFeatures.Contains(featureName))
            {
                var verbPhrase = (INLGElement) getFeatureAsElement(InternalFeature.VERB_PHRASE.ToString());
                if (verbPhrase != null || verbPhrase is VPPhraseSpec)
                {
                    if (verbPhrase is VPPhraseSpec)
                    {
                        return ((VPPhraseSpec) verbPhrase).getFeature(featureName);
                    }
                    return verbPhrase.getFeature(featureName);
                }
            }
            return null;
        }

        /**
         * @return VP for this clause
         */

        public INLGElement getVerbPhrase()
        {
            return getFeatureAsElement(InternalFeature.VERB_PHRASE.ToString());
        }

        public void setVerbPhrase(INLGElement vp)
        {
            setFeature(InternalFeature.VERB_PHRASE.ToString(), vp);
            vp.setParent(this); // needed for syntactic processing
        }

        /**
         * Set the verb of a clause
         * 
         * @param verb
         */

        public void setVerb(object verb)
        {
            // get verb phrase element (create if necessary)
            var verbPhraseElement = getVerbPhrase();

            // set head of VP to verb (if this is VPPhraseSpec, and not a coord)
            if (verbPhraseElement != null
                && verbPhraseElement is VPPhraseSpec)
            ((VPPhraseSpec) verbPhraseElement).setVerb(verb);

            /*
             * // WARNING - I don't understand verb phrase, so this may not work!!
             * NLGElement verbElement = getFactory().createWord(verb,
             * LexicalCategory.VERB);
             * 
             * // get verb phrase element (create if necessary) NLGElement
             * verbPhraseElement = getVerbPhrase();
             * 
             * // set head of VP to verb (if this is VPPhraseSpec, and not a coord)
             * if (verbPhraseElement != null && verbPhraseElement is
             * VPPhraseSpec) ((VPPhraseSpec)
             * verbPhraseElement).setHead(verbElement);
             */
        }

        /**
         * Returns the verb of a clause
         * 
         * @return verb of clause
         */

        public INLGElement getVerb()
        {

            // WARNING - I don't understand verb phrase, so this may not work!!
            var verbPhrase = (PhraseElement) getFeatureAsElement(InternalFeature.VERB_PHRASE.ToString());
            if (verbPhrase != null || verbPhrase is VPPhraseSpec)
                return ((VPPhraseSpec)verbPhrase).getHead();
            else
                // return null if VP is coordinated phrase
                return null;
        }

        /**
         * Sets the subject of a clause (assumes this is the only subject)
         * 
         * @param subject
         */

        public void setSubject(object subject)
        {
            INLGElement subjectPhrase;
            if (subject is PhraseElement
                || subject is CoordinatedPhraseElement)
                subjectPhrase = (INLGElement) subject;
            else
                subjectPhrase = getFactory().createNounPhrase(subject);
            var subjects = new List<INLGElement>();
            subjects.add(subjectPhrase);
            setFeature(InternalFeature.SUBJECTS.ToString(), subjects);
        }

        /**
         * Returns the subject of a clause (assumes there is only one)
         * 
         * @return subject of clause (assume only one)
         */

        public INLGElement getSubject()
        {
            List<INLGElement> subjects = getFeatureAsElementList(InternalFeature.SUBJECTS.ToString());
            if (subjects == null || subjects.isEmpty())
                return null;
            return subjects[0];
        }

        /**
         * Sets the direct object of a clause (assumes this is the only direct
         * object)
         * 
         * @param object
         */

        public void setObject(object objectf)
        {

            // get verb phrase element (create if necessary)
            var verbPhraseElement = getVerbPhrase();

            // set object of VP to verb (if this is VPPhraseSpec, and not a coord)
            if (verbPhraseElement != null && verbPhraseElement is VPPhraseSpec)
            ((VPPhraseSpec) verbPhraseElement).setObject(objectf);
        }

        /**
         * Returns the direct object of a clause (assumes there is only one)
         * 
         * @return subject of clause (assume only one)
         */

        public INLGElement getObject()
        {
            var verbPhrase = (PhraseElement) getFeatureAsElement(InternalFeature.VERB_PHRASE.ToString());
            if (verbPhrase != null || verbPhrase is VPPhraseSpec)
                return ((VPPhraseSpec) verbPhrase).getObject();
            else
                // return null if VP is coordinated phrase
                return null;
        }

        /**
         * Set the indirect object of a clause (assumes this is the only direct
         * indirect object)
         * 
         * @param indirectObject
         */

        public void setIndirectObject(object indirectObject)
        {

            // get verb phrase element (create if necessary)
            var verbPhraseElement = getVerbPhrase();

            // set head of VP to verb (if this is VPPhraseSpec, and not a coord)
            if (verbPhraseElement != null && verbPhraseElement is VPPhraseSpec)
                ((VPPhraseSpec) verbPhraseElement)
                    .setIndirectObject(indirectObject);
        }

        /**
         * Returns the indirect object of a clause (assumes there is only one)
         * 
         * @return subject of clause (assume only one)
         */

        public INLGElement getIndirectObject()
        {
            var verbPhrase = (PhraseElement) getFeatureAsElement(InternalFeature.VERB_PHRASE.ToString());
            if (verbPhrase != null || verbPhrase is VPPhraseSpec)
                return ((VPPhraseSpec) verbPhrase).getIndirectObject();
            else
                // return null if VP is coordinated phrase
                return null;
        }

        // note that addFrontModifier, addPostModifier, addPreModifier are inherited
        // from PhraseElement
        // likewise getFrontModifiers, getPostModifiers, getPreModifiers

        /**
         * Add a modifier to a clause Use heuristics to decide where it goes
         * 
         * @param modifier
         */


        public override void addModifier(object modifier)
        {
            // adverb is frontModifier if sentenceModifier
            // otherwise adverb is preModifier
            // string which is one lexicographic word is looked up in lexicon,
            // above rules apply if adverb
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

            // if no modifier element, must be a complex string
            if (modifierElement == null)
            {
                addPostModifier((string) modifier);
                return;
            }

            // AdvP is premodifer (probably should look at head to see if
            // sentenceModifier)
            if (modifierElement is AdvPhraseSpec)
            {
                addPreModifier(modifierElement);
                return;
            }

            // extract WordElement if modifier is a single word
            WordElement modifierWord = null;
            if (modifierElement != null && modifierElement is WordElement)
                modifierWord = (WordElement) modifierElement;
            else if (modifierElement != null  && modifierElement is InflectedWordElement)
                modifierWord = ((InflectedWordElement) modifierElement)
                    .getBaseWord();

            if (modifierWord != null  && (modifierWord.getCategory().enumType == (int)LexicalCategoryEnum.ADVERB))
            {
                // adverb rules
                if (modifierWord
                    .getFeatureAsBoolean(LexicalFeature.SENTENCE_MODIFIER))
                    addFrontModifier(modifierWord);
                else
                    addPreModifier(modifierWord);
                return;
            }

            // default case
            addPostModifier(modifierElement);
        }

    }
}