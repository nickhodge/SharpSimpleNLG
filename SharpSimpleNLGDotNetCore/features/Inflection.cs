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

namespace SimpleNLG
{
    public enum Inflection
    {

        /**
         * The morphology processor has simple rules for pluralising Greek and Latin
         * nouns. The full list can be found in the explanation of the morphology
         * processor. An example would be turning <em>focus</em> into <em>foci</em>.
         * The Greco-Latin rules are generally more complex than the basic rules.
         */
        GRECO_LATIN_REGULAR,

        /**
         * A word having an irregular pattern essentially means that none of the
         * supplied rules can be used to correctly inflect the word. The inflection
         * should be defined by the user or appear in the lexicon. <em>sheep</em> is
         * an example of an irregular noun.
         */
        IRREGULAR,

        /**
         * Regular patterns represent the default rules when dealing with
         * inflections. A full list can be found in the explanation of the
         * morphology processor. An example would be adding <em>-s</em> to the end
         * of regular nouns to pluralise them.
         */
        REGULAR,

        /**
         * Regular double patterns apply to verbs where the last consonant is
         * duplicated before applying the new suffix. For example, the verb
         * <em>tag</em> has a regular double pattern as its inflected forms include
         * <em>tagged</em> and <em>tagging</em>.
         */
        REGULAR_DOUBLE,

        /**
         * The value for uncountable nouns, which are not inflected in their plural
         * form.
         */
        UNCOUNT,

        /**
         * The value for words which are invariant, that is, are never inflected.
         */
        INVARIANT
    }
}