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
using NUnit.Framework;
using SimpleNLG;
using SimpleNLG.Extensions;

namespace SimpleNLGTests.realiser
{
    /**
     * JUnit test class for the {@link Realiser} class.
     * 
     * @author Saad Mahamood
     */

    public class RealiserTest
    {

        [SetUpFixture]
        public class SetupForRealiserTest
        {
            public static Lexicon lexicon = Lexicon.getDefaultLexicon();
            public static NLGFactory phraseFactory = new NLGFactory(lexicon);
            public static Realiser realiser = new Realiser(lexicon);
        }



        /**
         * Test the realization of List of NLGElements that is null
         */
        [Test]
        public void emptyNLGElementRealiserTest()
        {
            List<INLGElement> elements = new List<INLGElement>();
            List<INLGElement> realisedElements = SetupForRealiserTest.realiser.realise(elements);
            // Expect emtpy listed returned:
            Assert.IsNotNull(realisedElements);
            Assert.AreEqual(0, realisedElements.size());
        }


        /**
         * Test the realization of List of NLGElements that is null
         */
        [Test]
        public void nullNLGElementRealiserTest()
        {
            List<INLGElement> elements = null;
            List<INLGElement> realisedElements = SetupForRealiserTest.realiser.realise(elements);
            // Expect emtpy listed returned:
            Assert.IsNotNull(realisedElements);
            Assert.AreEqual(0, realisedElements.size());

        }

        /**
         * Tests the realization of multiple NLGElements in a list.
         */
        [Test]
        public void multipleNLGElementListRealiserTest()
        {
            List<INLGElement> elements = new List<INLGElement>();
            // Create test NLGElements to realize:

            // "The cat jumping on the counter."
            DocumentElement sentence1 = SetupForRealiserTest.phraseFactory.createSentence();
            NPPhraseSpec subject_1 = SetupForRealiserTest.phraseFactory.createNounPhrase("the", "cat");
            VPPhraseSpec verb_1 = SetupForRealiserTest.phraseFactory.createVerbPhrase("jump");
            verb_1.setFeature(Feature.FORM.ToString(), Form.PRESENT_PARTICIPLE);
            PPPhraseSpec prep_1 = SetupForRealiserTest.phraseFactory.createPrepositionPhrase();
            NPPhraseSpec object_1 = SetupForRealiserTest.phraseFactory.createNounPhrase();
            object_1.setDeterminer("the");
            object_1.setNoun("counter");
            prep_1.addComplement(object_1);
            prep_1.setPreposition("on");
            SPhraseSpec clause_1 = SetupForRealiserTest.phraseFactory.createClause();
            clause_1.setSubject(subject_1);
            clause_1.setVerbPhrase(verb_1);
            clause_1.setObject(prep_1);
            sentence1.addComponent(clause_1);

            // "The dog running on the counter."
            DocumentElement sentence2 = SetupForRealiserTest.phraseFactory.createSentence();
            NPPhraseSpec subject_2 = SetupForRealiserTest.phraseFactory.createNounPhrase("the", "dog");
            VPPhraseSpec verb_2 = SetupForRealiserTest.phraseFactory.createVerbPhrase("run");
            verb_2.setFeature(Feature.FORM.ToString(), Form.PRESENT_PARTICIPLE);
            PPPhraseSpec prep_2 = SetupForRealiserTest.phraseFactory.createPrepositionPhrase();
            NPPhraseSpec object_2 = SetupForRealiserTest.phraseFactory.createNounPhrase();
            object_2.setDeterminer("the");
            object_2.setNoun("counter");
            prep_2.addComplement(object_2);
            prep_2.setPreposition("on");
            SPhraseSpec clause_2 = SetupForRealiserTest.phraseFactory.createClause();
            clause_2.setSubject(subject_2);
            clause_2.setVerbPhrase(verb_2);
            clause_2.setObject(prep_2);
            sentence2.addComponent(clause_2);


            elements.add(sentence1);
            elements.add(sentence2);

            List<INLGElement> realisedElements = SetupForRealiserTest.realiser.realise(elements);

            Assert.IsNotNull(realisedElements);
            Assert.AreEqual(2, realisedElements.size());
            Assert.AreEqual("The cat jumping on the counter.", realisedElements.get(0).getRealisation());
            Assert.AreEqual("The dog running on the counter.", realisedElements.get(1).getRealisation());

        }

        /**
         * Tests the correct pluralization with possessives (GitHub issue #9)
         */
       [Test]
        public void correctPluralizationWithPossessives()
        {
            NPPhraseSpec sisterNP = SetupForRealiserTest.phraseFactory.createNounPhrase("sister");
            INLGElement word = SetupForRealiserTest.phraseFactory.createInflectedWord("Albert Einstein",
                new LexicalCategory_NOUN());
            word.setFeature(LexicalFeature.PROPER, true);
            NPPhraseSpec possNP = SetupForRealiserTest.phraseFactory.createNounPhrase(word);
            possNP.setFeature(Feature.POSSESSIVE.ToString(), true);
            sisterNP.setSpecifier(possNP);


            Assert.AreEqual("Albert Einstein's sister",
                SetupForRealiserTest.realiser.realise(sisterNP).getRealisation());

            sisterNP.setPlural(true);

            SetupForRealiserTest.realiser.setDebugMode(true);
            Assert.AreEqual("Albert Einstein's sisters",
                SetupForRealiserTest.realiser.realise(sisterNP).getRealisation());

            sisterNP.setPlural(false);

            possNP.setFeature(LexicalFeature.GENDER, Gender.MASCULINE);
            possNP.setFeature(Feature.PRONOMINAL.ToString(), true);
            Assert.AreEqual("his sister",
                SetupForRealiserTest.realiser.realise(sisterNP).getRealisation());
            sisterNP.setPlural(true);
            Assert.AreEqual("his sisters",
                SetupForRealiserTest.realiser.realise(sisterNP).getRealisation());
        }

    }
}