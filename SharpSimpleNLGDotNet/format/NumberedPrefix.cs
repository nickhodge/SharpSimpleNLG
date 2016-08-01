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

using SimpleNLG.Extensions;

namespace SimpleNLG
{
    /**
     * This class keeps track of the prefix for numbered lists.
     */

    public class NumberedPrefix
    {
        string prefix;

        public NumberedPrefix()
        {
            prefix = "0";
        }

        public void increment()
        {
            var dotPosition = prefix.LastIndexOf('.');
            if (dotPosition == -1)
            {
                var counter = int.Parse(prefix);
                counter++;
                prefix = counter.ToString();

            }
            else
            {
                var subCounterStr = prefix.Substring(dotPosition + 1);
                var subCounter = int.Parse(subCounterStr);
                subCounter++;
                prefix = prefix.substring(0, dotPosition) + "." + subCounter;
            }
        }

        /**
         * This method starts a new level to the prefix (e.g., 1.1 if the current is 1, 2.3.1 if current is 2.3, or 1 if the current is 0).
         */

        public void upALevel()
        {
            if (prefix.Equals("0"))
            {
                prefix = "1";
            }
            else
            {
                prefix = prefix + ".1";
            }
        }

        /**
         * This method removes a level from the prefix (e.g., 0 if current is a plain number, say, 7, or 2.4, if current is 2.4.1).
         */

        public void downALevel()
        {
            var dotPosition = prefix.LastIndexOf('.');
            if (dotPosition == -1)
            {
                prefix = "0";
            }
            else
            {
                prefix = prefix.substring(0, dotPosition);
            }
        }

        public string getPrefix()
        {
            return prefix;
        }

        public void setPrefix(string prefix)
        {
            this.prefix = prefix;
        }
    }
}