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
using System.Text;
using NUnit.Framework;
using SimpleNLG;
using SimpleNLG.Extensions;

namespace SimpleNLGTests.syntax
{
    public class OrthographyFormatTest : SimpleNLG4TestBase
    {

        public DocumentElement list1, list2;
        public DocumentElement listItem1, listItem2, listItem3;
        public string list1Realisation = new StringBuilder().Append("\n* behind the curtain").Append("\n").ToString();

        public string list2Realisation;

        [SetUp]
        public void setUp()
        {
            list2Realisation = new StringBuilder("* in the room").append("\n* ").append(list1Realisation).append("\n").ToString();

            // need to set formatter for realiser (set to null in the test
            // superclass)
            this.realiser.setFormatter(new TextFormatter());

            // a couple phrases as list items
            listItem1 = this.phraseFactory.createListItem(this.inTheRoom);
            listItem2 = this.phraseFactory
                .createListItem(this.behindTheCurtain);
            listItem3 = this.phraseFactory.createListItem(this.onTheRock);

            // a simple depth-1 list of phrases
            list1 = this.phraseFactory
                .createList( new List<INLGElement>
                {
                    listItem1, listItem2
                });

            /* a list consisting of one phrase (depth-1) + a list )(depth-2)
            list2 = this.phraseFactory.createList(
               new List<NLGElement> { new List<NLGElement>
                {
                    listItem3,
                    this.phraseFactory.createListItem(list1)
                }}); */
        }


        /**
         * Test the realisation of a list with an embedded list
         */

        //[Test]
        public void testEmbeddedListOrthography()
        {
            var realised = this.realiser.realise(list2);
            Assert.AreEqual(list2Realisation, realised.getRealisation());
        }

        /**
         * Test the realisation of appositive pre-modifiers with commas around them.
         */

        [Test]
        public void testAppositivePreModifiers()
        {
            var subject = this.phraseFactory.createNounPhrase("I");
            var obj = this.phraseFactory.createNounPhrase("a bag");

            var _s1 = this.phraseFactory.createClause(subject,
                "carry", obj);

            // add a PP complement
            var pp = this.phraseFactory.createPrepositionPhrase("on",
                this.phraseFactory.createNounPhrase("most", "Tuesdays"));
            _s1.addPreModifier(pp);

            //without appositive feature on pp
            Assert.AreEqual(
                "I on most Tuesdays carry a bag", this.realiser
                    .realise(_s1).getRealisation());

            //with appositive feature
            pp.setFeature(Feature.APPOSITIVE.ToString(), true);
            Assert.AreEqual(
                "I, on most Tuesdays, carry a bag", this.realiser
                    .realise(_s1).getRealisation());
        }


        /**
         * Test the realisation of appositive pre-modifiers with commas around them.
         */

        [Test]
        public void testCommaSeparatedFrontModifiers()
        {
            var subject = this.phraseFactory.createNounPhrase("I");
            var obj = this.phraseFactory.createNounPhrase("a bag");

            var _s1 = this.phraseFactory.createClause(subject,
                "carry", obj);

            // add a PP complement
            var pp1 = this.phraseFactory.createPrepositionPhrase("on",
                this.phraseFactory.createNounPhrase("most", "Tuesdays"));
            _s1.addFrontModifier(pp1);

            var pp2 = this.phraseFactory.createPrepositionPhrase("since",
                this.phraseFactory.createNounPhrase("1991"));
            _s1.addFrontModifier(pp2);
            pp1.setFeature(Feature.APPOSITIVE.ToString(), true);
            pp2.setFeature(Feature.APPOSITIVE.ToString(), true);

            //without setCommaSepCuephrase
            Assert.AreEqual(
                "on most Tuesdays since 1991 I carry a bag", this.realiser
                    .realise(_s1).getRealisation());

            //with setCommaSepCuephrase
            this.realiser.setCommaSepCuephrase(true);
            Assert.AreEqual(
                "on most Tuesdays, since 1991, I carry a bag", this.realiser
                    .realise(_s1).getRealisation());
        }

        /**
         * Ensure we don't end up with doubled commas.
         */

        [Test]
        public void testNoDoubledCommas()
        {
            var subject = this.phraseFactory.createNounPhrase("I");
            var obj = this.phraseFactory.createNounPhrase("a bag");

            var _s1 = this.phraseFactory.createClause(subject,
                "carry", obj);

            var pp1 = this.phraseFactory.createPrepositionPhrase("on",
                this.phraseFactory.createNounPhrase("most", "Tuesdays"));
            _s1.addFrontModifier(pp1);

            var pp2 = this.phraseFactory.createPrepositionPhrase("since",
                this.phraseFactory.createNounPhrase("1991"));
            var pp3 = this.phraseFactory.createPrepositionPhrase("except",
                this.phraseFactory.createNounPhrase("yesterday"));

            pp2.setFeature(Feature.APPOSITIVE.ToString(), true);
            pp3.setFeature(Feature.APPOSITIVE.ToString(), true);

            pp1.addPostModifier(pp2);
            pp1.addPostModifier(pp3);

            this.realiser.setCommaSepCuephrase(true);

            Assert.AreEqual(
                "on most Tuesdays, since 1991, except yesterday, I carry a bag", this.realiser
                    .realise(_s1).getRealisation());
            // without my fix (that we're testing here), you'd end up with 
            // "on most Tuesdays, since 1991,, except yesterday, I carry a bag"
        }

// <[on most Tuesdays, since 1991, except yesterday, ]I carry a bag> but was:<[]I carry a bag>


    }
}