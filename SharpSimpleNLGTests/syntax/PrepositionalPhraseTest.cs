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
    // TODO: Auto-generated Javadoc
    /**
     * This class groups together some tests for prepositional phrases and
     * coordinate prepositional phrases.
     * @author agatt
     */

    public class PrepositionalPhraseTest : SimpleNLG4TestBase
    {


        [Test]
        public void testBasic()
        {
            Assert.AreEqual("in the room", this.realiser 
                .realise(this.inTheRoom).getRealisation());
            Assert.AreEqual("behind the curtain", this.realiser 
                .realise(this.behindTheCurtain).getRealisation());
            Assert.AreEqual("on the rock", this.realiser 
                .realise(this.onTheRock).getRealisation());
        }

        /**
         * Test for coordinate NP complements of PPs.
         */

        [Test]
        public void testComplementation()
        {
            this.inTheRoom.clearComplements();
            this.inTheRoom.addComplement(new CoordinatedPhraseElement(
                this.phraseFactory.createNounPhrase("the", "room"),  
                this.phraseFactory.createNounPhrase("a", "car"))); 
            Assert.AreEqual("in the room and a car", this.realiser 
                .realise(this.inTheRoom).getRealisation());
        }

        /**
         * Test for PP coordination.
         */

        public void testCoordination()
        {
            // simple coordination

            var coord1 = new CoordinatedPhraseElement(
                this.inTheRoom, this.behindTheCurtain);
            Assert.AreEqual("in the room and behind the curtain", this.realiser 
                .realise(coord1).getRealisation());

            // change the conjunction
            coord1.setFeature(Feature.CONJUNCTION.ToString(), "or"); 
            Assert.AreEqual("in the room or behind the curtain", this.realiser 
                .realise(coord1).getRealisation());

            // new coordinate
            var coord2 = new CoordinatedPhraseElement(
                this.onTheRock, this.underTheTable);
            coord2.setFeature(Feature.CONJUNCTION.ToString(), "or"); 
            Assert.AreEqual("on the rock or under the table", this.realiser 
                .realise(coord2).getRealisation());

            // coordinate two coordinates
            var coord3 = new CoordinatedPhraseElement(coord1,
                coord2);

            var text = this.realiser.realise(coord3).getRealisation();
            Assert
                .AreEqual(
                    "in the room or behind the curtain and on the rock or under the table", 
                    text);
        }
    }
}