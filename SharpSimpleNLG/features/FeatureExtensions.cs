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

    internal static class FeatureExtensions
    {
        public static string ToString(this Feature f)
        {
            switch (f)
            {
                case Feature.ADJECTIVE_ORDERING:
                    return "adjective_ordering".ToUpperInvariant();
                case Feature.AGGREGATE_AUXILIARY:
                    return "aggregate_auxiliary".ToUpperInvariant();

                case Feature.COMPLEMENTISER:
                    return "complementiser".ToUpperInvariant();

                case Feature.CONJUNCTION:
                    return "conjunction".ToUpperInvariant();

                case Feature.CONJUNCTION_TYPE:
                    return "conjunction_type".ToUpperInvariant();

                case Feature.APPOSITIVE:
                    return "appositive".ToUpperInvariant();

                case Feature.CUE_PHRASE:
                    return "cue_phrase".ToUpperInvariant();

                case Feature.ELIDED:
                    return "elided".ToUpperInvariant();

                case Feature.FORM:
                    return "form".ToUpperInvariant();

                case Feature.INTERROGATIVE_TYPE:
                    return "interrogative_type".ToUpperInvariant();

                case Feature.IS_COMPARATIVE:
                    return "is_comparative".ToUpperInvariant();

                case Feature.IS_SUPERLATIVE:
                    return "is_superlative".ToUpperInvariant();

                case Feature.MODAL:
                    return "modal".ToUpperInvariant();

                case Feature.NEGATED:
                    return "negated".ToUpperInvariant();

                case Feature.NUMBER:
                    return "number".ToUpperInvariant();

                case Feature.PARTICLE:
                    return "particle".ToUpperInvariant();

                case Feature.PASSIVE:
                    return "passive".ToUpperInvariant();

                case Feature.PERFECT:
                    return "perfect".ToUpperInvariant();

                case Feature.PERSON:
                    return "person".ToUpperInvariant();

                case Feature.POSSESSIVE:
                    return "possessive".ToUpperInvariant();

                case Feature.PRONOMINAL:
                    return "pronominal".ToUpperInvariant();

                case Feature.PROGRESSIVE:
                    return "progressive".ToUpperInvariant();

                case Feature.RAISE_SPECIFIER:
                    return "raise_specifier".ToUpperInvariant();

                case Feature.SUPPRESS_GENITIVE_IN_GERUND:
                    return "suppress_genitive_in_gerund".ToUpperInvariant();

                case Feature.SUPRESSED_COMPLEMENTISER:
                    return "suppressed_complementiser".ToUpperInvariant();

                case Feature.TENSE:
                    return "tense".ToUpperInvariant();

                case Feature.REALISE_AUXILIARY:
                    return "InternalFeature Realise Auxiliary".ToUpperInvariant();

                default:
                    return "";
            }
        }
    }
}