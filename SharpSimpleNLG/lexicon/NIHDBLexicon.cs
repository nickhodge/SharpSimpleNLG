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

namespace SimpleNLG
{
    public class NIHDBLexicon : Lexicon
    {

        // default DB parameters
        private static string DB_HSQL_DRIVER = "org.hsqldb.jdbc.JDBCDriver"; // DB driver
        private static string DB_HQSL_JDBC = "jdbc:hsqldb:"; // JDBC specifier for
        // HSQL
        private static string DB_DEFAULT_USERNAME = "sa"; // DB username
        private static string DB_DEFAULT_PASSWORD = ""; // DB password
        private static string DB_HSQL_EXTENSION = ".data"; // filename extension for
        // HSQL DB

        // class variables
        private Connection conn = null; // DB connection
        private LexAccessApi lexdb = null; // Lexicon access object

        // if false, don't keep standard inflections in the Word object
        private bool keepStandardInflections = false;

        /****************************************************************************/
        // constructors
        /****************************************************************************/

        /**
         * set up lexicon using file which contains downloaded lexAccess HSQL DB and
         * default passwords
         * 
         * @param filename
         *            of HSQL DB
         */

        public NIHDBLexicon(string filename) : base()
        {

            // get rid of .data at end of filename if necessary
            var dbfilename = filename;
            if (dbfilename.endsWith(DB_HSQL_EXTENSION))
                dbfilename = dbfilename.substring(0, dbfilename.length()
                                                     - DB_HSQL_EXTENSION.length());

            // try to open DB and set up lexicon
            try
            {
                Class.forName(DB_HSQL_DRIVER);
                conn = DriverManager.getConnection(DB_HQSL_JDBC + dbfilename,
                    DB_DEFAULT_USERNAME, DB_DEFAULT_PASSWORD);
                // now set up lexical access object
                lexdb = new LexAccessApi(conn);

            }
            catch (Exception ex)
            {
                Console.WriteLine("Cannot open lexical db: " + ex.ToString());
                // probably should thrown an exception
            }
        }

        /**
         * set up lexicon using general DB parameters; DB must be NIH specialist
         * lexicon from lexAccess
         * 
         * @param driver
         * @param url
         * @param username
         * @param password
         */

        public NIHDBLexicon(string driver, string url, string username,
            string password)
        {
            // try to open DB and set up lexicon
            try
            {
                Class.forName(driver);
                conn = DriverManager.getConnection(url, username, password);
                // now set up lexical access object
                lexdb = new LexAccessApi(conn);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Cannot open lexical db: " + ex.ToString());
                // probably should thrown an exception
            }
        }

        // need more constructors for general case...

        /***************** methods to set global parameters ****************************/

        /**
         * reports whether Words include standard (derivable) inflections
         * 
         * @return true if standard inflections are kept
         */

        public bool isKeepStandardInflections()
        {
            return keepStandardInflections;
        }

        /**
         * set whether Words should include standard (derivable) inflections
         * 
         * @param keepStandardInflections
         *            - if true, standard inflections are kept
         */

        public void setKeepStandardInflections(bool keepStandardInflections)
        {
            this.keepStandardInflections = keepStandardInflections;
        }

        /****************************************************************************/
        // core methods to retrieve words from DB
        /****************************************************************************/

        /*
         * (non-Javadoc)
         * 
         * @see simplenlg.lexicon.Lexicon#getWords(java.lang.string,
         * simplenlg.features.LexicalCategory)
         */

        public List<WordElement> getWords(string baseForm, LexicalCategory category)
        {
            // get words from DB
            try
            {
                LexAccessApiResult lexResult = lexdb.GetLexRecordsByBase(baseForm,
                    LexAccessApi.B_EXACT);
                return getWordsFromLexResult(category, lexResult);
            }
            catch (SQLException ex)
            {
                Console.WriteLine("Lexical DB error: " + ex.ToString());
                // probably should thrown an exception
            }
            return null;
        }

        /*
         * (non-Javadoc)
         * 
         * @see simplenlg.lexicon.Lexicon#getWordsByID(java.lang.string)
         */

        public List<WordElement> getWordsByID(string id)
        {
            // get words from DB
            try
            {
                LexAccessApiResult lexResult = lexdb.GetLexRecords(id);
                return getWordsFromLexResult(new LexicalCategory_ANY(), lexResult);
            }
            catch (SQLException ex)
            {
                Console.WriteLine("Lexical DB error: " + ex.ToString());
                // probably should thrown an exception
            }
            return null;
        }

        /*
         * (non-Javadoc)
         * 
         * @see simplenlg.lexicon.Lexicon#getWordsFromVariant(java.lang.string,
         * simplenlg.features.LexicalCategory)
         */

        public List<WordElement> getWordsFromVariant(string variant,
            LexicalCategory category)
        {
            // get words from DB
            try
            {
                LexAccessApiResult lexResult = lexdb.GetLexRecords(variant);
                return getWordsFromLexResult(category, lexResult);
            }
            catch (SQLException ex)
            {
                Console.WriteLine("Lexical DB error: " + ex.ToString());
                // probably should thrown an exception
            }
            return null;
        }

        /****************************************************************************/
        // other methods
        /****************************************************************************/

        /*
         * (non-Javadoc)
         * 
         * @see simplenlg.lexicon.Lexicon#close()
         */

        public void close()
        {
            if (lexdb != null)
                lexdb.CleanUp();
        }

        /**
         * make a WordElement from a lexical record. Currently just specifies basic
         * params and inflections Should do more in the future!
         * 
         * @param record
         * @return
         */

        private WordElement makeWord(LexRecord record)
        {
            // get basic data
            string baseForm = record.GetBase();
            ILexicalCategory category = getSimplenlgCategory(record);
            string id = record.GetEui();

            // create word class
            var wordElement = new WordElement(baseForm, (LexicalCategory)category, id);

            // now add type information
            switch (category.lexType)
            {
                case LexicalCategoryEnum.ADJECTIVE:
                    addAdjectiveInfo(wordElement, record.GetCatEntry().GetAdjEntry());
                    break;
                case LexicalCategoryEnum.ADVERB:
                    addAdverbInfo(wordElement, record.GetCatEntry().GetAdvEntry());
                    break;
                case LexicalCategoryEnum.NOUN:
                    addNounInfo(wordElement, record.GetCatEntry().GetNounEntry());
                    break;
                case LexicalCategoryEnum.VERB:
                    addVerbInfo(wordElement, record.GetCatEntry().GetVerbEntry());
                    break;
                // ignore closed class words
            }

            var defaultInfl = (Inflection) wordElement
                .getDefaultInflectionalVariant();

            // now add inflected forms
            // if (keepStandardInflections || !standardInflections(record,
            // category)) {
            foreach (InflVar inflection in record.GetInflVarsAndAgreements()
                .GetInflValues())
            {
                string simplenlgInflection = getSimplenlgInflection(inflection
                    .GetInflection());

                if (simplenlgInflection != null)
                {
                    string inflectedForm = inflection.GetVar();
                    Inflection inflType = Inflection.getInflCode(inflection
                        .GetType());

                    // store all inflectional variants, except for regular ones
                    // unless explicitly set
                    if (inflType != null
                        && !(Inflection.REGULAR.Equals(inflType) && !this.keepStandardInflections))
                    {
                        wordElement.addInflectionalVariant(inflType,
                            simplenlgInflection, inflectedForm);
                    }

                    // if the infl variant is the default, also set this feature on
                    // the word
                    if (defaultInfl == null
                        || (defaultInfl.Equals(inflType) && !(Inflection.REGULAR
                                                                  .Equals(inflType) && !this.keepStandardInflections)))
                    {
                        wordElement.setFeature(simplenlgInflection, inflectedForm);
                    }

                    // wordElement
                    // .setFeature(simplenlgInflection, inflection.GetVar());
                }
            }
            // }

            // add acronym info
            addAcronymInfo(wordElement, record);

            // now add spelling variants
            addSpellingVariants(wordElement, record);

            return wordElement;
        }

        /**
         * return list of WordElement from LexAccessApiResult
         * 
         * @param category
         *            - desired category (eg, NOUN) (this filters list)
         * @param lexResult
         *            - the LexAccessApiResult
         * @return list of WordElement
         */

        private List<WordElement> getWordsFromLexResult(LexicalCategory category,
            LexAccessApiResult lexResult)
        {
            List<LexRecord> records = lexResult.GetJavaObjs();

            // set up array of words to return
            var wordElements = new List<WordElement>();

            // iterate through result records, adding to words as appropriate
            foreach (LexRecord record in records)
            {

                if (category == LexicalCategory.ANY
                    || category == getSimplenlgCategory(record))
                    wordElements.add(makeWord(record));
            }
            return wordElements;
        }

        /**
         * check if this record has a standard (regular) inflection
         * 
         * @param record
         * @param simplenlg
         *            syntactic category
         * @return true if standard (regular) inflection
         */

        private bool standardInflections(LexRecord record, LexicalCategory category)
        {
            List<string> variants = null;
            switch (((LexicalCategory)category).lexType)
            {
                case LexicalCategoryEnum.NOUN:
                    variants = record.GetCatEntry().GetNounEntry().GetVariants();
                    break;
                case LexicalCategoryEnum.ADJECTIVE:
                    variants = record.GetCatEntry().GetAdjEntry().GetVariants();
                    break;
                case LexicalCategoryEnum.ADVERB:
                    variants = record.GetCatEntry().GetAdvEntry().GetVariants();
                    break;
                case LexicalCategoryEnum.MODAL:
                    variants = record.GetCatEntry().GetModalEntry().GetVariant();
                    break;
                case LexicalCategoryEnum.VERB:
                    if (record.GetCatEntry().GetVerbEntry() != null) // aux verbs (eg
                        // be) won't
                        // have verb
                        // entries
                        variants = record.GetCatEntry().GetVerbEntry().GetVariants();
                    break;
            }

            return notEmpty(variants) && variants.contains("reg");
        }

        /***********************************************************************************/
        // The following methods map codes in the NIH Specialist Lexicon
        // into the codes used in simplenlg
        /***********************************************************************************/

        /**
         * get the simplenlg LexicalCategory of a record
         * 
         * @param cat
         * @return
         */

        private LexicalCategory getSimplenlgCategory(LexRecord record)
        {
            string cat = record.GetCategory();
            if (cat == null)
                return new LexicalCategory_ANY();
            else if (cat.equalsIgnoreCase("noun"))
                return new LexicalCategory_NOUN();
            else if (cat.equalsIgnoreCase("verb"))
                return new LexicalCategory_VERB();
            else if (cat.equalsIgnoreCase("aux")
                     && record.GetBase().equalsIgnoreCase("be")) // return aux "be"
                // as a VERB
                // not needed for other aux "have" and "do", they have a verb entry
                return new LexicalCategory_VERB();
            else if (cat.equalsIgnoreCase("adj"))
                return new LexicalCategory_ADJECTIVE();
            else if (cat.equalsIgnoreCase("adv"))
                return new LexicalCategory_ADVERB();
            else if (cat.equalsIgnoreCase("pron"))
                return new LexicalCategory_PRONOUN();
            else if (cat.equalsIgnoreCase("det"))
                return new LexicalCategory_DETERMINER();
            else if (cat.equalsIgnoreCase("prep"))
                return new LexicalCategory_PREPOSITION();
            else if (cat.equalsIgnoreCase("conj"))
                return new LexicalCategory_CONJUNCTION();
            else if (cat.equalsIgnoreCase("compl"))
                return new LexicalCategory_COMPLEMENTISER();
            else if (cat.equalsIgnoreCase("modal"))
                return new LexicalCategory_MODAL();

            // return ANY for other cats
            else
                return new LexicalCategory_ANY();
        }

        /**
         * convert an inflection type in NIH lexicon into one used by simplenlg
         * return null if no simplenlg equivalent to NIH inflection type
         * 
         * @param NIHInflection
         *            - inflection type in NIH lexicon
         * @return inflection type in simplenlg
         */

        private string getSimplenlgInflection(string NIHInflection)
        {
            if (NIHInflection == null)
                return null;
            if (NIHInflection.equalsIgnoreCase("comparative"))
                return LexicalFeature.COMPARATIVE;
            else if (NIHInflection.equalsIgnoreCase("superlative"))
                return LexicalFeature.SUPERLATIVE;
            else if (NIHInflection.equalsIgnoreCase("plural"))
                return LexicalFeature.PLURAL;
            else if (NIHInflection.equalsIgnoreCase("pres3s"))
                return LexicalFeature.PRESENT3S;
            else if (NIHInflection.equalsIgnoreCase("past"))
                return LexicalFeature.PAST;
            else if (NIHInflection.equalsIgnoreCase("pastPart"))
                return LexicalFeature.PAST_PARTICIPLE;
            else if (NIHInflection.equalsIgnoreCase("presPart"))
                return LexicalFeature.PRESENT_PARTICIPLE;
            else
                // no equvalent in simplenlg, eg clitic or negative
                return null;
        }

        /**
         * extract adj information from NIH AdjEntry record, and add to a simplenlg
         * WordElement For now just extract position info
         * 
         * @param wordElement
         * @param AdjEntry
         */

        private void addAdjectiveInfo(WordElement wordElement, AdjEntry adjEntry)
        {
            var qualitativeAdj = false;
            var colourAdj = false;
            var classifyingAdj = false;
            var predicativeAdj = false;
            List<string> positions = adjEntry.GetPosition();
            foreach (var position in positions)
            {
                if (position.startsWith("attrib(1)"))
                    qualitativeAdj = true;
                else if (position.startsWith("attrib(2)"))
                    colourAdj = true;
                else if (position.startsWith("attrib(3)"))
                    classifyingAdj = true;
                else if (position.startsWith("pred"))
                    predicativeAdj = true;
                // ignore other positions
            }
            // ignore (for now) other info in record
            wordElement.setFeature(LexicalFeature.QUALITATIVE, qualitativeAdj);
            wordElement.setFeature(LexicalFeature.COLOUR, colourAdj);
            wordElement.setFeature(LexicalFeature.CLASSIFYING, classifyingAdj);
            wordElement.setFeature(LexicalFeature.PREDICATIVE, predicativeAdj);
            return;
        }

        /**
         * extract adv information from NIH AdvEntry record, and add to a simplenlg
         * WordElement For now just extract modifier type
         * 
         * @param wordElement
         * @param AdvEntry
         */

        private void addAdverbInfo(WordElement wordElement, AdvEntry advEntry)
        {
            var verbModifier = false;
            var sentenceModifier = false;
            var intensifier = false;

            List<string> modifications = advEntry.GetModification();
            foreach (var modification in modifications)
            {
                if (modification.startsWith("verb_modifier"))
                    verbModifier = true;
                else if (modification.startsWith("sentence_modifier"))
                    sentenceModifier = true;
                else if (modification.startsWith("intensifier"))
                    intensifier = true;
                // ignore other modification types
            }
            // ignore (for now) other info in record
            wordElement.setFeature(LexicalFeature.VERB_MODIFIER, verbModifier);
            wordElement.setFeature(LexicalFeature.SENTENCE_MODIFIER,
                sentenceModifier);
            wordElement.setFeature(LexicalFeature.INTENSIFIER, intensifier);
            return;
        }

        /**
         * extract noun information from NIH NounEntry record, and add to a
         * simplenlg WordElement For now just extract whether count/non-count and
         * whether proper or not
         * 
         * @param wordElement
         * @param nounEntry
         */

        private void addNounInfo(WordElement wordElement, NounEntry nounEntry)
        {
            bool proper = nounEntry.IsProper();
            // bool nonCountVariant = false;
            // bool regVariant = false;

            // add the inflectional variants
            List<string> variants = nounEntry.GetVariants();

            if (!variants.isEmpty())
            {
                var wordVariants = new List<Inflection>();

                foreach (var v in variants)
                {
                    int index = v.indexOf("|");
                    string code;

                    if (index > -1)
                    {
                        code = v.substring(0, index).toLowerCase().trim();

                    }
                    else
                    {
                        code = v.toLowerCase().trim();
                    }

                    Inflection infl = Inflection.getInflCode(code);

                    if (infl != null)
                    {
                        wordVariants.add(infl);
                        wordElement.addInflectionalVariant(infl);
                    }
                }

                // if the variants include "reg", this is the default, otherwise
                // just a random pick
                Inflection defaultVariant = wordVariants
                                                .Contains(Inflection.REGULAR)
                                            || wordVariants.isEmpty()
                    ? Inflection.REGULAR
                    : wordVariants.get(0);
                wordElement.setFeature(LexicalFeature.DEFAULT_INFL, defaultVariant);
                wordElement.setDefaultInflectionalVariant(defaultVariant);
            }

            // for (string variant : variants) {
            // if (variant.startsWith("uncount")
            // || variant.startsWith("groupuncount"))
            // nonCountVariant = true;
            //
            // if (variant.startsWith("reg"))
            // regVariant = true;
            // // ignore other variant info
            // }

            // lots of words have both "reg" and "unCount", indicating they
            // can be used in either way. Regard such words as normal,
            // only flag as nonCount if unambiguous
            // wordElement.setFeature(LexicalFeature.NON_COUNT, nonCountVariant
            // && !regVariant);
            wordElement.setFeature(LexicalFeature.PROPER, proper);
            // ignore (for now) other info in record

            return;
        }

        /**
         * extract verb information from NIH VerbEntry record, and add to a
         * simplenlg WordElement For now just extract transitive, instransitive,
         * and/or ditransitive
         * 
         * @param wordElement
         * @param verbEntry
         */

        private void addVerbInfo(WordElement wordElement, VerbEntry verbEntry)
        {
            if (verbEntry == null)
            {
                // should only happen for aux verbs, which have
                // auxEntry instead of verbEntry in NIH Lex
                // just flag as transitive and return
                wordElement.setFeature(LexicalFeature.INTRANSITIVE, false);
                wordElement.setFeature(LexicalFeature.TRANSITIVE, true);
                wordElement.setFeature(LexicalFeature.DITRANSITIVE, false);
                return;
            }

            bool intransitiveVerb = notEmpty(verbEntry.GetIntran());
            bool transitiveVerb = notEmpty(verbEntry.GetTran())
                                  || notEmpty(verbEntry.GetCplxtran());
            bool ditransitiveVerb = notEmpty(verbEntry.GetDitran());

            wordElement.setFeature(LexicalFeature.INTRANSITIVE, intransitiveVerb);
            wordElement.setFeature(LexicalFeature.TRANSITIVE, transitiveVerb);
            wordElement.setFeature(LexicalFeature.DITRANSITIVE, ditransitiveVerb);

            // add the inflectional variants
            List<string> variants = verbEntry.GetVariants();

            if (!variants.isEmpty())
            {
                var wordVariants = new List<Inflection>();

                foreach (var v in variants)
                {
                    int index = v.indexOf("|");
                    string code;
                    Inflection infl;

                    if (index > -1)
                    {
                        code = v.substring(0, index).toLowerCase().trim();
                        infl = Inflection.getInflCode(code);

                    }
                    else
                    {
                        infl = Inflection.getInflCode(v.toLowerCase().trim());
                    }

                    if (infl != null)
                    {
                        wordElement.addInflectionalVariant(infl);
                        wordVariants.add(infl);
                    }
                }

                // if the variants include "reg", this is the default, otherwise
                // just a random pick
                Inflection defaultVariant = wordVariants
                                                .contains(Inflection.REGULAR)
                                            || wordVariants.isEmpty()
                    ? Inflection.REGULAR
                    : wordVariants.get(0);
//			wordElement.setFeature(LexicalFeature.INFLECTIONS, wordVariants);
//			wordElement.setFeature(LexicalFeature.DEFAULT_INFL, defaultVariant);
                wordElement.setDefaultInflectionalVariant(defaultVariant);
            }

            // ignore (for now) other info in record
            return;
        }

        /**
         * convenience method to test that a list is not null and not empty
         * 
         * @param list
         * @return
         */

        private bool notEmpty<T>(List<T> list)
        {
            return list != null && !list.isEmpty();
        }

        /**
         * extract information about acronyms from NIH record, and add to a
         * simplenlg WordElement.
         * 
         * <P>
         * Acronyms are represented as lists of word elements. Any acronym will have
         * a list of full form word elements, retrievable via
         * {@link LexicalFeature#ACRONYM_OF}
         * 
         * @param wordElement
         * @param record
         */

        private void addAcronymInfo(WordElement wordElement, LexRecord record)
        {
            // NB: the acronyms are actually the full forms of which the word is an
            // acronym
            List<string> acronyms = record.GetAcronyms();

            if (!acronyms.isEmpty())
            {
                // the list of full forms of which this word is an acronym
                List<INLGElement> acronymOf = wordElement
                    .getFeatureAsElementList(LexicalFeature.ACRONYM_OF);

                // keep all acronym full forms and set them up as wordElements
                foreach (var fullForm in acronyms)
                {
                    if (fullForm.contains("|"))
                    {
                        // get the acronym id
                        string acronymID = fullForm.substring(
                            fullForm.indexOf("|") + 1, fullForm.length());
                        // create the full form element
                        WordElement fullFormWE = this.getWordByID(acronymID);

                        if (fullForm != null)
                        {
                            // add as full form of this acronym
                            acronymOf.add(fullFormWE);

                            // List<NLGElement> fullFormAcronyms = fullFormWE
                            // .getFeatureAsElementList(LexicalFeature.ACRONYMS);
                            // fullFormAcronyms.add(wordElement);
                            // fullFormWE.setFeature(LexicalFeature.ACRONYMS,
                            // fullFormAcronyms);
                        }
                    }
                }

                // set all the full forms for this acronym
                wordElement.setFeature(LexicalFeature.ACRONYM_OF, acronymOf);
            }

            // if (!acronyms.isEmpty()) {
            //
            // string acronym = acronyms.get(0);
            // // remove anything after a |, this will be an NIH ID
            // if (acronym.contains("|"))
            // acronym = acronym.substring(0, acronym.indexOf("|"));
            // wordElement.setFeature(LexicalFeature.ACRONYM_OF, acronym);
            // }

            return;
        }

        /**
         * Extract info about the spelling variants of a word from an NIH record,
         * and add to the simplenlg Woordelement.
         * 
         * <P>
         * Spelling variants are represented as lists of strings, retrievable via
         * {@link LexicalFeature#SPELL_VARS}
         * 
         * @param wordElement
         * @param record
         */

        private void addSpellingVariants(WordElement wordElement, LexRecord record)
        {
            Vector<string> vars = record.GetSpellingVars();

            if (vars != null && !vars.isEmpty())
            {
                var wordVars = new List<string>();
                wordVars.addAll(vars);
                wordElement.setFeature(LexicalFeature.SPELL_VARS, wordVars);
            }

            // we set the default spelling var as the baseForm
            wordElement.setFeature(LexicalFeature.DEFAULT_SPELL, wordElement
                .getBaseForm());
        }

    }
    
}