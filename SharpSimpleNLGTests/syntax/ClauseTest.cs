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
using SimpleNLG.Extensions;

namespace SimpleNLGTests.syntax
{
    // TODO: Auto-generated Javadoc
    /**
     * The Class STest.
     */

    public class ClauseTest : SimpleNLG4TestBase
    {
        /** The realiser. */
 
        /**
         * Initial test for basic sentences.
         */

        [Test]
        public void testBasic()
        {
           Assert.AreEqual("the woman kisses the man", this.realiser 
               .realise(s1).getRealisation());
            Assert.AreEqual("there is the dog on the rock", this.realiser 
                .realise(s2).getRealisation());
            
            setUp();
            Assert.AreEqual("the man gives the woman John's flower", 
                this.realiser.realise(s3).getRealisation());
            this.realiser.setDebugMode(true);
            Assert
                .AreEqual(
                    "however tomorrow Jane and Andrew will pick up the balls in the shop", 
                    this.realiser.realise(s4).getRealisation());
        }

        /**
         * Test did not
         */

        [Test]
        public void testDidNot()
        {
            SPhraseSpec o = this.phraseFactory.createClause("John", "eat");
            o.setFeature(Feature.TENSE.ToString(), Tense.PAST);
            o.setFeature(Feature.NEGATED.ToString(), true);
            this.realiser.setLexicon(this.lexicon);
            Assert.AreEqual("John did not eat", this.realiser.realise(o).getRealisation());
        }

        /**
         * Test did not
         */

        [Test]
        public void testVPNegation()
        {
            // negate the VP
            var vp = this.phraseFactory.createVerbPhrase("lie");
            vp.setFeature(Feature.TENSE.ToString(), Tense.PAST);
            vp.setFeature(Feature.NEGATED.ToString(), true);
            var compl = this.phraseFactory.createVerbPhrase("etherize");
            compl.setFeature(Feature.TENSE.ToString(), Tense.PAST);
            vp.setComplement(compl);

            var s = this.phraseFactory.createClause(this.phraseFactory
                .createNounPhrase("the", "patient"), vp);

            Assert.AreEqual("the patient did not lie etherized", 
                this.realiser.realise(s).getRealisation());

        }

        /**
         * Test that pronominal args are being correctly cast as NPs.
         */

        [Test]
        public void testPronounArguments()
        {
            // the subject of s2 should have been cast into a pronominal NP
            var subj = s2.getFeatureAsElementList(
                InternalFeature.SUBJECTS.ToString()).get(0);
            Assert.IsTrue(subj.isA(PhraseCategoryEnum.NOUN_PHRASE));
            // Assert.assertTrue(LexicalCategory.PRONOUN.AreEqual(((PhraseElement)
            // subj)
            // .getCategory()));
        }

        /**
         * Tests for setting tense, aspect and passive from the sentence interface.
         */

        [Test]
        public void testTenses()
        {
            // simple past
            s3.setFeature(Feature.TENSE.ToString(), Tense.PAST);
            Assert.AreEqual("the man gave the woman John's flower", 
                this.realiser.realise(s3).getRealisation());

            // perfect
            s3.setFeature(Feature.PERFECT.ToString(), true);
            Assert.AreEqual("the man had given the woman John's flower", 
                this.realiser.realise(s3).getRealisation());

            // negation
            s3.setFeature(Feature.NEGATED.ToString(), true);
            Assert.AreEqual("the man had not given the woman John's flower", 
                this.realiser.realise(s3).getRealisation());

            s3.setFeature(Feature.PROGRESSIVE.ToString(), true);
            Assert.AreEqual(
                "the man had not been giving the woman John's flower", 
                this.realiser.realise(s3).getRealisation());

            // passivisation with direct and indirect object
            // s3.setFeature(Feature.PASSIVE.ToString(), true);
            // Assert.AreEqual(
            //				"John's flower had not been being given the woman by the man", 
            // realiser.realise(s3).getRealisation());
        }

        /**
         * Test what happens when a sentence is subordinated as complement of a
         * verb.
         */

        [Test]
        public void testSubordination()
        {

            // subordinate sentence by setting it as complement of a verb
            this.say.addComplement(s3);

            // check the getter
            Assert.AreEqual(ClauseStatus.SUBORDINATE, s3
                .getFeature(InternalFeature.CLAUSE_STATUS.ToString()));

            // check realisation
            Assert.AreEqual("says that the man gives the woman John's flower", 
                this.realiser.realise(this.say).getRealisation());
        }

        /**
         * Test the various forms of a sentence, including subordinates.
         */
        /**
         *
         */

        [Test]
        public void testForm()
        {

            // check the getter method
            Assert.AreEqual(Form.NORMAL, s1.getFeatureAsElement(
                InternalFeature.VERB_PHRASE.ToString()).getFeature(Feature.FORM.ToString()));

            // infinitive
            s1.setFeature(Feature.FORM.ToString(), Form.INFINITIVE);
            Assert
                .AreEqual(
                    "to kiss the man", this.realiser.realise(s1).getRealisation());

            // gerund with "there"
            s2.setFeature(Feature.FORM.ToString(), Form.GERUND);
            Assert.AreEqual("there being the dog on the rock", this.realiser
                .realise(s2).getRealisation());

            // gerund with possessive
            s3.setFeature(Feature.FORM.ToString(), Form.GERUND);
            Assert.AreEqual("the man's giving the woman John's flower",
                this.realiser.realise(s3).getRealisation());

            // imperative
            // the man gives the woman John's flower
            s3 = this.phraseFactory.createClause();

            s3.setVerbPhrase(this.give);
            s3.setFeature(Feature.FORM.ToString(), Form.IMPERATIVE);

            Assert.AreEqual("give the woman John's flower", this.realiser
                .realise(s3).getRealisation());
 


            this.say = this.phraseFactory.createVerbPhrase("say");
            this.say.addComplement(s3);
            this.realiser.setDebugMode(true);
            Assert.AreEqual("says to give the woman John's flower", 
                this.realiser.realise(this.say).getRealisation());

            // imperative -- case II
            s4.setFeature(Feature.FORM.ToString(), Form.IMPERATIVE);
            Assert.AreEqual("however tomorrow pick up the balls in the shop", 
                this.realiser.realise(s4).getRealisation());

            // infinitive -- case II
            s4 = this.phraseFactory.createClause();
            s4.setFeature(Feature.CUE_PHRASE.ToString(), "however"); 
            s4.addFrontModifier("tomorrow"); 

            var subject = new CoordinatedPhraseElement(
                this.phraseFactory.createNounPhrase("Jane"), this.phraseFactory 
                    .createNounPhrase("Andrew")); 

            s4.setFeature(InternalFeature.SUBJECTS.ToString(), subject);

            var pick = this.phraseFactory.createVerbPhrase("pick up"); 
            s4.setFeature(InternalFeature.VERB_PHRASE.ToString(), pick);
            s4.setObject("the balls"); 
            s4.addPostModifier("in the shop"); 
            s4.setFeature(Feature.TENSE.ToString(), Tense.FUTURE);
            s4.setFeature(Feature.FORM.ToString(), Form.INFINITIVE);
            Assert.AreEqual(
                "however to pick up the balls in the shop tomorrow", 
                this.realiser.realise(s4).getRealisation());
        }

        /**
         * Slightly more complex tests for forms.
         */

        [Test]
        public void testForm2()
        {
            // set s4 as subject of a new sentence
            var temp = this.phraseFactory.createClause(s4, "be", 
                "recommended"); 

            Assert.AreEqual(
                "however tomorrow Jane and Andrew's picking up the " + 
                "balls in the shop is recommended", 
                this.realiser.realise(temp).getRealisation());

            // compose this with a new sentence
            // ER - switched direct and indirect object in sentence
            var temp2 = this.phraseFactory.createClause("I", "tell", temp);  
            temp2.setFeature(Feature.TENSE.ToString(), Tense.FUTURE);

            var indirectObject = this.phraseFactory
                .createNounPhrase("John"); 

            temp2.setIndirectObject(indirectObject);

            Assert.AreEqual("I will tell John that however tomorrow Jane and " + 
                          "Andrew's picking up the balls in the shop is " + 
                          "recommended", 
                this.realiser.realise(temp2).getRealisation());

            // turn s4 to imperative and put it in indirect object position

            s4 = this.phraseFactory.createClause();
            s4.setFeature(Feature.CUE_PHRASE.ToString(), "however"); 
            s4.addFrontModifier("tomorrow"); 

            var subject = new CoordinatedPhraseElement(
                this.phraseFactory.createNounPhrase("Jane"), this.phraseFactory 
                    .createNounPhrase("Andrew")); 

            s4.setSubject(subject);

            var pick = this.phraseFactory.createVerbPhrase("pick up"); 
            s4.setVerbPhrase(pick);
            s4.setObject("the balls"); 
            s4.addPostModifier("in the shop"); 
            s4.setFeature(Feature.TENSE.ToString(), Tense.FUTURE);
            s4.setFeature(Feature.FORM.ToString(), Form.IMPERATIVE);

            temp2 = this.phraseFactory.createClause("I", "tell", s4);  
            indirectObject = this.phraseFactory.createNounPhrase("John"); 
            temp2.setIndirectObject(indirectObject);
            temp2.setFeature(Feature.TENSE.ToString(), Tense.FUTURE);

            Assert.AreEqual("I will tell John however to pick up the balls " 
                          + "in the shop tomorrow", this.realiser.realise(temp2) 
                .getRealisation());
        }

        /**
         * Tests for gerund forms and genitive subjects.
         */

        [Test]
        public void testGerundsubject()
        {

            // the man's giving the woman John's flower upset Peter
            var _s4 = this.phraseFactory.createClause();
            _s4.setVerbPhrase(this.phraseFactory.createVerbPhrase("upset")); 
            _s4.setFeature(Feature.TENSE.ToString(), Tense.PAST);
            _s4.setObject(this.phraseFactory.createNounPhrase("Peter")); 
            s3.setFeature(Feature.PERFECT.ToString(), true);

            // set the sentence as subject of another: makes it a gerund
            _s4.setSubject(s3);

            // suppress the genitive realisation of the NP subject in gerund
            // sentences
            s3.setFeature(Feature.SUPPRESS_GENITIVE_IN_GERUND.ToString(), true);

            // check the realisation: subject should not be genitive
            Assert.AreEqual(
                "the man having given the woman John's flower upset Peter", 
                this.realiser.realise(_s4).getRealisation());

        }

        /**
         * Some tests for multiple embedded sentences.
         */

        [Test]
        public void testComplexSentence1()
        {
            setUp();
            // the man's giving the woman John's flower upset Peter
            var complexS = this.phraseFactory.createClause();
            complexS.setVerbPhrase(this.phraseFactory.createVerbPhrase("upset")); 
            complexS.setFeature(Feature.TENSE.ToString(), Tense.PAST);
            complexS.setObject(this.phraseFactory.createNounPhrase("Peter")); 
            s3.setFeature(Feature.PERFECT.ToString(), true);
            complexS.setSubject(s3);

            // check the realisation: subject should be genitive
            Assert.AreEqual(
                "the man's having given the woman John's flower upset Peter", 
                this.realiser.realise(complexS).getRealisation());

            setUp();
            // coordinate sentences in subject position
            var s5 = this.phraseFactory.createClause();
            s5.setSubject(this.phraseFactory.createNounPhrase("some", "person"));  
            s5.setVerbPhrase(this.phraseFactory.createVerbPhrase("stroke")); 
            s5.setObject(this.phraseFactory.createNounPhrase("the", "cat"));  

            var coord = new CoordinatedPhraseElement(s3,
                s5);
            complexS = this.phraseFactory.createClause();
            complexS.setVerbPhrase(this.phraseFactory.createVerbPhrase("upset")); 
            complexS.setFeature(Feature.TENSE.ToString(), Tense.PAST);
            complexS.setObject(this.phraseFactory.createNounPhrase("Peter")); 
            complexS.setSubject(coord);
            s3.setFeature(Feature.PERFECT.ToString(), true);

            Assert.AreEqual("the man's having given the woman John's flower " 
                          + "and some person's stroking the cat upset Peter", 
                this.realiser.realise(complexS).getRealisation());

            setUp();
            // now subordinate the complex sentence
            // coord.setClauseStatus(SPhraseSpec.ClauseType.MAIN);
            var s6 = this.phraseFactory.createClause();
            s6.setVerbPhrase(this.phraseFactory.createVerbPhrase("tell")); 
            s6.setFeature(Feature.TENSE.ToString(), Tense.PAST);
            s6.setSubject(this.phraseFactory.createNounPhrase("the", "boy"));  
            // ER - switched indirect and direct object
            var indirect = this.phraseFactory.createNounPhrase("every", 
                "girl"); 
            s6.setIndirectObject(indirect);
            complexS = this.phraseFactory.createClause();
            complexS.setVerbPhrase(this.phraseFactory.createVerbPhrase("upset")); 
            complexS.setFeature(Feature.TENSE.ToString(), Tense.PAST);
            complexS.setObject(this.phraseFactory.createNounPhrase("Peter")); 
            s6.setObject(complexS);
            coord = new CoordinatedPhraseElement(s3, s5);
            complexS.setSubject(coord);
            s3.setFeature(Feature.PERFECT.ToString(), true);
            Assert.AreEqual(
                "the boy told every girl that the man's having given the woman " 
                + "John's flower and some person's stroking the cat " 
                + "upset Peter", 
                this.realiser.realise(s6).getRealisation());

        }

        /**
         * More coordination tests.
         */

        [Test]
        public void testComplexSentence3()
        {
            setUp();

            s1 = this.phraseFactory.createClause();
            s1.setSubject(this.woman);
            s1.setVerb("kiss");
            s1.setObject(this.man);

            var _man = this.phraseFactory.createNounPhrase("the", "man");  
            s3 = this.phraseFactory.createClause();
            s3.setSubject(_man);
            s3.setVerb("give");

            var flower = this.phraseFactory.createNounPhrase("flower"); 
            var john = this.phraseFactory.createNounPhrase("John"); 
            john.setFeature(Feature.POSSESSIVE.ToString(), true);
            flower.setSpecifier(john);
            s3.setObject(flower);

            var _woman = this.phraseFactory.createNounPhrase(
                "the", "woman");  
            s3.setIndirectObject(_woman);

            // the coordinate sentence allows us to raise and lower complementiser
            var coord2 = new CoordinatedPhraseElement(s1,
                s3);
            coord2.setFeature(Feature.TENSE.ToString(), Tense.PAST);

            this.realiser.setDebugMode(true);
            Assert
                .AreEqual(
                    "the woman kissed the man and the man gave the woman John's flower", 
                    this.realiser.realise(coord2).getRealisation());
        }

        // /**
        // * Sentence with clausal subject with verb "be" and a progressive feature
        // */
        // public void testComplexSentence2() {
        // SPhraseSpec subject = phraseFactory.createClause(
        // phraseFactory.createNounPhrase("the", "child"),
        // phraseFactory.createVerbPhrase("be"), phraseFactory
        // .createWord("difficult", LexicalCategory.ADJECTIVE));
        // subject.setFeature(Feature.PROGRESSIVE, true);
        // }

        /**
         * Tests recogition of strings in API.
         */

        [Test]
        public void testStringRecognition()
        {

            // test recognition of forms of "be"
            var _s1 = this.phraseFactory.createClause(
                "my cat", "be", "sad");   
            Assert.AreEqual(
                "my cat is sad", this.realiser.realise(_s1).getRealisation()); 

            // test recognition of pronoun for afreement
            var _s2 = this.phraseFactory
                .createClause("I", "want", "Mary");   

            Assert.AreEqual(
                "I want Mary", this.realiser.realise(_s2).getRealisation()); 

            // test recognition of pronoun for correct form
            var subject = this.phraseFactory.createNounPhrase("dog"); 
            subject.setFeature(InternalFeature.SPECIFIER.ToString(), "a"); 
            subject.addPostModifier("from next door"); 
            var obj = this.phraseFactory.createNounPhrase("I"); 
            var s = this.phraseFactory.createClause(subject,
                "chase", obj); 
            s.setFeature(Feature.PROGRESSIVE.ToString(), true);
            Assert.AreEqual("a dog from next door is chasing me", 
                this.realiser.realise(s).getRealisation());
        }

        /**
         * Tests complex agreement.
         */

        [Test]
        public void testAgreement()
        {

            // basic agreement
            var np = this.phraseFactory.createNounPhrase("dog"); 
            np.setSpecifier("the"); 
            np.addPreModifier("angry"); 
            var _s1 = this.phraseFactory
                .createClause(np, "chase", "John");  
            Assert.AreEqual("the angry dog chases John", this.realiser 
                .realise(_s1).getRealisation());

            // plural
            np = this.phraseFactory.createNounPhrase("dog"); 
            np.setSpecifier("the"); 
            np.addPreModifier("angry"); 
            np.setFeature(Feature.NUMBER.ToString(), NumberAgreement.PLURAL);
            _s1 = this.phraseFactory.createClause(np, "chase", "John");  
            Assert.AreEqual("the angry dogs chase John", this.realiser 
                .realise(_s1).getRealisation());

            // test agreement with "there is"
            np = this.phraseFactory.createNounPhrase("dog"); 
            np.addPreModifier("angry"); 
            np.setFeature(Feature.NUMBER.ToString(), NumberAgreement.SINGULAR);
            np.setSpecifier("a"); 
            var _s2 = this.phraseFactory.createClause("there", "be", np);  
            Assert.AreEqual("there is an angry dog", this.realiser 
                .realise(_s2).getRealisation());

            // plural with "there"
            np = this.phraseFactory.createNounPhrase("dog"); 
            np.addPreModifier("angry"); 
            np.setSpecifier("a"); 
            np.setFeature(Feature.NUMBER.ToString(), NumberAgreement.PLURAL);
            _s2 = this.phraseFactory.createClause("there", "be", np);  
            Assert.AreEqual("there are some angry dogs", this.realiser 
                .realise(_s2).getRealisation());
        }

        /**
         * Tests passive.
         */

        [Test]
        public void testPassive()
        {
            // passive with just complement
            var _s1 = this.phraseFactory.createClause(null,
                    "intubate", this.phraseFactory.createNounPhrase("the", "baby"));
                  
            _s1.setFeature(Feature.PASSIVE.ToString(), true);

            Assert.AreEqual("the baby is intubated", this.realiser 
                .realise(_s1).getRealisation());

            // passive with subject and complement
            _s1 = this.phraseFactory.createClause(null,
                    "intubate", this.phraseFactory.createNounPhrase("the", "baby"));
                  
            _s1.setSubject(this.phraseFactory.createNounPhrase("the nurse")); 
            _s1.setFeature(Feature.PASSIVE.ToString(), true);
            Assert.AreEqual("the baby is intubated by the nurse", 
                this.realiser.realise(_s1).getRealisation());

            // passive with subject and indirect object
            var _s2 = this.phraseFactory.createClause(null, "give", 
                this.phraseFactory.createNounPhrase("the", "baby"));  

            var morphine = this.phraseFactory
                .createNounPhrase("50ug of morphine"); 
            _s2.setIndirectObject(morphine);
            _s2.setFeature(Feature.PASSIVE.ToString(), true);
            Assert.AreEqual("the baby is given 50ug of morphine", 
                this.realiser.realise(_s2).getRealisation());

            // passive with subject, complement and indirect object
            _s2 = this.phraseFactory.createClause(this.phraseFactory
                    .createNounPhrase("the", "nurse"), "give",   
                this.phraseFactory.createNounPhrase("the", "baby"));  

            morphine = this.phraseFactory.createNounPhrase("50ug of morphine"); 
            _s2.setIndirectObject(morphine);
            _s2.setFeature(Feature.PASSIVE.ToString(), true);
            Assert.AreEqual("the baby is given 50ug of morphine by the nurse", 
                this.realiser.realise(_s2).getRealisation());

            // test agreement in passive
            var _s3 = this.phraseFactory.createClause(
                new CoordinatedPhraseElement("my dog", "your cat"), "chase",   
                "George"); 
            _s3.setFeature(Feature.TENSE.ToString(), Tense.PAST);
            _s3.addFrontModifier("yesterday"); 
            Assert.AreEqual("yesterday my dog and your cat chased George", 
                this.realiser.realise(_s3).getRealisation());

            _s3 = this.phraseFactory.createClause(new CoordinatedPhraseElement(
                    "my dog", "your cat"), "chase",   
                this.phraseFactory.createNounPhrase("George")); 
            _s3.setFeature(Feature.TENSE.ToString(), Tense.PAST);
            _s3.addFrontModifier("yesterday"); 
            _s3.setFeature(Feature.PASSIVE.ToString(), true);
            Assert.AreEqual(
                "yesterday George was chased by my dog and your cat", 
                this.realiser.realise(_s3).getRealisation());

            // test correct pronoun forms
            var _s4 = this.phraseFactory.createClause(this.phraseFactory
                    .createNounPhrase("he"), "chase",  
                this.phraseFactory.createNounPhrase("I")); 
            Assert.AreEqual("he chases me", this.realiser.realise(_s4) 
                .getRealisation());
            _s4 = this.phraseFactory
                    .createClause(
                        this.phraseFactory.createNounPhrase("he"), "chase", this.phraseFactory.createNounPhrase("me"));
                  
            _s4.setFeature(Feature.PASSIVE.ToString(), true);
            Assert
                .AreEqual(
                    "I am chased by him", this.realiser.realise(_s4).getRealisation()); 

            // same thing, but giving the S constructor "me". Should recognise
            // correct pro
            // anyway
            var _s5 = this.phraseFactory
                .createClause("him", "chase", "I");   
            Assert.AreEqual(
                "he chases me", this.realiser.realise(_s5).getRealisation()); 

            _s5 = this.phraseFactory.createClause("him", "chase", "I");   
            _s5.setFeature(Feature.PASSIVE.ToString(), true);
            Assert
                .AreEqual(
                    "I am chased by him", this.realiser.realise(_s5).getRealisation()); 
        }

        /**
         * Test that complements set within the VP are raised when sentence is
         * passivised.
         */

        [Test]
        public void testPassiveWithInternalVPComplement()
        {
            var vp = this.phraseFactory.createVerbPhrase(this.phraseFactory
                .createWord("upset", new LexicalCategory_VERB()));
            vp.addComplement(this.phraseFactory.createNounPhrase("the", "man"));
            var _s6 = this.phraseFactory.createClause(this.phraseFactory
                .createNounPhrase("the", "child"), vp);
            _s6.setFeature(Feature.TENSE.ToString(), Tense.PAST);
            Assert.AreEqual("the child upset the man", this.realiser.realise(
                _s6).getRealisation());

            _s6.setFeature(Feature.PASSIVE.ToString(), true);
            Assert.AreEqual("the man was upset by the child", this.realiser
                .realise(_s6).getRealisation());
        }

        /**
         * Tests tenses with modals.
         */

        [Test]
        public void testModal()
        {

            setUp();
            // simple modal in present tense
            s3.setFeature(Feature.MODAL.ToString(), "should"); 
            Assert.AreEqual("the man should give the woman John's flower", 
                this.realiser.realise(s3).getRealisation());

            // modal + future -- uses present
            setUp();
            s3.setFeature(Feature.MODAL.ToString(), "should"); 
            s3.setFeature(Feature.TENSE.ToString(), Tense.FUTURE);
            Assert.AreEqual("the man should give the woman John's flower", 
                this.realiser.realise(s3).getRealisation());

            // modal + present progressive
            setUp();
            s3.setFeature(Feature.MODAL.ToString(), "should"); 
            s3.setFeature(Feature.TENSE.ToString(), Tense.FUTURE);
            s3.setFeature(Feature.PROGRESSIVE.ToString(), true);
            Assert.AreEqual("the man should be giving the woman John's flower", 
                this.realiser.realise(s3).getRealisation());

            // modal + past tense
            setUp();
            s3.setFeature(Feature.MODAL.ToString(), "should"); 
            s3.setFeature(Feature.TENSE.ToString(), Tense.PAST);
            Assert.AreEqual(
                "the man should have given the woman John's flower", 
                this.realiser.realise(s3).getRealisation());

            // modal + past progressive
            setUp();
            s3.setFeature(Feature.MODAL.ToString(), "should"); 
            s3.setFeature(Feature.TENSE.ToString(), Tense.PAST);
            s3.setFeature(Feature.PROGRESSIVE.ToString(), true);

            Assert.AreEqual(
                "the man should have been giving the woman John's flower", 
                this.realiser.realise(s3).getRealisation());

        }

        /**
         * Test for passivisation with mdoals
         */

        [Test]
        public void testModalWithPassive()
        {
            var obj = this.phraseFactory.createNounPhrase("the",
                "pizza");
            var post = this.phraseFactory.createAdjectivePhrase("good");
            var aso  = this.phraseFactory.createAdverbPhrase("as");
            aso.addComplement(post);
            var verb = this.phraseFactory.createVerbPhrase("classify");
            verb.addPostModifier( aso );
            verb.addComplement(obj);
            var s = this.phraseFactory.createClause();
            s.setVerbPhrase(verb);
            s.setFeature(Feature.MODAL.ToString(), "can");
            // s.setFeature(Feature.FORM, Form.INFINITIVE);
            s.setFeature(Feature.PASSIVE.ToString(), true);
            Assert.AreEqual("the pizza can be classified as good",
                this.realiser.realise(s).getRealisation());
        }

        [Test]
        public void testPassiveWithPPCompl()
        {
            // passive with just complement
            var subject = this.phraseFactory.createNounPhrase("wave");
            subject.setFeature(Feature.NUMBER.ToString(), NumberAgreement.PLURAL);
            var obj = this.phraseFactory.createNounPhrase("surfer");
            obj.setFeature(Feature.NUMBER.ToString(), NumberAgreement.PLURAL);

            var _s1 = this.phraseFactory.createClause(subject,
                "carry", obj);   

            // add a PP complement
            var pp = this.phraseFactory.createPrepositionPhrase("to",
                this.phraseFactory.createNounPhrase("the", "shore"));
            pp.setFeature(InternalFeature.DISCOURSE_FUNCTION.ToString(),
                DiscourseFunction.INDIRECT_OBJECT);
            _s1.addComplement(pp);

            _s1.setFeature(Feature.PASSIVE.ToString(), true);
            this.realiser.setDebugMode(true);
            Assert.AreEqual(
                "surfers are carried to the shore by waves", this.realiser 
                    .realise(_s1).getRealisation());
        }

        [Test]
        public void testPassiveWithPPMod()
        {
            // passive with just complement
            var subject = this.phraseFactory.createNounPhrase("wave");
            subject.setFeature(Feature.NUMBER.ToString(), NumberAgreement.PLURAL);
            var obj = this.phraseFactory.createNounPhrase("surfer");
            obj.setFeature(Feature.NUMBER.ToString(), NumberAgreement.PLURAL);

            var _s1 = this.phraseFactory.createClause(subject,
                "carry", obj);   

            // add a PP complement
            var pp = this.phraseFactory.createPrepositionPhrase("to",
                this.phraseFactory.createNounPhrase("the", "shore"));
            _s1.addPostModifier(pp);

            _s1.setFeature(Feature.PASSIVE.ToString(), true);

            Assert.AreEqual(
                "surfers are carried to the shore by waves", this.realiser 
                    .realise(_s1).getRealisation());
        }

        [Test]
        public void testCuePhrase()
        {
            var subject = this.phraseFactory.createNounPhrase("wave");
            subject.setFeature(Feature.NUMBER.ToString(), NumberAgreement.PLURAL);
            var obj = this.phraseFactory.createNounPhrase("surfer");
            obj.setFeature(Feature.NUMBER.ToString(), NumberAgreement.PLURAL);

            var _s1 = this.phraseFactory.createClause(subject,
                "carry", obj);   

            // add a PP complement
            var pp = this.phraseFactory.createPrepositionPhrase("to",
                this.phraseFactory.createNounPhrase("the", "shore"));
            _s1.addPostModifier(pp);

            _s1.setFeature(Feature.PASSIVE.ToString(), true);

            _s1.addFrontModifier("however");


            //without comma separation of cue phrase
            Assert.AreEqual(
                "however surfers are carried to the shore by waves", this.realiser 
                    .realise(_s1).getRealisation());

            //with comma separation
            this.realiser.setCommaSepCuephrase(true);
            Assert.AreEqual(
                "however, surfers are carried to the shore by waves", this.realiser 
                    .realise(_s1).getRealisation());
        }


        /**
         * Check that setComplement replaces earlier complements
         */

        [Test]
        public void setComplementTest()
        {
            var s = this.phraseFactory.createClause();
            s.setSubject("I");
            s.setVerb("see");
            s.setObject("a dog");

            Assert.AreEqual("I see a dog", this.realiser.realise(s)
                .getRealisation());

            s.setObject("a cat");
            Assert.AreEqual("I see a cat", this.realiser.realise(s)
                .getRealisation());

            s.setObject("a wolf");
            Assert.AreEqual("I see a wolf", this.realiser.realise(s)
                .getRealisation());

        }


        /**
         * Test for subclauses involving WH-complements Based on a query by Owen
         * Bennett
         */

        [Test]
        public void subclausesTest()
        {
            // Once upon a time, there was an Accountant, called Jeff, who lived in
            // a forest.

            // main sentence
            var acct = this.phraseFactory.createNounPhrase("a",
                "accountant");

            // first postmodifier of "an accountant"
            var sub1 = this.phraseFactory.createVerbPhrase("call");
            sub1.addComplement("Jeff");
            sub1.setFeature(Feature.FORM.ToString(), Form.PAST_PARTICIPLE);
            // this is an appositive modifier, which makes simplenlg put it between
            // commas
            sub1.setFeature(Feature.APPOSITIVE.ToString(), true);
            acct.addPostModifier(sub1);

            // second postmodifier of "an accountant" is "who lived in a forest"
            var sub2 = this.phraseFactory.createClause();
            var subvp = this.phraseFactory.createVerbPhrase("live");
            subvp.setFeature(Feature.TENSE.ToString(), Tense.PAST);
            subvp.setComplement(this.phraseFactory.createPrepositionPhrase("in",
                this.phraseFactory.createNounPhrase("a", "forest")));
            sub2.setVerbPhrase(subvp);
            // simplenlg can't yet handle wh-clauses in NPs, so we need to hack it
            // by setting the subject to "who"
            sub2.setSubject("who");
            acct.addPostModifier(sub2);

            // main sentence
            var s = this.phraseFactory.createClause("there", "be", acct);
            s.setFeature(Feature.TENSE.ToString(), Tense.PAST);

            // add front modifier "once upon a time"
            s.addFrontModifier("once upon a time");

            Assert.AreEqual(
                "once upon a time there was an accountant, called Jeff, who lived in a forest",
                this.realiser.realise(s).getRealisation());

        }
    }

}
