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
using System.Text;

namespace SimpleNLG
{
    public class FunctionalSet
    {
        public List<INLGElement> components { get; set; }
        public DiscourseFunction function { get; set; }
        public IElementCategory category { get; set; }
        public Periphery periphery { get; set; }

        public static FunctionalSet newInstance(DiscourseFunction func,
            IElementCategory category, 
            Periphery periphery,
            List<INLGElement> components)
        {

            FunctionalSet pair = null;

            if (components.Count >= 2)
            {
                pair = new FunctionalSet(func, category, periphery, components);
            }

            return pair;

        }

        public FunctionalSet(DiscourseFunction _func, IElementCategory _category,
            Periphery _periphery, List<INLGElement> _components)
        {
            function = _func;
            category = _category;
            periphery = _periphery;
            components = _components;
        }

        public bool formIdentical()
        {
            var ident = true;
            var firstElement = components[0];

            for (var i = 1; i < components.Count && ident; i++)
            {
                ident = firstElement.Equals(components[i]);
            }

            return ident;
        }

        public bool lemmaIdentical()
        {
            return false;
        }

        public void elideLeftMost()
        {
            for (var i = 0; i < components.Count - 1; i++)
            {
                recursiveElide(components[i]);
            }
        }

        public void elideRightMost()
        {
            for (var i = components.Count - 1; i > 0; i--)
            {
                recursiveElide(components[i]);

            }
        }

        private void recursiveElide(INLGElement component)
        {
            if (component is ListElement)
            {
                foreach (var subcomponent in component.getFeatureAsElementList(InternalFeature.COMPONENTS.ToString()))
                {
                    recursiveElide(subcomponent);
                }
            }
            else
            {
                component.setFeature(Feature.ELIDED.ToString(), true);
            }
        }

        public DiscourseFunction getFunction()
        {
            return function;
        }

        public IElementCategory getCategory()
        {
            return category;
        }

        public Periphery getPeriphery()
        {
            return periphery;
        }

        public List<INLGElement> getComponents()
        {
            return components;
        }

        public override string ToString()
        {
            var buffer = new StringBuilder();

            foreach (var elem in components)
            {
                buffer.Append("ELEMENT: ").Append(elem).Append("\n");
            }

            return buffer.ToString();
        }

    }
}
