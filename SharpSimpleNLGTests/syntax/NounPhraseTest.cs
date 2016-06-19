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
using NUnit.Framework;
using SimpleNLG;

namespace SimpleNLGTests.syntax
{
    /**
     * Tests for the NPPhraseSpec and CoordinateNPPhraseSpec classes.
     * 
     * @author agatt
     */

    public class NounPhraseTest : SimpleNLG4TestBase
    {


        /**
         * Test the setPlural() method in noun phrases.
         */

        [Test]
        public void testPlural()
        {
            this.np4.setFeature(Feature.NUMBER.ToString(), NumberAgreement.PLURAL);
            Assert.AreEqual(
                    "the rocks", this.realiser.realise(this.np4).getRealisation());
                

            this.np5.setFeature(Feature.NUMBER.ToString(), NumberAgreement.PLURAL);
            Assert
                .AreEqual(
                    "the curtains", this.realiser.realise(this.np5).getRealisation());
                

            this.np5.setFeature(Feature.NUMBER.ToString(), NumberAgreement.SINGULAR);
            Assert.AreEqual(NumberAgreement.SINGULAR, this.np5
                .getFeature(Feature.NUMBER.ToString()));
            Assert
                .AreEqual(
                    "the curtain", this.realiser.realise(this.np5).getRealisation());
                

            this.np5.setFeature(Feature.NUMBER.ToString(), NumberAgreement.PLURAL);
            Assert
                .AreEqual(
                    "the curtains", this.realiser.realise(this.np5).getRealisation());
                
        }

        /**
         * Test the pronominalisation method for full NPs.
         */

        [Test]
        public void testPronominalisation()
        {
            // sing
            this.proTest1.setFeature(LexicalFeature.GENDER, Gender.FEMININE);
            this.proTest1.setFeature(Feature.PRONOMINAL.ToString(), true);
            Assert.AreEqual(
                    "she", this.realiser.realise(this.proTest1).getRealisation());
                

            // sing, possessive
            this.proTest1.setFeature(Feature.POSSESSIVE.ToString(), true);
            Assert.AreEqual(
                    "her", this.realiser.realise(this.proTest1).getRealisation());
                

            // plural pronoun
            this.proTest2.setFeature(Feature.NUMBER.ToString(), NumberAgreement.PLURAL);
            this.proTest2.setFeature(Feature.PRONOMINAL.ToString(), true);
            Assert.AreEqual(
                    "they", this.realiser.realise(this.proTest2).getRealisation());
                

            // accusative: "them"
            this.proTest2.setFeature(InternalFeature.DISCOURSE_FUNCTION.ToString(),
                DiscourseFunction.OBJECT);
            Assert.AreEqual(
                    "them", this.realiser.realise(this.proTest2).getRealisation());
                
        }

        /**
         * Test the pronominalisation method for full NPs (more thorough than above)
         */

        [Test]
        public void testPronominalisation2()
        {
            // Ehud - added extra pronominalisation tests
            var pro = this.phraseFactory.createNounPhrase("Mary");
            pro.setFeature(Feature.PRONOMINAL.ToString(), true);
            pro.setFeature(Feature.PERSON.ToString(), Person.FIRST);
            var sent = this.phraseFactory.createClause(pro, "like", "John");
            Assert
                .AreEqual("I like John.", this.realiser
                    .realiseSentence(sent));

            pro = this.phraseFactory.createNounPhrase("Mary");
            pro.setFeature(Feature.PRONOMINAL.ToString(), true);
            pro.setFeature(Feature.PERSON.ToString(), Person.SECOND);
            sent = this.phraseFactory.createClause(pro, "like", "John");
            Assert.AreEqual("You like John.", this.realiser
                .realiseSentence(sent));

            pro = this.phraseFactory.createNounPhrase("Mary");
            pro.setFeature(Feature.PRONOMINAL.ToString(), true);
            pro.setFeature(Feature.PERSON.ToString(), Person.THIRD);
            pro.setFeature(LexicalFeature.GENDER, Gender.FEMININE);
            sent = this.phraseFactory.createClause(pro, "like", "John");
            Assert.AreEqual("She likes John.", this.realiser
                .realiseSentence(sent));

            pro = this.phraseFactory.createNounPhrase("Mary");
            pro.setFeature(Feature.PRONOMINAL.ToString(), true);
            pro.setFeature(Feature.PERSON.ToString(), Person.FIRST);
            pro.setPlural(true);
            sent = this.phraseFactory.createClause(pro, "like", "John");
            Assert.AreEqual("We like John.", this.realiser
                .realiseSentence(sent));

            pro = this.phraseFactory.createNounPhrase("Mary");
            pro.setFeature(Feature.PRONOMINAL.ToString(), true);
            pro.setFeature(Feature.PERSON.ToString(), Person.SECOND);
            pro.setPlural(true);
            sent = this.phraseFactory.createClause(pro, "like", "John");
            Assert.AreEqual("You like John.", this.realiser
                .realiseSentence(sent));

            pro = this.phraseFactory.createNounPhrase("Mary");
            pro.setFeature(Feature.PRONOMINAL.ToString(), true);
            pro.setFeature(Feature.PERSON.ToString(), Person.THIRD);
            pro.setPlural(true);
            pro.setFeature(LexicalFeature.GENDER, Gender.FEMININE);
            sent = this.phraseFactory.createClause(pro, "like", "John");
            Assert.AreEqual("They like John.", this.realiser
                .realiseSentence(sent));

            pro = this.phraseFactory.createNounPhrase("John");
            pro.setFeature(Feature.PRONOMINAL.ToString(), true);
            pro.setFeature(Feature.PERSON.ToString(), Person.FIRST);
            sent = this.phraseFactory.createClause("Mary", "like", pro);
            Assert.AreEqual("Mary likes me.", this.realiser
                .realiseSentence(sent));

            pro = this.phraseFactory.createNounPhrase("John");
            pro.setFeature(Feature.PRONOMINAL.ToString(), true);
            pro.setFeature(Feature.PERSON.ToString(), Person.SECOND);
            sent = this.phraseFactory.createClause("Mary", "like", pro);
            Assert.AreEqual("Mary likes you.", this.realiser
                .realiseSentence(sent));

            pro = this.phraseFactory.createNounPhrase("John");
            pro.setFeature(Feature.PRONOMINAL.ToString(), true);
            pro.setFeature(Feature.PERSON.ToString(), Person.THIRD);
            pro.setFeature(LexicalFeature.GENDER, Gender.MASCULINE);
            sent = this.phraseFactory.createClause("Mary", "like", pro);
            Assert.AreEqual("Mary likes him.", this.realiser
                .realiseSentence(sent));

            pro = this.phraseFactory.createNounPhrase("John");
            pro.setFeature(Feature.PRONOMINAL.ToString(), true);
            pro.setFeature(Feature.PERSON.ToString(), Person.FIRST);
            pro.setPlural(true);
            sent = this.phraseFactory.createClause("Mary", "like", pro);
            Assert.AreEqual("Mary likes us.", this.realiser
                .realiseSentence(sent));

            pro = this.phraseFactory.createNounPhrase("John");
            pro.setFeature(Feature.PRONOMINAL.ToString(), true);
            pro.setFeature(Feature.PERSON.ToString(), Person.SECOND);
            pro.setPlural(true);
            sent = this.phraseFactory.createClause("Mary", "like", pro);
            Assert.AreEqual("Mary likes you.", this.realiser
                .realiseSentence(sent));

            pro = this.phraseFactory.createNounPhrase("John");
            pro.setFeature(Feature.PRONOMINAL.ToString(), true);
            pro.setFeature(Feature.PERSON.ToString(), Person.THIRD);
            pro.setFeature(LexicalFeature.GENDER, Gender.MASCULINE);
            pro.setPlural(true);
            sent = this.phraseFactory.createClause("Mary", "like", pro);
            Assert.AreEqual("Mary likes them.", this.realiser
                .realiseSentence(sent));
        }

        /**
         * Test premodification in NPS.
         */

        [Test]
        public void testPremodification()
        {
            this.man.addPreModifier(this.salacious);
            Assert.AreEqual("the salacious man", this.realiser 
                .realise(this.man).getRealisation());

            this.woman.addPreModifier(this.beautiful);
            Assert.AreEqual("the beautiful woman", this.realiser.realise( 
                this.woman).getRealisation());

            this.dog.addPreModifier(this.stunning);
            Assert.AreEqual("the stunning dog",
                this.realiser.realise(this.dog) 
                    .getRealisation());

            // premodification with a WordElement
            this.man.setPreModifier(this.phraseFactory.createWord("idiotic",
                new LexicalCategory_ADJECTIVE()));
            Assert.AreEqual("the idiotic man", this.realiser 
                .realise(this.man).getRealisation());

        }

        /**
         * Test prepositional postmodification.
         */

        [Test]
        public void testPostmodification()
        {
            this.man.addPostModifier(this.onTheRock);
            Assert.AreEqual("the man on the rock", this.realiser.realise( 
                this.man).getRealisation());

            this.woman.addPostModifier(this.behindTheCurtain);
            Assert.AreEqual("the woman behind the curtain", this.realiser 
                .realise(this.woman).getRealisation());

            // postmodification with a WordElement
            this.man.setPostModifier(this.phraseFactory.createWord("jack",
                new LexicalCategory_NOUN()));
            Assert.AreEqual("the man jack", this.realiser.realise( 
                this.man).getRealisation());
        }

        /**
         * Test nominal complementation
         */

        [Test]
        public void testComplementation()
        {
            // complementation with a WordElement
            this.man.setComplement(this.phraseFactory.createWord("jack",
                new LexicalCategory_NOUN()));
            Assert.AreEqual("the man jack", this.realiser.realise( 
                this.man).getRealisation());

            this.woman.addComplement(this.behindTheCurtain);
            Assert.AreEqual("the woman behind the curtain", this.realiser 
                .realise(this.woman).getRealisation());
        }

        /**
         * Test possessive constructions.
         */

        [Test]
        public void testPossessive()
        {

            // simple possessive 's: 'a man's'
            var possNP = this.phraseFactory.createNounPhrase("a", "man");
                 
            possNP.setFeature(Feature.POSSESSIVE.ToString(), true);
            Assert.AreEqual("a man's", this.realiser.realise(possNP) 
                .getRealisation());

            // now set this possessive as specifier of the NP 'the dog'
            this.dog.setFeature(InternalFeature.SPECIFIER.ToString(), possNP);
            Assert.AreEqual("a man's dog",
                this.realiser.realise(this.dog) 
                    .getRealisation());

            // convert possNP to pronoun and turn "a dog" into "his dog"
            // need to specify gender, as default is NEUTER
            possNP.setFeature(LexicalFeature.GENDER, Gender.MASCULINE);
            possNP.setFeature(Feature.PRONOMINAL.ToString(), true);
            Assert.AreEqual("his dog", this.realiser.realise(this.dog) 
                .getRealisation());

            // make it slightly more complicated: "his dog's rock"
            this.dog.setFeature(Feature.POSSESSIVE.ToString(), true); // his dog's

            // his dog's rock (substituting "the"
            // for the
            // entire phrase)
            this.np4.setFeature(InternalFeature.SPECIFIER.ToString(), this.dog);
            Assert.AreEqual("his dog's rock",
                this.realiser.realise(this.np4) 
                    .getRealisation());
        }

        /**
         * Test NP coordination.
         */

        [Test]
        public void testCoordination()
        {

            var cnp1 = new CoordinatedPhraseElement(this.dog,
                this.woman);
            // simple coordination
            Assert.AreEqual("the dog and the woman", this.realiser 
                .realise(cnp1).getRealisation());

            // simple coordination with complementation of entire coordinate NP
            cnp1.addComplement(this.behindTheCurtain);
            Assert.AreEqual("the dog and the woman behind the curtain", 
                this.realiser.realise(cnp1).getRealisation());

            // raise the specifier in this cnp
            // Assert.AreEqual(true, cnp1.raiseSpecifier()); // should return
            // true as all
            // sub-nps have same spec
            // Equals("the dog and woman behind the curtain",
            // realiser.realise(cnp1));
        }

        /**
         * Another battery of tests for NP coordination.
         */

        [Test]
        public void testCoordination2()
        {

            // simple coordination of complementised nps
            this.dog.clearComplements();
            this.woman.clearComplements();

            var cnp1 = new CoordinatedPhraseElement(this.dog,
                this.woman);
            cnp1.setFeature(Feature.RAISE_SPECIFIER.ToString(), true);
            var realised = this.realiser.realise(cnp1);
            Assert.AreEqual("the dog and woman", realised.getRealisation());

            this.dog.addComplement(this.onTheRock);
            this.woman.addComplement(this.behindTheCurtain);

            var cnp2 = new CoordinatedPhraseElement(this.dog,
                this.woman);

            this.woman.setFeature(InternalFeature.RAISED.ToString(), false);
            Assert.AreEqual(
                "the dog on the rock and the woman behind the curtain", 
                this.realiser.realise(cnp2).getRealisation());

            // complementised coordinates + outer pp modifier
            cnp2.addPostModifier(this.inTheRoom);
            Assert
                .AreEqual(
                    "the dog on the rock and the woman behind the curtain in the room", 
                    this.realiser.realise(cnp2).getRealisation());

            // set the specifier for this cnp; should unset specifiers for all inner
            // coordinates
            var every = this.phraseFactory.createWord(
                "every", new LexicalCategory_DETERMINER()); 

            cnp2.setFeature(InternalFeature.SPECIFIER.ToString(), every);

            Assert
                .AreEqual(
                    "every dog on the rock and every woman behind the curtain in the room", 
                    this.realiser.realise(cnp2).getRealisation());

            // pronominalise one of the constituents
            this.dog.setFeature(Feature.PRONOMINAL.ToString(), true); // ="it"
            this.dog.setFeature(InternalFeature.SPECIFIER.ToString(), this.phraseFactory
                .createWord("the", new LexicalCategory_DETERMINER()));
            // raising spec still returns true as spec has been set
            cnp2.setFeature(Feature.RAISE_SPECIFIER.ToString(), true);

            // CNP should be realised with pronominal internal const
            Assert.AreEqual(
                "it and every woman behind the curtain in the room", 
                this.realiser.realise(cnp2).getRealisation());
        }

        /**
         * Test possessives in coordinate NPs.
         */

        [Test]
        public void testPossessiveCoordinate()
        {
            // simple coordination
            var cnp2 = new CoordinatedPhraseElement(this.dog,
                this.woman);
            Assert.AreEqual("the dog and the woman", this.realiser 
                .realise(cnp2).getRealisation());

            // set possessive -- wide-scope by default
            cnp2.setFeature(Feature.POSSESSIVE.ToString(), true);
            Assert.AreEqual("the dog and the woman's", this.realiser.realise( 
                cnp2).getRealisation());

            // set possessive with pronoun
            this.dog.setFeature(Feature.PRONOMINAL.ToString(), true);
            this.dog.setFeature(Feature.POSSESSIVE.ToString(), true);
            cnp2.setFeature(Feature.POSSESSIVE.ToString(), true);
            Assert.AreEqual("its and the woman's", this.realiser.realise(cnp2) 
                .getRealisation());

        }

        /**
         * Test A vs An.
         */

        [Test]
        public void testAAn()
        {
            var _dog = this.phraseFactory.createNounPhrase("a", "dog");
                 
            Assert.AreEqual("a dog", this.realiser.realise(_dog) 
                .getRealisation());

            _dog.addPreModifier("enormous"); 

            Assert.AreEqual("an enormous dog", this.realiser.realise(_dog) 
                .getRealisation());

            var elephant = this.phraseFactory.createNounPhrase(
                "a", "elephant");  
            Assert.AreEqual("an elephant", this.realiser.realise(elephant) 
                .getRealisation());

            elephant.addPreModifier("big"); 
            Assert.AreEqual("a big elephant", this.realiser.realise(elephant) 
                .getRealisation());

            // test treating of plural specifiers
            _dog.setFeature(Feature.NUMBER.ToString(), NumberAgreement.PLURAL);

            Assert.AreEqual("some enormous dogs", this.realiser.realise(_dog) 
                .getRealisation());
        }

        /**
         * Further tests for a/an agreement with coordinated premodifiers
         */

        public void testAAnCoord()
        {
            var _dog = this.phraseFactory.createNounPhrase("a", "dog");
            _dog.addPreModifier(this.phraseFactory.createCoordinatedPhrase(
                "enormous", "black"));
            var realisation = this.realiser.realise(_dog).getRealisation();
            Assert.AreEqual("an enormous and black dog", realisation);
        }

        /**
         * Test for a/an agreement with numbers
         */

        public void testAAnWithNumbers()
        {
            var num = this.phraseFactory.createNounPhrase("a", "change");
            String realisation;

            // no an with "one"
            num.setPreModifier("one percent");
            realisation = this.realiser.realise(num).getRealisation();
            Assert.AreEqual("a one percent change", realisation);

            // an with "eighty"
            num.setPreModifier("eighty percent");
            realisation = this.realiser.realise(num).getRealisation();
            Assert.AreEqual("an eighty percent change", realisation);

            // an with 80
            num.setPreModifier("80%");
            realisation = this.realiser.realise(num).getRealisation();
            Assert.AreEqual("an 80% change", realisation);

            // an with 80000
            num.setPreModifier("80000");
            realisation = this.realiser.realise(num).getRealisation();
            Assert.AreEqual("an 80000 change", realisation);

            // an with 11,000
            num.setPreModifier("11,000");
            realisation = this.realiser.realise(num).getRealisation();
            Assert.AreEqual("an 11,000 change", realisation);

            // an with 18
            num.setPreModifier("18%");
            realisation = this.realiser.realise(num).getRealisation();
            Assert.AreEqual("an 18% change", realisation);

            // a with 180
            num.setPreModifier("180");
            realisation = this.realiser.realise(num).getRealisation();
            Assert.AreEqual("a 180 change", realisation);

            // a with 1100
            num.setPreModifier("1100");
            realisation = this.realiser.realise(num).getRealisation();
            Assert.AreEqual("a 1100 change", realisation);

            // a with 180,000
            num.setPreModifier("180,000");
            realisation = this.realiser.realise(num).getRealisation();
            Assert.AreEqual("a 180,000 change", realisation);

            // an with 11000
            num.setPreModifier("11000");
            realisation = this.realiser.realise(num).getRealisation();
            Assert.AreEqual("an 11000 change", realisation);

            // an with 18000
            num.setPreModifier("18000");
            realisation = this.realiser.realise(num).getRealisation();
            Assert.AreEqual("an 18000 change", realisation);

            // an with 18.1
            num.setPreModifier("18.1%");
            realisation = this.realiser.realise(num).getRealisation();
            Assert.AreEqual("an 18.1% change", realisation);

            // an with 11.1
            num.setPreModifier("11.1%");
            realisation = this.realiser.realise(num).getRealisation();
            Assert.AreEqual("an 11.1% change", realisation);

        }

        /**
         * Test Modifier "guess" placement.
         */

        [Test]
        public void testModifier()
        {
            var _dog = this.phraseFactory.createNounPhrase("a", "dog");
                 
            _dog.addPreModifier("angry"); 

            Assert.AreEqual("an angry dog", this.realiser.realise(_dog) 
                .getRealisation());

            _dog.addPostModifier("in the park"); 
            Assert.AreEqual("an angry dog in the park", this.realiser.realise( 
                _dog).getRealisation());

            var cat = this.phraseFactory.createNounPhrase("a", "cat");
                 
            cat.addPreModifier(this.phraseFactory.createAdjectivePhrase("angry")); 
            Assert.AreEqual("an angry cat", this.realiser.realise(cat) 
                .getRealisation());

            cat.addPostModifier(this.phraseFactory.createPrepositionPhrase(
                "in", "the park"));  
            Assert.AreEqual("an angry cat in the park", this.realiser.realise( 
                cat).getRealisation());

        }

        [Test]
        public void testPluralNounsBelongingToASingular()
        {

            var sent = this.phraseFactory.createClause("I", "count up");
            sent.setFeature(Feature.TENSE.ToString(), Tense.PAST);
            var obj = this.phraseFactory.createNounPhrase("digit");
            obj.setPlural(true);
            var possessor = this.phraseFactory.createNounPhrase("the", "box");
            possessor.setPlural(false);
            possessor.setFeature(Feature.POSSESSIVE.ToString(), true);
            obj.setSpecifier(possessor);
            sent.setObject(obj);

            Assert.AreEqual("I counted up the box's digits", this.realiser.realise(sent) 
                .getRealisation());
        }


        [Test]
        public void testSingularNounsBelongingToAPlural()
        {

            var sent = this.phraseFactory.createClause("I", "clean");
            sent.setFeature(Feature.TENSE.ToString(), Tense.PAST);
            var obj = this.phraseFactory.createNounPhrase("car");
            obj.setPlural(false);
            var possessor = this.phraseFactory.createNounPhrase("the", "parent");
            possessor.setPlural(true);
            possessor.setFeature(Feature.POSSESSIVE.ToString(), true);
            obj.setSpecifier(possessor);
            sent.setObject(obj);

            Assert.AreEqual("I cleaned the parents' car", this.realiser.realise(sent) 
                .getRealisation());
        }

        /**
         * Test for appositive postmodifiers
         */

        [Test]
        public void testAppositivePostmodifier()
        {
            var _dog = this.phraseFactory.createNounPhrase("the", "dog");
            var _rott = this.phraseFactory.createNounPhrase("a", "rottweiler");
            _rott.setFeature(Feature.APPOSITIVE.ToString(), true);
            _dog.addPostModifier(_rott);
            var _sent = this.phraseFactory.createClause(_dog, "ran");
            Assert.AreEqual("The dog, a rottweiler runs.", this.realiser.realiseSentence(_sent));
        }
    }
}