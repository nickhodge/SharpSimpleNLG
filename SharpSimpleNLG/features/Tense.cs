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


namespace SimpleNLG
{
    public enum Tense
    {

        /**
         * The action described by the verb will happen in the future. For example,
         * <em>John will kiss Mary</em>, <em>the dog will eat a bone</em>.
         */
        FUTURE,

        /**
         * The action described by the verb happened in the past. For example,
         * <em>John kissed Mary</em>, <em>the dog ate a bone</em>.
         */
        PAST,

        /**
         * The action described by the verb is happening in the present time. For
         * example, <em>John kisses Mary</em>, <em>the dog eats a bone</em>.
         */
        PRESENT
    }

    public static class TenseExtensions
    {
        public static Tense ToTense(this string s)
        {
            if (s.ToUpperInvariant().Contains("PAST"))
                return Tense.PAST;
            if (s.ToUpperInvariant().Contains("FUTURE"))
                return Tense.FUTURE;
            return Tense.PRESENT;
        }
    }
}
