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
             * This class is the base class for all JUnit simplenlg.test cases for
             * simplenlg. It sets up a a JUnit fixture, i.e. the basic objects (basic
             * constituents) that all other tests can use.
             * @author agatt
             */

    public class SimpleNLG4TestBase
    {

        public Realiser realiser { get; set; }

        public NLGFactory phraseFactory { get; set; }

        public Lexicon lexicon { get; set; }

        /** The pro test2. */
        public NPPhraseSpec man, woman, dog, boy, np4, np5, np6, proTest1, proTest2;

        /** The salacious. */
        public AdjPhraseSpec beautiful, stunning, salacious;

        /** The under the table. */
        public PPPhraseSpec onTheRock, behindTheCurtain, inTheRoom, underTheTable;

        /** The say. */
        public VPPhraseSpec kick, kiss, walk, talk, getUp, fallDown, give, say;
        // set up a few more fixtures
        /** The s4. */
        public SPhraseSpec s1, s2, s3, s4;


        /*
         * (non-Javadoc)
         *
         * @see simplenlg.test.SimplenlgTest#setUp()
         */

        [SetUp]
        protected void setUp()
        {
            this.lexicon = Lexicon.getDefaultLexicon();

            this.phraseFactory = new NLGFactory(this.lexicon);
            this.realiser = new Realiser(this.lexicon);

            this.man = this.phraseFactory.createNounPhrase("the", "man");  
            this.woman = this.phraseFactory.createNounPhrase("the", "woman"); 
            this.dog = this.phraseFactory.createNounPhrase("the", "dog");  
            this.boy = this.phraseFactory.createNounPhrase("the", "boy");  

            this.beautiful = this.phraseFactory.createAdjectivePhrase("beautiful"); 
            this.stunning = this.phraseFactory.createAdjectivePhrase("stunning"); 
            this.salacious = this.phraseFactory.createAdjectivePhrase("salacious"); 

            this.onTheRock = this.phraseFactory.createPrepositionPhrase("on"); 
            this.np4 = this.phraseFactory.createNounPhrase("the", "rock");  
            this.onTheRock.addComplement(this.np4);

            this.behindTheCurtain = this.phraseFactory.createPrepositionPhrase("behind"); 
            this.np5 = this.phraseFactory.createNounPhrase("the", "curtain");  
            this.behindTheCurtain.addComplement(this.np5);

            this.inTheRoom = this.phraseFactory.createPrepositionPhrase("in"); 
            this.np6 = this.phraseFactory.createNounPhrase("the", "room");  
            this.inTheRoom.addComplement(this.np6);

            this.underTheTable = this.phraseFactory.createPrepositionPhrase("under"); 
            this.underTheTable.addComplement(this.phraseFactory.createNounPhrase("the", "table"));
             

            this.proTest1 = this.phraseFactory.createNounPhrase("the", "singer");  
            this.proTest2 = this.phraseFactory.createNounPhrase("some", "person");  

            this.kick = this.phraseFactory.createVerbPhrase("kick"); 
            this.kiss = this.phraseFactory.createVerbPhrase("kiss"); 
            this.walk = this.phraseFactory.createVerbPhrase("walk"); 
            this.talk = this.phraseFactory.createVerbPhrase("talk"); 
            this.getUp = this.phraseFactory.createVerbPhrase("get up"); 
            this.fallDown = this.phraseFactory.createVerbPhrase("fall down"); 
            this.give = this.phraseFactory.createVerbPhrase("give"); 
            this.say = this.phraseFactory.createVerbPhrase("say"); 

            // the woman kisses the man
            s1 = this.phraseFactory.createClause();
            s1.setSubject(this.woman);
            s1.setVerbPhrase(this.kiss);
            s1.setObject(this.man);

            // there is the dog on the rock
            s2 = this.phraseFactory.createClause();
            s2.setSubject("there"); 
            s2.setVerb("be"); 
            s2.setObject(this.dog);
            s2.addPostModifier(this.onTheRock);

            // the man gives the woman John's flower
            s3 = this.phraseFactory.createClause();
            s3.setSubject(this.man);
            s3.setVerbPhrase(this.give);

            var flower = this.phraseFactory.createNounPhrase("flower"); 
            var john = this.phraseFactory.createNounPhrase("John"); 
            john.setFeature(Feature.POSSESSIVE.ToString(), true);
            flower.setFeature(InternalFeature.SPECIFIER.ToString(), john);
            s3.setObject(flower);
            s3.setIndirectObject(this.woman);

            s4 = this.phraseFactory.createClause();
            s4.setFeature(Feature.CUE_PHRASE.ToString(), "however"); 
            s4.addFrontModifier("tomorrow"); 

            var subject = this.phraseFactory
                .createCoordinatedPhrase(this.phraseFactory
                    .createNounPhrase("Jane"), this.phraseFactory 
                    .createNounPhrase("Andrew")); 

            s4.setSubject(subject);

            var pick = this.phraseFactory.createVerbPhrase("pick up"); 
            s4.setVerbPhrase(pick);
            s4.setObject("the balls"); 
            s4.addPostModifier("in the shop"); 
            s4.setFeature(Feature.TENSE.ToString(), Tense.FUTURE);
        }

        protected void reset()
        {
            this.phraseFactory = new NLGFactory(this.lexicon);
            this.realiser = new Realiser(this.lexicon);

            this.man = this.phraseFactory.createNounPhrase("the", "man");
            this.woman = this.phraseFactory.createNounPhrase("the", "woman");
            this.dog = this.phraseFactory.createNounPhrase("the", "dog");
            this.boy = this.phraseFactory.createNounPhrase("the", "boy");

            this.beautiful = this.phraseFactory.createAdjectivePhrase("beautiful");
            this.stunning = this.phraseFactory.createAdjectivePhrase("stunning");
            this.salacious = this.phraseFactory.createAdjectivePhrase("salacious");

            this.onTheRock = this.phraseFactory.createPrepositionPhrase("on");
            this.np4 = this.phraseFactory.createNounPhrase("the", "rock");
            this.onTheRock.addComplement(this.np4);

            this.behindTheCurtain = this.phraseFactory.createPrepositionPhrase("behind");
            this.np5 = this.phraseFactory.createNounPhrase("the", "curtain");
            this.behindTheCurtain.addComplement(this.np5);

            this.inTheRoom = this.phraseFactory.createPrepositionPhrase("in");
            this.np6 = this.phraseFactory.createNounPhrase("the", "room");
            this.inTheRoom.addComplement(this.np6);

            this.underTheTable = this.phraseFactory.createPrepositionPhrase("under");
            this.underTheTable.addComplement(this.phraseFactory.createNounPhrase("the", "table"));


            this.proTest1 = this.phraseFactory.createNounPhrase("the", "singer");
            this.proTest2 = this.phraseFactory.createNounPhrase("some", "person");

            this.kick = this.phraseFactory.createVerbPhrase("kick");
            this.kiss = this.phraseFactory.createVerbPhrase("kiss");
            this.walk = this.phraseFactory.createVerbPhrase("walk");
            this.talk = this.phraseFactory.createVerbPhrase("talk");
            this.getUp = this.phraseFactory.createVerbPhrase("get up");
            this.fallDown = this.phraseFactory.createVerbPhrase("fall down");
            this.give = this.phraseFactory.createVerbPhrase("give");
            this.say = this.phraseFactory.createVerbPhrase("say");

            // the woman kisses the man
            s1 = this.phraseFactory.createClause();
            s1.setSubject(this.woman);
            s1.setVerbPhrase(this.kiss);
            s1.setObject(this.man);

            // there is the dog on the rock
            s2 = this.phraseFactory.createClause();
            s2.setSubject("there");
            s2.setVerb("be");
            s2.setObject(this.dog);
            s2.addPostModifier(this.onTheRock);

            // the man gives the woman John's flower
            s3 = this.phraseFactory.createClause();
            s3.setSubject(this.man);
            s3.setVerbPhrase(this.give);

            var flower = this.phraseFactory.createNounPhrase("flower");
            var john = this.phraseFactory.createNounPhrase("John");
            john.setFeature(Feature.POSSESSIVE.ToString(), true);
            flower.setFeature(InternalFeature.SPECIFIER.ToString(), john);
            s3.setObject(flower);
            s3.setIndirectObject(this.woman);

            s4 = this.phraseFactory.createClause();
            s4.setFeature(Feature.CUE_PHRASE.ToString(), "however");
            s4.addFrontModifier("tomorrow");

            var subject = this.phraseFactory
                .createCoordinatedPhrase(this.phraseFactory
                    .createNounPhrase("Jane"), this.phraseFactory
                    .createNounPhrase("Andrew"));

            s4.setSubject(subject);

            var pick = this.phraseFactory.createVerbPhrase("pick up");
            s4.setVerbPhrase(pick);
            s4.setObject("the balls");
            s4.addPostModifier("in the shop");
            s4.setFeature(Feature.TENSE.ToString(), Tense.FUTURE);
        }

    }

}