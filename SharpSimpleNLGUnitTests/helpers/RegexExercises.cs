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
using Shouldly;

namespace SimpleNLG.Extensions
{
    /**
    * @author Nick Hodge, nhodge@mungr.com 
    */

    public class RegexExercises
    {
        // The Regex engine in Java is different to .NET, and there are a couple of 
        // idioms/semantic difference "exercised" and tested here.
        // ref: MorphologyRules.cs

        // this checks the "matches" and "replaceAll" extension methods as found
        [Test]
        public void TestDotNetRegex_vsJava_Nouns()
        {
            // Firstly, Noun Plural Morpholology

            var baseForm1 = "carry away";
            if (baseForm1.matches(@".*[b-z-[eiou]]y\\b"))
            {
                baseForm1.replaceAll(@"y\b", @"ies").ShouldBe("carried away");
            }

            var baseForm1a = "cazy away";
            if (baseForm1a.matches(@".*[b-z-[eiou]]y\\b"))
            {
                baseForm1a.replaceAll(@"y\b", @"ies").ShouldBe("cazies away");
            }

            var baseForm2 = "cash";
            if (baseForm2.matches(@".*([szx]|[cs]h)\\b"))
            {
                (baseForm2 + "es").ShouldBe("cashes");
            }

            var baseForm2a = "cax";
            if (baseForm2a.matches(@".*([szx]|[cs]h)\\b"))
            {
                (baseForm2a + "es").ShouldBe("caxes");
            }

            var baseForm3 = "documentus impartum";
            if (baseForm3.endsWith("us"))
            {
                baseForm3.replaceAll(@"us\b", "i").ShouldBe("documenti impartum");

            }

            var baseForm4 = "documentus impartum onton";
            if (baseForm4.matches(@".*[(um)(on)]\\b"))
            {
                baseForm4.replaceAll(@"[(um)(on)]\\b", "a").ShouldBe("documentus imparta onta");
            }

            var baseForm5 = "sepsis";
            if (baseForm5.endsWith("sis"))
            {
                baseForm5.replaceAll(@"sis\b", "ses").ShouldBe("sepses");
            }

            var baseForm6 = "markis";
            if (baseForm6.endsWith("is"))
            {
                baseForm6.replaceAll(@"is\b", "ides").ShouldBe("markides");
            }

            var baseForm7 = "cognomen";
            if (baseForm7.endsWith("men"))
            {
                baseForm7.replaceAll(@"men\b", "mina").ShouldBe("cognomina");
            }

            var baseForm8 = "index";
            if (baseForm8.endsWith("ex"))
            {
                baseForm8.replaceAll(@"ex\b", "ices").ShouldBe("indices");
            }

            var baseForm9 = "wax";
            if (baseForm9.endsWith("x"))
            {
                baseForm9.replaceAll(@"x\b", "ces").ShouldBe("waces");
            }

        }

        [Test]
        public void TestDotNetRegex_vsJava_Verbs()
        {
            var baseForm1 = "come";
            if (baseForm1.matches(@".*[^iyeo]e\\b"))
            {
                baseForm1.replaceAll(@"e\b", "ing").ShouldBe("coming");
            }

            var baseForm1a = "cooee";
            if (baseForm1a.matches(@".*[^iyeo]e\\b"))
            {
                // actually shouldnt match so want fall through to here
                baseForm1a.replaceAll(@"e\b", "ing").ShouldBe("banana");
            }

            var baseForm2 = "carry";
            if (baseForm2.matches(@".*[b-z-[eiou]]y\\b"))
                {
                baseForm2.replaceAll(@"y\b", "ied").ShouldBe("carried");
            }
        }

        [Test]
        public void AN_AGREEMENT_DeterminierAgrHelper()
        {
            var AN_AGREEMENT = @"\A(a|e|i|o|u).*";
            var lowercaseInput = "enormous".toLowerCase();

            lowercaseInput.matches(AN_AGREEMENT).ShouldBeTrue();


        }
    }
}