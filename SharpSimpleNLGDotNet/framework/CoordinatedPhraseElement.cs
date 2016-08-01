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
    public class CoordinatedPhraseElement : NLGElement, INLGElement
    {

        /** Coordinators which make the coordinate plural (eg, "and" but not "or")*/
        private static List<string> PLURAL_COORDINATORS = new List<string> {"and"};

        /**
         * Creates a blank coordinated phrase ready for new coordinates to be added.
         * The default conjunction used is <em>and</em>.
         */

        public CoordinatedPhraseElement() : base()
        {
            this.setFeature(Feature.CONJUNCTION.ToString(), "and"); 
        }

        /**
         * Creates a coordinated phrase linking the two phrase together. The default
         * conjunction used is <em>and</em>.
         * 
         * @param coordinate1
         *            the first coordinate.
         * @param coordinate2
         *            the second coordinate.
         */

        public CoordinatedPhraseElement(object coordinate1, object coordinate2)
        {

            this.addCoordinate(coordinate1);
            this.addCoordinate(coordinate2);
            this.setFeature(Feature.CONJUNCTION.ToString(), "and"); 
        }

        /**
         * Adds a new coordinate to this coordination. If the new coordinate is a
         * <code>NLGElement</code> then it is added directly to the coordination. If
         * the new coordinate is a <code>string</code> a <code>StringElement</code>
         * is created and added to the coordination. <code>StringElement</code>s
         * will have their complementisers suppressed by default. In the case of
         * clauses, complementisers will be suppressed if the clause is not the
         * first element in the coordination.
         * 
         * @param newCoordinate
         *            the new coordinate to be added.
         */

        public void addCoordinate(object newCoordinate)
        {
            List<INLGElement> coordinates = getFeatureAsElementList(InternalFeature.COORDINATES.ToString());
            if (coordinates == null)
            {
                coordinates = new List<INLGElement>();
                setFeature(InternalFeature.COORDINATES.ToString(), coordinates);
            }
            else if (coordinates.Count == 0)
            {
                setFeature(InternalFeature.COORDINATES.ToString(), coordinates);
            }
            if (newCoordinate is INLGElement)
            {
                if (((INLGElement) newCoordinate).isA(PhraseCategoryEnum.CLAUSE)
                    && coordinates.Count > 0)
                {

                    ((INLGElement) newCoordinate).setFeature(
                        Feature.SUPRESSED_COMPLEMENTISER.ToString(), true);
                }
                coordinates.Add((INLGElement) newCoordinate);
            }
            else if (newCoordinate is string)
            {
                INLGElement coordElement = new StringElement((string) newCoordinate);
                coordElement.setFeature(Feature.SUPRESSED_COMPLEMENTISER.ToString(), true);
                coordinates.Add(coordElement);
            }
            setFeature(InternalFeature.COORDINATES.ToString(), coordinates);
        }


        public override List<INLGElement> getChildren()
        {
            return this.getFeatureAsElementList(InternalFeature.COORDINATES.ToString());
        }

        /**
         * Clears the existing coordinates in this coordination. It performs exactly
         * the same as <code>removeFeature(Feature.COORDINATES)</code>.
         */

        public void clearCoordinates()
        {
            removeFeature(InternalFeature.COORDINATES.ToString());
        }

        /**
         * Adds a new pre-modifier to the phrase element. Pre-modifiers will be
         * realised in the syntax before the coordinates.
         * 
         * @param newPreModifier
         *            the new pre-modifier as an <code>NLGElement</code>.
         */

        public void addPreModifier(INLGElement newPreModifier)
        {
            List<INLGElement> preModifiers = getFeatureAsElementList(InternalFeature.PREMODIFIERS.ToString());
            if (preModifiers == null)
            {
                preModifiers = new List<INLGElement>();
            }
            preModifiers.Add(newPreModifier);
            setFeature(InternalFeature.PREMODIFIERS.ToString(), preModifiers);
        }

        /**
         * Adds a new pre-modifier to the phrase element. Pre-modifiers will be
         * realised in the syntax before the coordinates.
         * 
         * @param newPreModifier
         *            the new pre-modifier as a <code>string</code>. It is used to
         *            create a <code>StringElement</code>.
         */

        public void addPreModifier(string newPreModifier)
        {
            List<INLGElement> preModifiers = getFeatureAsElementList(InternalFeature.PREMODIFIERS.ToString());
            if (preModifiers == null)
            {
                preModifiers = new List<INLGElement>();
            }
            preModifiers.Add(new StringElement(newPreModifier));
            setFeature(InternalFeature.PREMODIFIERS.ToString(), preModifiers);
        }

        /**
         * Retrieves the list of pre-modifiers currently associated with this
         * coordination.
         * 
         * @return a <code>List</code> of <code>NLGElement</code>s.
         */

        public List<INLGElement> getPreModifiers()
        {
            return getFeatureAsElementList(InternalFeature.PREMODIFIERS.ToString());
        }

        /**
         * Retrieves the list of complements currently associated with this
         * coordination.
         * 
         * @return a <code>List</code> of <code>NLGElement</code>s.
         */

        public List<INLGElement> getComplements()
        {
            return getFeatureAsElementList(InternalFeature.COMPLEMENTS.ToString());
        }

        /**
         * Adds a new post-modifier to the phrase element. Post-modifiers will be
         * realised in the syntax after the coordinates.
         * 
         * @param newPostModifier
         *            the new post-modifier as an <code>NLGElement</code>.
         */

        public void addPostModifier(INLGElement newPostModifier)
        {
            List<INLGElement> postModifiers = getFeatureAsElementList(InternalFeature.POSTMODIFIERS.ToString());
            if (postModifiers == null)
            {
                postModifiers = new List<INLGElement>();
            }
            postModifiers.Add(newPostModifier);
            setFeature(InternalFeature.POSTMODIFIERS.ToString(), postModifiers);
        }

        /**
         * Adds a new post-modifier to the phrase element. Post-modifiers will be
         * realised in the syntax after the coordinates.
         * 
         * @param newPostModifier
         *            the new post-modifier as a <code>string</code>. It is used to
         *            create a <code>StringElement</code>.
         */

        public void addPostModifier(string newPostModifier)
        {
            List<INLGElement> postModifiers = getFeatureAsElementList(InternalFeature.POSTMODIFIERS.ToString());
            if (postModifiers == null)
            {
                postModifiers = new List<INLGElement>();
            }
            postModifiers.Add(new StringElement(newPostModifier));
            setFeature(InternalFeature.POSTMODIFIERS.ToString(), postModifiers);
        }

        /**
         * Retrieves the list of post-modifiers currently associated with this
         * coordination.
         * 
         * @return a <code>List</code> of <code>NLGElement</code>s.
         */

        public List<INLGElement> getPostModifiers()
        {
            return getFeatureAsElementList(InternalFeature.POSTMODIFIERS.ToString());
        }

        public override string printTree(string indent)
        {
            var thisIndent = indent == null ? " |-" : indent + " |-";  
            var childIndent = indent == null ? " | " : indent + " | ";  
            var lastIndent = indent == null ? " \\-" : indent + " \\-";  
            var lastChildIndent = indent == null ? "   " : indent + "   ";  
            var print = new StringBuilder();
            print.Append("CoordinatedPhraseElement:\n"); 

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
         * Adds a new complement to the phrase element. Complements will be realised
         * in the syntax after the coordinates. Complements differ from
         * post-modifiers in that complements are crucial to the understanding of a
         * phrase whereas post-modifiers are optional.
         * 
         * @param newComplement
         *            the new complement as an <code>NLGElement</code>.
         */

        public void addComplement(INLGElement newComplement)
        {
            List<INLGElement> complements = getFeatureAsElementList(InternalFeature.COMPLEMENTS.ToString());
            if (complements == null)
            {
                complements = new List<INLGElement>();
            }
            complements.Add(newComplement);
            setFeature(InternalFeature.COMPLEMENTS.ToString(), complements);
        }

        /**
         * Adds a new complement to the phrase element. Complements will be realised
         * in the syntax after the coordinates. Complements differ from
         * post-modifiers in that complements are crucial to the understanding of a
         * phrase whereas post-modifiers are optional.
         * 
         * @param newComplement
         *            the new complement as a <code>string</code>. It is used to
         *            create a <code>StringElement</code>.
         */

        public void addComplement(string newComplement)
        {
            List<INLGElement> complements = getFeatureAsElementList(InternalFeature.COMPLEMENTS.ToString());
            if (complements == null)
            {
                complements = new List<INLGElement>();
            }
            complements.Add(new StringElement(newComplement));
            setFeature(InternalFeature.COMPLEMENTS.ToString(), complements);
        }

        /**
         * A convenience method for retrieving the last coordinate in this
         * coordination.
         * 
         * @return the last coordinate as represented by a <code>NLGElement</code>
         */

        public INLGElement getLastCoordinate()
        {
            List<INLGElement> children = getChildren();
            return children != null && children.Count > 0
                ? children[children.Count - 1]
                : null;
        }

        /** set the conjunction to be used in a coordinatedphraseelement
         * @param conjunction
         */

        public void setConjunction(string conjunction)
        {
            setFeature(Feature.CONJUNCTION.ToString(), conjunction);
        }

        /**
         * @return  conjunction used in coordinatedPhraseElement
         */

        public string getConjunction()
        {
            return getFeatureAsString(Feature.CONJUNCTION.ToString());
        }

        /**
         * @return true if this coordinate is plural in a syntactic sense
         */

        public bool checkIfPlural()
        {
            // doing this right is quite complex, take simple approach for now
            var size = getChildren().Count;
            if (size == 1)
                return (NumberAgreement.PLURAL.Equals(getLastCoordinate().getFeature(Feature.NUMBER.ToString())));
            else
                return PLURAL_COORDINATORS.Contains(getConjunction());
        }
    }
}