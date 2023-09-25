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


namespace SimpleNLG.Extensions
{
    /**
     * A method to determine if the {@code InterrogativeType} is a question
     * concerning an element with the discourse function of an object.
     * 
     * @param type
     *            the interrogative type to be checked
     * @return <code>true</code> if the type concerns an object,
     *         <code>false</code> otherwise.
     */

    internal static class InterrogativeTypeExtensions
    {
        /**
         * A method to determine if the {@code InterrogativeType} is a question
         * concerning an element with the discourse function of an indirect object.
         * 
         * @param type
         *            the interrogative type to be checked
         * @return <code>true</code> if the type concerns an indirect object,
         *         <code>false</code> otherwise.
         */

        public static bool isIndirectObject(object type)
        {
            return InterrogativeType.WHO_INDIRECT_OBJECT.Equals(type);
        }

        /**
         * Convenience method to return the string corresponding to the question
         * word. Useful, since the types in this enum aren't all simply convertible
         * to strings (e.g. <code>WHO_SUBJCT</code> and <code>WHO_OBJECT</code> both
         * correspond to string <i>Who</i>)
         * 
         * @return the string corresponding to the question word
         */

        public static string getString(this InterrogativeType i)
        {
            var s = "";

            switch (i)
            {
                case InterrogativeType.HOW:
                case InterrogativeType.HOW_PREDICATE:
                    s = "how";
                    break;
                case InterrogativeType.WHAT_OBJECT:
                case InterrogativeType.WHAT_SUBJECT:
                    s = "what";
                    break;
                case InterrogativeType.WHERE:
                    s = "where";
                    break;
                case InterrogativeType.WHO_INDIRECT_OBJECT:
                case InterrogativeType.WHO_OBJECT:
                case InterrogativeType.WHO_SUBJECT:
                    s = "who";
                    break;
                case InterrogativeType.WHY:
                    s = "why";
                    break;
                case InterrogativeType.HOW_MANY:
                    s = "how many";
                    break;
                case InterrogativeType.YES_NO:
                    s = "yes/no";
                    break;
            }

            return s;
        }

        public static bool isAndObject(this object type)
        {
            return InterrogativeType.WHO_OBJECT.Equals(type) || InterrogativeType.WHAT_OBJECT.Equals(type);
        }

    }
}