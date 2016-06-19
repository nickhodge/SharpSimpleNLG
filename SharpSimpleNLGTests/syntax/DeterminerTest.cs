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

using NUnit.Framework;

namespace SimpleNLGTests.syntax
{

    /**
     * Some determiner tests -- in particular for indefinite articles like "a" or "an".
     *
     * @author Saad Mahamood, Data2Text Limited.
     *
     */

    public class DeterminerTest : SimpleNLG4TestBase
    {

        /**
         * testLowercaseConstant - Test for when there is a lower case constant
         */

        [Test]
        public void testLowercaseConstant()
        {

            var sentence = this.phraseFactory.createClause();

            var subject = this.phraseFactory.createNounPhrase("a", "dog");
            sentence.setSubject(subject);

            var output = this.realiser.realiseSentence(sentence);

            Assert.AreEqual("A dog.", output);
        }

        /**
         * testLowercaseVowel - Test for "an" as a specifier.
         */

        [Test]
        public void testLowercaseVowel()
        {
            var sentence = this.phraseFactory.createClause();

            var subject = this.phraseFactory.createNounPhrase("a", "owl");
            sentence.setSubject(subject);

            var output = this.realiser.realiseSentence(sentence);

            Assert.AreEqual("An owl.", output);
        }

        /**
         * testUppercaseConstant - Test for when there is a upper case constant
         */

        [Test]
        public void testUppercaseConstant()
        {

            var sentence = this.phraseFactory.createClause();

            var subject = this.phraseFactory.createNounPhrase("a", "Cat");
            sentence.setSubject(subject);

            var output = this.realiser.realiseSentence(sentence);

            Assert.AreEqual("A Cat.", output);
        }

        /**
         * testUppercaseVowel - Test for "an" as a specifier for upper subjects.
         */

        [Test]
        public void testUppercaseVowel()
        {
            var sentence = this.phraseFactory.createClause();

            var subject = this.phraseFactory.createNounPhrase("a", "Emu");
            sentence.setSubject(subject);

            var output = this.realiser.realiseSentence(sentence);

            Assert.AreEqual("An Emu.", output);
        }

        /**
         * testNumericA - Test for "a" specifier with a numeric subject 
         */

        [Test]
        public void testNumericA()
        {
            var sentence = this.phraseFactory.createClause();

            var subject = this.phraseFactory.createNounPhrase("a", "7");
            sentence.setSubject(subject);

            var output = this.realiser.realiseSentence(sentence);

            Assert.AreEqual("A 7.", output);
        }

        /**
         * testNumericAn - Test for "an" specifier with a numeric subject 
         */

        [Test]
        public void testNumericAn()
        {
            var sentence = this.phraseFactory.createClause();

            var subject = this.phraseFactory.createNounPhrase("a", "11");
            sentence.setSubject(subject);

            var output = this.realiser.realiseSentence(sentence);

            Assert.AreEqual("An 11.", output);
        }

        /**
         * testIrregularSubjects - Test irregular subjects that don't conform to the
         * vowel vs. constant divide. 
         */

        [Test]
        public void testIrregularSubjects()
        {
            var sentence = this.phraseFactory.createClause();

            var subject = this.phraseFactory.createNounPhrase("a", "one");
            sentence.setSubject(subject);

            var output = this.realiser.realiseSentence(sentence);

            Assert.AreEqual("A one.", output);
        }

        /**
         * testSingluarThisDeterminerNPObject - Test for "this" when used in the singular form as a determiner in a NP Object
         */

        [Test]
        public void testSingluarThisDeterminerNPObject()
        {
            var sentence_1 = this.phraseFactory.createClause();

            var nounPhrase_1 = this.phraseFactory.createNounPhrase("this", "monkey");
            sentence_1.setObject(nounPhrase_1);

            Assert.AreEqual("This monkey.", this.realiser.realiseSentence(sentence_1));
        }

        /**
         * testPluralThisDeterminerNPObject - Test for "this" when used in the plural form as a determiner in a NP Object
         */

        [Test]
        public void testPluralThisDeterminerNPObject()
        {
            var sentence_1 = this.phraseFactory.createClause();

            var nounPhrase_1 = this.phraseFactory.createNounPhrase("monkey");
            nounPhrase_1.setPlural(true);
            nounPhrase_1.setDeterminer("this");
            sentence_1.setObject(nounPhrase_1);

            Assert.AreEqual("These monkeys.", this.realiser.realiseSentence(sentence_1));

        }

        /**
         * testSingluarThatDeterminerNPObject - Test for "that" when used in the singular form as a determiner in a NP Object
         */

        [Test]
        public void testSingluarThatDeterminerNPObject()
        {
            var sentence_1 = this.phraseFactory.createClause();

            var nounPhrase_1 = this.phraseFactory.createNounPhrase("that", "monkey");
            sentence_1.setObject(nounPhrase_1);

            Assert.AreEqual("That monkey.", this.realiser.realiseSentence(sentence_1));
        }

        /**
         * testPluralThatDeterminerNPObject - Test for "that" when used in the plural form as a determiner in a NP Object
         */

        [Test]
        public void testPluralThatDeterminerNPObject()
        {
            var sentence_1 = this.phraseFactory.createClause();

            var nounPhrase_1 = this.phraseFactory.createNounPhrase("monkey");
            nounPhrase_1.setPlural(true);
            nounPhrase_1.setDeterminer("that");
            sentence_1.setObject(nounPhrase_1);

            this.realiser.setDebugMode(true);
            Assert.AreEqual("Those monkeys.", this.realiser.realiseSentence(sentence_1));

        }

        /**
         * testSingularThoseDeterminerNPObject - Test for "those" when used in the singular form as a determiner in a NP Object
         */

        [Test]
        public void testSingularThoseDeterminerNPObject()
        {
            var sentence_1 = this.phraseFactory.createClause();

            var nounPhrase_1 = this.phraseFactory.createNounPhrase("monkey");
            nounPhrase_1.setPlural(false);
            nounPhrase_1.setDeterminer("those");
            sentence_1.setObject(nounPhrase_1);

            Assert.AreEqual("That monkey.", this.realiser.realiseSentence(sentence_1));

        }

        /**
         * testSingularTheseDeterminerNPObject - Test for "these" when used in the singular form as a determiner in a NP Object
         */

        [Test]
        public void testSingularTheseDeterminerNPObject()
        {
            var sentence_1 = this.phraseFactory.createClause();

            var nounPhrase_1 = this.phraseFactory.createNounPhrase("monkey");
            nounPhrase_1.setPlural(false);
            nounPhrase_1.setDeterminer("these");
            sentence_1.setObject(nounPhrase_1);

            Assert.AreEqual("This monkey.", this.realiser.realiseSentence(sentence_1));

        }

        /**
         * testPluralThoseDeterminerNPObject - Test for "those" when used in the plural form as a determiner in a NP Object
         */

        [Test]
        public void testPluralThoseDeterminerNPObject()
        {
            var sentence_1 = this.phraseFactory.createClause();

            var nounPhrase_1 = this.phraseFactory.createNounPhrase("monkey");
            nounPhrase_1.setPlural(true);
            nounPhrase_1.setDeterminer("those");
            sentence_1.setObject(nounPhrase_1);

            Assert.AreEqual("Those monkeys.", this.realiser.realiseSentence(sentence_1));

        }

        /**
         * testPluralTheseDeterminerNPObject - Test for "these" when used in the plural form as a determiner in a NP Object
         */

        [Test]
        public void testPluralTheseDeterminerNPObject()
        {
            var sentence_1 = this.phraseFactory.createClause();

            var nounPhrase_1 = this.phraseFactory.createNounPhrase("monkey");
            nounPhrase_1.setPlural(true);
            nounPhrase_1.setDeterminer("these");
            sentence_1.setObject(nounPhrase_1);

            Assert.AreEqual("These monkeys.", this.realiser.realiseSentence(sentence_1));

        }

        /**
         * testSingularTheseDeterminerNPObject - Test for "these" when used in the singular form as a determiner in a NP Object
         *                                       using the NIHDB Lexicon.
         */

/*
        public void testSingularTheseDeterminerNPObject_NIHDBLexicon()
        {
            this.lexicon = new NIHDBLexicon(DB_FILENAME);
            this.phraseFactory = new NLGFactory(this.lexicon);
            this.realiser = new Realiser(this.lexicon);

            SPhraseSpec sentence_1 = this.phraseFactory.createClause();

            NPPhraseSpec nounPhrase_1 = this.phraseFactory.createNounPhrase("monkey");
            nounPhrase_1.setPlural(false);
            nounPhrase_1.setDeterminer("these");
            sentence_1.setObject(nounPhrase_1);

            Assert.AreEqual("This monkey.", this.realiser.realiseSentence(sentence_1));

        }
        */

        /**
         * testSingularThoseDeterminerNPObject - Test for "those" when used in the singular form as a determiner in a NP Object
         *                                       using the NIHDB Lexicon
         */

/*
        public void testSingularThoseDeterminerNPObject_NIHDBLexicon()
        {
            this.lexicon = new NIHDBLexicon(DB_FILENAME);
            this.phraseFactory = new NLGFactory(this.lexicon);
            this.realiser = new Realiser(this.lexicon);

            SPhraseSpec sentence_1 = this.phraseFactory.createClause();

            NPPhraseSpec nounPhrase_1 = this.phraseFactory.createNounPhrase("monkey");
            nounPhrase_1.setPlural(false);
            nounPhrase_1.setDeterminer("those");
            sentence_1.setObject(nounPhrase_1);

            Assert.AreEqual("That monkey.", this.realiser.realiseSentence(sentence_1));

        }
        */


        /**
         * testPluralThatDeterminerNPObject - Test for "that" when used in the plural form as a determiner in a NP Object
         *                                    using the NIHDB Lexicon.
         */

       /*
        public void testPluralThatDeterminerNPObject_NIHDBLexicon()
        {
            this.lexicon = new NIHDBLexicon(DB_FILENAME);
            this.phraseFactory = new NLGFactory(this.lexicon);
            this.realiser = new Realiser(this.lexicon);

            SPhraseSpec sentence_1 = this.phraseFactory.createClause();

            NPPhraseSpec nounPhrase_1 = this.phraseFactory.createNounPhrase("monkey");
            nounPhrase_1.setPlural(true);
            nounPhrase_1.setDeterminer("that");
            sentence_1.setObject(nounPhrase_1);

            Assert.AreEqual("Those monkeys.", this.realiser.realiseSentence(sentence_1));

        }
        */

        /**
         * testPluralThisDeterminerNPObject - Test for "this" when used in the plural form as a determiner in a NP Object
         *                                    using the NIHDBLexicon.
         */

       /*
        public void testPluralThisDeterminerNPObject_NIHDBLexicon()
        {
            this.lexicon = new NIHDBLexicon(DB_FILENAME);
            this.phraseFactory = new NLGFactory(this.lexicon);
            this.realiser = new Realiser(this.lexicon);

            SPhraseSpec sentence_1 = this.phraseFactory.createClause();

            NPPhraseSpec nounPhrase_1 = this.phraseFactory.createNounPhrase("monkey");
            nounPhrase_1.setPlural(true);
            nounPhrase_1.setDeterminer("this");
            sentence_1.setObject(nounPhrase_1);

            Assert.AreEqual("These monkeys.", this.realiser.realiseSentence(sentence_1));

        }
        */
    }
}