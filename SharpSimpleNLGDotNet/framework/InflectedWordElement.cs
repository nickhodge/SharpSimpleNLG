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

namespace SimpleNLG
{
    /*
    * Internal class. This maintains inflectional variants of the word, which
    * may be available in the lexicon. For example, a word may have both a
    * regular and an irregular variant. If the desired type is the irregular,
    * it is convenient to have the set of irregular inflected forms available
    * without necessitating a new call to the lexicon to get the forms.
    */

    public class InflectedWordElement : NLGElement
    {

        /**
         * Constructs a new inflected word using the giving word as the base form.
         * Constructing the word also requires a lexical category (such as noun,
         * verb).
         * 
         * @param word
         *            the base form for this inflected word.
         * @param category
         *            the lexical category for the word.
         */

        public InflectedWordElement(string word, IElementCategory category)
        {
            setFeature(LexicalFeature.BASE_FORM, word);
            setCategory(category);
        }

        /**
         * Constructs a new inflected word from a WordElement
         * 
         * @param word
         *            underlying wordelement
         */

        public InflectedWordElement(WordElement word)
        {
            setFeature(InternalFeature.BASE_WORD.ToString(), word);
            // AG: changed to use the default spelling variant
            // setFeature(LexicalFeature.BASE_FORM, word.getBaseForm());
            var defaultSpelling = word.getDefaultSpellingVariant();
            setFeature(LexicalFeature.BASE_FORM, defaultSpelling);
            setCategory(word.getCategory());
        }

        /**
         * This method returns null as the inflected word has no child components.
         */

        public override List<INLGElement> getChildren()
        {
            return null;
        }


        public override string ToString()
        {
            return "InflectedWordElement[" + getBaseForm() + ':' 
                   + getCategory().ToString() + ']';
        }

        public override string printTree(string indent)
        {
            var print = new StringBuilder();
            print.Append("InflectedWordElement: base=").Append(getBaseForm()) 
                .Append(", category=").Append(getCategory()).Append( 
                    ", ").Append(base.ToString()).Append('\n'); 
            return print.ToString();
        }

        /**
         * Retrieves the base form for this element. The base form is the originally
         * supplied word.
         * 
         * @return a <code>string</code> forming the base form of the element.
         */

        public string getBaseForm()
        {
            return getFeatureAsString(LexicalFeature.BASE_FORM);
        }

        /**
         * Sets the base word for this element.
         * 
         * @param word
         *            the <code>WordElement</code> representing the base word as
         *            read from the lexicon.
         */

        public void setBaseWord(WordElement word)
        {
            setFeature(InternalFeature.BASE_WORD.ToString(), word);
        }

        /**
         * Retrieves the base word for this element.
         * 
         * @return the <code>WordElement</code> representing the base word as read
         *         from the lexicon.
         */

        public WordElement getBaseWord()
        {
            INLGElement baseWord = getFeatureAsElement(InternalFeature.BASE_WORD.ToString());
            return baseWord as WordElement;
        }
    }
}