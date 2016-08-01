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
      public static class InternalFeatureExtensions
     {
         public static string ToString(this InternalFeature intf)
         {
             switch (intf)
             {
                 case InternalFeature.ACRONYM:
                     return "acronym";
                 case InternalFeature.BASE_WORD:
                     return "base_word";
                 case InternalFeature.CLAUSE_STATUS:
                     return "clause_status";
                 case InternalFeature.COMPLEMENTS:
                     return "complements";
                 case InternalFeature.COMPONENTS:
                     return "components";
                 case InternalFeature.COORDINATES:
                     return "coordinates";
                 case InternalFeature.DISCOURSE_FUNCTION:
                     return "discourse_function";
                 case InternalFeature.NON_MORPH:
                     return "non_morph";
                 case InternalFeature.FRONT_MODIFIERS:
                     return "front_modifiers";
                 case InternalFeature.HEAD:
                     return "head";
                 case InternalFeature.IGNORE_MODAL:
                     return "ignore_modal";
                 case InternalFeature.INTERROGATIVE:
                     return "interrogative";
                 case InternalFeature.POSTMODIFIERS:
                     return "postmodifiers";
                 case InternalFeature.PREMODIFIERS:
                     return "premodifiers";
                 case InternalFeature.RAISED:
                     return "raised";
                 case InternalFeature.REALISE_AUXILIARY:
                     return "realise_auxiliary";
                 case InternalFeature.SPECIFIER:
                     return "specifier";
                 case InternalFeature.SUBJECTS:
                     return "subjects";
                 case InternalFeature.VERB_PHRASE:
                     return "verb_phrase";
                 default:
                     return "";
             }
         }
     }
}