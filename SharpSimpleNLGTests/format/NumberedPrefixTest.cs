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

namespace SimpleNLGTests.morphology
{
    public class NumberedPrefixTest
    {

        [Test]
        public void testNewInstancePrefixIsZero()
        {
            NumberedPrefix prefix = new NumberedPrefix();
            Assert.AreEqual("0", prefix.getPrefix());
        }

        [Test]
        public void testIncrementFromNewInstanceIsOne()
        {
            NumberedPrefix prefix = new NumberedPrefix();
            prefix.increment();
            Assert.AreEqual("1", prefix.getPrefix());
        }

        [Test]
        public void testIncrementForTwoPointTwoIsTwoPointThree()
        {
            NumberedPrefix prefix = new NumberedPrefix();
            prefix.setPrefix("2.2");
            prefix.increment();
            Assert.AreEqual("2.3", prefix.getPrefix());
        }

        [Test]
        public void testIncrementForThreePointFourPointThreeIsThreePointFourPointFour()
        {
            NumberedPrefix prefix = new NumberedPrefix();
            prefix.setPrefix("3.4.3");
            prefix.increment();
            Assert.AreEqual("3.4.4", prefix.getPrefix());
        }

        [Test]
        public void testUpALevelForNewInstanceIsOne()
        {
            NumberedPrefix prefix = new NumberedPrefix();
            prefix.upALevel();
            Assert.AreEqual("1", prefix.getPrefix());
        }

        [Test]
        public void testDownALevelForNewInstanceIsZero()
        {
            NumberedPrefix prefix = new NumberedPrefix();
            prefix.downALevel();
            Assert.AreEqual("0", prefix.getPrefix());
        }

        [Test]
        public void testDownALevelForSevenIsZero()
        {
            NumberedPrefix prefix = new NumberedPrefix();
            prefix.setPrefix("7");
            prefix.downALevel();
            Assert.AreEqual("0", prefix.getPrefix());
        }

        [Test]
        public void testDownALevelForTwoPointSevenIsTwo()
        {
            NumberedPrefix prefix = new NumberedPrefix();
            prefix.setPrefix("2.7");
            prefix.downALevel();
            Assert.AreEqual("2", prefix.getPrefix());
        }

        [Test]
        public void testDownALevelForThreePointFourPointThreeIsThreePointFour()
        {
            NumberedPrefix prefix = new NumberedPrefix();
            prefix.setPrefix("3.4.3");
            prefix.downALevel();
            Assert.AreEqual("3.4", prefix.getPrefix());
        }
    }
}