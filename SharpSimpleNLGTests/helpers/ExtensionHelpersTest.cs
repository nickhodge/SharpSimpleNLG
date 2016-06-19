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
using Shouldly;
using SimpleNLG.Extensions;

namespace SimpleNLGTests.helpers
{

    /**
     * @author Nick Hodge, nhodge@mungr.com
     * 
     */

    public class ExtensionHelpersTests
    {
        // String and Char ones first


        //ref: https://docs.oracle.com/javase/7/docs/api/java/lang/Character.html#isDigit(char)
        [TestCase(' ', ExpectedResult = false, TestName = "isDigit: space case")]
        [TestCase('a', ExpectedResult = false, TestName = "isDigit: single char a")]
        [TestCase('0', ExpectedResult = true, TestName = "isDigit: single char 0")]
        [TestCase('1', ExpectedResult = true, TestName = "isDigit: single char 1")]
        [TestCase('2', ExpectedResult = true, TestName = "isDigit: single char 2")]
        [TestCase('3', ExpectedResult = true, TestName = "isDigit: single char 3")]
        [TestCase('4', ExpectedResult = true, TestName = "isDigit: single char 4")]
        [TestCase('5', ExpectedResult = true, TestName = "isDigit: single char 5")]
        [TestCase('6', ExpectedResult = true, TestName = "isDigit: single char 6")]
        [TestCase('7', ExpectedResult = true, TestName = "isDigit: single char 7")]
        [TestCase('8', ExpectedResult = true, TestName = "isDigit: single char 8")]
        [TestCase('9', ExpectedResult = true, TestName = "isDigit: single char 9")]
        [TestCase('\u0660', ExpectedResult = true, TestName = "isDigit: arabic/indic start")]
        [TestCase('\u0669', ExpectedResult = true, TestName = "isDigit: arabic/indic end")]
        [TestCase('\u06F0', ExpectedResult = true, TestName = "isDigit: arabic extended start")]
        [TestCase('\u06F9', ExpectedResult = true, TestName = "isDigit: arabic extended end")]
        [TestCase('\u0966', ExpectedResult = true, TestName = "isDigit: Devanagari extended start")]
        [TestCase('\u096F', ExpectedResult = true, TestName = "isDigit: Devanagari extended end")]
        [TestCase('\uFF10', ExpectedResult = true, TestName = "isDigit: Fullwidth extended start")]
        [TestCase('\uFF19', ExpectedResult = true, TestName = "isDigit: Fullwidth extended end")]
        public bool isDigit_ToCsharp(char c)
        {
            return c.isDigit();
        }

        // ref: https://docs.oracle.com/javase/7/docs/api/java/lang/String.html#length()
        [TestCase("", ExpectedResult = 0, TestName = "length: null case")]
        [TestCase(" ", ExpectedResult = 1, TestName = "length: 1")]
        [TestCase("  ", ExpectedResult = 2, TestName = "length: 2")]
        [TestCase("cat", ExpectedResult = 3, TestName = "length: 3")]
        [TestCase("\uFF19cat", ExpectedResult = 4, TestName = "length: 4 with Unicode")]
        public int length_ToCsharp(string s)
        {
            return s.length();
        }

        // ref: https://docs.oracle.com/javase/7/docs/api/java/lang/String.html#trim()
        [TestCase("", ExpectedResult = "", TestName = "trim: null case")]
        [TestCase("cat", ExpectedResult = "cat", TestName = "trim: 3 chars")]
        [TestCase(" \uFF19cat", ExpectedResult = "\uFF19cat",
             TestName = "trim: 4 chars with Unicode, leading and trailing")]
        [TestCase(" cat", ExpectedResult = "cat", TestName = "trim: 4 chars with leading space")]
        [TestCase("cat ", ExpectedResult = "cat", TestName = "trim: 4 chars with trailing space")]
        [TestCase(" cat ", ExpectedResult = "cat", TestName = "trim: 4 chars leading and trailing space")]
        [TestCase("\uFF19cat", ExpectedResult = "\uFF19cat", TestName = "trim: 4 chars with Unicode, leading spcae")]
        [TestCase(" \uFF19cat", ExpectedResult = "\uFF19cat",
             TestName = "trim: 4 chars with Unicode, leading and trailing")]
        [TestCase("\uFF19cat ", ExpectedResult = "\uFF19cat",
             TestName = "trim: 4 chars with Unicode, leading and trailing")]
        [TestCase(" \uFF19cat ", ExpectedResult = "\uFF19cat", TestName = "trim: 4 chars with Unicode")]
        [TestCase("\r\uFF19cat ", ExpectedResult = "\uFF19cat",
             TestName = "trim: 4 chars with Unicode, return char at start")]
        [TestCase("\r\uFF19cat \n", ExpectedResult = "\uFF19cat",
             TestName = "trim: 4 chars with Unicode, return at start, new line char at end")]
        public string trim_ToCsharp(string s)
        {
            return s.trim();
        }


        // ref: https://docs.oracle.com/javase/7/docs/api/java/lang/String.html#toLowerCase()
        [TestCase("", ExpectedResult = "", TestName = "toLower: null")]
        [TestCase("cat", ExpectedResult = "cat", TestName = "toLower: simple")]
        [TestCase("CAT", ExpectedResult = "cat", TestName = "toLower: all caps")]
        [TestCase("cAt", ExpectedResult = "cat", TestName = "toLower: mixed caps")]
        public string tolower_ToCsharp(string s)
        {
            return s.toLowerCase();
        }


        // ref: https://docs.oracle.com/javase/7/docs/api/java/lang/String.html#toUpperCase()
        [TestCase("", ExpectedResult = "", TestName = "toUpper: null")]
        [TestCase("cat", ExpectedResult = "CAT", TestName = "toUpper: simple")]
        [TestCase("CAT", ExpectedResult = "CAT", TestName = "toUpper: all caps")]
        [TestCase("cAt", ExpectedResult = "CAT", TestName = "toUpper: mixed caps")]
        public string toupper_ToCsharp(string s)
        {
            return s.toUpperCase();
        }

        // ref: https://docs.oracle.com/javase/7/docs/api/java/lang/String.html#equalsIgnoreCase(java.lang.String)
        [TestCase("cat", ExpectedResult = true, TestName = "equalsIgnoreCase: lower, true")]
        [TestCase("dog", ExpectedResult = false, TestName = "equalsIgnoreCase: lower, false")]
        [TestCase("Cat", ExpectedResult = true, TestName = "equalsIgnoreCase: mixed 1, true")]
        [TestCase("Dog", ExpectedResult = false, TestName = "equalsIgnoreCase: mixed, false")]
        [TestCase("CAT", ExpectedResult = true, TestName = "equalsIgnoreCase: tupper, rue")]
        [TestCase("CaT", ExpectedResult = true, TestName = "equalsIgnoreCase: mixed 2,true")]
        public bool equalsIgnoreCase_ToCsharp(string s)
        {
            return s.equalsIgnoreCase("cat");
        }

        // ref: https://docs.oracle.com/javase/7/docs/api/java/lang/String.html#indexOf(int)
        [TestCase("", ExpectedResult = -1, TestName = "indexOf string: null is negative")]
        [TestCase("CaT", ExpectedResult = 1, TestName = "indexOf string: in cat 1")]
        [TestCase("dog", ExpectedResult = -1, TestName = "indexOf string: in dog negative")]
        [TestCase("cta", ExpectedResult = 2, TestName = "indexOf string: in cat end")]
        [TestCase("atac", ExpectedResult = 0, TestName = "indexOf string: in cat beginning")]
        public int indexOf_string_ToCsharp(string s)
        {
            return s.indexOf("a");
        }

        [TestCase("", ExpectedResult = -1, TestName = "indexOf char: null is negative")]
        [TestCase("CaT", ExpectedResult = 1, TestName = "indexOf char: in cat 1")]
        public int indexOf_char_ToCsharp(string s)
        {
            return s.indexOf('a');
        }

        // ref: https://docs.oracle.com/javase/7/docs/api/java/lang/String.html#charAt(int)
        [TestCase(0, ExpectedResult = 'c', TestName = "charAt: in cat 1")]
        [TestCase(1, ExpectedResult = 'a', TestName = "charAt: in cat 2")]
        [TestCase(2, ExpectedResult = 't', TestName = "charAt: in cat 3")]
        [TestCase(3, ExpectedResult = '\uFF19', TestName = "charAt: in cat 4")]
        public char charAt_ToCsharp(int i)
        {
            return "cat\uFF19".charAt(i);
        }

        [TestCase("\uFF19", ExpectedResult = true, TestName = "endsWith: 1 in cat Unicode")]
        [TestCase("t\uFF19", ExpectedResult = true, TestName = "endsWith: 2 in cat Unicode")]
        [TestCase("at\uFF19", ExpectedResult = true, TestName = "endsWith: 3 in cat Unicode")]
        [TestCase("cat\uFF19", ExpectedResult = true, TestName = "endsWith: 4 in cat Unicode")]
        [TestCase("dog", ExpectedResult = false, TestName = "endsWith: false")]
        public bool endsWith_ToCsharp(string s)
        {
            return "cat\uFF19".endsWith(s);
        }


        [TestCase("dog", ExpectedResult = false, TestName = "contains: false")]
        [TestCase("cat", ExpectedResult = true, TestName = "contains: true cat")]
        [TestCase("\uFF19", ExpectedResult = true, TestName = "contains: true Unicode")]
        public bool contains_ToCsharp(string s)
        {
            return "cat\uFF19".contains(s);
        }


        [TestCase("c", ExpectedResult = true, TestName = "startsWith: 1 in cat Unicode")]
        [TestCase("ca", ExpectedResult = true, TestName = "startsWith: 2 in cat Unicode")]
        [TestCase("cat", ExpectedResult = true, TestName = "startsWith: 3 in cat Unicode")]
        [TestCase("cat\uFF19", ExpectedResult = true, TestName = "startsWith: 4 in cat Unicode")]
        [TestCase("dog", ExpectedResult = false, TestName = "startsWith: false")]
        public bool startsWith_ToCsharp(string s)
        {
            return "cat\uFF19".startsWith(s);
        }

        // ref: https://docs.oracle.com/javase/7/docs/api/java/lang/String.html#substring(int,%20int)
        // and contrast: https://msdn.microsoft.com/en-us/library/aka44szs(v=vs.110).aspx
        [Test]
        public void substring_ToCsharp()
        {
            // NOTE Java semantics: public String substring(int beginIndex, int endIndex) base 1
            // NOTE C# semantics: 

            "hamburger".substring(4, 8).ShouldBe("urge");
            "smiles".substring(1, 5).ShouldBe("mile");

            var s = "the cat sat on the matz";
            s.substring(0, 1).ShouldBe("t");
            s.substring(0, 2).ShouldBe("th");
            s.substring((s.Length - 1), (s.Length)).ShouldBe("z"); // just the last char
            s.substring(0, (s.Length - 1)).ShouldBe("the cat sat on the mat"); // all except last char
        }

        // ref: https://docs.oracle.com/javase/7/docs/api/java/lang/String.html#isEmpty()
        [TestCase("", ExpectedResult = true, TestName = "isEmpty: true")]
        [TestCase(" ", ExpectedResult = false, TestName = "isEmpty: true")]
        [TestCase("\uFF19", ExpectedResult = false, TestName = "isEmpty: true")]
        public bool isEmpty_ToCsharp(string s)
        {
            return s.isEmpty();
        }


        [TestCase("", ExpectedResult = false, TestName = "matches: false")]
        [TestCase("word", ExpectedResult = true, TestName = "matches: true")]
        public bool matches_ToCsharp(string s)
        {
            return s.matches(@"[\w]+");
        }

        [TestCase("word", ExpectedResult = "nerd", TestName = "replaceAll: nerd")]
        [TestCase("wordwo", ExpectedResult = "nerdne", TestName = "replaceAll: nerdier")]
        [TestCase("rd", ExpectedResult = "rd", TestName = "replaceAll: rd")]
        public string replaceAll_ToCsharp(string s)
        {
            return s.replaceAll(@"wo", "ne");
        }

        // ref: https://docs.oracle.com/javase/7/docs/api/java/util/ArrayList.html
        [Test]
        public void Lists_ToCsharp()
        {
            var one = new List<string>();
            var two = new List<string>() {"cat", "dog"};
            var three = new List<string>() {"cat", "dog", "cat"};

            one.isEmpty().ShouldBe(true);
            two.isEmpty().ShouldBe(false);

            one.size().ShouldBe(0);
            two.size().ShouldBe(2);

            two.get(0).ShouldBe(two[0]);
            two.get(0).ShouldBe("cat");

            two.get(1).ShouldBe(two[1]);
            two.get(1).ShouldBe("dog");

            three.indexOf("cat").ShouldBe(0);

            one.contains("cat").ShouldBe(false);
            two.contains("dog").ShouldBe(true);
            two.contains("tree").ShouldBe(false);

            three.remove("cat").ShouldBe(true); // remove first
            three.remove("cat").ShouldBe(true); // remove second
            three.remove("cat").ShouldBe(false); // no more to remove, so should return false

            three = new List<string>() {"cat", "dog", "cat"};
            three.removeAll(new List<string> {"cat"}).ShouldBe(true); // remove second
            three.remove("cat").ShouldBe(false); // no more to remove, so should return false


            two.clear();
            two.size().ShouldBe(0);
            two.isEmpty().ShouldBe(true);

            // now two is empty, let's continue
            two.add("horse").ShouldBe(true);
            two.size().ShouldBe(1);

            two.addAll(new List<string> {"cat", "dog"}).ShouldBe(true);
            two.size().ShouldBe(3);

            two.set(0, "cow").ShouldBe("cow");
            two[0].ShouldBe("cow");
            two[1].ShouldBe("cat");

        }

        // ref: https://docs.oracle.com/javase/7/docs/api/java/util/HashSet.html
        [Test]
        public void Hashset_ToCsharp()
        {
            var two = new HashSet<string>() {"cat", "dog"};
            two.add("horse").ShouldBe(true);
            two.add("cat").ShouldBe(false);
        }

        // ref: https://docs.oracle.com/javase/7/docs/api/java/util/Stack.html
        [Test]
        public void Stack_ToCsharp()
        {
            var two = new Stack<string>();
            two.push("cat").ShouldBe("cat");
            two.push("dog").ShouldBe("dog");
            two.isEmpty().ShouldBe(false);
            two.pop().ShouldBe("dog"); // LIFO
            two.pop().ShouldBe("cat");
            two.isEmpty().ShouldBe(true);
         }


        // Dictionary ones

        // ref: https://docs.oracle.com/javase/7/docs/api/java/util/HashMap.html
        [Test]
        public void HashMap_ToCsharp_Dictionary()
        {
            var two = new Dictionary<string,string>();
            two.put("cat","lucy").ShouldBe("lucy");
            two.put("dog", "benny").ShouldBe("benny");

            two.Keys.Count.ShouldBe(2);
            two["cat"].ShouldBe("lucy");

            two.put("cat", "emi").ShouldBe("emi");
            two["cat"].ShouldBe("emi");

            two.containsKey("dog").ShouldBe(true);
            two.containsKey("camel").ShouldBe(false);

        }

    }
}