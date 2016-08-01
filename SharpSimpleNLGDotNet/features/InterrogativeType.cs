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
    public enum InterrogativeType
    {

        /**
         * The type of interrogative relating to the manner in which an event
         * happened. For example, <em>John kissed Mary</em> becomes
         * <em>How did John kiss
         * Mary?</em>
         */
        HOW,

        /**
         * A how question related to a predicative sentence, such as <i>John is fine</i>, which becomes <i>How is John?</i> 
         */
        HOW_PREDICATE,

        /**
         * This type of interrogative is a question pertaining to the object of a
         * phrase. For example, <em>John bought a horse</em> becomes <em>what did 
         * John buy?</em> while <em>John gave Mary a flower</em> becomes
         * <em>What did 
         * John give Mary?</em>
         */
        WHAT_OBJECT,

        /**
         * This type of interrogative is a question pertaining to the subject of a
         * phrase. For example, <em>A hurricane destroyed the house</em> becomes
         * <em>what destroyed the house?</em>
         */
        WHAT_SUBJECT,

        /**
         * This type of interrogative concerns the object of a verb that is to do
         * with location. For example, <em>John went to the beach</em> becomes
         * <em>Where did John go?</em>
         */
        WHERE,

        /**
         * This type of interrogative is a question pertaining to the indirect
         * object of a phrase when the indirect object is a person. For example,
         * <em>John gave Mary a flower</em> becomes
         * <em>Who did John give a flower to?</em>
         */
        WHO_INDIRECT_OBJECT,

        /**
         * This type of interrogative is a question pertaining to the object of a
         * phrase when the object is a person. For example,
         * <em>John kissed Mary</em> becomes <em>who did John kiss?</em>
         */
        WHO_OBJECT,

        /**
         * This type of interrogative is a question pertaining to the subject of a
         * phrase when the subject is a person. For example,
         * <em>John kissed Mary</em> becomes <em>Who kissed Mary?</em> while
         * <em>John gave Mary a flower</em> becomes <em>Who gave Mary a flower?</em>
         */
        WHO_SUBJECT,

        /**
         * The type of interrogative relating to the reason for an event happening.
         * For example, <em>John kissed Mary</em> becomes <em>Why did John kiss
         * Mary?</em>
         */
        WHY,

        /**
         * This represents a simple yes/no questions. So taking the example phrases
         * of <em>John is a professor</em> and <em>John kissed Mary</em> we can
         * construct the questions <em>Is John a professor?</em> and
         * <em>Did John kiss Mary?</em>
         */
        YES_NO,

        /**
         * This represents a "how many" questions. For example of
         * <em>dogs chased John/em> becomes <em>How many dogs chased John</em>
         */
        HOW_MANY
    }

}