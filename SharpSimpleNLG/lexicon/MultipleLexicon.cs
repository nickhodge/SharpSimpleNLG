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
    public class MultipleLexicon : Lexicon
    {

        /* if this flag is true, all lexicons are searched for
         * this word, even after a match is found
         * it is false by default
         * */
        private bool alwaysSearchAll { get; set; }

        /* list of lexicons, in order in which they are searched */
        private List<Lexicon> lexiconList = null;

        /**********************************************************************/
        // constructors
        /**********************************************************************/

        /**
         * create an empty multi lexicon
         */

        public MultipleLexicon()
        {
            lexiconList = new List<Lexicon>();
            alwaysSearchAll = false;
        }

        /** create a multi lexicon with the specified lexicons
         * @param lexicons
         */

        public MultipleLexicon(IEnumerable<Lexicon> lexicons)
        {
            foreach (var lex in lexicons)
            lexiconList.Add(lex);
        }

        /**********************************************************************/
        // routines to add more lexicons, change flags
        /**********************************************************************/

        /** add lexicon at beginning of list (is searched first)
         * @param lex
         */

        public void addInitialLexicon(Lexicon lex)
        {
            lexiconList.Add(lex);
        }

        /** add lexicon at end of list (is searched last)
         * @param lex
         */

        public void addFinalLexicon(Lexicon lex)
        {
            lexiconList.Add(lex);
        }

        /**
         * @return the alwaysSearchAll
         */

        public bool isAlwaysSearchAll()
        {
            return alwaysSearchAll;
        }


        /**********************************************************************/
        // main methods
        /**********************************************************************/

        /* (non-Javadoc)
         * @see simplenlg.lexicon.Lexicon#getWords(java.lang.string, simplenlg.features.LexicalCategory)
         */
  

        public override List<WordElement> getWords(string baseForm, ILexicalCategory category)
        {
            var result = new List<WordElement>();
            foreach (var lex in lexiconList)
            {
                var lexResult = lex.getWords(baseForm, category);
                if (lexResult != null && !lexResult.isEmpty())
                {
                    result.AddRange(lexResult);
                    if (!alwaysSearchAll)
                        return result;
                }
            }
            return result;
        }

        /* (non-Javadoc)
         * @see simplenlg.lexicon.Lexicon#getWordsByID(java.lang.string)
         */


        public override List<WordElement> getWordsByID(string id)
        {
            var result = new List<WordElement>();
            foreach (var lex in lexiconList)
            {
                var lexResult = lex.getWordsByID(id);
                if (lexResult != null && !lexResult.isEmpty())
                {
                    result.AddRange(lexResult);
                    if (!alwaysSearchAll)
                        return result;
                }
            }
            return result;
        }

        /* (non-Javadoc)
         * @see simplenlg.lexicon.Lexicon#getWordsFromVariant(java.lang.string, simplenlg.features.LexicalCategory)
         */

        public override List<WordElement> getWordsFromVariant(string variant, ILexicalCategory category)
        {
            var result = new List<WordElement>();
            foreach (var lex in lexiconList)
            {
                var lexResult = lex.getWordsFromVariant(variant, category);
                if (lexResult != null && !lexResult.isEmpty())
                {
                    result.AddRange(lexResult);
                    if (!alwaysSearchAll)
                        return result;
                }
            }
            return result;
        }


        /**********************************************************************/
        // other methods
        /**********************************************************************/

        /* (non-Javadoc)
         * @see simplenlg.lexicon.Lexicon#close()
         */


        public void close()
        {
            // close component lexicons
            foreach (var lex in lexiconList)
            lex.close();
        }

    }
}