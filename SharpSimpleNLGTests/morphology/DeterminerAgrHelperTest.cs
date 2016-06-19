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

namespace SimpleNLGTests.external
{

    public class DeterminerAgrHelperTest
    {

        [Test]
        public void testRequiresAn()
        {

            Assert.IsTrue(DeterminerAgrHelper.requiresAn("elephant"));

            Assert.IsFalse(DeterminerAgrHelper.requiresAn("cow"));

            // Does not hand phonetics
            Assert.IsFalse(DeterminerAgrHelper.requiresAn("hour"));

            // But does have exceptions for some numerals
            Assert.IsFalse(DeterminerAgrHelper.requiresAn("one"));

            Assert.IsFalse(DeterminerAgrHelper.requiresAn("100"));

        }

        [Test]
        public void testCheckEndsWithIndefiniteArticle1()
        {

            var cannedText = "I see a";

            var np = "elephant";

            var expected = "I see an";

            var actual = DeterminerAgrHelper.checkEndsWithIndefiniteArticle(cannedText, np);

            Assert.AreEqual(expected, actual);

        }

        [Test]
        public void testCheckEndsWithIndefiniteArticle2()
        {

            var cannedText = "I see a";

            var np = "cow";

            var expected = "I see a";

            var actual = DeterminerAgrHelper.checkEndsWithIndefiniteArticle(cannedText, np);

            Assert.AreEqual(expected, actual);

        }

        [Test]
        public void testCheckEndsWithIndefiniteArticle3()
        {

            var cannedText = "I see an";

            var np = "cow";

            // Does not handle "an" -> "a"
            var expected = "I see an";

            var actual = DeterminerAgrHelper.checkEndsWithIndefiniteArticle(cannedText, np);

            Assert.AreEqual(expected, actual);

        }
    }
}
