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
using System.Linq;

namespace SimpleNLG.Extensions
{
    public static class EqualsExtensions
    {
        public static bool equals(this Dictionary<string, object> s, Dictionary<string, object> d)
        {
            var sk = s.Keys.ToList();
            var dk = s.Keys.ToList();
            if (sk.Count != dk.Count) return false; // keys length doesn't match, quick exit
            if (sk.Except(dk).Any()) return false; // key list doesn't match, exit
            foreach (var ski in sk) // now we have matching keys, check each of the values
            {
                if (!d.ContainsKey(ski)) return false;
                object sob = s[ski];
                object dob = d[ski];
                if (sob.GetType() != dob.GetType()) return false;
                if (sob.ToString() != dob.ToString()) return false; // poor person's check of contents of values
            }

            return true;
        }

        public static bool equals(this IEnumerable<INLGElement> s, IEnumerable<INLGElement> o)
        {
            var sl = s.ToList(); // ToList to get index
            var ol = o.ToList();
            var sc = sl.Count - 1; // remove 1 for the for below
            var oc = ol.Count - 1;
            if (sc != oc) return false; // different features, must be different

            for (int si = 0; si <= sc; si++)
            {
                if (sl[si].getCategory().enumType != ol[si].getCategory().enumType) return false;
                if (!sl[si].getAllFeatures().equals(ol[si].getAllFeatures())) return false;
            }

            return true;
        }

        public static bool equals(this INLGElement s, object o)
        {
            var eq = false;

            var nlgElement = o as INLGElement;
            if (nlgElement != null)
            {
                eq = s.category.enumType == nlgElement.category.enumType && s.features.equals(nlgElement.features);
            }

            return eq;
        }


        private static bool realisationsMatch(StringElement s, StringElement d)
        {
            if (s.getRealisation() == null)
            {
                return d.getRealisation() == null;
            }
            else
                return s.getRealisation().Equals(d.getRealisation());
        }


        public static bool equals(this StringElement s, object d)
        {
            return s.Equals(d) && (d is StringElement) && realisationsMatch((StringElement)s, (StringElement)d);
        }


        public static bool equals(this WordElement o, WordElement d)
        {
            if (o != null)
            {
                var we = o;

                return (o.baseForm == d.baseForm || o.baseForm
                            .Equals(d.baseForm))
                       && (o.id == d.id || o.id.Equals(d.id))
                       && d.features.equals(o.features);
            }

            return false;
        }

    }
}