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
    public class ListElement : NLGElement
    {

        /**
         * Creates a new list element containing the given components.
         * 
         * @param components
         *            the initial components for this list element.
         */

        public ListElement()
        {
            
        }

        public ListElement(List<INLGElement> components)
        {
            addComponents(components);
        }

        public override List<INLGElement> getChildren()
        {
            return getFeatureAsElementList(InternalFeature.COMPONENTS.ToString());
        }

        /**
         * Creates a new list element containing the given component.
         * 
         * @param newComponent
         *            the initial component for this list element.
         */

        public ListElement(INLGElement newComponent)
        {
            addComponent(newComponent);
        }

        /**
         * Adds the given component to the list element.
         * 
         * @param newComponent
         *            the <code>NLGElement</code> component to be added.
         */

        public void addComponent(INLGElement newComponent)
        {
            List<INLGElement> components = getFeatureAsElementList(InternalFeature.COMPONENTS.ToString());
            if (components == null)
            {
                components = new List<INLGElement>();
            }
            setFeature(InternalFeature.COMPONENTS.ToString(), components);
            components.Add(newComponent);
        }

        /**
         * Adds the given components to the list element.
         * 
         * @param newComponents
         *            a <code>List</code> of <code>NLGElement</code>s to be added.
         */

        public void addComponents(List<INLGElement> newComponents)
        {
            var components = getFeatureAsElementList(InternalFeature.COMPONENTS.ToString());
            if (components == null)
            {
                components = new List<INLGElement>();
            }
            setFeature(InternalFeature.COMPONENTS.ToString(), components);
            components.AddRange(newComponents);
        }

        /**
         * Replaces the current components in the list element with the given list.
         * 
         * @param newComponents
         *            a <code>List</code> of <code>NLGElement</code>s to be used as
         *            the components.
         */

        public void setComponents(List<INLGElement> newComponents)
        {
            setFeature(InternalFeature.COMPONENTS.ToString(), newComponents);
        }

        public override string ToString()
        {
            return getChildren().ToString();
        }

        public override string printTree(string indent)
        {
            var thisIndent = indent == null ? " |-" : indent + " |-";  
            var childIndent = indent == null ? " | " : indent + " | ";  
            var lastIndent = indent == null ? " \\-" : indent + " \\-";  
            var lastChildIndent = indent == null ? "   " : indent + "   ";  
            var print = new StringBuilder();
            print.Append("ListElement: features={"); 

            var features = getAllFeatures();
            foreach (var eachFeature in features.Keys)
            {
                print.Append(eachFeature).Append('=').Append(
                    features[eachFeature]).Append(' ');
            }
            print.Append("}\n"); 

            List<INLGElement> children = getChildren();
            var length = children.Count - 1;
            var index = 0;

            for (index = 0; index < length; index++)
            {
                print.Append(thisIndent).Append(
                    children[index].printTree(childIndent));
            }
            if (length >= 0)
            {
                print.Append(lastIndent).Append(
                    children[length].printTree(lastChildIndent));
            }
            return print.ToString();
        }

        /**
         * Retrieves the number of components in this list element.
         * @return the number of components.
         */

        public int size()
        {
            return getChildren().Count;
        }

        /**
         * Retrieves the first component in the list.
         * @return the <code>NLGElement</code> at the top of the list.
         */

        public INLGElement getFirst()
        {
            var children = getChildren();
            return children?[0];
        }
    }
}