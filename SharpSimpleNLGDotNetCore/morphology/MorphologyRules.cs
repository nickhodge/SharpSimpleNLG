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

 using System.Text;
using SimpleNLG.Extensions;

namespace SimpleNLG
{
    /**
     * <p>
     * This abstract class contains a number of rules for doing simple inflection.
     * </p>
     *
     * <p>
     * As a matter of course, the processor will first use any user-defined
     * inflection for the world. If no inflection is provided then the lexicon, if
     * it exists, will be examined for the correct inflection. Failing this a set of
     * very basic rules will be examined to inflect the word.
     * </p>
     *
     * <p>
     * All processing modules perform realisation on a tree of
     * <code>NLGElement</code>s. The modules can alter the tree in whichever way
     * they wish. For example, the syntax processor replaces phrase elements with
     * list elements consisting of inflected words while the morphology processor
     * replaces inflected words with string elements.
     * </p>
     *
     * <p>
     * <b>N.B.</b> the use of <em>module</em>, <em>processing module</em> and
     * <em>processor</em> is interchangeable. They all mean an instance of this
     * class.
     * </p>
     *
     *
     * @author D. Westwater, University of Aberdeen.
     * @version 4.0 16-Mar-2011 modified to use correct base form (ER)
     */

    public abstract class MorphologyRules : NLGModule
    {

        /**
         * A triple array of Pronouns organised by singular/plural,
         * possessive/reflexive/subjective/objective and by gender/person.
         */

        private static string[][][] PRONOUNS = new string[][][]
        { new string[][]
            {
               new string[] {"I", "you", "he", "she", "it"},
                new string[]{"me", "you", "him", "her", "it"},
                new string[]{"myself", "yourself", "himself", "herself", "itself"},
                new string[]{"mine", "yours", "his", "hers", "its"},
                new string[] {"my", "your", "his", "her", "its"}
            },
            new string[][]{
                new string[]{"we", "you", "they", "they", "they"},
                new string[]{"us", "you", "them", "them", "them"},
                new string[]{
                    "ourselves",
                    "yourselves",
                    "themselves",
                    "themselves",
                    "themselves"
                },
                new string[]{"ours", "yours", "theirs", "theirs", "theirs"},
                new string[] {"our", "your", "their", "their", "their"}
            }
        };

        private static string[] WH_PRONOUNS = {"who", "what", "which", "where", "why", "how", "how many"};

        /**
         * This method performs the morphology for nouns.
         *
         * @param element
         *            the <code>InflectedWordElement</code>.
         * @param baseWord
         *            the <code>WordElement</code> as created from the lexicon
         *            entry.
         * @return a <code>StringElement</code> representing the word after
         *         inflection.
         */

        public static StringElement doNounMorphology(InflectedWordElement element, WordElement baseWord)
        {
            var realised = new StringBuilder();

            // base form from baseWord if it exists, otherwise from element
            var baseForm = getBaseForm(element, baseWord);

            if (element.isPlural() && !element.getFeatureAsBoolean(LexicalFeature.PROPER))
            {

                string pluralForm = null;

                // AG changed: now check if default infl is uncount
                // if (element.getFeatureAsBoolean(LexicalFeature.NON_COUNT)
                // ) {
                // pluralForm = baseForm;
                var elementDefaultInfl = element.getFeature(LexicalFeature.DEFAULT_INFL);

                if (elementDefaultInfl != null && Inflection.UNCOUNT.Equals(elementDefaultInfl))
                {
                    pluralForm = baseForm;

                }
                else
                {
                    pluralForm = element.getFeatureAsString(LexicalFeature.PLURAL);
                }

                if (pluralForm == null && baseWord != null)
                {
                    // AG changed: now check if default infl is uncount
                    // if (baseWord.getFeatureAsBoolean(LexicalFeature.NON_COUNT)
                    // ) {
                    // pluralForm = baseForm;
                    var baseDefaultInfl = baseWord.getFeatureAsString(LexicalFeature.DEFAULT_INFL);
                    if (baseDefaultInfl != null && baseDefaultInfl.Equals("uncount"))
                    {
                        pluralForm = baseForm;
                    }
                    else
                    {
                        pluralForm = baseWord.getFeatureAsString(LexicalFeature.PLURAL);
                    }
                }

                if (pluralForm == null)
                {
                    var pattern = element.getFeature(LexicalFeature.DEFAULT_INFL);
                    if (Inflection.GRECO_LATIN_REGULAR.Equals(pattern))
                    {
                        pluralForm = buildGrecoLatinPluralNoun(baseForm);
                    }
                    else
                    {
                        pluralForm = buildRegularPluralNoun(baseForm);
                    }
                }
                realised.append(pluralForm);

            }
            else
            {
                realised.append(baseForm);
            }

            checkPossessive(element, realised);
            var realisedElement = new StringElement(realised.ToString());
            realisedElement.setFeature(InternalFeature.DISCOURSE_FUNCTION.ToString(),
                element.getFeature(InternalFeature.DISCOURSE_FUNCTION.ToString()));
            return realisedElement;
        }

        /**
         * Builds a plural for regular nouns. The rules are performed in this order:
         * <ul>
         * <li>For nouns ending <em>-Cy</em>, where C is any consonant, the ending
         * becomes <em>-ies</em>. For example, <em>fly</em> becomes <em>flies</em>.</li>
         * <li>For nouns ending <em>-ch</em>, <em>-s</em>, <em>-sh</em>, <em>-x</em>
         * or <em>-z</em> the ending becomes <em>-es</em>. For example, <em>box</em>
         * becomes <em>boxes</em>.</li>
         * <li>All other nouns have <em>-s</em> appended the other end. For example,
         * <em>dog</em> becomes <em>dogs</em>.</li>
         * </ul>
         *
         * @param baseForm
         *            the base form of the word.
         * @return the inflected word.
         */

        private static string buildRegularPluralNoun(string baseForm)
        {
            string plural = null;
            if (baseForm != null)
            {
                // .NET doesnt support character class subtraction the same was as Java
                // was .*[b-z-[eiou]]y\\b
                if (baseForm.matches(@".*[b-z-[eiou]]y\b"))
                {
                    
                    plural = baseForm.replaceAll(@"y\b", @"ies");  

                    //AG: changed regex from ".*[szx(ch)(sh)]\\b" (tip of the hat to Ian Tabolt)				
                }
                else if (baseForm.matches(@".*([szx]|[cs]h)\b"))
                {
                    
                    plural = baseForm + "es"; 

                }
                else
                {
                    plural = baseForm + "s"; 
                }
            }
            return plural;
        }

        /**
         * Builds a plural for Greco-Latin regular nouns. The rules are performed in
         * this order:
         * <ul>
         * <li>For nouns ending <em>-us</em> the ending becomes <em>-i</em>. For
         * example, <em>focus</em> becomes <em>foci</em>.</li>
         * <li>For nouns ending <em>-ma</em> the ending becomes <em>-mata</em>. For
         * example, <em>trauma</em> becomes <em>traumata</em>.</li>
         * <li>For nouns ending <em>-a</em> the ending becomes <em>-ae</em>. For
         * example, <em>larva</em> becomes <em>larvae</em>.</li>
         * <li>For nouns ending <em>-um</em> or <em>-on</em> the ending becomes
         * <em>-a</em>. For example, <em>taxon</em> becomes <em>taxa</em>.</li>
         * <li>For nouns ending <em>-sis</em> the ending becomes <em>-ses</em>. For
         * example, <em>analysis</em> becomes <em>analyses</em>.</li>
         * <li>For nouns ending <em>-is</em> the ending becomes <em>-ides</em>. For
         * example, <em>cystis</em> becomes <em>cystides</em>.</li>
         * <li>For nouns ending <em>-men</em> the ending becomes <em>-mina</em>. For
         * example, <em>foramen</em> becomes <em>foramina</em>.</li>
         * <li>For nouns ending <em>-ex</em> the ending becomes <em>-ices</em>. For
         * example, <em>index</em> becomes <em>indices</em>.</li>
         * <li>For nouns ending <em>-x</em> the ending becomes <em>-ces</em>. For
         * example, <em>matrix</em> becomes <em>matrices</em>.</li>
         * </ul>
         *
         * @param baseForm
         *            the base form of the word.
         * @return the inflected word.
         */

        private static string buildGrecoLatinPluralNoun(string baseForm)
        {
            string plural = null;
            if (baseForm != null)
            {
                if (baseForm.endsWith("us"))
                {
                    
                    plural = baseForm.replaceAll(@"us\b", "i");  
                }
                else if (baseForm.endsWith("ma"))
                {
                    
                    plural = baseForm + "ta"; 
                }
                else if (baseForm.endsWith("a"))
                {
                    
                    plural = baseForm + "e"; 
                }
                else if (baseForm.matches(@".*[(um)(on)]\\b"))
                {
                    
                    plural = baseForm.replaceAll(@"[(um)(on)]\\b", "a");  
                }
                else if (baseForm.endsWith("sis"))
                {
                    
                    plural = baseForm.replaceAll(@"sis\b", "ses");  
                }
                else if (baseForm.endsWith("is"))
                {
                    
                    plural = baseForm.replaceAll(@"is\b", "ides");  
                }
                else if (baseForm.endsWith("men"))
                {
                    
                    plural = baseForm.replaceAll(@"men\b", "mina");  
                }
                else if (baseForm.endsWith("ex"))
                {
                    
                    plural = baseForm.replaceAll(@"ex\b", "ices");  
                }
                else if (baseForm.endsWith("x"))
                {
                    
                    plural = baseForm.replaceAll(@"x\b", "ces");  
                }
                else
                {
                    plural = baseForm;
                }
            }
            return plural;
        }

        /**
         * This method performs the morphology for verbs.
         *
         * @param element
         *            the <code>InflectedWordElement</code>.
         * @param baseWord
         *            the <code>WordElement</code> as created from the lexicon
         *            entry.
         * @return a <code>StringElement</code> representing the word after
         *         inflection.
         */

        public static INLGElement doVerbMorphology(InflectedWordElement element, WordElement baseWord)
        {

            string realised = null;
            var numberValue = element.getFeature(Feature.NUMBER.ToString());
            var personValue = element.getFeature(Feature.PERSON.ToString());
            var tenseValue = element.getFeatureTense(Feature.TENSE.ToString());


            var formValue = element.getFeature(Feature.FORM.ToString());
            var patternValue = element.getFeature(LexicalFeature.DEFAULT_INFL);

            // base form from baseWord if it exists, otherwise from element
            var baseForm = getBaseForm(element, baseWord);

            if (element.getFeatureAsBoolean(Feature.NEGATED.ToString()) || Form.BARE_INFINITIVE.Equals(formValue))
            {
                realised = baseForm;

            }
            else if (Form.PRESENT_PARTICIPLE.Equals(formValue))
            {
                realised = element.getFeatureAsString(LexicalFeature.PRESENT_PARTICIPLE);

                if (realised == null && baseWord != null)
                {
                    realised = baseWord.getFeatureAsString(LexicalFeature.PRESENT_PARTICIPLE);
                }

                if (realised == null)
                {
                    if (Inflection.REGULAR_DOUBLE.Equals(patternValue))
                    {
                        realised = buildDoublePresPartVerb(baseForm);
                    }
                    else
                    {
                        realised = buildRegularPresPartVerb(baseForm);
                    }
                }

            }
            else if (Tense.PAST.Equals(tenseValue) || Form.PAST_PARTICIPLE.Equals(formValue))
            {

                if (Form.PAST_PARTICIPLE.Equals(formValue))
                {
                    realised = element.getFeatureAsString(LexicalFeature.PAST_PARTICIPLE);

                    if (realised == null && baseWord != null)
                    {
                        realised = baseWord.getFeatureAsString(LexicalFeature.PAST_PARTICIPLE);
                    }

                    if (realised == null)
                    {
                        if ("be".equalsIgnoreCase(baseForm))
                        {
                            
                            realised = "been"; 
                        }
                        else if (Inflection.REGULAR_DOUBLE.Equals(patternValue))
                        {
                            realised = buildDoublePastVerb(baseForm);
                        }
                        else
                        {
                            realised = buildRegularPastVerb(baseForm, numberValue, personValue);
                        }
                    }

                }
                else
                {
                    realised = element.getFeatureAsString(LexicalFeature.PAST);

                    if (realised == null && baseWord != null)
                    {
                        realised = baseWord.getFeatureAsString(LexicalFeature.PAST);
                    }

                    if (realised == null)
                    {
                        if (Inflection.REGULAR_DOUBLE.Equals(patternValue))
                        {
                            realised = buildDoublePastVerb(baseForm);
                        }
                        else
                        {
                            realised = buildRegularPastVerb(baseForm, numberValue, personValue);
                        }
                    }
                }

            }
            else if ((numberValue == null || NumberAgreement.SINGULAR.Equals(numberValue)) && (personValue == null
                                                                                               || Person.THIRD.Equals(
                                                                                                   personValue)) &&
                     (Tense.PRESENT.Equals(tenseValue)))
            {

                realised = element.getFeatureAsString(LexicalFeature.PRESENT3S);

                if (realised == null && baseWord != null && !"be".equalsIgnoreCase(baseForm))
                {
                    
                    realised = baseWord.getFeatureAsString(LexicalFeature.PRESENT3S);
                }
                if (realised == null)
                {
                    realised = buildPresent3SVerb(baseForm);
                }

            }
            else
            {
                if ("be".equalsIgnoreCase(baseForm))
                {
                    
                    if (Person.FIRST.Equals(personValue) && (NumberAgreement.SINGULAR.Equals(numberValue)
                                                             || numberValue == null))
                    {
                        realised = "am"; 
                    }
                    else
                    {
                        realised = "are"; 
                    }
                }
                else
                {
                    realised = baseForm;
                }
            }
            var realisedElement = new StringElement(realised);
            realisedElement.setFeature(InternalFeature.DISCOURSE_FUNCTION.ToString(),
                element.getFeature(InternalFeature.DISCOURSE_FUNCTION.ToString()));
            return realisedElement;
        }

        /**
         * return the base form of a word
         *
         * @param element
         * @param baseWord
         * @return
         */

        private static string getBaseForm(InflectedWordElement element, WordElement baseWord)
        {
            // unclear what the right behaviour should be
            // for now, prefer baseWord.getBaseForm() to element.getBaseForm() for
            // verbs (ie, "is" mapped to "be")
            // but prefer element.getBaseForm() to baseWord.getBaseForm() for other
            // words (ie, "children" not mapped to "child")

            // AG: changed this to get the default spelling variant
            // needed to preserve spelling changes in the VP

            if ((int)LexicalCategoryEnum.VERB == element.getCategory().enumType)
            {
                if (baseWord != null && baseWord.getDefaultSpellingVariant() != null)
                    return baseWord.getDefaultSpellingVariant();
                else
                    return element.getBaseForm();
            }
            else
            {
                if (element.getBaseForm() != null)
                    return element.getBaseForm();
                else if (baseWord == null)
                    return null;
                else
                    return baseWord.getDefaultSpellingVariant();
            }

            // if (LexicalCategory.VERB == element.getCategory()) {
            // if (baseWord != null && baseWord.getBaseForm() != null)
            // return baseWord.getBaseForm();
            // else
            // return element.getBaseForm();
            // } else {
            // if (element.getBaseForm() != null)
            // return element.getBaseForm();
            // else if (baseWord == null)
            // return null;
            // else
            // return baseWord.getBaseForm();
            // }
        }

        /**
         * Checks to see if the noun is possessive. If it is then nouns in ending in
         * <em>-s</em> become <em>-s'</em> while every other noun has <em>-'s</em> appended to
         * the end.
         *
         * @param element
         *            the <code>InflectedWordElement</code>
         * @param realised
         *            the realisation of the word.
         */

        private static void checkPossessive(InflectedWordElement element, StringBuilder realised)
        {

            if (element.getFeatureAsBoolean(Feature.POSSESSIVE.ToString()))
            {
                if (realised.charAt(realised.length() - 1) == 's')
                {
                    realised.append("\'");

                }
                else
                {
                    realised.append("'s"); 
                }
            }
        }

        /**
         * Builds the third-person singular form for regular verbs. The rules are
         * performed in this order:
         * <ul>
         * <li>If the verb is <em>be</em> the realised form is <em>is</em>.</li>
         * <li>For verbs ending <em>-ch</em>, <em>-s</em>, <em>-sh</em>, <em>-x</em>
         * or <em>-z</em> the ending becomes <em>-es</em>. For example,
         * <em>preach</em> becomes <em>preaches</em>.</li>
         * <li>For verbs ending <em>-y</em> the ending becomes <em>-ies</em>. For
         * example, <em>fly</em> becomes <em>flies</em>.</li>
         * <li>For every other verb, <em>-s</em> is added to the end of the word.</li>
         * </ul>
         *
         * @param baseForm
         *            the base form of the word.
         * @return the inflected word.
         */

        private static string buildPresent3SVerb(string baseForm)
        {
            string morphology = null;
            if (baseForm != null)
            {
                if (baseForm.equalsIgnoreCase("be"))
                {
                    
                    morphology = "is"; 
                }
                else if (baseForm.matches(@".*[szx(ch)(sh)]\\b"))
                {
                    
                    morphology = baseForm + "es"; 
                }
                else if (baseForm.matches(@".*[b-z-[eiou]]y\\b"))
                {
                    
                    morphology = baseForm.replaceAll(@"y\b", "ies");  
                }
                else
                {
                    morphology = baseForm + "s"; 
                }
            }
            return morphology;
        }

        /**
         * Builds the past-tense form for regular verbs. The rules are performed in
         * this order:
         * <ul>
         * <li>If the verb is <em>be</em> and the number agreement is plural then
         * the realised form is <em>were</em>.</li>
         * <li>If the verb is <em>be</em> and the number agreement is singular then
         * the realised form is <em>was</em>, unless the person is second, in which
         * case it's <em>were</em>.</li>
         * <li>For verbs ending <em>-e</em> the ending becomes <em>-ed</em>. For
         * example, <em>chased</em> becomes <em>chased</em>.</li>
         * <li>For verbs ending <em>-Cy</em>, where C is any consonant, the ending
         * becomes <em>-ied</em>. For example, <em>dry</em> becomes <em>dried</em>.</li>
         * <li>For every other verb, <em>-ed</em> is added to the end of the word.</li>
         * </ul>
         *
         * @param baseForm
         *            the base form of the word.
         * @param number
         *            the number agreement for the word.
         * @param person
         *            the person
         * @return the inflected word.
         */

        private static string buildRegularPastVerb(string baseForm, object number, object person)
        {
            string morphology = null;
            if (baseForm != null)
            {
                if (baseForm.equalsIgnoreCase("be"))
                {
                    
                    if (NumberAgreement.PLURAL.Equals(number))
                    {
                        morphology = "were"; 

                        // AG - bug fix to handle second person past (courtesy of
                        // Minh Le)
                    }
                    else if (Person.SECOND.Equals(person))
                    {
                        morphology = "were"; 
                    }
                    else
                    {
                        morphology = "was";
                    }
                }
                else if (baseForm.endsWith("e"))
                {
                    
                    morphology = baseForm + "d"; 
                }
                // NOTE .NET does not support character class subtraction
                // ref: http://www.rexegg.com/regex-class-operations.html#intersection_workaround
                // Java previous was: .*[b-z&&[^eiou]]y\\b
                else if (baseForm.matches(".*[b-z-[eiou]]y\\b"))
                {
                    
                    morphology = baseForm.replaceAll(@"y\b", "ied");  
                }
                else
                {
                    morphology = baseForm + "ed"; 
                }
            }
            return morphology;
        }

        /**
         * Builds the past-tense form for verbs that follow the doubling form of the
         * last consonant. <em>-ed</em> is added to the end after the last consonant
         * is doubled. For example, <em>tug</em> becomes <em>tugged</em>.
         *
         * @param baseForm
         *            the base form of the word.
         * @return the inflected word.
         */

        private static string buildDoublePastVerb(string baseForm)
        {
            string morphology = null;
            if (baseForm != null)
            {
                morphology = baseForm + baseForm.charAt(baseForm.length() - 1) + "ed"; 
            }
            return morphology;
        }

        /**
         * Builds the present participle form for regular verbs. The rules are
         * performed in this order:
         * <ul>
         * <li>If the verb is <em>be</em> then the realised form is <em>being</em>.</li>
         * <li>For verbs ending <em>-ie</em> the ending becomes <em>-ying</em>. For
         * example, <em>tie</em> becomes <em>tying</em>.</li>
         * <li>For verbs ending <em>-ee</em>, <em>-oe</em> or <em>-ye</em> then
         * <em>-ing</em> is added to the end. For example, <em>canoe</em> becomes
         * <em>canoeing</em>.</li>
         * <li>For other verbs ending in <em>-e</em> the ending becomes
         * <em>-ing</em>. For example, <em>chase</em> becomes <em>chasing</em>.</li>
         * <li>For all other verbs, <em>-ing</em> is added to the end. For example,
         * <em>dry</em> becomes <em>drying</em>.</li>
         * </ul>
         *
         * @param baseForm
         *            the base form of the word.
         * @return the inflected word.
         */

        private static string buildRegularPresPartVerb(string baseForm)
        {
            string morphology = null;
            if (baseForm != null)
            {
                if (baseForm.equalsIgnoreCase("be"))
                {
                    
                    morphology = "being"; 
                }
                else if (baseForm.endsWith("ie"))
                {
                    
                    morphology = baseForm.replaceAll(@"ie\b", "ying");  
                }
                else if (baseForm.matches(".*[^iyeo]e\\b"))
                {
                    
                    morphology = baseForm.replaceAll(@"e\b", "ing");  
                }
                else
                {
                    morphology = baseForm + "ing"; 
                }
            }
            return morphology;
        }

        /**
         * Builds the present participle form for verbs that follow the doubling
         * form of the last consonant. <em>-ing</em> is added to the end after the
         * last consonant is doubled. For example, <em>tug</em> becomes
         * <em>tugging</em>.
         *
         * @param baseForm
         *            the base form of the word.
         * @return the inflected word.
         */

        private static string buildDoublePresPartVerb(string baseForm)
        {
            string morphology = null;
            if (baseForm != null)
            {
                morphology = baseForm + baseForm.charAt(baseForm.length() - 1) + "ing"; 
            }
            return morphology;
        }

        /**
         * This method performs the morphology for adjectives.
         *
         * @param element
         *            the <code>InflectedWordElement</code>.
         * @param baseWord
         *            the <code>WordElement</code> as created from the lexicon
         *            entry.
         * @return a <code>StringElement</code> representing the word after
         *         inflection.
         */

        public static INLGElement doAdjectiveMorphology(InflectedWordElement element, WordElement baseWord)
        {

            string realised = null;
            var patternValue = element.getFeature(LexicalFeature.DEFAULT_INFL);

            // base form from baseWord if it exists, otherwise from element
            var baseForm = getBaseForm(element, baseWord);

            if (element.getFeatureAsBoolean(Feature.IS_COMPARATIVE.ToString()))
            {
                realised = element.getFeatureAsString(LexicalFeature.COMPARATIVE);

                if (realised == null && baseWord != null)
                {
                    realised = baseWord.getFeatureAsString(LexicalFeature.COMPARATIVE);
                }
                if (realised == null)
                {
                    if (Inflection.REGULAR_DOUBLE.Equals(patternValue))
                    {
                        realised = buildDoubleCompAdjective(baseForm);
                    }
                    else
                    {
                        realised = buildRegularComparative(baseForm);
                    }
                }
            }
            else if (element.getFeatureAsBoolean(Feature.IS_SUPERLATIVE.ToString()))
            {

                realised = element.getFeatureAsString(LexicalFeature.SUPERLATIVE);

                if (realised == null && baseWord != null)
                {
                    realised = baseWord.getFeatureAsString(LexicalFeature.SUPERLATIVE);
                }
                if (realised == null)
                {
                    if (Inflection.REGULAR_DOUBLE.Equals(patternValue))
                    {
                        realised = buildDoubleSuperAdjective(baseForm);
                    }
                    else
                    {
                        realised = buildRegularSuperlative(baseForm);
                    }
                }
            }
            else
            {
                realised = baseForm;
            }
            var realisedElement = new StringElement(realised);
            realisedElement.setFeature(InternalFeature.DISCOURSE_FUNCTION.ToString(),
                element.getFeature(InternalFeature.DISCOURSE_FUNCTION.ToString()));
            return realisedElement;
        }

        /**
         * Builds the comparative form for adjectives that follow the doubling form
         * of the last consonant. <em>-er</em> is added to the end after the last
         * consonant is doubled. For example, <em>fat</em> becomes <em>fatter</em>.
         *
         * @param baseForm
         *            the base form of the word.
         * @return the inflected word.
         */

        private static string buildDoubleCompAdjective(string baseForm)
        {
            string morphology = null;
            if (baseForm != null)
            {
                morphology = baseForm + baseForm.charAt(baseForm.length() - 1) + "er"; 
            }
            return morphology;
        }

        /**
         * Builds the comparative form for regular adjectives. The rules are
         * performed in this order:
         * <ul>
         * <li>For adjectives ending <em>-Cy</em>, where C is any consonant, the
         * ending becomes <em>-ier</em>. For example, <em>brainy</em> becomes
         * <em>brainier</em>.</li>
         * <li>For adjectives ending <em>-e</em> the ending becomes <em>-er</em>.
         * For example, <em>fine</em> becomes <em>finer</em>.</li>
         * <li>For all other adjectives, <em>-er</em> is added to the end. For
         * example, <em>clear</em> becomes <em>clearer</em>.</li>
         * </ul>
         *
         * @param baseForm
         *            the base form of the word.
         * @return the inflected word.
         */

        private static string buildRegularComparative(string baseForm)
        {
            string morphology = null;
            if (baseForm != null)
            {
                if (baseForm.matches(@".*[b-z-[eiou]]y\\b"))
                {
                    
                    morphology = baseForm.replaceAll(@"y\b", "ier");  
                }
                else if (baseForm.endsWith("e"))
                {
                    
                    morphology = baseForm + "r"; 
                }
                else
                {
                    morphology = baseForm + "er"; 
                }
            }
            return morphology;
        }

        /**
         * Builds the superlative form for adjectives that follow the doubling form
         * of the last consonant. <em>-est</em> is added to the end after the last
         * consonant is doubled. For example, <em>fat</em> becomes <em>fattest</em>.
         *
         * @param baseForm
         *            the base form of the word.
         * @return the inflected word.
         */

        private static string buildDoubleSuperAdjective(string baseForm)
        {
            string morphology = null;
            if (baseForm != null)
            {
                morphology = baseForm + baseForm.charAt(baseForm.length() - 1) + "est"; 
            }
            return morphology;
        }

        /**
         * Builds the superlative form for regular adjectives. The rules are
         * performed in this order:
         * <ul>
         * <li>For verbs ending <em>-Cy</em>, where C is any consonant, the ending
         * becomes <em>-iest</em>. For example, <em>brainy</em> becomes
         * <em>brainiest</em>.</li>
         * <li>For verbs ending <em>-e</em> the ending becomes <em>-est</em>. For
         * example, <em>fine</em> becomes <em>finest</em>.</li>
         * <li>For all other verbs, <em>-est</em> is added to the end. For example,
         * <em>clear</em> becomes <em>clearest</em>.</li>
         * </ul>
         *
         * @param baseForm
         *            the base form of the word.
         * @return the inflected word.
         */

        private static string buildRegularSuperlative(string baseForm)
        {
            string morphology = null;
            if (baseForm != null)
            {
                if (baseForm.matches(@".*[b-z-[eiou]]y\\b"))
                {
                    
                    morphology = baseForm.replaceAll(@"y\b", "iest");  
                }
                else if (baseForm.endsWith("e"))
                {
                    
                    morphology = baseForm + "st"; 
                }
                else
                {
                    morphology = baseForm + "est"; 
                }
            }
            return morphology;
        }

        /**
         * This method performs the morphology for adverbs.
         *
         * @param element
         *            the <code>InflectedWordElement</code>.
         * @param baseWord
         *            the <code>WordElement</code> as created from the lexicon
         *            entry.
         * @return a <code>StringElement</code> representing the word after
         *         inflection.
         */

        public static INLGElement doAdverbMorphology(InflectedWordElement element, WordElement baseWord)
        {

            string realised = null;

            // base form from baseWord if it exists, otherwise from element
            var baseForm = getBaseForm(element, baseWord);

            if (element.getFeatureAsBoolean(Feature.IS_COMPARATIVE.ToString()))
            {
                realised = element.getFeatureAsString(LexicalFeature.COMPARATIVE);

                if (realised == null && baseWord != null)
                {
                    realised = baseWord.getFeatureAsString(LexicalFeature.COMPARATIVE);
                }
                if (realised == null)
                {
                    realised = buildRegularComparative(baseForm);
                }
            }
            else if (element.getFeatureAsBoolean(Feature.IS_SUPERLATIVE.ToString()))
            {

                realised = element.getFeatureAsString(LexicalFeature.SUPERLATIVE);

                if (realised == null && baseWord != null)
                {
                    realised = baseWord.getFeatureAsString(LexicalFeature.SUPERLATIVE);
                }
                if (realised == null)
                {
                    realised = buildRegularSuperlative(baseForm);
                }
            }
            else
            {
                realised = baseForm;
            }
            var realisedElement = new StringElement(realised);
            realisedElement.setFeature(InternalFeature.DISCOURSE_FUNCTION.ToString(),
                element.getFeature(InternalFeature.DISCOURSE_FUNCTION.ToString()));
            return realisedElement;
        }

        /**
         * This method performs the morphology for pronouns.
         *
         * @param element
         *            the <code>InflectedWordElement</code>.
         * @return a <code>StringElement</code> representing the word after
         *         inflection.
         */

        public static INLGElement doPronounMorphology(InflectedWordElement element)
        {
            string realised = null;

            if (!element.getFeatureAsBoolean(InternalFeature.NON_MORPH.ToString()) && !isWHPronoun(element))
            {
                var genderValue = element.getFeature(LexicalFeature.GENDER);
                var personValue = element.getFeature(Feature.PERSON.ToString());
                var discourseValue = element.getFeature(InternalFeature.DISCOURSE_FUNCTION.ToString());

                var numberIndex = element.isPlural() ? 1 : 0;
                var genderIndex = (genderValue is Gender) ? ((Gender) genderValue).ordinal() : 2;

                var personIndex = (personValue is Person) ? ((Person) personValue).ordinal() : 2;

                if (personIndex == 2)
                {
                    personIndex += genderIndex;
                }

                var positionIndex = 0;

                if (element.getFeatureAsBoolean(LexicalFeature.REFLEXIVE))
                {
                    positionIndex = 2;
                }
                else if (element.getFeatureAsBoolean(Feature.POSSESSIVE.ToString()))
                {
                    positionIndex = 3;
                    if (DiscourseFunction.SPECIFIER.Equals(discourseValue))
                    {
                        positionIndex++;
                    }
                }
                else
                {
                    positionIndex = (DiscourseFunction.SUBJECT.Equals(discourseValue) && !element.getFeatureAsBoolean(
                                         Feature.PASSIVE.ToString())) ||
                                    (DiscourseFunction.OBJECT.Equals(discourseValue) &&
                                     element.getFeatureAsBoolean(Feature.PASSIVE.ToString()))
                                    || DiscourseFunction.SPECIFIER.Equals(discourseValue) || (
                                        DiscourseFunction.COMPLEMENT.Equals(discourseValue)
                                        && element.getFeatureAsBoolean(Feature.PASSIVE.ToString()))
                        ? 0
                        : 1;
                }
                realised = PRONOUNS[numberIndex][positionIndex][personIndex];
            }
            else
            {
                realised = element.getBaseForm();
            }
            var realisedElement = new StringElement(realised);
            realisedElement.setFeature(InternalFeature.DISCOURSE_FUNCTION.ToString(),
                element.getFeature(InternalFeature.DISCOURSE_FUNCTION.ToString()));

            return realisedElement;
        }

        private static bool isWHPronoun(InflectedWordElement word)
        {
            var bases = word.getBaseForm();
            var wh = false;

            if (bases != null)
            {
                for (var i = 0; i < WH_PRONOUNS.Length && !wh; i++)
                {
                    wh = WH_PRONOUNS[i].Equals(bases);
                }
            }

            return wh;

        }

        /**
         * This method performs the morphology for determiners.
         *
         * @param determiner
         *            the <code>InflectedWordElement</code>.
         * @param realisation
         *            the current realisation of the determiner.
         */

        public static void doDeterminerMorphology(INLGElement determiner, string realisation)
        {

            if (realisation != null)
            {

                if (!(determiner.getRealisation().Equals("a")))
                {
                    if (determiner.isPlural())
                    {
                        // Use default inflection rules:
                        if ("that".Equals(determiner.getRealisation()))
                        {
                            determiner.setRealisation("those");
                        }
                        else if ("this".Equals(determiner.getRealisation()))
                        {
                            determiner.setRealisation("these");
                        }
                    }
                    else if (!determiner.isPlural())
                    {
                        // Use default push back to base form rules:
                        if ("those".Equals(determiner.getRealisation()))
                        {
                            determiner.setRealisation("that");
                        }
                        else if ("these".Equals(determiner.getRealisation()))
                        {
                            determiner.setRealisation("this");
                        }

                    }
                }

                // Special "a" determiner and perform a/an agreement:
                if (determiner.getRealisation().Equals("a"))
                {
                    
                    if (determiner.isPlural())
                    {
                        determiner.setRealisation("some");
                    }
                    else if (DeterminerAgrHelper.requiresAn(realisation))
                    {
                        determiner.setRealisation("an");
                    }
                }

            }
        }
    }
}