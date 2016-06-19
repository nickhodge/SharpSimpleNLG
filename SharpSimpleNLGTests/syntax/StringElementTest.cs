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
using SimpleNLG;

namespace SimpleNLGTests.syntax
{
    /**
     * Tests for string elements as parts of larger phrases
     * 
     * @author bertugatt
     * 
     */

    public class StringElementTest : SimpleNLG4TestBase
    {

        /**
         * Test that string elements can be used as heads of NP
         */

        [Test]
        public void stringElementAsHeadTest()
        {
            var np = this.phraseFactory.createNounPhrase();
            np.setHead(this.phraseFactory.createStringElement("dogs and cats"));
            np.setSpecifier(this.phraseFactory.createWord("the",
                new LexicalCategory_DETERMINER()));
            Assert.AreEqual("the dogs and cats", this.realiser.realise(np)
                .getRealisation());
        }

        /**
         * Sentences whose VP is a canned string
         */

        [Test]
        public void stringElementAsVPTest()
        {
            var s = this.phraseFactory.createClause();
            s.setVerbPhrase(this.phraseFactory.createStringElement("eats and drinks"));
            s.setSubject(this.phraseFactory.createStringElement("the big fat man"));
            Assert.AreEqual("the big fat man eats and drinks", this.realiser
                .realise(s).getRealisation());
        }

        /**
         * Test for when the SPhraseSpec has a NPSpec added directly after it:
         * "Mary loves NP[the cow]."
         */

        [Test]
        public void tailNPStringElementTest()
        {
            var senSpec = this.phraseFactory.createClause();
            senSpec.addComplement((this.phraseFactory.createStringElement("mary loves")));
            var np = this.phraseFactory.createNounPhrase();
            np.setHead("cow");
            np.setDeterminer("the");
            senSpec.addComplement(np);
            var completeSen = this.phraseFactory.createSentence();
            completeSen.addComponent(senSpec);
            Assert.AreEqual("Mary loves the cow.", this.realiser.realise(completeSen).getRealisation());
        }

        /**
         * Test for a NP followed by a canned text: "NP[A cat] loves a dog".
         */

        [Test]
        public void frontNPStringElementTest()
        {
            var senSpec = this.phraseFactory.createClause();
            var np = this.phraseFactory.createNounPhrase();
            np.setHead("cat");
            np.setDeterminer("the");
            senSpec.addComplement(np);
            senSpec.addComplement(this.phraseFactory.createStringElement("loves a dog"));
            var completeSen = this.phraseFactory.createSentence();
            completeSen.addComponent(senSpec);
            Assert.AreEqual("The cat loves a dog.", this.realiser.realise(completeSen).getRealisation());
        }


        /**
         * Test for a StringElement followed by a NP followed by a StringElement
         * "The world loves NP[ABBA] but not a sore loser."
         */

        [Test]
        public void mulitpleStringElementsTest()
        {
            var senSpec = this.phraseFactory.createClause();
            senSpec.addComplement(this.phraseFactory.createStringElement("the world loves"));
            var np = this.phraseFactory.createNounPhrase();
            np.setHead("ABBA");
            senSpec.addComplement(np);
            senSpec.addComplement(this.phraseFactory.createStringElement("but not a sore loser"));
            var completeSen = this.phraseFactory.createSentence();
            completeSen.addComponent(senSpec);
            Assert.AreEqual("The world loves ABBA but not a sore loser.", this.realiser.realise(completeSen).getRealisation());
        }

        /**
         * Test for multiple NP phrases with a single StringElement phrase:
         * "NP[John is] a trier NP[for cheese]."
         */

        [Test]
        public void mulitpleNPElementsTest()
        {
            var senSpec = this.phraseFactory.createClause();
            var frontNoun = this.phraseFactory.createNounPhrase();
            frontNoun.setHead("john");
            senSpec.addComplement(frontNoun);
            senSpec.addComplement(this.phraseFactory.createStringElement("is a trier"));
            var backNoun = this.phraseFactory.createNounPhrase();
            backNoun.setDeterminer("for");
            backNoun.setNoun("cheese");
            senSpec.addComplement(backNoun);
            var completeSen = this.phraseFactory.createSentence();
            completeSen.addComponent(senSpec);
            Assert.AreEqual("John is a trier for cheese.", this.realiser.realise(completeSen).getRealisation());

        }


        /**
         * White space check: Test to see how SNLG deals with additional whitespaces: 
         * 
         * NP[The Nasdaq] rose steadily during NP[early trading], however it plummeted due to NP[a shock] after NP[IBM] announced poor 
         * NP[first quarter results].
         */

        [Test]
        public void whiteSpaceNPTest()
        {
            var senSpec = this.phraseFactory.createClause();
            var firstNoun = this.phraseFactory.createNounPhrase();
            firstNoun.setDeterminer("the");
            firstNoun.setNoun("Nasdaq");
            senSpec.addComplement(firstNoun);
            senSpec.addComplement(this.phraseFactory.createStringElement(" rose steadily during "));
            var secondNoun = this.phraseFactory.createNounPhrase();
            secondNoun.setSpecifier("early");
            secondNoun.setNoun("trading");
            senSpec.addComplement(secondNoun);
            senSpec.addComplement(this.phraseFactory.createStringElement(" , however it plummeted due to"));
            var thirdNoun = this.phraseFactory.createNounPhrase();
            thirdNoun.setSpecifier("a");
            thirdNoun.setNoun("shock");
            senSpec.addComplement(thirdNoun);
            senSpec.addComplement(this.phraseFactory.createStringElement(" after "));
            var fourthNoun = this.phraseFactory.createNounPhrase();
            fourthNoun.setNoun("IBM");
            senSpec.addComplement(fourthNoun);
            senSpec.addComplement(this.phraseFactory.createStringElement("announced poor    "));
            var fifthNoun = this.phraseFactory.createNounPhrase();
            fifthNoun.setSpecifier("first quarter");
            fifthNoun.setNoun("results");
            fifthNoun.setPlural(true);
            senSpec.addComplement(fifthNoun);
            var completeSen = this.phraseFactory.createSentence();
            completeSen.addComponent(senSpec);
            Assert.AreEqual(
                "The Nasdaq rose steadily during early trading, however it plummeted due to a shock after IBM announced poor first quarter results.",
                this.realiser.realise(completeSen).getRealisation());
        }

        /**
         * Point absorption test: Check to see if SNLG respects abbreviations at the end of a sentence.
         * "NP[Yahya] was sleeping his own and dreaming etc."
         */

        public void pointAbsorptionTest()
        {
            var senSpec = this.phraseFactory.createClause();
            var firstNoun = this.phraseFactory.createNounPhrase();
            firstNoun.setNoun("yaha");
            senSpec.addComplement(firstNoun);
            senSpec.addComplement("was sleeping on his own and dreaming etc.");
            var completeSen = this.phraseFactory.createSentence();
            completeSen.addComponent(senSpec);
            Assert.AreEqual("Yaha was sleeping on his own and dreaming etc.",
                this.realiser.realise(completeSen).getRealisation());


        }

        /**
         * Point absorption test: As above, but with trailing white space.
         * "NP[Yaha] was sleeping his own and dreaming etc.      "
         */

        public void pointAbsorptionTrailingWhiteSpaceTest()
        {
            var senSpec = this.phraseFactory.createClause();
            var firstNoun = this.phraseFactory.createNounPhrase();
            firstNoun.setNoun("yaha");
            senSpec.addComplement(firstNoun);
            senSpec.addComplement("was sleeping on his own and dreaming etc.      ");
            var completeSen = this.phraseFactory.createSentence();
            completeSen.addComponent(senSpec);
            Assert.AreEqual("Yaha was sleeping on his own and dreaming etc.",
                this.realiser.realise(completeSen).getRealisation());
        }

        /**
         * Abbreviation test: Check to see how SNLG deals with abbreviations in the middle of a sentence.
         * 
         * "NP[Yahya] and friends etc. went to NP[the park] to play."
         */

        [Test]
        public void middleAbbreviationTest()
        {
            var senSpec = this.phraseFactory.createClause();
            var firstNoun = this.phraseFactory.createNounPhrase();
            firstNoun.setNoun("yahya");
            senSpec.addComplement(firstNoun);
            senSpec.addComplement(this.phraseFactory.createStringElement("and friends etc. went to"));
            var secondNoun = this.phraseFactory.createNounPhrase();
            secondNoun.setDeterminer("the");
            secondNoun.setNoun("park");
            senSpec.addComplement(secondNoun);
            senSpec.addComplement("to play");
            var completeSen = this.phraseFactory.createSentence();
            completeSen.addComponent(senSpec);
            Assert.AreEqual("Yahya and friends etc. went to the park to play.",
                this.realiser.realise(completeSen).getRealisation());
        }


        /**
         * Indefinite Article Inflection: StringElement to test how SNLG handles a/an situations.
         * "I see an NP[elephant]" 
         */

        [Test]
        public void stringIndefiniteArticleInflectionVowelTest()
        {
            var senSpec = this.phraseFactory.createClause();
            senSpec.addComplement(this.phraseFactory.createStringElement("I see a"));
            var firstNoun = this.phraseFactory.createNounPhrase("elephant");
            senSpec.addComplement(firstNoun);
            var completeSen = this.phraseFactory.createSentence();
            completeSen.addComponent(senSpec);
            Assert.AreEqual("I see an elephant.",
                this.realiser.realise(completeSen).getRealisation());

        }

        /**
         * Indefinite Article Inflection: StringElement to test how SNLG handles a/an situations.
         * "I see NP[a elephant]" --> 
         */

        [Test]
        public void NPIndefiniteArticleInflectionVowelTest()
        {
            var senSpec = this.phraseFactory.createClause();
            senSpec.addComplement(this.phraseFactory.createStringElement("I see"));
            var firstNoun = this.phraseFactory.createNounPhrase("elephant");
            firstNoun.setDeterminer("a");
            senSpec.addComplement(firstNoun);
            var completeSen = this.phraseFactory.createSentence();
            completeSen.addComponent(senSpec);
            Assert.AreEqual("I see an elephant.",
                this.realiser.realise(completeSen).getRealisation());

        }


        /**
         * Indefinite Article Inflection: StringElement to test how SNLG handles a/an situations.
         * "I see an NP[cow]" 
         */

        // Unknown status in Java Library [Test]
        public void stringIndefiniteArticleInflectionConsonantTest()
        {
            var senSpec = this.phraseFactory.createClause();
            senSpec.addComplement(this.phraseFactory.createStringElement("I see an"));
            var firstNoun = this.phraseFactory.createNounPhrase("cow");
            senSpec.addComplement(firstNoun);
            var completeSen = this.phraseFactory.createSentence();
            completeSen.addComponent(senSpec);
            this.realiser.setDebugMode(true);
            // Do not attempt "an" -> "a"
            Assert.AreNotEqual("I see an cow.",
                this.realiser.realise(completeSen).getRealisation());
        }

        /**
         * Indefinite Article Inflection: StringElement to test how SNLG handles a/an situations.
         * "I see NP[an cow]" --> 
         */

        [Test]
        public void NPIndefiniteArticleInflectionConsonantTest()
        {
            var senSpec = this.phraseFactory.createClause();
            senSpec.addComplement(this.phraseFactory.createStringElement("I see"));
            var firstNoun = this.phraseFactory.createNounPhrase("cow");
            firstNoun.setDeterminer("an");
            senSpec.addComplement(firstNoun);
            var completeSen = this.phraseFactory.createSentence();
            completeSen.addComponent(senSpec);
            // Do not attempt "an" -> "a"
            Assert.AreEqual("I see an cow.",
                this.realiser.realise(completeSen).getRealisation());
        }


        /**
         * aggregationStringElementTest: Test to see if we can aggregate two StringElements in a CoordinatedPhraseElement.
         */

        [Test]
        public void aggregationStringElementTest()
        {

            var coordinate =
                this.phraseFactory.createCoordinatedPhrase(new StringElement("John is going to Tesco"),
                    new StringElement("Mary is going to Sainsburys"));
            var sentence = this.phraseFactory.createClause();
            sentence.addComplement(coordinate);

            Assert.AreEqual("John is going to Tesco and Mary is going to Sainsburys.",
                this.realiser.realiseSentence(sentence));
        }


        /**
         * Tests that no empty space is added when a StringElement is instantiated with an empty string
         * or null object.
         */

        [Test]
        public void nullAndEmptyStringElementTest()
        {

            var nullStringElement = this.phraseFactory.createStringElement(null);
            var emptyStringElement = this.phraseFactory.createStringElement("");
            var beautiful = this.phraseFactory.createStringElement("beautiful");
            var horseLike = this.phraseFactory.createStringElement("horse-like");
            var creature = this.phraseFactory.createStringElement("creature");

            // Test1: null or empty at beginning
            var test1 = this.phraseFactory.createClause("a unicorn", "be", "regarded as a");
            test1.addPostModifier(emptyStringElement);
            test1.addPostModifier(beautiful);
            test1.addPostModifier(horseLike);
            test1.addPostModifier(creature);
              Assert.AreEqual("A unicorn is regarded as a beautiful horse-like creature.",
                this.realiser.realiseSentence(test1));

            // Test2: empty or null at end
            var test2 = this.phraseFactory.createClause("a unicorn", "be", "regarded as a");
            test2.addPostModifier(beautiful);
            test2.addPostModifier(horseLike);
            test2.addPostModifier(creature);
            test2.addPostModifier(nullStringElement);
             Assert.AreEqual("A unicorn is regarded as a beautiful horse-like creature.",
                this.realiser.realiseSentence(test2));

            // Test3: empty or null in the middle
            var test3 = this.phraseFactory.createClause("a unicorn", "be", "regarded as a");
            test3.addPostModifier("beautiful");
            test3.addPostModifier("horse-like");
            test3.addPostModifier("");
            test3.addPostModifier("creature");
              Assert.AreEqual("A unicorn is regarded as a beautiful horse-like creature.",
                this.realiser.realiseSentence(test3));

            // Test4: empty or null in the middle with empty or null at beginning
            var test4 = this.phraseFactory.createClause("a unicorn", "be", "regarded as a");
            test4.addPostModifier("");
            test4.addPostModifier("beautiful");
            test4.addPostModifier("horse-like");
            test4.addPostModifier(nullStringElement);
            test4.addPostModifier("creature");
            Assert.AreEqual("A unicorn is regarded as a beautiful horse-like creature.",
                this.realiser.realiseSentence(test4));

        }

    }
}