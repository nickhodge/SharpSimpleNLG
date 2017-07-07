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

namespace SimpleNLGTests.syntax
{
/**
     * Some tests for aggregation.
     * 
     * @author Albert Gatt, University of Malta & University of Aberdeen
     * 
     */

    public class ClauseAggregationTest : SimpleNLG4TestBase
    {
        // set up a few more fixtures
        /** The s4. */
        public SPhraseSpec s5, s6;
        public Aggregator aggregator = new Aggregator();
        public ClauseCoordinationRule coord  = new ClauseCoordinationRule();
        public ForwardConjunctionReductionRule fcr = new ForwardConjunctionReductionRule();
        public BackwardConjunctionReductionRule bcr = new BackwardConjunctionReductionRule();

        /**
         * Instantiates a new clause aggregation test.
         * 
         * @param name
         *            the name
         */

		[SetUp]
        protected override void setUp()
        {
            aggregator.initialise();

            // the woman kissed the man behind the curtain
            s1 = this.phraseFactory.createClause();
            s1.setSubject(this.woman);
            s1.setVerbPhrase(this.phraseFactory.createVerbPhrase("kiss"));
            s1.setObject(this.man);
            s1.addPostModifier(this.phraseFactory
                .createPrepositionPhrase("behind", this.phraseFactory
                    .createNounPhrase("the", "curtain")));

            // the woman kicked the dog on the rock
            s2 = this.phraseFactory.createClause();
            s2.setSubject(this.phraseFactory.createNounPhrase("the", "woman")); 
            s2.setVerbPhrase(this.phraseFactory.createVerbPhrase("kick")); 
            s2.setObject(this.phraseFactory.createNounPhrase("the", "dog"));
            s2.addPostModifier(this.onTheRock);

            // the woman kicked the dog behind the curtain
            s3 = this.phraseFactory.createClause();
            s3.setSubject(this.phraseFactory.createNounPhrase("the", "woman")); 
            s3.setVerbPhrase(this.phraseFactory.createVerbPhrase("kick")); 
            s3.setObject(this.phraseFactory.createNounPhrase("the", "dog"));
            s3.addPostModifier(this.phraseFactory
                .createPrepositionPhrase("behind", this.phraseFactory
                    .createNounPhrase("the", "curtain")));

            // the man kicked the dog behind the curtain
            s4 = this.phraseFactory.createClause();
            s4.setSubject(this.man); 
            s4.setVerbPhrase(this.phraseFactory.createVerbPhrase("kick")); 
            s4.setObject(this.phraseFactory.createNounPhrase("the", "dog"));
            s4.addPostModifier(this.behindTheCurtain);

            // the girl kicked the dog behind the curtain
            s5 = this.phraseFactory.createClause();
            s5.setSubject(this.phraseFactory.createNounPhrase("the", "girl")); 
            s5.setVerbPhrase(this.phraseFactory.createVerbPhrase("kick")); 
            s5.setObject(this.phraseFactory.createNounPhrase("the", "dog"));
            s5.addPostModifier(this.behindTheCurtain);

            // the woman kissed the dog behind the curtain
            s6 = this.phraseFactory.createClause();
            s6.setSubject(this.phraseFactory.createNounPhrase("the", "woman")); 
            s6.setVerbPhrase(this.phraseFactory.createVerbPhrase("kiss")); 
            s6.setObject(this.phraseFactory.createNounPhrase("the", "dog"));
            s6.addPostModifier(this.phraseFactory
                .createPrepositionPhrase("behind", this.phraseFactory
                    .createNounPhrase("the", "curtain")));
        }


        [TearDown]
        public void tearDown()
        {
            s1 = null;
            s2 = null;
            s3 = null;
            s4 = null;
            s5 = null;
            s6 = null;

        }
				/**
         * Test clause coordination with two sentences with same subject but
         * different postmodifiers: fails
         */
        [Test]
        public void testCoordinationSameSubjectFail()
        {
            var elements =  new List<INLGElement> { this.s1, this.s2};
            var result = this.coord.apply(elements);
            Assert.AreEqual(1, result.size());
        }

        /**
         * Test clause coordination with two sentences one of which is passive:
         * fails
         */
        [Test]
        public void testCoordinationPassiveFail()
        {
            s1.setFeature(Feature.PASSIVE.ToString(), true);
            var elements = new List<INLGElement> {s1, s2};
            var result = coord.apply(elements);
            Assert.AreEqual(2, result.size());
        }

        //	/**
        //	 * Test clause coordination with 2 sentences with same subject: succeeds
        //	 */
        //	[Test]
        //	public void testCoordinationSameSubjectSuccess() {
        //		List<NLGElement> elements = Arrays.asList((NLGElement) this.s1,
        //				(NLGElement) this.s3);
        //		List<NLGElement> result = this.coord.apply(elements);
        //		Assert.assertTrue(result.size() == 1); // should only be one sentence
        //		NLGElement aggregated = result.get(0);
        //		Assert
        //				.AreEqual(
        //						"the woman kisses the man and kicks the dog behind the curtain", 
        //						this.realiser.realise(aggregated).getRealisation());
        //	}

        /**
         * Test clause coordination with 2 sentences with same VP: succeeds
         */
        [Test]
        public void testCoordinationSameVP()
        {
            var elements = new List<INLGElement> { s3, s4 };
            var result = coord.apply(elements);
            Assert.IsTrue(result.size() == 1); // should only be one sentence
            var aggregated = result.get(0);
            Assert.AreEqual(
                "the woman kicks the dog behind the curtain", 
                this.realiser.realise(aggregated).getRealisation());
       }

          [Test]
        public void testCoordinationWithModifiers()
        {
            // now add a couple of front modifiers
            s3.addFrontModifier(this.phraseFactory
                .createAdverbPhrase("however"));
            s4.addFrontModifier(this.phraseFactory
                .createAdverbPhrase("however"));
            var elements = new List<INLGElement> { s3, s4 };
            var result = coord.apply(elements);
            Assert.IsTrue(result.size() == 1); // should only be one sentence
            var aggregated = result.get(0);
            Assert
                .AreEqual(
                    "however the woman kicks the dog behind the curtain", 
                    this.realiser.realise(aggregated).getRealisation());
        }

        /**
         * Test coordination of 3 sentences with the same VP
         */
       // [Test]
        public void testCoordinationSameVP2()
        {
            var elements = new List<INLGElement> { s3, s4, s5 };
            var result = coord.apply(elements);
            Assert.IsTrue(result.size() == 1); // should only be one sentence
            var aggregated = result.get(0);
            Assert
                .AreEqual(
                    "the woman and the man and the girl kick the dog behind the curtain", 
                    this.realiser.realise(aggregated).getRealisation());
        }

        /**
         * Forward conjunction reduction test
         */
        [Test]
        public void testForwardConjReduction()
        {
            var aggregated = fcr.apply(s2, s3);
            Assert
                .AreEqual(
                    "the woman kicks the dog on the rock and kicks the dog behind the curtain", 
                    this.realiser.realise(aggregated).getRealisation());
        }

        /**
         * Backward conjunction reduction test
         */
        [Test]
        public void testBackwardConjunctionReduction()
        {
            var aggregated = bcr.apply(s3, s6);
            Assert
                .AreEqual(
                    "the woman kicks and the woman kisses the dog behind the curtain",
                    this.realiser.realise(aggregated).getRealisation());
        }

        /**
         * Test multiple aggregation procedures in a single aggregator. 
         */
//	[Test]
//	public void testForwardBackwardConjunctionReduction() {
//		this.aggregator.addRule(this.fcr);
//		this.aggregator.addRule(this.bcr);
//		realiser.setDebugMode(true);
//		List<NLGElement> result = this.aggregator.realise(Arrays.asList((NLGElement) this.s2, (NLGElement) this.s3));
//		Assert.assertTrue(result.size() == 1); // should only be one sentence
//		NLGElement aggregated = result.get(0);
//		NLGElement aggregated = this.phraseFactory.createdCoordinatedPhrase(this.s2, this.s3);
//		Assert
//				.AreEqual(
//						"the woman kicks the dog on the rock and behind the curtain", 
//						this.realiser.realise(aggregated).getRealisation());
//	}

    }
}