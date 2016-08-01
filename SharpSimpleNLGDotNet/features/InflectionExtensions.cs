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

using System;

namespace SimpleNLG.Extensions
{
    /**
     * convenience method: parses an inflectional code such as
     * "irreg|woman|women" to retrieve the first element, which is the code
     * itself, then maps it to the value of <code>Inflection</code>.
     * 
     * @param code
     *            -- the string representing the inflection. The strings are
     *            those defined in the NIH Lexicon.
     * @return the Inflection
     */

    public static class InflectionExtensions
    {
        public static Tuple<bool,Inflection> getInflCode(string code)
        {
            var found = false;
            var infl = Inflection.REGULAR;

            code = code.ToLower().Trim();

            if (code.Equals("reg"))
            {
                infl = Inflection.REGULAR;
                found = true;
            }
            else if (code.Equals("irreg"))
            {
                infl = Inflection.IRREGULAR;
                found = true;
            }
            else if (code.Equals("regd"))
            {
                infl = Inflection.REGULAR_DOUBLE;
                found = true;
            }
            else if (code.Equals("glreg"))
            {
                infl = Inflection.GRECO_LATIN_REGULAR;
                found = true;
            }
            else if (code.Equals("uncount") || code.Equals("noncount")
                     || code.Equals("groupuncount"))
            {
                infl = Inflection.UNCOUNT;
                found = true;
            }
            else if (code.Equals("inv"))
            {
                infl = Inflection.INVARIANT;
                found = true;
            }
            return  new Tuple<bool, Inflection>(found, infl);
        }
    }
}