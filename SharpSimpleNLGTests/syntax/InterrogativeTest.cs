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
     * JUnit test case for interrogatives.
     * 
     * @author agatt
     */

    public class InterrogativeTest : SimpleNLG4TestBase
    {

        // set up a few more fixtures
        /** The s5. */


        [SetUp]
        protected override void setUp()
        {
            base.setUp();

            // // the man gives the woman John's flower
            var john = this.phraseFactory.createNounPhrase("John"); 
            john.setFeature(Feature.POSSESSIVE.ToString(), true);
            var flower = this.phraseFactory.createNounPhrase(john,
                "flower"); 
            var _woman = this.phraseFactory.createNounPhrase(
                "the", "woman");  
            s3 = this.phraseFactory.createClause(this.man, this.give, flower);
            s3.setIndirectObject(_woman);

            var subjects = new CoordinatedPhraseElement(
                this.phraseFactory.createNounPhrase("Jane"), 
                this.phraseFactory.createNounPhrase("Andrew")); 
            s4 = this.phraseFactory.createClause(subjects, "pick up", 
                "the balls"); 
            s4.addPostModifier("in the shop"); 
            s4.setFeature(Feature.CUE_PHRASE.ToString(), "however"); 
            s4.addFrontModifier("tomorrow"); 
            s4.setFeature(Feature.TENSE.ToString(), Tense.FUTURE);
            // s5 = new SPhraseSpec();
            // s5.setSubject(new NPPhraseSpec("the", "dog"));
            // s5.setHead("be");
            // s5.setComplement(new NPPhraseSpec("the", "rock"),
            // DiscourseFunction.OBJECT);

        }

        /**
         * Tests a couple of fairly simple questions.
         */

        [Test]
        public void testSimpleQuestions()
        {
            setUp();
            this.phraseFactory.setLexicon(this.lexicon);
            this.realiser.setLexicon(this.lexicon);

            // simple present
            s1 = this.phraseFactory.createClause(this.woman, this.kiss,
                this.man);
            s1.setFeature(Feature.TENSE.ToString(), Tense.PRESENT);
            s1.setFeature(Feature.INTERROGATIVE_TYPE.ToString(), InterrogativeType.YES_NO);

            var docFactory = new NLGFactory(this.lexicon);
            var sent = docFactory.createSentence(s1);
            Assert.AreEqual("Does the woman kiss the man?", this.realiser 
                .realise(sent).getRealisation());

            // simple past
            // sentence: "the woman kissed the man"
            s1 = this.phraseFactory.createClause(this.woman, this.kiss,
                this.man);
            s1.setFeature(Feature.TENSE.ToString(), Tense.PAST);
            s1.setFeature(Feature.INTERROGATIVE_TYPE.ToString(), InterrogativeType.YES_NO);
            Assert.AreEqual("did the woman kiss the man", this.realiser 
                .realise(s1).getRealisation());

            // copular/existential: be-fronting
            // sentence = "there is the dog on the rock"
            s2 = this.phraseFactory.createClause("there", "be", this.dog);  
            s2.addPostModifier(this.onTheRock);
            s2.setFeature(Feature.INTERROGATIVE_TYPE.ToString(), InterrogativeType.YES_NO);
            Assert.AreEqual("is there the dog on the rock", this.realiser 
                .realise(s2).getRealisation());

            // perfective
            // sentence -- "there has been the dog on the rock"
            s2 = this.phraseFactory.createClause("there", "be", this.dog);  
            s2.addPostModifier(this.onTheRock);
            s2.setFeature(Feature.INTERROGATIVE_TYPE.ToString(), InterrogativeType.YES_NO);
            s2.setFeature(Feature.PERFECT.ToString(), true);
            Assert.AreEqual("has there been the dog on the rock", 
                this.realiser.realise(s2).getRealisation());
            
            // progressive
            // sentence: "the man was giving the woman John's flower"
            var john = this.phraseFactory.createNounPhrase("John"); 
            john.setFeature(Feature.POSSESSIVE.ToString(), true);
            var flower = this.phraseFactory.createNounPhrase(john,
                "flower"); 
            var _woman = this.phraseFactory.createNounPhrase(
                "the", "woman");  
            s3 = this.phraseFactory.createClause(this.man, this.give, flower);
            s3.setIndirectObject(_woman);
            s3.setFeature(Feature.TENSE.ToString(), Tense.PAST);
            s3.setFeature(Feature.PROGRESSIVE.ToString(), true);
            s3.setFeature(Feature.INTERROGATIVE_TYPE.ToString(), InterrogativeType.YES_NO);
            var realised = this.realiser.realise(s3);
            Assert.AreEqual("was the man giving the woman John's flower", 
                realised.getRealisation());

            // modal
            // sentence: "the man should be giving the woman John's flower"
            setUp();
            john = this.phraseFactory.createNounPhrase("John"); 
            john.setFeature(Feature.POSSESSIVE.ToString(), true);
            flower = this.phraseFactory.createNounPhrase(john, "flower"); 
            _woman = this.phraseFactory.createNounPhrase("the", "woman");  
            s3 = this.phraseFactory.createClause(this.man, this.give, flower);
            s3.setIndirectObject(_woman);
            s3.setFeature(Feature.TENSE.ToString(), Tense.PAST);
            s3.setFeature(Feature.INTERROGATIVE_TYPE.ToString(), InterrogativeType.YES_NO);
            s3.setFeature(Feature.MODAL.ToString(), "should"); 
            Assert.AreEqual(
                "should the man have given the woman John's flower", 
                this.realiser.realise(s3).getRealisation());

            // complex case with cue phrases
            // sentence: "however, tomorrow, Jane and Andrew will pick up the balls
            // in the shop"
            // this gets the front modifier "tomorrow" shifted to the end
            setUp();
            var subjects = new CoordinatedPhraseElement(
                this.phraseFactory.createNounPhrase("Jane"), 
                this.phraseFactory.createNounPhrase("Andrew")); 
            s4 = this.phraseFactory.createClause(subjects, "pick up", 
                "the balls"); 
            s4.addPostModifier("in the shop"); 
            s4.setFeature(Feature.CUE_PHRASE.ToString(), "however,"); 
            s4.addFrontModifier("tomorrow"); 
            s4.setFeature(Feature.TENSE.ToString(), Tense.FUTURE);
            s4.setFeature(Feature.INTERROGATIVE_TYPE.ToString(), InterrogativeType.YES_NO);
            Assert.AreEqual(
                "however, will Jane and Andrew pick up the balls in the shop tomorrow", 
                this.realiser.realise(s4).getRealisation());
        }

        /**
         * Test for sentences with negation.
         */

        [Test]
        public void testNegatedQuestions()
        {
            setUp();
            this.phraseFactory.setLexicon(this.lexicon);
            this.realiser.setLexicon(this.lexicon);

            // sentence: "the woman did not kiss the man"
            s1 = this.phraseFactory.createClause(this.woman, "kiss", this.man);
            s1.setFeature(Feature.TENSE.ToString(), Tense.PAST);
            s1.setFeature(Feature.NEGATED.ToString(), true);
            s1.setFeature(Feature.INTERROGATIVE_TYPE.ToString(), InterrogativeType.YES_NO);
            Assert.AreEqual("did the woman not kiss the man", this.realiser 
                .realise(s1).getRealisation());

            // sentence: however, tomorrow, Jane and Andrew will not pick up the
            // balls in the shop
            var subjects = new CoordinatedPhraseElement(
                this.phraseFactory.createNounPhrase("Jane"), 
                this.phraseFactory.createNounPhrase("Andrew")); 
            s4 = this.phraseFactory.createClause(subjects, "pick up", 
                "the balls"); 
            s4.addPostModifier("in the shop"); 
            s4.setFeature(Feature.CUE_PHRASE.ToString(), "however,"); 
            s4.addFrontModifier("tomorrow"); 
            s4.setFeature(Feature.NEGATED.ToString(), true);
            s4.setFeature(Feature.TENSE.ToString(), Tense.FUTURE);
            s4.setFeature(Feature.INTERROGATIVE_TYPE.ToString(), InterrogativeType.YES_NO);
            Assert.AreEqual(
                "however, will Jane and Andrew not pick up the balls in the shop tomorrow", 
                this.realiser.realise(s4).getRealisation());
        }

        /**
         * Tests for coordinate VPs in question form.
         */

       // Disabled due to test setup issues [Test]
        public void testCoordinateVPQuestions()
        {

            // create a complex vp: "kiss the dog and walk in the room"
            reset();
            var complex = new CoordinatedPhraseElement(this.kiss, this.walk);
            this.kiss.addComplement(this.dog);
            this.walk.addComplement(this.inTheRoom);

            // sentence: "However, tomorrow, Jane and Andrew will kiss the dog and
            // will walk in the room"
            var subjects = new CoordinatedPhraseElement(
                this.phraseFactory.createNounPhrase("Jane"), 
                this.phraseFactory.createNounPhrase("Andrew")); 
            s4 = this.phraseFactory.createClause(subjects, complex);
            s4.setFeature(Feature.CUE_PHRASE.ToString(), "however"); 
            s4.addFrontModifier("tomorrow"); 
            s4.setFeature(Feature.TENSE.ToString(), Tense.FUTURE);

            Assert.AreEqual(
                "however tomorrow Jane and Andrew will kiss the dog and will walk in the room", 
                this.realiser.realise(s4).getRealisation());

            // setting to interrogative should automatically give us a single,
            // wide-scope aux
            reset();
            subjects = new CoordinatedPhraseElement(
                this.phraseFactory.createNounPhrase("Jane"), 
                this.phraseFactory.createNounPhrase("Andrew")); 
            complex = new CoordinatedPhraseElement(this.kiss, this.walk);
            s4 = this.phraseFactory.createClause(subjects, complex);
            s4.setFeature(Feature.CUE_PHRASE.ToString(), "however"); 
            s4.addFrontModifier("tomorrow"); 
            s4.setFeature(Feature.TENSE.ToString(), Tense.FUTURE);
            s4.setFeature(Feature.INTERROGATIVE_TYPE.ToString(), InterrogativeType.YES_NO);

            Assert.AreEqual(
                "however will Jane and Andrew kiss the dog and walk in the room tomorrow", 
                this.realiser.realise(s4).getRealisation());

            // slightly more complex -- perfective
            reset();
            this.realiser.setLexicon(this.lexicon);
            subjects = new CoordinatedPhraseElement(
                this.phraseFactory.createNounPhrase("Jane"), 
                this.phraseFactory.createNounPhrase("Andrew")); 
            complex = new CoordinatedPhraseElement(this.kiss, this.walk);
            this.kiss.addComplement(this.dog);
            this.walk.addComplement(this.inTheRoom);
            s4 = this.phraseFactory.createClause(subjects, complex);
            s4.setFeature(Feature.CUE_PHRASE.ToString(), "however"); 
            s4.addFrontModifier("tomorrow"); 
            s4.setFeature(Feature.TENSE.ToString(), Tense.FUTURE);
            s4.setFeature(Feature.INTERROGATIVE_TYPE.ToString(), InterrogativeType.YES_NO);
            s4.setFeature(Feature.PERFECT.ToString(), true);

            Assert.AreEqual(
                "however will Jane and Andrew have kissed the dog and walked in the room tomorrow", 
                this.realiser.realise(s4).getRealisation());
        }

        /**
         * Test for simple WH questions in present tense.
         */

        [Test]
        public void testSimpleQuestions2()
        {
            setUp();
            this.realiser.setLexicon(this.lexicon);
            var s = this.phraseFactory.createClause("the woman", "kiss",  
                "the man"); 

            // try with the simple yes/no type first
            s.setFeature(Feature.INTERROGATIVE_TYPE.ToString(), InterrogativeType.YES_NO);
            Assert.AreEqual("does the woman kiss the man", this.realiser 
                .realise(s).getRealisation());

            // now in the passive
            s = this.phraseFactory.createClause("the woman", "kiss",  
                "the man"); 
            s.setFeature(Feature.INTERROGATIVE_TYPE.ToString(), InterrogativeType.YES_NO);
            s.setFeature(Feature.PASSIVE.ToString(), true);
            Assert.AreEqual("is the man kissed by the woman", this.realiser 
                .realise(s).getRealisation());

            // // subject interrogative with simple present
            // // sentence: "the woman kisses the man"
            s = this.phraseFactory.createClause("the woman", "kiss",  
                "the man"); 
            s.setFeature(Feature.INTERROGATIVE_TYPE.ToString(), InterrogativeType.WHO_SUBJECT);

            Assert.AreEqual("who kisses the man", this.realiser.realise(s) 
                .getRealisation());

            // object interrogative with simple present
            s = this.phraseFactory.createClause("the woman", "kiss",  
                "the man"); 
            s.setFeature(Feature.INTERROGATIVE_TYPE.ToString().ToString(), InterrogativeType.WHO_OBJECT);
            Assert.AreEqual("who does the woman kiss", this.realiser 
                .realise(s).getRealisation());

            // subject interrogative with passive
            s = this.phraseFactory.createClause("the woman", "kiss",  
                "the man"); 
            s.setFeature(Feature.INTERROGATIVE_TYPE.ToString(), InterrogativeType.WHO_SUBJECT);
            s.setFeature(Feature.PASSIVE.ToString(), true);
            Assert.AreEqual("who is the man kissed by", this.realiser 
                .realise(s).getRealisation());
        }

        /**
         * Test for wh questions.
         */

        [Test]
        public void testWHQuestions()
        {

            // subject interrogative
            setUp();
            this.realiser.setLexicon(this.lexicon);
            s4.setFeature(Feature.INTERROGATIVE_TYPE.ToString(),
                InterrogativeType.WHO_SUBJECT);
            Assert.AreEqual(
                "however who will pick up the balls in the shop tomorrow", 
                this.realiser.realise(s4).getRealisation());

            // subject interrogative in passive
            setUp();
            s4.setFeature(Feature.PASSIVE.ToString(), true);
            s4.setFeature(Feature.INTERROGATIVE_TYPE.ToString(),
                InterrogativeType.WHO_SUBJECT);

            Assert.AreEqual(
                "however who will the balls be picked up in the shop by tomorrow", 
                this.realiser.realise(s4).getRealisation());

            // object interrogative
            setUp();
            s4.setFeature(Feature.INTERROGATIVE_TYPE.ToString(),
                InterrogativeType.WHAT_OBJECT);
            Assert.AreEqual(
                "however what will Jane and Andrew pick up in the shop tomorrow", 
                this.realiser.realise(s4).getRealisation());

            // object interrogative with passive
            setUp();
            s4.setFeature(Feature.INTERROGATIVE_TYPE.ToString(),
                InterrogativeType.WHAT_OBJECT);
            s4.setFeature(Feature.PASSIVE.ToString(), true);

            Assert.AreEqual(
                "however what will be picked up in the shop by Jane and Andrew tomorrow", 
                this.realiser.realise(s4).getRealisation());

            // how-question + passive
            setUp();
            s4.setFeature(Feature.PASSIVE.ToString(), true);
            s4.setFeature(Feature.INTERROGATIVE_TYPE.ToString(), InterrogativeType.HOW);
            Assert.AreEqual(
                "however how will the balls be picked up in the shop by Jane and Andrew tomorrow", 
                this.realiser.realise(s4).getRealisation());

            // // why-question + passive
            setUp();
            s4.setFeature(Feature.PASSIVE.ToString(), true);
            s4.setFeature(Feature.INTERROGATIVE_TYPE.ToString(), InterrogativeType.WHY);
            Assert.AreEqual(
                "however why will the balls be picked up in the shop by Jane and Andrew tomorrow", 
                this.realiser.realise(s4).getRealisation());

            // how question with modal
            setUp();
            s4.setFeature(Feature.PASSIVE.ToString(), true);
            s4.setFeature(Feature.INTERROGATIVE_TYPE.ToString(), InterrogativeType.HOW);
            s4.setFeature(Feature.MODAL.ToString(), "should"); 
            Assert.AreEqual(
                "however how should the balls be picked up in the shop by Jane and Andrew tomorrow", 
                this.realiser.realise(s4).getRealisation());

            // indirect object
            setUp();
            this.realiser.setLexicon(this.lexicon);
            s3.setFeature(Feature.INTERROGATIVE_TYPE.ToString(),
                InterrogativeType.WHO_INDIRECT_OBJECT);
            Assert.AreEqual("who does the man give John's flower to", 
                this.realiser.realise(s3).getRealisation());
        }

        /**
         * WH movement in the progressive
         */

        [Test]
        public void testProgrssiveWHSubjectQuestions()
        {
            var p = this.phraseFactory.createClause();
            p.setSubject("Mary");
            p.setVerb("eat");
            p.setObject(this.phraseFactory.createNounPhrase("the", "pie"));
            p.setFeature(Feature.PROGRESSIVE.ToString(), true);
            p.setFeature(Feature.INTERROGATIVE_TYPE.ToString(), InterrogativeType.WHO_SUBJECT);
            Assert.AreEqual("who is eating the pie", 
                this.realiser.realise(p).getRealisation());
        }

        /**
         * WH movement in the progressive
         */

        [Test]
        public void testProgrssiveWHObjectQuestions()
        {
            var p = this.phraseFactory.createClause();
            p.setSubject("Mary");
            p.setVerb("eat");
            p.setObject(this.phraseFactory.createNounPhrase("the", "pie"));
            p.setFeature(Feature.PROGRESSIVE.ToString(), true);
            p.setFeature(Feature.INTERROGATIVE_TYPE.ToString(), InterrogativeType.WHAT_OBJECT);
            Assert.AreEqual("what is Mary eating", 
                this.realiser.realise(p).getRealisation());

            // AG -- need to check this; it doesn't work
            // p.setFeature(Feature.NEGATED, true);
            //		Assert.AreEqual("what is Mary not eating", 
            // this.realiser.realise(p).getRealisation());

        }

        /**
         * Negation with WH movement for subject
         */

        [Test]
        public void testNegatedWHSubjQuestions()
        {
            var p = this.phraseFactory.createClause();
            p.setSubject("Mary");
            p.setVerb("eat");
            p.setObject(this.phraseFactory.createNounPhrase("the", "pie"));
            p.setFeature(Feature.NEGATED.ToString(), true);
            p.setFeature(Feature.INTERROGATIVE_TYPE.ToString(), InterrogativeType.WHO_SUBJECT);
            Assert.AreEqual("who does not eat the pie", 
                this.realiser.realise(p).getRealisation());
        }

        /**
         * Negation with WH movement for object
         */

        [Test]
        public void testNegatedWHObjQuestions()
        {
            var p = this.phraseFactory.createClause();
            p.setSubject("Mary");
            p.setVerb("eat");
            p.setObject(this.phraseFactory.createNounPhrase("the", "pie"));
            p.setFeature(Feature.NEGATED.ToString(), true);

            p.setFeature(Feature.INTERROGATIVE_TYPE.ToString(), InterrogativeType.WHAT_OBJECT);
            var realisation = this.realiser.realise(p);
            Assert.AreEqual("what does Mary not eat", 
                realisation.getRealisation());
        }

        /**
         * Test questyions in the tutorial.
         */

        [Test]
        public void testTutorialQuestions()
        {
            setUp();
            this.realiser.setLexicon(this.lexicon);

            var p = this.phraseFactory.createClause("Mary", "chase",  
                "George"); 
            p.setFeature(Feature.INTERROGATIVE_TYPE.ToString(), InterrogativeType.YES_NO);
            Assert.AreEqual("does Mary chase George", this.realiser.realise(p) 
                .getRealisation());

            p = this.phraseFactory.createClause("Mary", "chase",  
                "George"); 
            p.setFeature(Feature.INTERROGATIVE_TYPE.ToString(), InterrogativeType.WHO_OBJECT);
            Assert.AreEqual("who does Mary chase", this.realiser.realise(p) 
                .getRealisation());

        }

        /**
         * Subject WH Questions with modals
         */

        [Test]
        public void testModalWHSubjectQuestion()
        {
            var p = this.phraseFactory.createClause(this.dog, "upset",
                this.man);
            p.setFeature(Feature.TENSE.ToString(), Tense.PAST);
            Assert.AreEqual("the dog upset the man", this.realiser.realise(p)
                .getRealisation());

            // first without modal
            p.setFeature(Feature.INTERROGATIVE_TYPE.ToString(), InterrogativeType.WHO_SUBJECT);
            Assert.AreEqual("who upset the man", this.realiser.realise(p)
                .getRealisation());

            p.setFeature(Feature.INTERROGATIVE_TYPE.ToString(), InterrogativeType.WHAT_SUBJECT);
            Assert.AreEqual("what upset the man", this.realiser.realise(p)
                .getRealisation());

            // now with modal auxiliary
            p.setFeature(Feature.MODAL.ToString(), "may");

            p.setFeature(Feature.INTERROGATIVE_TYPE.ToString(), InterrogativeType.WHO_SUBJECT);
            Assert.AreEqual("who may have upset the man", this.realiser
                .realise(p).getRealisation());

            p.setFeature(Feature.TENSE.ToString(), Tense.FUTURE);
            Assert.AreEqual("who may upset the man", this.realiser.realise(p)
                .getRealisation());

            p.setFeature(Feature.TENSE.ToString(), Tense.PAST);
            p.setFeature(Feature.INTERROGATIVE_TYPE.ToString(), InterrogativeType.WHAT_SUBJECT);
            Assert.AreEqual("what may have upset the man", this.realiser
                .realise(p).getRealisation());

            p.setFeature(Feature.TENSE.ToString(), Tense.FUTURE);
            Assert.AreEqual("what may upset the man", this.realiser.realise(p)
                .getRealisation());
        }

        /**
         * Subject WH Questions with modals
         */

        [Test]
        public void testModalWHObjectQuestion()
        {
            var p = this.phraseFactory.createClause(this.dog, "upset",
                this.man);
            p.setFeature(Feature.TENSE.ToString(), Tense.PAST);
            p.setFeature(Feature.INTERROGATIVE_TYPE.ToString(), InterrogativeType.WHO_OBJECT);

            Assert.AreEqual("who did the dog upset", this.realiser.realise(p)
                .getRealisation());

            p.setFeature(Feature.MODAL.ToString(), "may");
            Assert.AreEqual("who may the dog have upset", this.realiser
                .realise(p).getRealisation());

            p.setFeature(Feature.INTERROGATIVE_TYPE.ToString(), InterrogativeType.WHAT_OBJECT);
            Assert.AreEqual("what may the dog have upset", this.realiser
                .realise(p).getRealisation());

            p.setFeature(Feature.TENSE.ToString(), Tense.FUTURE);
            p.setFeature(Feature.INTERROGATIVE_TYPE.ToString(), InterrogativeType.WHO_OBJECT);
            Assert.AreEqual("who may the dog upset", this.realiser.realise(p)
                .getRealisation());

            p.setFeature(Feature.INTERROGATIVE_TYPE.ToString(), InterrogativeType.WHAT_OBJECT);
            Assert.AreEqual("what may the dog upset", this.realiser.realise(p)
                .getRealisation());
        }

        /**
         * Questions with tenses requiring auxiliaries + subject WH
         */

        [Test]
        public void testAuxWHSubjectQuestion()
        {
            var p = this.phraseFactory.createClause(this.dog, "upset",
                this.man);
            p.setFeature(Feature.TENSE.ToString(), Tense.PRESENT);
            p.setFeature(Feature.PERFECT.ToString(), true);
            Assert.AreEqual("the dog has upset the man",
                this.realiser.realise(p).getRealisation());

            p.setFeature(Feature.INTERROGATIVE_TYPE.ToString(), InterrogativeType.WHO_SUBJECT);
            Assert.AreEqual("who has upset the man", this.realiser.realise(p)
                .getRealisation());

            p.setFeature(Feature.INTERROGATIVE_TYPE.ToString(), InterrogativeType.WHAT_SUBJECT);
            Assert.AreEqual("what has upset the man", this.realiser.realise(p)
                .getRealisation());
        }

        /**
         * Questions with tenses requiring auxiliaries + subject WH
         */

        [Test]
        public void testAuxWHObjectQuestion()
        {
            setUp();

            var p = this.phraseFactory.createClause(this.dog, "upset",
                this.man);

            // first without any aux
            p.setFeature(Feature.TENSE.ToString(), Tense.PAST);
            p.setFeature(Feature.INTERROGATIVE_TYPE.ToString(), InterrogativeType.WHAT_OBJECT);
            Assert.AreEqual("what did the dog upset", this.realiser.realise(p)
                .getRealisation());

            p.setFeature(Feature.INTERROGATIVE_TYPE.ToString(), InterrogativeType.WHO_OBJECT);
            Assert.AreEqual("who did the dog upset", this.realiser.realise(p)
                .getRealisation());

            p.setFeature(Feature.TENSE.ToString(), Tense.PRESENT);
            p.setFeature(Feature.PERFECT.ToString().ToString(), true);

            p.setFeature(Feature.INTERROGATIVE_TYPE.ToString(), InterrogativeType.WHO_OBJECT);
            Assert.AreEqual("who has the dog upset", this.realiser.realise(p)
                .getRealisation());

            p.setFeature(Feature.INTERROGATIVE_TYPE.ToString(), InterrogativeType.WHAT_OBJECT);
            Assert.AreEqual("what has the dog upset", this.realiser.realise(p)
                .getRealisation());

            p.setFeature(Feature.TENSE.ToString(), Tense.FUTURE);
            p.setFeature(Feature.PERFECT.ToString(), true);

            p.setFeature(Feature.INTERROGATIVE_TYPE.ToString(), InterrogativeType.WHO_OBJECT);
            Assert.AreEqual("who will the dog have upset", this.realiser
                .realise(p).getRealisation());

            p.setFeature(Feature.INTERROGATIVE_TYPE.ToString(), InterrogativeType.WHAT_OBJECT);
            Assert.AreEqual("what will the dog have upset", this.realiser
                .realise(p).getRealisation());

        }

        /**
         * Test for questions with "be"
         */

        [Test]
        public void testBeQuestions()
        {
            setUp();

            var p = this.phraseFactory.createClause(
                this.phraseFactory.createNounPhrase("a", "ball"),
                this.phraseFactory.createWord("be", new LexicalCategory_VERB()),
                this.phraseFactory.createNounPhrase("a", "toy"));

            p.setFeature(Feature.INTERROGATIVE_TYPE.ToString(), InterrogativeType.WHAT_OBJECT);
            Assert.AreEqual("what is a ball", this.realiser.realise(p)
                .getRealisation());

            p.setFeature(Feature.INTERROGATIVE_TYPE.ToString(), InterrogativeType.YES_NO);
            Assert.AreEqual("is a ball a toy", this.realiser.realise(p)
                .getRealisation());

            p.setFeature(Feature.INTERROGATIVE_TYPE.ToString(), InterrogativeType.WHAT_SUBJECT);
            Assert.AreEqual("what is a toy", this.realiser.realise(p)
                .getRealisation());

            var p2 = this.phraseFactory.createClause("Mary", "be",
                "beautiful");
            p2.setFeature(Feature.INTERROGATIVE_TYPE.ToString(), InterrogativeType.WHY);
            Assert.AreEqual("why is Mary beautiful", this.realiser.realise(p2)
                .getRealisation());

            p2.setFeature(Feature.INTERROGATIVE_TYPE.ToString(), InterrogativeType.WHERE);
            Assert.AreEqual("where is Mary beautiful", this.realiser.realise(p2)
                .getRealisation());

            p2.setFeature(Feature.INTERROGATIVE_TYPE.ToString(), InterrogativeType.WHO_SUBJECT);
            Assert.AreEqual("who is beautiful", this.realiser.realise(p2)
                .getRealisation());
        }

        /**
         * Test for questions with "be" in future tense
         */

        [Test]
        public void testBeQuestionsFuture()
        {
            setUp();

            var p = this.phraseFactory.createClause(
                this.phraseFactory.createNounPhrase("a", "ball"),
                this.phraseFactory.createWord("be", new LexicalCategory_VERB()),
                this.phraseFactory.createNounPhrase("a", "toy"));
            p.setFeature(Feature.TENSE.ToString(), Tense.FUTURE);

            p.setFeature(Feature.INTERROGATIVE_TYPE.ToString(), InterrogativeType.WHAT_OBJECT);
            Assert.AreEqual("what will a ball be", this.realiser.realise(p)
                .getRealisation());

            p.setFeature(Feature.INTERROGATIVE_TYPE.ToString(), InterrogativeType.YES_NO);
            Assert.AreEqual("will a ball be a toy", this.realiser.realise(p)
                .getRealisation());

            p.setFeature(Feature.INTERROGATIVE_TYPE.ToString(), InterrogativeType.WHAT_SUBJECT);
            Assert.AreEqual("what will be a toy", this.realiser.realise(p)
                .getRealisation());

            var p2 = this.phraseFactory.createClause("Mary", "be",
                "beautiful");
            p2.setFeature(Feature.TENSE.ToString(), Tense.FUTURE);
            p2.setFeature(Feature.INTERROGATIVE_TYPE.ToString(), InterrogativeType.WHY);
            Assert.AreEqual("why will Mary be beautiful", this.realiser
                .realise(p2).getRealisation());

            p2.setFeature(Feature.INTERROGATIVE_TYPE.ToString(), InterrogativeType.WHERE);
            Assert.AreEqual("where will Mary be beautiful", this.realiser
                .realise(p2).getRealisation());

            p2.setFeature(Feature.INTERROGATIVE_TYPE.ToString(), InterrogativeType.WHO_SUBJECT);
            Assert.AreEqual("who will be beautiful", this.realiser.realise(p2)
                .getRealisation());
        }

        /**
         * Tests for WH questions with be in past tense
         */

        [Test]
        public void testBeQuestionsPast()
        {
            setUp();

            var p = this.phraseFactory.createClause(
                this.phraseFactory.createNounPhrase("a", "ball"),
                this.phraseFactory.createWord("be", new LexicalCategory_VERB()),
                this.phraseFactory.createNounPhrase("a", "toy"));
            p.setFeature(Feature.TENSE.ToString(), Tense.PAST);

            p.setFeature(Feature.INTERROGATIVE_TYPE.ToString(), InterrogativeType.WHAT_OBJECT);
            Assert.AreEqual("what was a ball", this.realiser.realise(p)
                .getRealisation());

            p.setFeature(Feature.INTERROGATIVE_TYPE.ToString(), InterrogativeType.YES_NO);
            Assert.AreEqual("was a ball a toy", this.realiser.realise(p)
                .getRealisation());

            p.setFeature(Feature.INTERROGATIVE_TYPE.ToString(), InterrogativeType.WHAT_SUBJECT);
            Assert.AreEqual("what was a toy", this.realiser.realise(p)
                .getRealisation());

            var p2 = this.phraseFactory.createClause("Mary", "be",
                "beautiful");
            p2.setFeature(Feature.TENSE.ToString(), Tense.PAST);
            p2.setFeature(Feature.INTERROGATIVE_TYPE.ToString(), InterrogativeType.WHY);
            Assert.AreEqual("why was Mary beautiful", this.realiser.realise(p2)
                .getRealisation());

            p2.setFeature(Feature.INTERROGATIVE_TYPE.ToString(), InterrogativeType.WHERE);
            Assert.AreEqual("where was Mary beautiful", this.realiser.realise(p2)
                .getRealisation());

            p2.setFeature(Feature.INTERROGATIVE_TYPE.ToString(), InterrogativeType.WHO_SUBJECT);
            Assert.AreEqual("who was beautiful", this.realiser.realise(p2)
                .getRealisation());
        }


        /**
         * Test WHERE, HOW and WHY questions, with copular predicate "be"
         */

        public void testSimpleBeWHQuestions()
        {
            setUp();

            var p = this.phraseFactory.createClause("I", "be");

            p.setFeature(Feature.INTERROGATIVE_TYPE.ToString(), InterrogativeType.WHERE);
            Assert.AreEqual("Where am I?", this.realiser.realiseSentence(p));

            p.setFeature(Feature.INTERROGATIVE_TYPE.ToString(), InterrogativeType.WHY);
            Assert.AreEqual("Why am I?", this.realiser.realiseSentence(p));

            p.setFeature(Feature.INTERROGATIVE_TYPE.ToString(), InterrogativeType.HOW);
            Assert.AreEqual("How am I?", this.realiser.realiseSentence(p));

        }

        /**
         * Test a simple "how" question, based on query from Albi Oxa
         */

        [Test]
        public void testHowPredicateQuestion()
        {
            setUp();

            var test = this.phraseFactory.createClause();
            var subject = this.phraseFactory.createNounPhrase("You");

            subject.setFeature(Feature.PRONOMINAL.ToString(), true);
            subject.setFeature(Feature.PERSON.ToString(), Person.SECOND);
            test.setSubject(subject);
            test.setVerb("be");

            test.setFeature(Feature.INTERROGATIVE_TYPE.ToString(),
                InterrogativeType.HOW_PREDICATE);
            test.setFeature(Feature.TENSE.ToString(), Tense.PRESENT);

            var result = this.realiser.realiseSentence(test);
            Assert.AreEqual("How are you?", result);

        }

        /**
         * Case 1 checks that "What do you think about John?" can be generated.
         * 
         * Case 2 checks that the same clause is generated, even when an object is
         * declared.
         */

        [Test]
        public void testWhatObjectInterrogative()
        {
            var lexicon = Lexicon.getDefaultLexicon();
            var nlg = new NLGFactory(lexicon);
            var realiser = new Realiser(lexicon);

            // Case 1, no object is explicitly given:
            var clause = nlg.createClause("you", "think");
            var aboutJohn = nlg.createPrepositionPhrase("about", "John");
            clause.addPostModifier(aboutJohn);
            clause.setFeature(Feature.INTERROGATIVE_TYPE.ToString(),
                InterrogativeType.WHAT_OBJECT);
            var realisation = realiser.realiseSentence(clause);
             Assert.AreEqual("What do you think about John?", realisation);

            // Case 2:
            // Add "bad things" as the object so the object doesn't remain null:
            clause.setObject("bad things");
            realisation = realiser.realiseSentence(clause);
            Assert.AreEqual("What do you think about John?", realisation);
        }
    }
}