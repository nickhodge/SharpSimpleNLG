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
using System.Diagnostics;
using System.IO;
using System.Xml;
using SimpleNLG.Extensions;

namespace SimpleNLG
{
    public class XMLLexicon : Lexicon
    {

        // node names in lexicon XML files
        private static string XML_BASE = "base"; // base form of Word
        private static string XML_CATEGORY = "category"; // base form of Word
        private static string XML_ID = "id"; // base form of Word
        private static string XML_WORD = "word"; // node defining a word

        // lexicon
        public HashSet<WordElement> words; // set of words
        public Dictionary<string, WordElement> indexByID; // map from ID to word
        public Dictionary<string, List<WordElement>> indexByBase; // map from base to set
        // of words with this
        // baseform
        public Dictionary<string, List<WordElement>> indexByVariant; // map from variants

        // to set of words
        // with this variant

        public XMLLexicon(string path = null)
        {
//            if (path == null) // try the embedded resource first
//            {
//                createLexiconFromEmbeddedResource(@"SharpSimpleNLG.lexicon.default-lexicon.xml");
//            }
//            else
//            {
                createLexiconFromPath(path);
//            }
        }

        public void createLexiconFromPath(string path)
        {
            try
            {
                var rawlexicontext = File.ReadAllText(path);
                createLexicon(rawlexicontext);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        public void createLexicon(string rawlexicontext)
        {
            // initialise objects
            words = new HashSet<WordElement>();
            indexByID = new Dictionary<string, WordElement>();
            indexByBase = new Dictionary<string, List<WordElement>>();
            indexByVariant = new Dictionary<string, List<WordElement>>();

                var doc = new XmlDocument();
                doc.LoadXml(rawlexicontext);

                if (doc != null)
                {
                    var lexRoot = doc.DocumentElement;
                    var wordNodes = lexRoot.GetElementsByTagName("word");
                    for (var i = 0; i < wordNodes?.Count; i++)
                    {
                        var wordNode = wordNodes[i];
                        // ignore things that aren't elements
                        if (wordNode.NodeType == XmlNodeType.Element)
                        {
                            var word = convertNodeToWord(wordNode);
                            if (word != null)
                            {
                                words.add(word);
                                IndexWord(word);
                            }
                        }
                    }
                }
 
            addSpecialCases();
        }

        /**
         * add special cases to lexicon
         * 
         */

        private void addSpecialCases()
        {
            // add variants of "be"
            var be = getWord("be", new LexicalCategory_VERB());
            if (be != null)
            {
                updateIndex(be, "is", indexByVariant);
                updateIndex(be, "am", indexByVariant);
                updateIndex(be, "are", indexByVariant);
                updateIndex(be, "was", indexByVariant);
                updateIndex(be, "were", indexByVariant);
            }
        }

        /**
         * create a simplenlg WordElement from a Word node in a lexicon XML file
         * 
         * @param wordNode
         * @return
         * @throws XPathUtilException
         */

        private WordElement convertNodeToWord(XmlNode wordNode)
        {
            // if this isn't a Word node, ignore it
            if (!wordNode.LocalName.equalsIgnoreCase(XML_WORD))
                return null;

            // // if there is no base, flag an error and return null
            // string base = XPathUtil.extractValue(wordNode, Constants.XML_BASE);
            // if (base == null) {
            // Console.WriteLine("Error in loading XML lexicon: Word with no base");
            // return null;
            // }

            // create word
            var word = new WordElement();
            var inflections = new List<Inflection>();

            // now copy features
            var nodes = wordNode.ChildNodes;
            for (var i = 0; i < nodes.Count; i++)
            {
                var featureNode = nodes[i];

                if (featureNode.NodeType == XmlNodeType.Element)
                {
                    var feature = featureNode.LocalName.trim();
                    var value = featureNode.InnerText;

                    if (value != null)
                        value = value.trim();

                    if (feature == null)
                    {
                        Debug.WriteLine("Error in XML lexicon node for " + word);
                        break;
                    }

                    if (feature.equalsIgnoreCase(XML_BASE))
                    {
                        word.setBaseForm(value);
                    }
                    else if (feature.equalsIgnoreCase(XML_CATEGORY))
                    {
                        var c = LexicalCategoryExtensions.valueOf(value.toUpperCase());
                        word.setCategory(c);
                    }
                    else if (feature.equalsIgnoreCase(XML_ID))
                        word.setId(value);

                    else if (value == null || value.Equals(""))
                    {
                        // if this is an infl code, add it to inflections
                        Tuple<bool,Inflection> infl = InflectionExtensions.getInflCode(feature);

                        if (infl.Item1)
                        {
                            inflections.Add(infl.Item2);
                        }
                        else
                        {
                            word.setFeature(feature, true);
                        }
                    }
                    else
                        word.setFeature(feature, value);
                }

            }

            // if no infl specified, assume regular
            if (inflections.isEmpty())
            {
                inflections.Add(Inflection.REGULAR);
            }

            // default inflection code is "reg" if we have it, else random pick form
            // infl codes available
            var defaultInfl = inflections.Contains(Inflection.REGULAR)
                ? Inflection.REGULAR
                : inflections[0];

            word.setFeature(LexicalFeature.DEFAULT_INFL, defaultInfl);
            word.setDefaultInflectionalVariant(defaultInfl);

            foreach (var infl in inflections)
            {
                word.addInflectionalVariant(infl);
            }

            // done, return word
            return word;
        }

        /**
         * add word to internal indices
         * 
         * @param word
         */

        private void IndexWord(WordElement word)
        {
            // first index by base form
            var basef = word.getBaseForm();
            // shouldn't really need is, as all words have base forms
            if (basef != null)
            {
                updateIndex(word, basef, indexByBase);
            }

            // now index by ID, which should be unique (if present)
            var id = word.getId();
            if (id != null)
            {
                if (indexByID.ContainsKey(id))
                    Console.WriteLine($"Lexicon error: ID {id} occurs more than once");
                indexByID.Add(id, word);
            }

            // now index by variant
            foreach (var variant in getVariants(word))
            {
                updateIndex(word, variant, indexByVariant);
            }

            // done
        }

        /**
         * convenience method to update an index
         * 
         * @param word
         * @param base
         * @param index
         */

        private void updateIndex(WordElement word, string basef,
            Dictionary<string, List<WordElement>> index)
        {
            if (!index.ContainsKey(basef))
                index.Add(basef, new List<WordElement>());
            index[basef].Add(word);
        }

        /******************************************************************************************/
        // main methods to get data from lexicon
        /******************************************************************************************/

        /*
         * (non-Javadoc)
         * 
         * @see simplenlg.lexicon.Lexicon#getWords(java.lang.string,
         * simplenlg.features.LexicalCategory)
         */

        public override List<WordElement> getWords(string baseForm, ILexicalCategory category)
        {
            return getWordsFromIndex(baseForm, category, indexByBase);
        }

        /**
         * get matching keys from an index map
         * 
         * @param indexKey
         * @param category
         * @param indexMap
         * @return
         */

        private List<WordElement> getWordsFromIndex(string indexKey,
            ILexicalCategory category, Dictionary<string, List<WordElement>> indexMap)
        {
            var result = new List<WordElement>();

            // case 1: unknown, return empty list
            if (!indexMap.ContainsKey(indexKey))
            {
                return result;
            }

            // case 2: category is ANY, return everything
            if (category.enumType == (int)LexicalCategoryEnum.ANY)
            {
                foreach (var word in indexMap[indexKey])
                {
                    result.Add(new WordElement(word));
                }
                return result;
            }
            else
            {
                // case 3: other category, search for match
                foreach (var word in indexMap[indexKey])
                {
                    if (word.getCategory().enumType == category.enumType)
                    {
                        result.Add(new WordElement(word));
                    }
                }
            }
            return result;
        }

        /*
         * (non-Javadoc)
         * 
         * @see simplenlg.lexicon.Lexicon#getWordsByID(java.lang.string)
         */

        public override List<WordElement> getWordsByID(string id)
        {
            var result = new List<WordElement>();
            if (indexByID.ContainsKey(id))
            {
                result.Add(new WordElement(indexByID[id]));
            }
            return result;
        }

        /*
         * (non-Javadoc)
         * 
         * @see simplenlg.lexicon.Lexicon#getWordsFromVariant(java.lang.string,
         * simplenlg.features.LexicalCategory)
         */

        public override List<WordElement> getWordsFromVariant(string variant,
            ILexicalCategory category)
        {
            return getWordsFromIndex(variant, category, indexByVariant);
        }

        /**
         * quick-and-dirty routine for getting morph variants should be replaced by
         * something better!
         * 
         * @param word
         * @return
         */

        public HashSet<string> getVariants(WordElement word)
        {
            var variants = new HashSet<string>();
            variants.Add(word.getBaseForm());
            var category = word.getCategory();
            if (category is ILexicalCategory)
            {
                switch (category.enumType)
                {
                    case (int)LexicalCategoryEnum.NOUN:
                        variants.add(getVariant(word, LexicalFeature.PLURAL, "s"));
                        break;

                    case (int)LexicalCategoryEnum.ADJECTIVE:
                        variants
                            .add(getVariant(word, LexicalFeature.COMPARATIVE, "er"));
                        variants
                            .add(getVariant(word, LexicalFeature.SUPERLATIVE, "est"));
                        break;

                    case (int)LexicalCategoryEnum.VERB:
                        variants.add(getVariant(word, LexicalFeature.PRESENT3S, "s"));
                        variants.add(getVariant(word, LexicalFeature.PAST, "ed"));
                        variants.add(getVariant(word, LexicalFeature.PAST_PARTICIPLE,
                            "ed"));
                        variants.add(getVariant(word,
                            LexicalFeature.PRESENT_PARTICIPLE, "ing"));
                        break;
                }
            }
            return variants;
        }

        /**
         * quick-and-dirty routine for computing morph forms Should be replaced by
         * something better!
         * 
         * @param word
         * @param feature
         * @param string
         * @return
         */

        private string getVariant(WordElement word, string feature, string suffix)
        {
            if (word.hasFeature(feature))
                return word.getFeatureAsString(feature);
            else
                return getForm(word.getBaseForm(), suffix);
        }

        /**
         * quick-and-dirty routine for standard orthographic changes Should be
         * replaced by something better!
         * 
         * @param base
         * @param suffix
         * @return
         */

        private string getForm(string basef, string suffix)
        {
            // add a suffix to a base form, with orthographic changes

            // rule 1 - convert final "y" to "ie" if suffix does not start with "i"
            // eg, cry + s = cries , not crys
            if (basef.endsWith("y") && !suffix.startsWith("i"))
                basef = basef.substring(0, basef.Length - 1) + "ie";

            // rule 2 - drop final "e" if suffix starts with "e" or "i"
            // eg, like+ed = liked, not likeed
            if (basef.endsWith("e")
                && (suffix.startsWith("e") || suffix.startsWith("i")))
                basef = basef.substring(0, basef.Length - 1);

            // rule 3 - insert "e" if suffix is "s" and base ends in s, x, z, ch, sh
            // eg, watch+s -> watches, not watchs
            if (suffix.startsWith("s")
                && (basef.endsWith("s") || basef.endsWith("x")
                    || basef.endsWith("z") || basef.endsWith("ch") || basef
                        .endsWith("sh")))
                basef = basef + "e";

            // have made changes, now append and return
            return basef + suffix; // eg, want + s = wants
        }
    }
}
