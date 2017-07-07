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

namespace SimpleNLGTests.syntax
{
    /**
     * Tests from SimpleNLG tutorial
     * <hr>
     * 
     * <p>
     * Copyright (C) 2011, University of Aberdeen
     * </p>
     * 
     * @author Ehud Reiter
     * 
     */

    public class TutorialTest
    {


  
        [Test]
        public void section3_Test()
        {
            var lexicon = Lexicon.getDefaultLexicon(); // default simplenlg lexicon
            var nlgFactory = new NLGFactory(lexicon); // factory based on lexicon

            NLGElement s1 = nlgFactory.createSentence("my dog is happy");

            var r = new Realiser(lexicon);

            var output = r.realiseSentence(s1);

            Assert.AreEqual("My dog is happy.", output);
        }

        /**
         * test section 5 code
         */

        [Test]
        public void section5_Test()
        {
            var lexicon = Lexicon.getDefaultLexicon(); // default simplenlg lexicon
            var nlgFactory = new NLGFactory(lexicon); // factory based on lexicon
            var realiser = new Realiser(lexicon);

            var p = nlgFactory.createClause();
            p.setSubject("my dog");
            p.setVerb("chase");
            p.setObject("George");

            var output = realiser.realiseSentence(p);
            Assert.AreEqual("My dog chases George.", output);
        }

        /**
         * test section 6 code
         */

        [Test]
        public void section6_Test()
        {
            var lexicon = Lexicon.getDefaultLexicon(); // default simplenlg lexicon
            var nlgFactory = new NLGFactory(lexicon); // factory based on lexicon
            var realiser = new Realiser(lexicon);

            var p = nlgFactory.createClause();
            p.setSubject("Mary");
            p.setVerb("chase");
            p.setObject("George");

            p.setFeature(Feature.TENSE.ToString(), Tense.PAST);
            var output = realiser.realiseSentence(p);
            Assert.AreEqual("Mary chased George.", output);

            p.setFeature(Feature.TENSE.ToString(), Tense.FUTURE);
            output = realiser.realiseSentence(p);
            Assert.AreEqual("Mary will chase George.", output);

            p.setFeature(Feature.NEGATED.ToString(), true);
            output = realiser.realiseSentence(p);
            Assert.AreEqual("Mary will not chase George.", output);

            p = nlgFactory.createClause();
            p.setSubject("Mary");
            p.setVerb("chase");
            p.setObject("George");

            p.setFeature(Feature.INTERROGATIVE_TYPE.ToString(),
                InterrogativeType.YES_NO);
            output = realiser.realiseSentence(p);
            Assert.AreEqual("Does Mary chase George?", output);

            p.setSubject("Mary");
            p.setVerb("chase");
            p.setFeature(Feature.INTERROGATIVE_TYPE.ToString(), InterrogativeType.WHO_OBJECT);
            output = realiser.realiseSentence(p);
            Assert.AreEqual("Who does Mary chase?", output);

            p = nlgFactory.createClause();
            p.setSubject("the dog");
            p.setVerb("wake up");
            output = realiser.realiseSentence(p);
            Assert.AreEqual("The dog wakes up.", output);

        }

        /**
         * test ability to use variant words
         */

        [Test]
        public void variantsTest()
        {
            var lexicon = Lexicon.getDefaultLexicon(); // default simplenlg lexicon
            var nlgFactory = new NLGFactory(lexicon); // factory based on lexicon
            var realiser = new Realiser(lexicon);

            var p = nlgFactory.createClause();
            p.setSubject("my dog");
            p.setVerb("is"); // variant of be
            p.setObject("George");

            var output = realiser.realiseSentence(p);
            Assert.AreEqual("My dog is George.", output);

            p = nlgFactory.createClause();
            p.setSubject("my dog");
            p.setVerb("chases"); // variant of chase
            p.setObject("George");

            output = realiser.realiseSentence(p);
            Assert.AreEqual("My dog chases George.", output);


            p = nlgFactory.createClause();
            p.setSubject(nlgFactory.createNounPhrase("the", "dogs")); // variant of "dog"
            p.setVerb("is"); // variant of be
            p.setObject("happy"); // variant of happy
            output = realiser.realiseSentence(p);
            Assert.AreEqual("The dog is happy.", output);

            p = nlgFactory.createClause();
            p.setSubject(nlgFactory.createNounPhrase("the", "children")); // variant of "child"
            p.setVerb("is"); // variant of be
            p.setObject("happy"); // variant of happy
            output = realiser.realiseSentence(p);
            Assert.AreEqual("The child is happy.", output);

            // following functionality is enabled
            p = nlgFactory.createClause();
            p.setSubject(nlgFactory.createNounPhrase("the", "dogs")); // variant of "dog"
            p.setVerb("is"); // variant of be
            p.setObject("happy"); // variant of happy
            output = realiser.realiseSentence(p);
            Assert.AreEqual("The dog is happy.", output); //corrected automatically
        }

        /* Following code tests the section 5 to 15
         * sections 5 & 6 are repeated here in order to match the simplenlg tutorial version 4
         * James Christie
         * June 2011
         */

        /**
         * test section 5 to match simplenlg tutorial version 4's code
         */

        [Test]
        public void section5A_Test()
        {
            var lexicon = Lexicon.getDefaultLexicon(); // default simplenlg lexicon
            var nlgFactory = new NLGFactory(lexicon); // factory based on lexicon
            var realiser = new Realiser(lexicon);

            var p = nlgFactory.createClause();
            p.setSubject("Mary");
            p.setVerb("chase");
            p.setObject("the monkey");

            var output = realiser.realiseSentence(p);
            Assert.AreEqual("Mary chases the monkey.", output);
        } // testSection5A

        /**
         * test section 6 to match simplenlg tutorial version 4' code
         */

        [Test]
        public void section6A_Test()
        {
            var lexicon = Lexicon.getDefaultLexicon(); // default simplenlg lexicon
            var nlgFactory = new NLGFactory(lexicon); // factory based on lexicon
            var realiser = new Realiser(lexicon);

            var p = nlgFactory.createClause();
            p.setSubject("Mary");
            p.setVerb("chase");
            p.setObject("the monkey");

            p.setFeature(Feature.TENSE.ToString(), Tense.PAST);
            var output = realiser.realiseSentence(p);
            Assert.AreEqual("Mary chased the monkey.", output);

            p.setFeature(Feature.TENSE.ToString(), Tense.FUTURE);
            output = realiser.realiseSentence(p);
            Assert.AreEqual("Mary will chase the monkey.", output);

            p.setFeature(Feature.NEGATED.ToString(), true);
            output = realiser.realiseSentence(p);
            Assert.AreEqual("Mary will not chase the monkey.", output);

            p = nlgFactory.createClause();
            p.setSubject("Mary");
            p.setVerb("chase");
            p.setObject("the monkey");

            p.setFeature(Feature.INTERROGATIVE_TYPE.ToString(), InterrogativeType.YES_NO);
            output = realiser.realiseSentence(p);
            Assert.AreEqual("Does Mary chase the monkey?", output);

            p.setSubject("Mary");
            p.setVerb("chase");
            p.setFeature(Feature.INTERROGATIVE_TYPE.ToString(), InterrogativeType.WHO_OBJECT);
            output = realiser.realiseSentence(p);
            Assert.AreEqual("Who does Mary chase?", output);
        }

        /**
         * test section 7 code
         */

        [Test]
        public void section7_Test()
        {
            var lexicon = Lexicon.getDefaultLexicon(); // default simplenlg lexicon
            var nlgFactory = new NLGFactory(lexicon); // factory based on lexicon
            var realiser = new Realiser(lexicon);

            var p = nlgFactory.createClause();
            p.setSubject("Mary");
            p.setVerb("chase");
            p.setObject("the monkey");
            p.addComplement("very quickly");
            p.addComplement("despite her exhaustion");

            var output = realiser.realiseSentence(p);
            Assert.AreEqual("Mary chases the monkey very quickly despite her exhaustion.", output);
        }

        /**
         * test section 8 code
         */

        [Test]
        public void section8_Test()
        {
            var lexicon = Lexicon.getDefaultLexicon(); // default simplenlg lexicon
            var nlgFactory = new NLGFactory(lexicon); // factory based on lexicon
            var realiser = new Realiser(lexicon);
            //realiser.setDebugMode(true);
            var subject = nlgFactory.createNounPhrase("Mary");
            var obj = nlgFactory.createNounPhrase("the monkey");
            var verb = nlgFactory.createVerbPhrase("chase");
            subject.addModifier("fast");

            var p = nlgFactory.createClause();
            p.setSubject(subject);
            p.setVerb(verb);
            p.setObject(obj);

            var outputA = realiser.realiseSentence(p);
            Assert.AreEqual("Fast Mary chases the monkey.", outputA);

            verb.addModifier("quickly");

            var outputB = realiser.realiseSentence(p);
            Assert.AreEqual("Fast Mary quickly chases the monkey.", outputB);
        }

        // there is no code specified in section 9

        /**
         * test section 10 code
         */

        [Test]
        public void section10_Test()
        {
            var lexicon = Lexicon.getDefaultLexicon(); // default simplenlg lexicon
            var nlgFactory = new NLGFactory(lexicon); // factory based on lexicon
            var realiser = new Realiser(lexicon);

            var subject1 = nlgFactory.createNounPhrase("Mary");
            var subject2 = nlgFactory.createNounPhrase("your", "giraffe");

            // next line is not correct ~ should be nlgFactory.createCoordinatedPhrase ~ may be corrected in the API
            var subj = nlgFactory.createCoordinatedPhrase(subject1, subject2);

            var verb = nlgFactory.createVerbPhrase("chase");

            var p = nlgFactory.createClause();
            p.setSubject(subj);
            p.setVerb(verb);
            p.setObject("the monkey");

            var outputA = realiser.realiseSentence(p);
            Assert.AreEqual("Mary and your giraffe chase the monkey.", outputA);

            var object1 = nlgFactory.createNounPhrase("the monkey");
            var object2 = nlgFactory.createNounPhrase("George");

            // next line is not correct ~ should be nlgFactory.createCoordinatedPhrase ~ may be corrected in the API
            var obj = nlgFactory.createCoordinatedPhrase(object1, object2);
            obj.addCoordinate("Martha");
            p.setObject(obj);

            var outputB = realiser.realiseSentence(p);
            Assert.AreEqual("Mary and your giraffe chase the monkey, George and Martha.", outputB);

            obj.setFeature(Feature.CONJUNCTION.ToString(), "or");

            var outputC = realiser.realiseSentence(p);
            Assert.AreEqual("Mary and your giraffe chase the monkey, George or Martha.", outputC);
        }

        /**
         * test section 11 code
         */

        [Test]
        public void section11_Test()
        {
            var lexicon = Lexicon.getDefaultLexicon(); // default simplenlg lexicon
            var nlgFactory = new NLGFactory(lexicon); // factory based on lexicon

            var realiser = new Realiser(lexicon);

            var pA = nlgFactory.createClause("Mary", "chase", "the monkey");
            pA.addComplement("in the park");

            var outputA = realiser.realiseSentence(pA);
            Assert.AreEqual("Mary chases the monkey in the park.", outputA);

            // alternative build paradigm
            var place = nlgFactory.createNounPhrase("park");
            var pB = nlgFactory.createClause("Mary", "chase", "the monkey");

            // next line is depreciated ~ may be corrected in the API
            place.setDeterminer("the");
            var pp = nlgFactory.createPrepositionPhrase();
            pp.addComplement(place);
            pp.setPreposition("in");

            pB.addComplement(pp);

            var outputB = realiser.realiseSentence(pB);
            Assert.AreEqual("Mary chases the monkey in the park.", outputB);

            place.addPreModifier("leafy");

            var outputC = realiser.realiseSentence(pB);
            Assert.AreEqual("Mary chases the monkey in the leafy park.", outputC);
        } // testSection11

        // section12 only has a code table as illustration

        /**
         * test section 13 code
         */

        [Test]
        public void section13_Test()
        {
            var lexicon = Lexicon.getDefaultLexicon(); // default simplenlg lexicon
            var nlgFactory = new NLGFactory(lexicon); // factory based on lexicon

            var realiser = new Realiser(lexicon);

            var s1 = nlgFactory.createClause("my cat", "like", "fish");
            var s2 = nlgFactory.createClause("my dog", "like", "big bones");
            var s3 = nlgFactory.createClause("my horse", "like", "grass");

            var c = nlgFactory.createCoordinatedPhrase();
            c.addCoordinate(s1);
            c.addCoordinate(s2);
            c.addCoordinate(s3);

            var outputA = realiser.realiseSentence(c);
            Assert.AreEqual("My cat likes fish, my dog likes big bones and my horse likes grass.", outputA);

            var p = nlgFactory.createClause("I", "be", "happy");
            var q = nlgFactory.createClause("I", "eat", "fish");
            q.setFeature(Feature.COMPLEMENTISER.ToString(), "because");
            q.setFeature(Feature.TENSE.ToString(), Tense.PAST);
            p.addComplement(q);

            var outputB = realiser.realiseSentence(p);
            Assert.AreEqual("I am happy because I ate fish.", outputB);
        }

        /**
         * test section 14 code
         */

        [Test]
        public void section14_Test()
        {
            var lexicon = Lexicon.getDefaultLexicon(); // default simplenlg lexicon
            var nlgFactory = new NLGFactory(lexicon); // factory based on lexicon

            var realiser = new Realiser(lexicon);

            var p1 = nlgFactory.createClause("Mary", "chase", "the monkey");
            var p2 = nlgFactory.createClause("The monkey", "fight back");
            var p3 = nlgFactory.createClause("Mary", "be", "nervous");

            var s1 = nlgFactory.createSentence(p1);
            var s2 = nlgFactory.createSentence(p2);
            var s3 = nlgFactory.createSentence(p3);

            var par1 = nlgFactory.createParagraph(new List<INLGElement> { s1, s2, s3});

            var output14a = realiser.realise(par1).getRealisation();
            Assert.AreEqual("Mary chases the monkey. The monkey fights back. Mary is nervous.\n\n", output14a);

            var section = nlgFactory.createSection("The Trials and Tribulation of Mary and the Monkey");
            section.addComponent(par1);
            var output14b = realiser.realise(section).getRealisation();
            Assert.AreEqual(
                "The Trials and Tribulation of Mary and the Monkey\nMary chases the monkey. The monkey fights back. Mary is nervous.\n\n",
                output14b);
        }

    }
}