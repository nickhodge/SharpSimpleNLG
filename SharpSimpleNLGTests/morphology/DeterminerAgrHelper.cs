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

using System.Text;
using SimpleNLG.Extensions;

namespace SharpNLG.Extensions
{
/**
     * This class is used to parse numbers that are passed as figures, to determine
     * whether they should take "a" or "an" as determiner.
     * 
     * @author bertugatt
     * 
     */

    public class DeterminerAgrHelper
    {
        /*
         * An array of strings which are exceptions to the rule that "an" comes
         * before vowels
         */
        private static string[] AN_EXCEPTIONS = {"one", "180", "110"};

        /*
         * Start of string involving vowels, for use of "an"
         */
        private static string AN_AGREEMENT = @"\A(a|e|i|o|u).*";

        /*
         * Start of string involving numbers, for use of "an" -- courtesy of Chris
         * Howell, Agfa healthcare corporation
         */
        // private static final string AN_NUMERAL_AGREEMENT =
        // "^(((8((\\d+)|(\\d+(\\.|,)\\d+))?).*)|((11|18)(\\d{3,}|\\D)).*)$";

        /**
         * Check whether this string starts with a number that needs "an" (e.g.
         * "an 18% increase")
         * 
         * @param string
         *            the string
         * @return <code>true</code> if this string starts with 11, 18, or 8,
         *         excluding strings that start with 180 or 110
         */

        public static bool requiresAn(string stringa)
        {
            var req = false;

            var lowercaseInput = stringa.toLowerCase();

            if (lowercaseInput.matches(AN_AGREEMENT) && !isAnException(lowercaseInput))
            {
                req = true;

            }
            else
            {
                var numPref = getNumericPrefix(lowercaseInput);

                if (numPref != null && numPref.length() > 0
                    && numPref.matches(@"^(8|11|18).*$"))
                {
                    var num = int.Parse(numPref);
                    req = checkNum(num);
                }
            }

            return req;
        }

        /*
         * check whether a string beginning with a vowel is an exception and doesn't
         * take "an" (e.g. "a one percent change")
         * 
         * @return
         */

        private static bool isAnException(string stringa)
        {
            foreach (var ex in AN_EXCEPTIONS)
            {
                if (stringa.matches("^" + ex + ".*"))
                {
                    // if (string.equalsIgnoreCase(ex)) {
                    return true;
                }
            }

            return false;
        }

        /*
         * Returns <code>true</code> if the number starts with 8, 11 or 18 and is
         * either less than 100 or greater than 1000, but excluding 180,000 etc.
         */

        private static bool checkNum(int num)
        {
            var needsAn = false;

            // eight, eleven, eighty and eighteen
            if (num == 11 || num == 18 || num == 8 || (num >= 80 && num < 90))
            {
                needsAn = true;

            }
            else if (num > 1000)
            {
                // num = Math.Round(num / 1000);
                num = num/1000;
                needsAn = checkNum(num);
            }

            return needsAn;
        }

        /*
         * Retrieve the numeral prefix of a string.
         */

        private static string getNumericPrefix(string stringa)
        {
            var numeric = new StringBuilder();

            if (stringa != null)
            {
                stringa = stringa.Trim();

                if (stringa.length() > 0)
                {

                    var buffer = new StringBuilder(stringa);
                    var first = buffer.charAt(0);

                    if (first.isDigit())
                    {
                        numeric.append(first);

                        for (var i = 1; i < buffer.length(); i++)
                        {
                            var next = buffer.charAt(i);

                            if (next.isDigit())
                            {
                                numeric.append(next);

                                // skip commas within numbers
                            }
                            else if (next.Equals(','))
                            {
                                continue;

                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
            }

            return numeric.length() == 0 ? null : numeric.ToString();
        }


        /**
         * Check to see if a string ends with the indefinite article "a" and it agrees with {@code np}. 
         * @param text
         * @param np
         * @return an altered version of {@code text} to use "an" if it agrees with {@code np}, the original string otherwise.
         */

        public static string checkEndsWithIndefiniteArticle(string text, string np)
        {

            var tokens = text.Split(' ');

            var lastToken = tokens[tokens.Length - 1];

            if (lastToken.equalsIgnoreCase("a") && DeterminerAgrHelper.requiresAn(np))
            {

                tokens[tokens.Length - 1] = "an";

                return stringArrayToString(tokens);

            }

            return text;

        }

        // Turns ["a","b","c"] into "a b c"
        private static string stringArrayToString(string[] sArray)
        {

            var buf = new StringBuilder();

            for (var i = 0; i < sArray.Length; i++)
            {

                buf.Append(sArray[i]);

                if (i != sArray.Length - 1)
                {

                    buf.Append(" ");

                }

            }

            return buf.ToString();

        }

    }
}