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
     * These are tests for the verb phrase and coordinate VP classes.
     * @author agatt
     */

    public class VerbPhraseTest : SimpleNLG4TestBase
    {


        /**
         * Some tests to check for an early bug which resulted in reduplication of
         * verb particles in the past tense e.g. "fall down down" or "creep up up"
         */

        [Test]
        public void testVerbParticle()
        {
            var v = this.phraseFactory.createVerbPhrase("fall down");

            Assert.AreEqual(
                "down", v.getFeatureAsString(Feature.PARTICLE.ToString()));

            Assert.AreEqual(
                "fall", ((WordElement) v.getVerb()).getBaseForm());

            v.setFeature(Feature.TENSE.ToString(), Tense.PAST);
            v.setFeature(Feature.PERSON.ToString(), Person.THIRD);
            v.setFeature(Feature.NUMBER.ToString(), NumberAgreement.PLURAL);

            Assert.AreEqual(
                "fell down", this.realiser.realise(v).getRealisation());

            v.setFeature(Feature.FORM.ToString(), Form.PAST_PARTICIPLE);
            Assert.AreEqual(
                "fallen down", this.realiser.realise(v).getRealisation());
        }

        /**
         * Tests for the tense and aspect.
         */

        [Test]
        public void simplePastTest()
        {
            // "fell down"
            this.fallDown.setFeature(Feature.TENSE.ToString(), Tense.PAST);
            Assert.AreEqual(
                "fell down", this.realiser.realise(this.fallDown).getRealisation());

        }

        /**
         * Test tense aspect.
         */

        [Test]
        public void tenseAspectTest()
        {
            // had fallen down
            this.realiser.setLexicon(this.lexicon);
            this.fallDown.setFeature(Feature.TENSE.ToString(), Tense.PAST);
            this.fallDown.setFeature(Feature.PERFECT.ToString(), true);

            Assert.AreEqual("had fallen down", this.realiser.realise(
                this.fallDown).getRealisation());

            // had been falling down
            this.fallDown.setFeature(Feature.PROGRESSIVE.ToString(), true);
            Assert.AreEqual("had been falling down", this.realiser.realise(
                this.fallDown).getRealisation());

            // will have been kicked
            this.kick.setFeature(Feature.PASSIVE.ToString(), true);
            this.kick.setFeature(Feature.PERFECT.ToString(), true);
            this.kick.setFeature(Feature.TENSE.ToString(), Tense.FUTURE);
            Assert.AreEqual("will have been kicked", this.realiser.realise(
                this.kick).getRealisation());

            // will have been being kicked
            this.kick.setFeature(Feature.PROGRESSIVE.ToString(), true);
            Assert.AreEqual("will have been being kicked", this.realiser
                .realise(this.kick).getRealisation());

            // will not have been being kicked
            this.kick.setFeature(Feature.NEGATED.ToString(), true);
            Assert.AreEqual("will not have been being kicked", this.realiser
                .realise(this.kick).getRealisation());

            // passivisation should suppress the complement
            this.kick.clearComplements();
            this.kick.addComplement(this.man);
            Assert.AreEqual("will not have been being kicked", this.realiser
                .realise(this.kick).getRealisation());

            // de-passivisation should now give us "will have been kicking the man"
            this.kick.setFeature(Feature.PASSIVE.ToString(), false);
            Assert.AreEqual("will not have been kicking the man", this.realiser
                .realise(this.kick).getRealisation());

            // remove the future tense --
            // this is a test of an earlier bug that would still realise "will"
            this.kick.setFeature(Feature.TENSE.ToString(), Tense.PRESENT);
            Assert.AreEqual("has not been kicking the man", this.realiser
                .realise(this.kick).getRealisation());
        }

        /**
         * Test for realisation of VP complements.
         */

        [Test]
        public void complementationTest()
        {

            // was kissing Mary
            var mary = this.phraseFactory.createNounPhrase("Mary");
            mary.setFeature(InternalFeature.DISCOURSE_FUNCTION.ToString(), DiscourseFunction.OBJECT);
            this.kiss.clearComplements();
            this.kiss.addComplement(mary);
            this.kiss.setFeature(Feature.PROGRESSIVE.ToString(), true);
            this.kiss.setFeature(Feature.TENSE.ToString(), Tense.PAST);

            Assert.AreEqual("was kissing Mary", this.realiser
                .realise(this.kiss).getRealisation());

            var mary2 = new CoordinatedPhraseElement(mary,
                this.phraseFactory.createNounPhrase("Susan"));
            // add another complement -- should come out as "Mary and Susan"
            this.kiss.clearComplements();
            this.kiss.addComplement(mary2);
            Assert.AreEqual("was kissing Mary and Susan", this.realiser
                .realise(this.kiss).getRealisation());

            // passivise -- should make the direct object complement disappear
            // Note: The verb doesn't come out as plural because agreement
            // is determined by the sentential subjects and this VP isn't inside a
            // sentence
            this.kiss.setFeature(Feature.PASSIVE.ToString(), true);
            Assert.AreEqual("was being kissed", this.realiser
                .realise(this.kiss).getRealisation());

            // make it plural (this is usually taken care of in SPhraseSpec)
            this.kiss.setFeature(Feature.NUMBER.ToString(), NumberAgreement.PLURAL);
            Assert.AreEqual("were being kissed", this.realiser.realise(
                this.kiss).getRealisation());

            // depassivise and add post-mod: yields "was kissing Mary in the room"
            this.kiss.addPostModifier(this.inTheRoom);
            this.kiss.setFeature(Feature.PASSIVE.ToString(), false);
            this.kiss.setFeature(Feature.NUMBER.ToString(), NumberAgreement.SINGULAR);
            Assert.AreEqual("was kissing Mary and Susan in the room",
                this.realiser.realise(this.kiss).getRealisation());

            // passivise again: should make direct object disappear, but not postMod
            // ="was being kissed in the room"
            this.kiss.setFeature(Feature.PASSIVE.ToString(), true);
            this.kiss.setFeature(Feature.NUMBER.ToString(), NumberAgreement.PLURAL);
            Assert.AreEqual("were being kissed in the room", this.realiser
                .realise(this.kiss).getRealisation());
        }

        /**
         * This tests for the default complement ordering, relative to pre and
         * postmodifiers.
         */

        [Test]
        public void complementationTest_2()
        {
            // give the woman the dog
            this.woman.setFeature(InternalFeature.DISCOURSE_FUNCTION.ToString(),
                DiscourseFunction.INDIRECT_OBJECT);
            this.dog.setFeature(InternalFeature.DISCOURSE_FUNCTION.ToString(),
                DiscourseFunction.OBJECT);
            this.give.clearComplements();
            this.give.addComplement(this.dog);
            this.give.addComplement(this.woman);
            Assert.AreEqual("gives the woman the dog", this.realiser.realise(
                this.give).getRealisation());

            // add a few premodifiers and postmodifiers
            this.give.addPreModifier("slowly");
            this.give.addPostModifier(this.behindTheCurtain);
            this.give.addPostModifier(this.inTheRoom);
            Assert.AreEqual(
                "slowly gives the woman the dog behind the curtain in the room",
                this.realiser.realise(this.give).getRealisation());

            // reset the arguments
            this.give.clearComplements();
            this.give.addComplement(this.dog);
            var womanBoy = new CoordinatedPhraseElement(
                this.woman, this.boy);
            womanBoy.setFeature(InternalFeature.DISCOURSE_FUNCTION.ToString(),
                DiscourseFunction.INDIRECT_OBJECT);
            this.give.addComplement(womanBoy);

            // if we unset the passive, we should get the indirect objects
            // they won't be coordinated
            this.give.setFeature(Feature.PASSIVE.ToString(), false);
            Assert.AreEqual(
                "slowly gives the woman and the boy the dog behind the curtain in the room",
                this.realiser.realise(this.give).getRealisation());

            // set them to a coordinate instead
            // set ONLY the complement INDIRECT_OBJECT, leaves OBJECT intact
            this.give.clearComplements();
            this.give.addComplement(womanBoy);
            this.give.addComplement(this.dog);
            var complements = this.give
                .getFeatureAsElementList(InternalFeature.COMPLEMENTS.ToString());

            var indirectCount = 0;
            foreach (var eachElement in complements)
            {
                if (DiscourseFunction.INDIRECT_OBJECT.Equals(eachElement
                    .getFeature(InternalFeature.DISCOURSE_FUNCTION.ToString())))
                {
                    indirectCount++;
                }
            }
            Assert.AreEqual(1, indirectCount); // only one indirect object
            // where
            // there were two before

            Assert.AreEqual(
                "slowly gives the woman and the boy the dog behind the curtain in the room",
                this.realiser.realise(this.give).getRealisation());
        }

        /**
         * Test for complements raised in the passive case.
         */

        [Test]
        public void passiveComplementTest()
        {
            NPPhraseSpec woman2 = this.phraseFactory.createNounPhrase("the", "woman");
            NPPhraseSpec dog2 = this.phraseFactory.createNounPhrase("the", "dog");
            VPPhraseSpec give2 = this.phraseFactory.createVerbPhrase("give");
            PPPhraseSpec behindTheCurtain2 = this.phraseFactory.createPrepositionPhrase("behind");
            NPPhraseSpec np52 = this.phraseFactory.createNounPhrase("the", "curtain");
            behindTheCurtain2.addComplement(np52);

            PPPhraseSpec inTheRoom2 = this.phraseFactory.createPrepositionPhrase("in");
            NPPhraseSpec np62 = this.phraseFactory.createNounPhrase("the", "room");
            inTheRoom2.addComplement(np62);


            // add some arguments
            dog2.setFeature(InternalFeature.DISCOURSE_FUNCTION.ToString(),
                DiscourseFunction.OBJECT);
            woman2.setFeature(InternalFeature.DISCOURSE_FUNCTION.ToString(),
                DiscourseFunction.INDIRECT_OBJECT);
            give2.addComplement(dog2);
            give2.addComplement(woman2);
            Assert.AreEqual("gives the woman the dog", this.realiser.realise(
                give2).getRealisation());

            // add a few premodifiers and postmodifiers
            give2.addPreModifier("slowly");
            give2.addPostModifier(behindTheCurtain2);
            give2.addPostModifier(inTheRoom2);
            Assert.AreEqual(
                "slowly gives the woman the dog behind the curtain in the room",
                this.realiser.realise(give2).getRealisation());

            // passivise: This should suppress "the dog"
            give2.clearComplements();
            give2.addComplement(dog2);
            give2.addComplement(woman2);
            give2.setFeature(Feature.PASSIVE.ToString(), true);

            Assert.AreEqual(
                "is slowly given the woman behind the curtain in the room",
                this.realiser.realise(give2).getRealisation());
        }

        /**
         * Test VP with sentential complements. This tests for structures like "said
         * that John was walking"
         */

        [Test]
        public void clausalComplementTest()
        {
            this.phraseFactory.setLexicon(this.lexicon);
            var s = this.phraseFactory.createClause();

            s.setSubject(this.phraseFactory
                .createNounPhrase("John"));

            // Create a sentence first
            var maryAndSusan = new CoordinatedPhraseElement(
                this.phraseFactory.createNounPhrase("Mary"),
                this.phraseFactory.createNounPhrase("Susan"));

            this.kiss.clearComplements();
            s.setVerbPhrase(this.kiss);
            s.setObject(maryAndSusan);
            s.setFeature(Feature.PROGRESSIVE.ToString(), true);
            s.setFeature(Feature.TENSE.ToString(), Tense.PAST);
            s.addPostModifier(this.inTheRoom);
            Assert.AreEqual("John was kissing Mary and Susan in the room",
                this.realiser.realise(s).getRealisation());

            // make the main VP past
            this.say.setFeature(Feature.TENSE.ToString(), Tense.PAST);
            Assert.AreEqual("said", this.realiser.realise(this.say)
                .getRealisation());

            // now add the sentence as complement of "say". Should make the sentence
            // subordinate
            // note that sentential punctuation is suppressed
            this.say.addComplement(s);
            Assert.AreEqual(
                "said that John was kissing Mary and Susan in the room",
                this.realiser.realise(this.say).getRealisation());

            // add a postModifier to the main VP
            // yields [says [that John was kissing Mary and Susan in the room]
            // [behind the curtain]]
            this.say.addPostModifier(this.behindTheCurtain);
            Assert.AreEqual(
                "said that John was kissing Mary and Susan in the room behind the curtain",
                this.realiser.realise(this.say).getRealisation());

            // create a new sentential complement
            var s2 = this.phraseFactory.createClause(this.phraseFactory
                    .createNounPhrase("all"),
                "be",
                this.phraseFactory.createAdjectivePhrase("fine"));

            s2.setFeature(Feature.TENSE.ToString(), Tense.FUTURE);
            Assert.AreEqual("all will be fine", this.realiser.realise(s2)
                .getRealisation());

            // add the new complement to the VP
            // yields [said [that John was kissing Mary and Susan in the room and
            // all will be fine] [behind the curtain]]
            var s3 = new CoordinatedPhraseElement(s, s2);
            this.say.clearComplements();
            this.say.addComplement(s3);

            // first with outer complementiser suppressed
            s3.setFeature(Feature.SUPRESSED_COMPLEMENTISER.ToString(), true);
            Assert.AreEqual(
                "said that John was kissing Mary and Susan in the room "
                + "and all will be fine behind the curtain",
                this.realiser.realise(this.say).getRealisation());

            setUp();

            s = this.phraseFactory.createClause();

            s.setSubject(this.phraseFactory
                .createNounPhrase("John"));

            // Create a sentence first
            maryAndSusan = new CoordinatedPhraseElement(
                this.phraseFactory.createNounPhrase("Mary"),
                this.phraseFactory.createNounPhrase("Susan"));

            s.setVerbPhrase(this.kiss);
            s.setObject(maryAndSusan);
            s.setFeature(Feature.PROGRESSIVE.ToString(), true);
            s.setFeature(Feature.TENSE.ToString(), Tense.PAST);
            s.addPostModifier(this.inTheRoom);
            s2 = this.phraseFactory.createClause(this.phraseFactory
                    .createNounPhrase("all"),
                "be",
                this.phraseFactory.createAdjectivePhrase("fine"));

            s2.setFeature(Feature.TENSE.ToString(), Tense.FUTURE);
            // then with complementiser not suppressed and not aggregated
            s3 = new CoordinatedPhraseElement(s, s2);
            this.say.addComplement(s3);
            this.say.setFeature(Feature.TENSE.ToString(), Tense.PAST);
            this.say.addPostModifier(this.behindTheCurtain);

            this.realiser.setDebugMode(true);
            Assert.AreEqual(
                "said that John was kissing Mary and Susan in the room and "
                + "that all will be fine behind the curtain",
                this.realiser.realise(this.say).getRealisation());

        }

        /**
         * Test VP coordination and aggregation:
         * <OL>
         * <LI>If the simplenlg.features of a coordinate VP are set, they should be
         * inherited by its daughter VP;</LI>
         * <LI>2. We can aggregate the coordinate VP so it's realised with one
         * wide-scope auxiliary</LI>
         */

        [Test]
        public void coordinationTest()
        {
            // simple case
            VPPhraseSpec kiss2 = this.phraseFactory.createVerbPhrase("kiss");
            kiss2.addComplement(this.dog);
            VPPhraseSpec kick2 = this.phraseFactory.createVerbPhrase("kick");
            kick2.addComplement(this.boy);

            var coord1 = new CoordinatedPhraseElement(
                kiss2, kick2);

            coord1.setFeature(Feature.PERSON.ToString(), Person.THIRD);
            coord1.setFeature(Feature.TENSE.ToString(), Tense.PAST);
            Assert.AreEqual("kissed the dog and kicked the boy", this.realiser
                .realise(coord1).getRealisation());

            // with negation: should be inherited by all components
            coord1.setFeature(Feature.NEGATED.ToString(), true);
            this.realiser.setLexicon(this.lexicon);
            Assert.AreEqual("did not kiss the dog and did not kick the boy",
                this.realiser.realise(coord1).getRealisation());

            // set a modal
            coord1.setFeature(Feature.MODAL.ToString(), "could");
            Assert.AreEqual(
                "could not have kissed the dog and could not have kicked the boy",
                this.realiser.realise(coord1).getRealisation());

            // set perfect and progressive
            coord1.setFeature(Feature.PERFECT.ToString(), true);
            coord1.setFeature(Feature.PROGRESSIVE.ToString(), true);
            Assert.AreEqual("could not have been kissing the dog and "
                            + "could not have been kicking the boy", this.realiser.realise(
                coord1).getRealisation());

            // now aggregate
            coord1.setFeature(Feature.AGGREGATE_AUXILIARY.ToString(), true);
            Assert.AreEqual(
                "could not have been kissing the dog and kicking the boy",
                this.realiser.realise(coord1).getRealisation());
        }
    }
}