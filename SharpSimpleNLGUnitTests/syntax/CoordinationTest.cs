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
     * Some tests for coordination, especially of coordinated VPs with modifiers.
     * 
     * @author Albert Gatt
     * 
     */

    public class CoordinationTest : SimpleNLG4TestBase
    {



        /**
         * Check that empty coordinate phrases are not realised as "null"
         */
        [Test]

        public void emptyCoordinationTest()
        {
            //this.realiser.setDebugMode(true);
               // first a simple phrase with no coordinates
            var coord = this.phraseFactory.createCoordinatedPhrase();
            //Assert.AreEqual("", this.realiser.realise(coord).getRealisation());

            // now one with a premodifier and nothing else
            coord.addPreModifier(this.phraseFactory.createAdjectivePhrase("nice"));
            Assert.AreEqual("nice", this.realiser.realise(coord)
                .getRealisation());
        }

        /**
         * Test pre and post-modification of coordinate VPs inside a sentence.
         */
        [Test]

        public void testModifiedCoordVP()
        {
            var coord = this.phraseFactory
                .createCoordinatedPhrase(this.getUp, this.fallDown);
            coord.setFeature(Feature.TENSE.ToString(), Tense.PAST);
            Assert.AreEqual("got up and fell down", this.realiser
                .realise(coord).getRealisation());

            // add a premodifier
            coord.addPreModifier("slowly");
            Assert.AreEqual("slowly got up and fell down", this.realiser
                .realise(coord).getRealisation());

            // adda postmodifier
            coord.addPostModifier(this.behindTheCurtain);
            Assert.AreEqual("slowly got up and fell down behind the curtain",
                this.realiser.realise(coord).getRealisation());

            // put within the context of a sentence
            var s = this.phraseFactory.createClause("Jake", coord);
            s.setFeature(Feature.TENSE.ToString(), Tense.PAST);
            Assert.AreEqual(
                "Jake slowly got up and fell down behind the curtain",
                this.realiser.realise(s).getRealisation());

            // add premod to the sentence
            s.addPreModifier(this.lexicon
                .getWord("however", new LexicalCategory_ADVERB()));
            Assert.AreEqual(
                "Jake however slowly got up and fell down behind the curtain",
                this.realiser.realise(s).getRealisation());

            // add postmod to the sentence
            s.addPostModifier(this.inTheRoom);
            Assert.AreEqual(
                "Jake however slowly got up and fell down behind the curtain in the room",
                this.realiser.realise(s).getRealisation());
        }

        /**
         * Test due to Chris Howell -- create a complex sentence with front modifier
         * and coordinateVP. this is a version in which we create the coordinate
         * phrase directly.
         */
        [Test]

        public void testCoordinateVPComplexSubject()
        {
            // "As a result of the procedure the patient had an adverse contrast media reaction and went into cardiogenic shock."
            var s = this.phraseFactory.createClause();

            s.setSubject(this.phraseFactory.createNounPhrase("the", "patient"));

            // first VP
            var vp1 = this.phraseFactory.createVerbPhrase(this.lexicon
                .getWord("have", new LexicalCategory_VERB()));
            var np1 = this.phraseFactory.createNounPhrase("a",
                this.lexicon.getWord("contrast media reaction",
                    new LexicalCategory_NOUN()));
            np1.addPreModifier(this.lexicon.getWord("adverse",
                new LexicalCategory_ADJECTIVE()));
            vp1.addComplement(np1);

            // second VP
            var vp2 = this.phraseFactory.createVerbPhrase(this.lexicon
                .getWord("go", new LexicalCategory_VERB()));
            var pp = this.phraseFactory
                .createPrepositionPhrase("into", this.lexicon.getWord(
                    "cardiogenic shock", new LexicalCategory_NOUN()));
            vp2.addComplement(pp);

            // coordinate
            var coord = this.phraseFactory
                .createCoordinatedPhrase(vp1, vp2);
            coord.setFeature(Feature.TENSE.ToString(), Tense.PAST);
            Assert.AreEqual(
                "had an adverse contrast media reaction and went into cardiogenic shock",
                this.realiser.realise(coord).getRealisation());

            // now put this in the sentence
            s.setVerbPhrase(coord);
            s.addFrontModifier("As a result of the procedure");
            Assert.AreEqual(
                "As a result of the procedure the patient had an adverse contrast media reaction and went into cardiogenic shock",
                this.realiser.realise(s).getRealisation());

        }

        /**
         * Test setting a conjunction to null
         */

        public void testNullConjunction()
        {
            var p = this.phraseFactory.createClause("I", "be", "happy");
            var q = this.phraseFactory.createClause("I", "eat", "fish");
            var pq = this.phraseFactory
                .createCoordinatedPhrase();
            pq.addCoordinate(p);
            pq.addCoordinate(q);
            pq.setFeature(Feature.CONJUNCTION.ToString(), "");

            // should come out without conjunction
            Assert.AreEqual("I am happy I eat fish", this.realiser.realise(pq)
                .getRealisation());

            // should come out without conjunction
            pq.setFeature(Feature.CONJUNCTION.ToString(), null);
            Assert.AreEqual("I am happy I eat fish", this.realiser.realise(pq)
                .getRealisation());

        }

        /**
         * Check that the negation feature on a child of a coordinate phrase remains
         * as set, unless explicitly set otherwise at the parent level.
         */
        [Test]

        public void testNegationFeature()
        {
            var s1 = this.phraseFactory
                .createClause("he", "have", "asthma");
            var s2 = this.phraseFactory.createClause("he", "have",
                "diabetes");
            s1.setFeature(Feature.NEGATED.ToString(), true);
            var coord = this.phraseFactory
                .createCoordinatedPhrase(s1, s2);
            var realisation = this.realiser.realise(coord).getRealisation();
            Assert.AreEqual("he does not have asthma and he has diabetes",
                realisation);
        }
    }
}