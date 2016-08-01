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


using System;
using System.Collections.Generic;
using SimpleNLG.Extensions;

namespace SimpleNLG
{
    public class NLGFactory
    {

        /***
         * CODING COMMENTS The original version of phraseFactory created a crude
         * realisation of the phrase in the BASE_FORM feature. This was just for
         * debugging purposes (note BASE_FORM on a WordElement is meaningful), I've
         * zapped this as it was makig things too complex
         * 
         * This version of phraseFactory replicates WordElements (instead of reusing
         * them). I think this is because elemente are linked to their parent
         * phrases, via the parent data member. May be good to check if this is
         * actually necessary
         * 
         * The explicit list of pronouns below should be replaced by a reference to
         * the lexicon
         * 
         * Things to sort out at some point...
         * 
         */
        /** The lexicon to be used with this factory. */
        private Lexicon lexicon;

        /** The list of English pronouns. */

        private static List<string> PRONOUNS = new List<string>
            {
                "I",
                "you",
                "he",
                "she",
                "it",
                "me",
                "you",
                "him",
                "her",
                "it",
                "myself",
                "yourself",
                "himself",
                "herself",
                "itself",
                "mine",
                "yours",
                "his",
                "hers",
                "its",
                "we",
                "you",
                "they",
                "they",
                "they",
                "us",
                "you",
                "them",
                "them",
                "them",
                "ourselves",
                "yourselves",
                "themselves",
                "themselves",
                "themselves",
                "ours",
                "yours",
                "theirs",
                "theirs",
                "theirs",
                "there"
            };

        /** The list of first-person English pronouns. */

        private static List<string> FIRST_PRONOUNS = new List<string>
        {
            "I",
            "me",
            "myself",
            "we",
            "us",
            "ourselves",
            "mine",
            "my",
            "ours",
            "our"
        };

        /** The list of second person English pronouns. */

        private static List<string> SECOND_PRONOUNS = new List<string>
            {
                "you",
                "yourself",
                "yourselves",
                "yours",
                "your"
            };


        private static List<string> REFLEXIVE_PRONOUNS = new List<string>
            {
                "myself",
                "yourself",
                "himself",
                "herself",
                "itself",
                "ourselves",
                "yourselves",
                "themselves"
            };

    /** The list of masculine English pronouns. */
        private static List<string> MASCULINE_PRONOUNS = new List<string>
            {"he", "him", "himself", "his"};

        /** The list of feminine English pronouns. */
        private static List<string> FEMININE_PRONOUNS = new List<string>
            {"she", "her", "herself", "hers"};

        /** The list of possessive English pronouns. */
         private static List<string> POSSESSIVE_PRONOUNS = new List<string>
            {"mine",
        "ours",
        "yours",
        "his",
        "hers",
        "its",
        "theirs",
        "my",
        "our",
        "your",
        "her",
        "their"};

        /** The list of plural English pronouns. */
        private static List<string> PLURAL_PRONOUNS = new

        List<string>
        {
            "we",
            "us",
            "ourselves",
            "ours",
            "our",
            "they",
            "them",
            "theirs",
            "their"
        };

        /** The list of English pronouns that can be singular or plural. */
        private static List<string> EITHER_NUMBER_PRONOUNS= new List<string>
            {"there"};

        /** The list of expletive English pronouns. */
        private static List<string> EXPLETIVE_PRONOUNS = new List<string> {"there"};

        /** regex for determining if a string is a single word or not **/
        private static string WORD_REGEX = "\\w*";

        public NLGFactory()
        {
            
        }
 
        /**
         * Creates a new phrase factory with the associated lexicon.
         * 
         * @param newLexicon
         *            the <code>Lexicon</code> to be used with this factory.
         */

        public NLGFactory(Lexicon newLexicon)
        {
            setLexicon(newLexicon);
        }

        /**
         * Sets the lexicon to be used by this factory. Passing a parameter of
         * <code>null</code> will remove any existing lexicon from the factory.
         * 
         * @param newLexicon
         *            the new <code>Lexicon</code> to be used.
         */

        public void setLexicon(Lexicon newLexicon)
        {
            this.lexicon = newLexicon;
        }

        /**
         * Creates a new element representing a word. If the word passed is already
         * an <code>NLGElement</code> then that is returned unchanged. If a
         * <code>string</code> is passed as the word then the factory will look up
         * the <code>Lexicon</code> if one exists and use the details found to
         * create a new <code>WordElement</code>.
         * 
         * @param word
         *            the base word for the new element. This can be a
         *            <code>NLGElement</code>, which is returned unchanged, or a
         *            <code>string</code>, which is used to construct a new
         *            <code>WordElement</code>.
         * @param category
         *            the <code>LexicalCategory</code> for the word.
         * 
         * @return an <code>NLGElement</code> representing the word.
         */

        public WordElement createWord(object word, ILexicalCategory category)
        {
            WordElement wordElement = null;
            if (word is WordElement)
            {
                wordElement = (WordElement)word;
            }
             else if (word is string && this.lexicon != null)
            {
                // AG: change: should create a WordElement, not an
                // InflectedWordElement
                // wordElement = new InflectedWordElement(
                // (string) word, category);
                // if (this.lexicon != null) {
                // doLexiconLookUp(category, (string) word, wordElement);
                // }
                // wordElement = lexicon.getWord((string) word, category);
                wordElement = lexicon.lookupWord((string) word, category);
                if (PRONOUNS.Contains((string)word))
                {
                    setPronounFeatures(wordElement, (string) word);
                }
            }

            return wordElement;
        }

        /**
         * Create an inflected word element. InflectedWordElement represents a word
         * that already specifies the morphological and other features that it
         * should exhibit in a realisation. While normally, phrases are constructed
         * using <code>WordElement</code>s, and features are set on phrases, it is
         * sometimes desirable to set features directly on words (for example, when
         * one wants to elide a specific word, but not its parent phrase).
         * 
         * <P>
         * If the object passed is already a <code>WordElement</code>, then a new
         * 
         * <code>InflectedWordElement<code> is returned which wraps this <code>WordElement</code>
         * . If the object is a <code>string</code>, then the
         * <code>WordElement</code> representing this <code>string</code> is looked
         * up, and a new
         * <code>InflectedWordElement<code> wrapping this is returned. If no such <code>WordElement</code>
         * is found, the element returned is an <code>InflectedWordElement</code>
         * with the supplied string as baseform and no base <code>WordElement</code>
         * . If an <code>NLGElement</code> is passed, this is returned unchanged.
         * 
         * @param word
         *            the word
         * @param category
         *            the category
         * @return an <code>InflectedWordElement</code>, or the original supplied
         *         object if it is an <code>NLGElement</code>.
         */

        public INLGElement createInflectedWord(object word, ILexicalCategory category)
        {
            // first get the word element
            INLGElement inflElement = null;

            if (word is WordElement)
            {
                inflElement = new InflectedWordElement((WordElement) word);

            }
            else
            if (word is string)
            {
                var baseword = createWord((string) word, category);

                if (baseword != null && baseword is WordElement)
                {
                    inflElement = new InflectedWordElement((WordElement) baseword);
                }
                else
                {
                    inflElement = new InflectedWordElement((string) word, category);
                }

            }
            else if (word is INLGElement)
            {
                inflElement = (INLGElement) word;
            }

            return inflElement;

        }

        /**
         * A helper method to set the features on newly created pronoun words.
         * 
         * @param wordElement
         *            the created element representing the pronoun.
         * @param word
         *            the base word for the pronoun.
         */

        private void setPronounFeatures(INLGElement wordElement, string word)
        {
            wordElement.setCategory(new LexicalCategory_PRONOUN());
            if (FIRST_PRONOUNS.Contains(word))
            {
                wordElement.setFeature(Feature.PERSON.ToString(), Person.FIRST);
            }
            else if (SECOND_PRONOUNS.Contains(word))
            {
                wordElement.setFeature(Feature.PERSON.ToString(), Person.SECOND);

                if ("yourself".equalsIgnoreCase(word))
                {
                    
                    wordElement.setPlural(false);
                }
                else if ("yourselves".equalsIgnoreCase(word))
                {
                    
                    wordElement.setPlural(true);
                }
                else
                {
                    wordElement.setFeature(Feature.NUMBER.ToString(), NumberAgreement.BOTH);
                }
            }
            else
            {
                wordElement.setFeature(Feature.PERSON.ToString(), Person.THIRD);
            }
            if (REFLEXIVE_PRONOUNS.Contains(word))
            {
                wordElement.setFeature(LexicalFeature.REFLEXIVE, true);
            }
            else
            {
                wordElement.setFeature(LexicalFeature.REFLEXIVE, false);
            }
            if (MASCULINE_PRONOUNS.Contains(word))
            {
                wordElement.setFeature(LexicalFeature.GENDER, Gender.MASCULINE);
            }
            else if (FEMININE_PRONOUNS.Contains(word))
            {
                wordElement.setFeature(LexicalFeature.GENDER, Gender.FEMININE);
            }
            else
            {
                wordElement.setFeature(LexicalFeature.GENDER, Gender.NEUTER);
            }

            if (POSSESSIVE_PRONOUNS.Contains(word))
            {
                wordElement.setFeature(Feature.POSSESSIVE.ToString(), true);
            }
            else
            {
                wordElement.setFeature(Feature.POSSESSIVE.ToString(), false);
            }

            if (PLURAL_PRONOUNS.Contains(word) && !SECOND_PRONOUNS.Contains(word))
            {
                wordElement.setPlural(true);
            }
            else if (!EITHER_NUMBER_PRONOUNS.Contains(word))
            {
                wordElement.setPlural(false);
            }

            if (EXPLETIVE_PRONOUNS.Contains(word))
            {
                wordElement.setFeature(InternalFeature.NON_MORPH.ToString(), true);
                wordElement.setFeature(LexicalFeature.EXPLETIVE_SUBJECT, true);
            }
        }

        /**
         * A helper method to look up the lexicon for the given word.
         * 
         * @param category
         *            the <code>LexicalCategory</code> of the word.
         * @param word
         *            the base form of the word.
         * @param wordElement
         *            the created element representing the word.
         */

        private void doLexiconLookUp(ILexicalCategory category, string word, INLGElement wordElement)
        {
            WordElement baseWord = null;

            if (category.lexType == LexicalCategoryEnum.NOUN && this.lexicon.hasWord(word, new LexicalCategory_PRONOUN()))
            {
                baseWord = this.lexicon.lookupWord(word, new LexicalCategory_PRONOUN());

                if (baseWord != null)
                {
                    wordElement.setFeature(InternalFeature.BASE_WORD.ToString(), baseWord);
                    wordElement.setCategory(new LexicalCategory_PRONOUN());
                    if (!PRONOUNS.Contains(word))
                    {
                        wordElement.setFeature(InternalFeature.NON_MORPH.ToString(), true);
                    }
                }
            }
            else
            {
                baseWord = this.lexicon.lookupWord(word, category);
                wordElement.setFeature(InternalFeature.BASE_WORD.ToString(), baseWord);
            }
        }

        /**
         * Creates a blank preposition phrase with no preposition or complements.
         * 
         * @return a <code>PPPhraseSpec</code> representing this phrase.
         */

        public PPPhraseSpec createPrepositionPhrase()
        {
            return createPrepositionPhrase(null, null);
        }

        /**
         * Creates a preposition phrase with the given preposition.
         * 
         * @param preposition
         *            the preposition to be used.
         * @return a <code>PPPhraseSpec</code> representing this phrase.
         */

        public PPPhraseSpec createPrepositionPhrase(object preposition)
        {
            return createPrepositionPhrase(preposition, null);
        }

        /**
         * Creates a preposition phrase with the given preposition and complement.
         * An <code>NLGElement</code> representing the preposition is added as the
         * head feature of this phrase while the complement is added as a normal
         * phrase complement.
         * 
         * @param preposition
         *            the preposition to be used.
         * @param complement
         *            the complement of the phrase.
         * @return a <code>PPPhraseSpec</code> representing this phrase.
         */

        public PPPhraseSpec createPrepositionPhrase(object preposition, object complement)
        {

            var phraseElement = new PPPhraseSpec(this);

            var prepositionalElement = createNLGElement(preposition, new LexicalCategory_PREPOSITION());
            setPhraseHead(phraseElement, prepositionalElement);

            if (complement != null)
            {
                setComplement(phraseElement, complement);
            }
            return phraseElement;
        }

        /**
         * A helper method for setting the complement of a phrase.
         * 
         * @param phraseElement
         *            the created element representing this phrase.
         * @param complement
         *            the complement to be added.
         */

        private void setComplement(PhraseElement phraseElement, object complement)
        {
            var complementElement = createNLGElement(complement);
            phraseElement.addComplement(complementElement);
        }

        /**
         * this method creates an NLGElement from an object If object is null,
         * return null If the object is already an NLGElement, it is returned
         * unchanged Exception: if it is an InflectedWordElement, return underlying
         * WordElement If it is a string which matches a lexicon entry or pronoun,
         * the relevant WordElement is returned If it is a different string, a
         * wordElement is created if the string is a single word Otherwise a
         * StringElement is returned Otherwise throw an exception
         * 
         * @param element
         *            - object to look up
         * @param category
         *            - default lexical category of object
         * @return NLGelement
         */

        public INLGElement createNLGElement(object element, ILexicalCategory category)
        {
            if (element == null)
                return null;

            // InflectedWordElement - return underlying word
            else if (element
            is InflectedWordElement)
            return ((InflectedWordElement) element).getBaseWord();

            // StringElement - look up in lexicon if it is a word
            // otherwise return element
            else
            if (element
            is StringElement)
            {
                if (stringIsWord(((StringElement) element).getRealisation(), category))
                    return createWord(((StringElement) element).getRealisation(), category);
                else
                    return (StringElement) element;
            }

            // other NLGElement - return element
            else
            if (element is INLGElement)
            return (INLGElement) element;

            // string - look up in lexicon if a word, otherwise return StringElement
            else
            if (element  is string )
            {
                if (stringIsWord((string) element, category))
                    return createWord(element, category);
                else
                    return new StringElement((string) element);
            }

            throw new ArgumentException(element.ToString() + " is not a valid type");
        }

        /**
         * return true if string is a word
         * 
         * @param string
         * @param category
         * @return
         */
        private bool stringIsWord(string  str , ILexicalCategory category )
        {
            return lexicon != null
                   && (lexicon.hasWord(str, category) || PRONOUNS.Contains(str) || (str.matches(WORD_REGEX)));
        }

        /**
         * create an NLGElement from the element, no default lexical category
         * 
         * @param element
         * @return NLGelement
         */

        public INLGElement createNLGElement(object element)
        {
            return createNLGElement(element, new LexicalCategory_ANY());
        }

        /**
         * Creates a blank noun phrase with no subject or specifier.
         * 
         * @return a <code>NPPhraseSpec</code> representing this phrase.
         */

        public NPPhraseSpec createNounPhrase()
        {
            return createNounPhrase(null, null);
        }

        /**
         * Creates a noun phrase with the given subject but no specifier.
         * 
         * @param noun
         *            the subject of the phrase.
         * @return a <code>NPPhraseSpec</code> representing this phrase.
         */

        public NPPhraseSpec createNounPhrase(object noun)
        {
            if (noun is NPPhraseSpec)
            return (NPPhraseSpec) noun;
            else
            return createNounPhrase(null, noun);
        }

        /**
         * Creates a noun phrase with the given specifier and subject.
         * 
         * @param specifier
         *            the specifier or determiner for the noun phrase.
         * @param noun
         *            the subject of the phrase.
         * @return a <code>NPPhraseSpec</code> representing this phrase.
         */

        public NPPhraseSpec createNounPhrase(object specifier, object noun)
        {
            if (noun is NPPhraseSpec)
            return (NPPhraseSpec) noun;

            var phraseElement = new NPPhraseSpec(this);
            var nounElement = createNLGElement(noun, new LexicalCategory_NOUN());
            setPhraseHead(phraseElement, nounElement);

            if (specifier != null)
                phraseElement.setSpecifier(specifier);

            return phraseElement;
        }

        /**
         * A helper method to set the head feature of the phrase.
         * 
         * @param phraseElement
         *            the phrase element.
         * @param headElement
         *            the head element.
         */

        private void setPhraseHead(PhraseElement phraseElement, INLGElement headElement)
        {
            if (headElement != null)
            {
                if (phraseElement is NPPhraseSpec)
                    ((NPPhraseSpec)phraseElement).setHead(headElement);
                else
                    phraseElement.setHead(headElement);
                headElement.setParent(phraseElement);
            }
        }

        /**
         * Creates a blank adjective phrase with no base adjective set.
         * 
         * @return a <code>AdjPhraseSpec</code> representing this phrase.
         */

        public AdjPhraseSpec createAdjectivePhrase()
        {
            return createAdjectivePhrase(null);
        }

        /**
         * Creates an adjective phrase wrapping the given adjective.
         * 
         * @param adjective
         *            the main adjective for this phrase.
         * @return a <code>AdjPhraseSpec</code> representing this phrase.
         */

        public AdjPhraseSpec createAdjectivePhrase(object adjective)
        {
            var phraseElement = new AdjPhraseSpec(this);

            var adjectiveElement = createNLGElement(adjective, new LexicalCategory_ADJECTIVE());
            setPhraseHead(phraseElement, adjectiveElement);

            return phraseElement;
        }

        /**
         * Creates a blank verb phrase with no main verb.
         * 
         * @return a <code>VPPhraseSpec</code> representing this phrase.
         */

        public VPPhraseSpec createVerbPhrase()
        {
            return createVerbPhrase(null);
        }

        /**
         * Creates a verb phrase wrapping the main verb given. If a
         * <code>string</code> is passed in then some parsing is done to see if the
         * verb contains a particle, for example <em>fall down</em>. The first word
         * is taken to be the verb while all other words are assumed to form the
         * particle.
         * 
         * @param verb
         *            the verb to be wrapped.
         * @return a <code>VPPhraseSpec</code> representing this phrase.
         */

        public VPPhraseSpec createVerbPhrase(object verb)
        {
            var phraseElement = new VPPhraseSpec(this);
            phraseElement.setVerb(verb);
            setPhraseHead(phraseElement, phraseElement.getVerb());
            return phraseElement;
        }

        /**
         * Creates a blank adverb phrase that has no adverb.
         * 
         * @return a <code>AdvPhraseSpec</code> representing this phrase.
         */

        public AdvPhraseSpec createAdverbPhrase()
        {
            return createAdverbPhrase(null);
        }

        /**
         * Creates an adverb phrase wrapping the given adverb.
         * 
         * @param adverb
         *            the adverb for this phrase.
         * @return a <code>AdvPhraseSpec</code> representing this phrase.
         */

        public AdvPhraseSpec createAdverbPhrase(string adverb)
        {
            var phraseElement = new AdvPhraseSpec(this);

            var adverbElement = createNLGElement(adverb, new LexicalCategory_ADVERB());
            setPhraseHead(phraseElement, adverbElement);
            return phraseElement;
        }

        /**
         * Creates a blank clause with no subject, verb or objects.
         * 
         * @return a <code>SPhraseSpec</code> representing this phrase.
         */

        public SPhraseSpec createClause()
        {
            return createClause(null, null, null);
        }

        /**
         * Creates a clause with the given subject and verb but no objects.
         * 
         * @param subject
         *            the subject for the clause as a <code>NLGElement</code> or
         *            <code>string</code>. This forms a noun phrase.
         * @param verb
         *            the verb for the clause as a <code>NLGElement</code> or
         *            <code>string</code>. This forms a verb phrase.
         * @return a <code>SPhraseSpec</code> representing this phrase.
         */

        public SPhraseSpec createClause(object subject, object verb)
        {
            return createClause(subject, verb, null);
        }

        /**
         * Creates a clause with the given subject, verb or verb phrase and direct
         * object but no indirect object.
         * 
         * @param subject
         *            the subject for the clause as a <code>NLGElement</code> or
         *            <code>string</code>. This forms a noun phrase.
         * @param verb
         *            the verb for the clause as a <code>NLGElement</code> or
         *            <code>string</code>. This forms a verb phrase.
         * @param directObject
         *            the direct object for the clause as a <code>NLGElement</code>
         *            or <code>string</code>. This forms a complement for the
         *            clause.
         * @return a <code>SPhraseSpec</code> representing this phrase.
         */

        public SPhraseSpec createClause(object subject, object verb, object directObject)
        {

            var phraseElement = new SPhraseSpec(this);

            if (verb != null)
            {
                // AG: fix here: check if "verb" is a VPPhraseSpec or a Verb
                if (verb
                is PhraseElement)
                {
                    phraseElement.setVerbPhrase((PhraseElement) verb);
                }
                else
                {
                    phraseElement.setVerb(verb);
                }
            }

            if (subject != null)
                phraseElement.setSubject(subject);

            if (directObject != null)
            {
                phraseElement.setObject(directObject);
            }

            return phraseElement;
        }


        public INLGElement createStringElement()
        {
            return createStringElement(null);
        }

        /**
         * Creates a canned text phrase with the given text.
         * 
         * @param text
         *            the canned text.
         * @return a <code>PhraseElement</code> representing this phrase.
         */

        public INLGElement createStringElement(string text)
        {
            return new StringElement(text);
        }

        /**
         * Creates a new (empty) coordinated phrase
         * 
         * @return empty <code>CoordinatedPhraseElement</code>
         */

        public CoordinatedPhraseElement createCoordinatedPhrase()
        {
            return new CoordinatedPhraseElement();
        }

        /**
         * Creates a new coordinated phrase with two elements (initially)
         * 
         * @param coord1
         *            - first phrase to be coordinated
         * @param coord2
         *            = second phrase to be coordinated
         * @return <code>CoordinatedPhraseElement</code> for the two given elements
         */

        public CoordinatedPhraseElement createCoordinatedPhrase(object coord1, object coord2)
        {
            return new CoordinatedPhraseElement(coord1, coord2);
        }

        /***********************************************************************************
         * Document level stuff
         ***********************************************************************************/

        /**
         * Creates a new document element with no title.
         * 
         * @return a <code>DocumentElement</code>
         */

        public DocumentElement createDocument()
        {
            return createDocument(null);
        }

        /**
         * Creates a new document element with the given title.
         * 
         * @param title
         *            the title for this element.
         * @return a <code>DocumentElement</code>.
         */

        public DocumentElement createDocument(string title)
        {
            return new DocumentElement(new DocumentCategory_DOCUMENT(), title);
        }

        /**
         * Creates a new document element with the given title and adds all of the
         * given components in the list
         * 
         * @param title
         *            the title of this element.
         * @param components
         *            a <code>List</code> of <code>NLGElement</code>s that form the
         *            components of this element.
         * @return a <code>DocumentElement</code>
         */

        public DocumentElement createDocument(string title, List<INLGElement> components)
        {

            var document = new DocumentElement(new DocumentCategory_DOCUMENT(), title);
            if (components != null)
            {
                document.addComponents(components);
            }
            return document;
        }

        /**
         * Creates a new document element with the given title and adds the given
         * component.
         * 
         * @param title
         *            the title for this element.
         * @param component
         *            an <code>NLGElement</code> that becomes the first component of
         *            this document element.
         * @return a <code>DocumentElement</code>
         */

        public DocumentElement createDocument(string title, INLGElement component)
        {
            var element = new DocumentElement(new DocumentCategory_DOCUMENT(), title);

            if (component != null)
            {
                element.addComponent(component);
            }
            return element;
        }

        /**
         * Creates a new list element with no components.
         * 
         * @return a <code>DocumentElement</code> representing the list.
         */

        public DocumentElement createList()
        {
            return new DocumentElement(new DocumentCategory_LIST(), null);
        }

        /**
         * Creates a new list element and adds all of the given components in the
         * list
         * 
         * @param textComponents
         *            a <code>List</code> of <code>NLGElement</code>s that form the
         *            components of this element.
         * @return a <code>DocumentElement</code> representing the list.
         */

        public DocumentElement createList(List<INLGElement> textComponents)
        {
            var list = new DocumentElement(new DocumentCategory_LIST(), null);
            list.addComponents(textComponents);
            return list;
        }

        /**
         * Creates a new section element with the given title and adds the given
         * component.
         * 
         * @param component
         *            an <code>NLGElement</code> that becomes the first component of
         *            this document element.
         * @return a <code>DocumentElement</code> representing the section.
         */

        public DocumentElement createList(INLGElement component)
        {
            var list = new DocumentElement(new DocumentCategory_LIST(), null);
            list.addComponent(component);
            return list;
        }

        /**
         * Creates a new enumerated list element with no components.
         * 
         * @return a <code>DocumentElement</code> representing the list.
         * @author Rodrigo de Oliveira - Data2Text Ltd
         */

        public DocumentElement createEnumeratedList()
        {
            return new DocumentElement(new DocumentCategory_ENUMERATED_LIST(), null);
        }

        /**
         * Creates a new enumerated list element and adds all of the given components in the
         * list
         * 
         * @param textComponents
         *            a <code>List</code> of <code>NLGElement</code>s that form the
         *            components of this element.
         * @return a <code>DocumentElement</code> representing the list.
         * @author Rodrigo de Oliveira - Data2Text Ltd
         */

        public DocumentElement createEnumeratedList(List<INLGElement> textComponents)
        {
            var list = new DocumentElement(new DocumentCategory_ENUMERATED_LIST(), null);
            list.addComponents(textComponents);
            return list;
        }

        /**
         * Creates a new section element with the given title and adds the given
         * component.
         * 
         * @param component
         *            an <code>NLGElement</code> that becomes the first component of
         *            this document element.
         * @return a <code>DocumentElement</code> representing the section.
         * @author Rodrigo de Oliveira - Data2Text Ltd
         */

        public DocumentElement createEnumeratedList(INLGElement component)
        {
            var list = new DocumentElement(new DocumentCategory_ENUMERATED_LIST(), null);
            list.addComponent(component);
            return list;
        }

        /**
         * Creates a list item for adding to a list element.
         * 
         * @return a <code>DocumentElement</code> representing the list item.
         */

        public DocumentElement createListItem()
        {
            return new DocumentElement(new DocumentCategory_LIST_ITEM(), null);
        }

        /**
         * Creates a list item for adding to a list element. The list item has the
         * given component.
         * 
         * @return a <code>DocumentElement</code> representing the list item.
         */

        public DocumentElement createListItem(INLGElement component)
        {
            var listItem = new DocumentElement(new DocumentCategory_LIST_ITEM(), null);
            listItem.addComponent(component);
            return listItem;
        }

        /**
         * Creates a new paragraph element with no components.
         * 
         * @return a <code>DocumentElement</code> representing this paragraph
         */

        public DocumentElement createParagraph()
        {
            return new DocumentElement(new DocumentCategory_PARAGRAPH(), null);
        }

        /**
         * Creates a new paragraph element and adds all of the given components in
         * the list
         * 
         * @param components
         *            a <code>List</code> of <code>NLGElement</code>s that form the
         *            components of this element.
         * @return a <code>DocumentElement</code> representing this paragraph
         */

        public DocumentElement createParagraph(List<INLGElement> components)
        {
            var paragraph = new DocumentElement(new DocumentCategory_PARAGRAPH(), null);
            if (components != null)
            {
                paragraph.addComponents(components);
            }
            return paragraph;
        }

        /**
         * Creates a new paragraph element and adds the given component
         * 
         * @param component
         *            an <code>NLGElement</code> that becomes the first component of
         *            this document element.
         * @return a <code>DocumentElement</code> representing this paragraph
         */

        public DocumentElement createParagraph(INLGElement component)
        {
            var paragraph = new DocumentElement(new DocumentCategory_PARAGRAPH(), null);
            if (component != null)
            {
                paragraph.addComponent(component);
            }
            return paragraph;
        }

        /**
         * Creates a new section element.
         * 
         * @return a <code>DocumentElement</code> representing the section.
         */

        public DocumentElement createSection()
        {
            return new DocumentElement(new DocumentCategory_SECTION(), null);
        }

        /**
         * Creates a new section element with the given title.
         * 
         * @param title
         *            the title of the section.
         * @return a <code>DocumentElement</code> representing the section.
         */

        public DocumentElement createSection(string title)
        {
            return new DocumentElement(new DocumentCategory_SECTION(), title);
        }

        /**
         * Creates a new section element with the given title and adds all of the
         * given components in the list
         * 
         * @param title
         *            the title of this element.
         * @param components
         *            a <code>List</code> of <code>NLGElement</code>s that form the
         *            components of this element.
         * @return a <code>DocumentElement</code> representing the section.
         */

        public DocumentElement createSection(string title, List<INLGElement> components)
        {

            var section = new DocumentElement(new DocumentCategory_SECTION(), title);
            if (components != null)
            {
                section.addComponents(components);
            }
            return section;
        }

        /**
         * Creates a new section element with the given title and adds the given
         * component.
         * 
         * @param title
         *            the title for this element.
         * @param component
         *            an <code>NLGElement</code> that becomes the first component of
         *            this document element.
         * @return a <code>DocumentElement</code> representing the section.
         */

        public DocumentElement createSection(string title, INLGElement component)
        {
            var section = new DocumentElement(new DocumentCategory_SECTION(), title);
            if (component != null)
            {
                section.addComponent(component);
            }
            return section;
        }

        /**
         * Creates a new sentence element with no components.
         * 
         * @return a <code>DocumentElement</code> representing this sentence
         */

        public DocumentElement createSentence()
        {
            return new DocumentElement(new DocumentCategory_SENTENCE(), null);
        }

        /**
         * Creates a new sentence element and adds all of the given components.
         * 
         * @param components
         *            a <code>List</code> of <code>NLGElement</code>s that form the
         *            components of this element.
         * @return a <code>DocumentElement</code> representing this sentence
         */

        public DocumentElement createSentence(List<INLGElement> components)
        {
            var sentence = new DocumentElement(new DocumentCategory_SENTENCE(), null);
            sentence.addComponents(components);
            return sentence;
        }

        /**
         * Creates a new sentence element
         * 
         * @param components
         *            an <code>NLGElement</code> that becomes the first component of
         *            this document element.
         * @return a <code>DocumentElement</code> representing this sentence
         */

        public DocumentElement createSentence(INLGElement components)
        {
            var sentence = new DocumentElement(new DocumentCategory_SENTENCE(), null);
            sentence.addComponent(components);
            return sentence;
        }

        /**
         * Creates a sentence with the given subject and verb. The phrase factory is
         * used to construct a clause that then forms the components of the
         * sentence.
         * 
         * @param subject
         *            the subject of the sentence.
         * @param verb
         *            the verb of the sentence.
         * @return a <code>DocumentElement</code> representing this sentence
         */

        public DocumentElement createSentence(object subject, object verb)
        {
            return createSentence(subject, verb, null);
        }

        /**
         * Creates a sentence with the given subject, verb and direct object. The
         * phrase factory is used to construct a clause that then forms the
         * components of the sentence.
         * 
         * @param subject
         *            the subject of the sentence.
         * @param verb
         *            the verb of the sentence.
         * @param complement
         *            the object of the sentence.
         * @return a <code>DocumentElement</code> representing this sentence
         */

        public DocumentElement createSentence(object subject, object verb, object complement)
        {

            var sentence = new DocumentElement(new DocumentCategory_SENTENCE(), null);
            sentence.addComponent(createClause(subject, verb, complement));
            return sentence;
        }

        /**
         * Creates a new sentence with the given canned text. The canned text is
         * used to form a canned phrase (from the phrase factory) which is then
         * added as the component to sentence element.
         * 
         * @param cannedSentence
         *            the canned text as a <code>string</code>.
         * @return a <code>DocumentElement</code> representing this sentence
         */

        public DocumentElement createSentence(string cannedSentence)
        {
            var sentence = new DocumentElement(new DocumentCategory_SENTENCE(), null);

            if (cannedSentence != null)
            {
                sentence.addComponent(createStringElement(cannedSentence));
            }
            return sentence;
        }
    }
}