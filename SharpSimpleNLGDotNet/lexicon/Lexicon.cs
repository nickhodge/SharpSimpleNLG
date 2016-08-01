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
    public abstract class Lexicon
    {

        /****************************************************************************/
        // constructors and related
        /****************************************************************************/

        /**
         * returns the default built-in lexicon
         * 
         * @return default lexicon
         */

        public static Lexicon getDefaultLexicon()
        {
            return new XMLLexicon();
        }

        /**
         * create a default WordElement. May be overridden by specific types of
         * lexicon
         * 
         * @param baseForm
         *            - base form of word
         * @param category
         *            - category of word
         * @return WordElement entry for specified info
         */

        protected WordElement createWord(string baseForm, ILexicalCategory category)
        {
            return new WordElement(baseForm, category); // return default
            // WordElement of this
            // baseForm, category
        }

        /**
         * create a default WordElement. May be overridden by specific types of
         * lexicon
         * 
         * @param baseForm
         *            - base form of word
         * @return WordElement entry for specified info
         */

        protected WordElement createWord(string baseForm)
        {
            return new WordElement(baseForm); // return default WordElement of this
            // baseForm
        }

        /***************************************************************************/
        // default methods for looking up words
        // These try the following (in this order)
        // 1) word with matching base
        // 2) word with matching variant
        // 3) word with matching ID
        // 4) create a new workd
        /***************************************************************************/

        /**
         * General word lookup method, tries base form, variant, ID (in this order)
         * Creates new word if can't find existing word
         * 
         * @param baseForm
         * @param category
         * @return word
         */

        public WordElement lookupWord(string baseForm, ILexicalCategory category)
        {
            if (hasWord(baseForm, category))
                return getWord(baseForm, category);
            else if (hasWordFromVariant(baseForm, category))
                return getWordFromVariant(baseForm, category);
            else if (hasWordByID(baseForm))
                return getWordByID(baseForm);
            else
                return createWord(baseForm, category);
        }

        /**
         * General word lookup method, tries base form, variant, ID (in this order)
         * Creates new word if can't find existing word
         * 
         * @param baseForm
         * @return word
         */

        public WordElement lookupWord(string baseForm)
        {
            return lookupWord(baseForm, new LexicalCategory_ANY());
        }

        /****************************************************************************/
        // get words by baseform and category
        // fundamental version is getWords(string baseForm, Category category),
        // this must be defined by subclasses. Other versions are convenience
        // methods. These may be overriden for efficiency, but this is not required.
        /****************************************************************************/

        /**
         * returns all Words which have the specified base form and category
         * 
         * @param baseForm
         *            - base form of word, eg "be" or "dog" (not "is" or "dogs")
         * @param category
         *            - syntactic category of word (ANY for unknown)
         * @return collection of all matching Words (may be empty)
         */

        public abstract List<WordElement> getWords(string baseForm, ILexicalCategory category);

        /**
         * get a WordElement which has the specified base form and category
         * 
         * @param baseForm
         *            - base form of word, eg "be" or "dog" (not "is" or "dogs")
         * @param category
         *            - syntactic category of word (ANY for unknown)
         * @return if Lexicon contains such a WordElement, it is returned (the first
         *         match is returned if there are several matches). If the Lexicon
         *         does not contain such a WordElement, a new WordElement is created
         *         and returned
         */

        public WordElement getWord(string baseForm, ILexicalCategory category)
        {
// convenience
            // method
            // derived
            // from
            // other
            // methods
            var wordElements = getWords(baseForm, category);
            if (wordElements.isEmpty())
                return createWord(baseForm, category); // return default WordElement
            // of this baseForm,
            // category
            else
                return selectMatchingWord(wordElements, baseForm);
        }

        /** choose a single WordElement from a list of WordElements.  Prefer one
         * which exactly matches the baseForm
         * @param wordElements
         *           - list of WordElements retrieved from lexicon
         * @param baseForm
                     - base form of word, eg "be" or "dog" (not "is" or "dogs")
         * @return single WordElement (from list)
         */

        private WordElement selectMatchingWord(List<WordElement> wordElements, string baseForm)
        {
            // EHUD REITER  - this method added because some DBs are case-insensitive,
            // so a query on "man" returns both "man" and "MAN".  In such cases, the
            // exact match (eg, "man") should be returned

            // below check is redundant, since caller should check this
            if (wordElements == null || wordElements.isEmpty())
                return createWord(baseForm);

            // look for exact match in base form
            foreach (var wordElement in wordElements)
            if (wordElement.getBaseForm().Equals(baseForm))
                return wordElement;

            // Roman Kutlak: I don't think it is a good idea to return a word whose
            // case does not match because if a word appears in the lexicon
            // as an acronym only, it will be replaced as such. For example,
            // "foo" will return as the acronym "FOO". This does not seem desirable.
            // else return first element in list
            if (wordElements[0].getBaseForm().equalsIgnoreCase(baseForm))
            {
                return createWord(baseForm, new LexicalCategory_ANY());
            }

            return wordElements[0];

        }

        /**
         * return <code>true</code> if the lexicon contains a WordElement which has
         * the specified base form and category
         * 
         * @param baseForm
         *            - base form of word, eg "be" or "dog" (not "is" or "dogs")
         * @param category
         *            - syntactic category of word (ANY for unknown)
         * @return <code>true</code> if Lexicon contains such a WordElement
         */

        public bool hasWord(string baseForm, ILexicalCategory category)
        {
// convenience
            // method
            // derived
            // from
            // other
            // methods)
            // {
            return !getWords(baseForm, category).isEmpty();
        }

        /**
         * returns all Words which have the specified base form
         * 
         * @param baseForm
         *            - base form of word, eg "be" or "dog" (not "is" or "dogs")
         * @return collection of all matching Words (may be empty)
         */

        public List<WordElement> getWords(string baseForm)
        {
            // convenience method
            // derived from
            // other methods
            return getWords(baseForm, new LexicalCategory_ANY());
        }

        /**
         * get a WordElement which has the specified base form (of any category)
         * 
         * @param baseForm
         *            - base form of word, eg "be" or "dog" (not "is" or "dogs")
         * @return if Lexicon contains such a WordElement, it is returned (the first
         *         match is returned if there are several matches). If the Lexicon
         *         does not contain such a WordElement, a new WordElement is created
         *         and returned
         */

        public WordElement getWord(string baseForm)
        {
            // convenience method derived
            // from other methods
            var wordElements = getWords(baseForm);

            if (wordElements.isEmpty())
                return createWord(baseForm); // return default WordElement of this
            // baseForm
            else
                return selectMatchingWord(wordElements, baseForm);
        }

        /**
         * return <code>true</code> if the lexicon contains a WordElement which has
         * the specified base form (in any category)
         * 
         * @param baseForm
         *            - base form of word, eg "be" or "dog" (not "is" or "dogs")
         * @return <code>true</code> if Lexicon contains such a WordElement
         */

        public bool hasWord(string baseForm)
        {
// convenience method derived from
            // other methods) {
            return !getWords(baseForm).isEmpty();
        }

        /****************************************************************************/
        // get words by ID
        // fundamental version is getWordsByID(string id),
        // this must be defined by subclasses.
        // Other versions are convenience methods
        // These may be overriden for efficiency, but this is not required.
        /****************************************************************************/

        /**
         * returns a List of WordElement which have this ID. IDs are
         * lexicon-dependent, and should be unique. Therefore the list should
         * contain either zero elements (if no such word exists) or one element (if
         * the word is found)
         * 
         * @param id
         *            - internal lexicon ID for a word
         * @return either empty list (if no word with this ID exists) or list
         *         containing the matching word
         */
        public abstract List<WordElement> getWordsByID(string id);

        /**
         * get a WordElement with the specified ID
         * 
         * @param id
         *            internal lexicon ID for a word
         * @return WordElement with this ID if found; otherwise a new WordElement is
         *         created with the ID as the base form
         */

        public WordElement getWordByID(string id)
        {
            var wordElements = getWordsByID(id);
            if (wordElements.isEmpty())
                return createWord(id); // return WordElement based on ID; may help
            // in debugging...
            else
                return wordElements[0]; // else return first match
        }

        /**
         * return <code>true</code> if the lexicon contains a WordElement which the
         * specified ID
         * 
         * @param id
         *            - internal lexicon ID for a word
         * @return <code>true</code> if Lexicon contains such a WordElement
         */

        public bool hasWordByID(string id)
        {
// convenience method derived from
            // other methods) {
            return !getWordsByID(id).isEmpty();
        }

        /****************************************************************************/
        // get words by variant - try to return a WordElement given an inflectional
        // or spelling
        // variant. For the moment, acronyms are considered as separate words, not
        // variants
        // (this may change in the future)
        // fundamental version is getWordsFromVariant(string baseForm, Category
        // category),
        // this must be defined by subclasses. Other versions are convenience
        // methods. These may be overriden for efficiency, but this is not required.
        /****************************************************************************/

        /**
         * returns Words which have an inflected form and/or spelling variant that
         * matches the specified variant, and are in the specified category. <br>
         * <I>Note:</I> the returned word list may not be complete, it depends on
         * how it is implemented by the underlying lexicon
         * 
         * @param variant
         *            - base form, inflected form, or spelling variant of word
         * @param category
         *            - syntactic category of word (ANY for unknown)
         * @return list of all matching Words (empty list if no matching WordElement
         *         found)
         */

        public abstract List<WordElement> getWordsFromVariant(string variant,
            ILexicalCategory category);

        /**
         * returns a WordElement which has the specified inflected form and/or
         * spelling variant that matches the specified variant, of the specified
         * category
         * 
         * @param variant
         *            - base form, inflected form, or spelling variant of word
         * @param category
         *            - syntactic category of word (ANY for unknown)
         * @return a matching WordElement (if found), otherwise a new word is
         *         created using thie variant as the base form
         */

        public WordElement getWordFromVariant(string variant,
            ILexicalCategory category)
        {
            var wordElements = getWordsFromVariant(variant, category);
            if (wordElements.isEmpty())
                return createWord(variant, category); // return default WordElement
            // using variant as base
            // form
            else
                return selectMatchingWord(wordElements, variant);

        }

        /**
         * return <code>true</code> if the lexicon contains a WordElement which
         * matches the specified variant form and category
         * 
         * @param variant
         *            - base form, inflected form, or spelling variant of word
         * @param category
         *            - syntactic category of word (ANY for unknown)
         * @return <code>true</code> if Lexicon contains such a WordElement
         */

        public bool hasWordFromVariant(string variant, ILexicalCategory category)
        {
// convenience
            // method
            // derived
            // from
            // other
            // methods)
            // {
            return !getWordsFromVariant(variant, category).isEmpty();
        }

        /**
         * returns Words which have an inflected form and/or spelling variant that
         * matches the specified variant, of any category. <br>
         * <I>Note:</I> the returned word list may not be complete, it depends on
         * how it is implemented by the underlying lexicon
         * 
         * @param variant
         *            - base form, inflected form, or spelling variant of word
         * @return list of all matching Words (empty list if no matching WordElement
         *         found)
         */

        public List<WordElement> getWordsFromVariant(string variant)
        {
            return getWordsFromVariant(variant, new LexicalCategory_ANY());
        }

        /**
         * returns a WordElement which has the specified inflected form and/or
         * spelling variant that matches the specified variant, of any category.
         * 
         * @param variant
         *            - base form, inflected form, or spelling variant of word
         * @return a matching WordElement (if found), otherwise a new word is
         *         created using thie variant as the base form
         */

        public WordElement getWordFromVariant(string variant)
        {
            var wordElements = getWordsFromVariant(variant);
            if (wordElements.isEmpty())
                return createWord(variant); // return default WordElement using
            // variant as base form
            else
                return wordElements[0]; // else return first match
        }

        /**
         * return <code>true</code> if the lexicon contains a WordElement which
         * matches the specified variant form (in any category)
         * 
         * @param variant
         *            - base form, inflected form, or spelling variant of word
         * @return <code>true</code> if Lexicon contains such a WordElement
         */

        public bool hasWordFromVariant(string variant)
        {
// convenience method
            // derived from other
            // methods) {
            return !getWordsFromVariant(variant).isEmpty();
        }

        /****************************************************************************/
        // other methods
        /****************************************************************************/

        /**
         * close the lexicon (if necessary) if lexicon does not need to be closed,
         * this does nothing
         */

        public void close()
        {
            // default method does nothing
        }

    }
}
