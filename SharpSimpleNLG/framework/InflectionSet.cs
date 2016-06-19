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

namespace SimpleNLG
{
    public class InflectionSet
    {
        // the infl type
        Inflection infl;

        // the forms, mapping values of LexicalFeature to actual word forms
        private Dictionary<string, string> forms = new Dictionary<string, string>();

        public InflectionSet(Inflection infl)
        {
            this.infl = infl;
        }

        /*
         * set an inflectional form
         * 
         * @param feature
         * 
         * @param form
         */

        public void addForm(string feature, string form)
        {
            if (forms.ContainsKey(feature))
                forms[feature] = form;
            else forms.Add(feature, form);
        }

        /*
         * get an inflectional form
         */

        public string getForm(string feature)
        {
            return forms.ContainsKey(feature) ? forms[feature] : null;
        }
    }


}
