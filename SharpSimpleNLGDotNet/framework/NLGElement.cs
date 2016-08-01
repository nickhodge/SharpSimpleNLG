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
using System.Text;
using SimpleNLG.Extensions;

namespace SimpleNLG
{
    public interface INLGElement
    {
        IElementCategory category { get; set; }
        Dictionary<string, object> features { get; }

        string printTree(string indent);
        void setCategory(IElementCategory newCategory);
        IElementCategory getCategory();
        void setFeature(string featureName, object featureValue);
        void setFeature(string featureName, bool featureValue);
        void setFeature(string featureName, long featureValue);
        void setFeature(string featureName, float featureValue);
        void setFeature(string featureName, double featureValue);
        object getFeature(string featureName);
        Tense getFeatureTense(string featureName);
        string getFeatureAsString(string featureName);
        List<INLGElement> getFeatureAsElementList(string featureName);
        bool getFeatureAsBoolean(string featureName);
        INLGElement getFeatureAsElement(string featureName);
        void setFactory(NLGFactory factory);
        NLGFactory getFactory();
        bool isPlural();
        void setPlural(bool isPlural);
        List<string> getAllFeatureNames();
        List<INLGElement> getChildren();
        void removeFeature(string featureName);
        bool hasFeature(string featureName);
        Dictionary<string, object> getAllFeatures();
        bool isA(PhraseCategoryEnum checkPhraseCategory);
        bool isA(LexicalCategoryEnum checkLexicalCategory);
        string getRealisation();
        void setRealisation(string realised);
        INLGElement getParent();
        void setParent(INLGElement newParent);
    }

    public abstract class NLGElement : INLGElement
    {
        public IElementCategory category { get; set; }
        public Dictionary<string, object> features => _features;

        protected Dictionary<string, object> _features = new Dictionary<string, object>();
        protected INLGElement parent { get; set; }
        protected string realisation { get; set; }
        protected NLGFactory factory { get; set; }

        /**
         * Sets the category of this element.
         * 
         * @param newCategory
         *            the new <code>ElementCategory</code> for this element.
         */
        public virtual void setCategory(IElementCategory newCategory)
        {
            category = newCategory;
        }

        /**
         * Retrieves the category for this element.
         * 
         * @return the category as a <code>ElementCategory</code>.
         */
        public virtual IElementCategory getCategory()
        {
            return category;
        }

        /**
         * Adds a feature to the feature map. If the feature already exists then it
         * is given the new value. If the value provided is <code>null</code> the
         * feature is removed from the map.
         * 
         * @param featureName
         *            the name of the feature.
         * @param featureValue
         *            the new value of the feature or <code>null</code> if the
         *            feature is to be removed.
         */

        public virtual void setFeature(string featureName, object featureValue)
        {
              if (featureName != null)
            {
                if (featureValue == null)
                {
                    if (_features.ContainsKey(featureName))
                    {
                        _features.Remove(featureName);
                    }
                }
                if (!_features.ContainsKey(featureName))
                {
                    _features.Add(featureName, featureValue);
                }
                else
                {
                    _features[featureName] = featureValue;
                }
            }
        }

        /**
         * A convenience method for setting bool features.
         * 
         * @param featureName
         *            the name of the feature.
         * @param featureValue
         *            the <code>bool</code> value of the feature.
         */

        public virtual void setFeature(string featureName, bool featureValue)
        {
            if (featureName != null)
            {
                if (!_features.ContainsKey(featureName))
                {
                    _features.Add(featureName, featureValue);
                }
                else
                {
                    _features[featureName] = featureValue;
                }
            }

        }

        public virtual void setFeature(string featureName, long featureValue)
        {
            if (featureName != null)
            {
                if (!_features.ContainsKey(featureName))
                {
                    _features.Add(featureName, featureValue);
                }
                else
                {
                    _features[featureName] = featureValue;
                }
            }
        }

        /**
         * A convenience method for setting floating point number features.
         * 
         * @param featureName
         *            the name of the feature.
         * @param featureValue
         *            the <code>float</code> value of the feature.
         */

        public virtual void setFeature(string featureName, float featureValue)
        {
            if (featureName != null)
            {
                if (!_features.ContainsKey(featureName))
                {
                    _features.Add(featureName, featureValue);
                }
                else
                {
                    _features[featureName] = featureValue;
                }
            }
        }

        /**
         * A convenience method for setting double precision floating point number
         * features.
         * 
         * @param featureName
         *            the name of the feature.
         * @param featureValue
         *            the <code>double</code> value of the feature.
         */

        public virtual void setFeature(string featureName, double featureValue)
        {
            if (featureName != null)
            {
                if (!_features.ContainsKey(featureName))
                {
                    _features.Add(featureName, featureValue);
                }
                else
                {
                    _features[featureName] = featureValue;
                }
            }

        }

        /**
         * Retrieves the value of the feature.
         * 
         * @param featureName
         *            the name of the feature.
         * @return the <code>object</code> value of the feature.
         */

        public virtual object getFeature(string featureName)
        {
            if (featureName == null) return null;
            if (!_features.ContainsKey(featureName)) return null;
            return _features[featureName];
        }

        public virtual Tense getFeatureTense(string featureName)
        {
            if (featureName == null) return Tense.PRESENT;
            if (!_features.ContainsKey(featureName)) return Tense.PRESENT;
            var v = _features[featureName];
            if (v is Tense)
                return (Tense) v;
            return Tense.PRESENT;
        }


        /**
         * Retrieves the value of the feature as a string. If the feature doesn't
         * exist then <code>null</code> is returned.
         * 
         * @param featureName
         *            the name of the feature.
         * @return the <code>string</code> representation of the value. This is
         *         taken by calling the object's <code>ToString()</code> method.
         */

        public virtual string getFeatureAsString(string featureName)
        {
            var value = getFeature(featureName);
            string stringValue = null;

            if (value != null)
            {
                stringValue = value.ToString();
            }
            return stringValue;
        }

        /**
         * <p>
         * Retrieves the value of the feature as a list of elements. If the feature
         * is a single <code>NLGElement</code> then it is wrapped in a list. If the
         * feature is a <code>Collection</code> then each object in the collection
         * is checked and only <code>NLGElement</code>s are returned in the list.
         * </p>
         * <p>
         * If the feature does not exist then an empty list is returned.
         * </p>
         * 
         * @param featureName
         *            the name of the feature.
         * @return the <code>List</code> of <code>NLGElement</code>s
         */

        public virtual List<INLGElement> getFeatureAsElementList(string featureName)
        {
            var list = new List<INLGElement>();
            var value = getFeature(featureName);
            if (value == null) return list;

            var item = value as INLGElement;
            if (item != null)
            {
                list.Add(item);
            }
            else
            {
                var elements = value as IEnumerable<INLGElement>;
                if (elements == null) return list;
                list.AddRange(elements);                
            }
            return list;
        }


        /**
         * Retrieves the value of the feature as a <code>Boolean</code>. If the
         * feature does not exist or is not a bool then
         * <code>Boolean.FALSE</code> is returned.
         * 
         * @param featureName
         *            the name of the feature.
         * @return the <code>Boolean</code> representation of the value. Any
         *         non-Boolean type will return <code>Boolean.FALSE</code>.
         */

        public virtual bool getFeatureAsBoolean(string featureName)
        {
            var value = getFeature(featureName);
            if (value is bool)
            {
                return (bool)value;
            }
            var s = value as string;
            return s != null && bool.Parse(s);
        }

        /**
         * Retrieves the value of the feature as a <code>NLGElement</code>. If the
         * value is a string then it is wrapped in a <code>StringElement</code>. If
         * the feature does not exist or is of any other type then <code>null</code>
         * is returned.
         * 
         * @param featureName
         *            the name of the feature.
         * @return the <code>NLGElement</code>.
         */

        public virtual INLGElement getFeatureAsElement(string featureName)
        {
            var value = getFeature(featureName);
            INLGElement elementValue = null;

            var element = value as INLGElement;
            if (element != null)
            {
                elementValue = element;
            }
            else if (value is string)
            {
                elementValue = new StringElement((string) value);
            }
            return elementValue;
        }

        /**
         * Retrieves the map containing all the features for this element.
         * 
         * @return a <code>Map</code> of <code>string</code>, <code>object</code>.
         */

        public virtual Dictionary<string, object> getAllFeatures()
        {
            return _features;
        }

        /**
         * Checks the feature map to see if the named feature is present in the map.
         * 
         * @param featureName
         *            the name of the feature to look for.
         * @return <code>true</code> if the feature exists, <code>false</code>
         *         otherwise.
         */

        public virtual bool hasFeature(string featureName)
        {
            if (featureName == null) return false;
            if (!_features.ContainsKey(featureName)) return false;
            if (_features[featureName] == null) return false;
            return true;
        }

        /**
         * Deletes the named feature from the map.
         * 
         * @param featureName
         *            the name of the feature to be removed.
         */

        public virtual void removeFeature(string featureName)
        {
            _features.Remove(featureName);
        }



        /**
         * Sets the parent element of this element.
         * 
         * @param newParent
         *            the <code>NLGElement</code> that is the parent of this
         *            element.
         */

        public virtual void setParent(INLGElement newParent)
        {
            parent = newParent;
        }

        /**
         * Retrieves the parent of this element.
         * 
         * @return the <code>NLGElement</code> that is the parent of this element.
         */

        public virtual INLGElement getParent()
        {
            return parent;
        }

        /**
         * Sets the realisation of this element.
         * 
         * @param realised
         *            the <code>string</code> representing the final realisation for
         *            this element.
         */

        public virtual void setRealisation(string realised)
        {
            realisation = realised;
        }

        /**
         * Retrieves the final realisation of this element.
         * 
         * @return the <code>string</code> representing the final realisation for
         *         this element.
         */

        public virtual string getRealisation()
        {
            var start = 0;
            var end = 0;
            if (null != realisation)
            {
                end = realisation.Length;

                while (start < realisation.Length   
                       && ' ' == realisation.charAt(start))
                {
                    start++;
                }
                if (start == realisation.Length)
                {
                    realisation = null;
                }
                else
                {
                    while (end > 0 && ' ' == realisation.charAt(end - 1))
                    {
                        end--;
                    }
                }
            }

            // AG: changed this to return the empty string if the realisation is
            // null
            // avoids spurious nulls appearing in output for empty phrases.
            return realisation == null
                ? ""
                : realisation.substring(
                    start, end);
        }

        public override string ToString()
        {
            var buffer = new StringBuilder("{realisation=").Append(realisation); 
            if (category != null)
            {
                buffer.Append(", category=").Append(category); 
            }
            /*if (_features != null)
            {
                buffer.Append(", features=").Append(_features); 
                foreach (var pair in _features)
                {
                    buffer.Append($"/k:{pair.Key}/v:{pair.Value}/\n");
                }
                
            }*/
            buffer.Append('}');
            return buffer.ToString();
        }


        public bool isA(LexicalCategoryEnum checkLexicalCategory)
        {
            var isA = false;
            var thiscategory = category as ILexicalCategory;
            if (thiscategory != null)
            {
                isA = thiscategory.lexType == checkLexicalCategory;
            }          
            return isA;
        }


        public bool isA(PhraseCategoryEnum checkPhraseCategory)
        {
            var isA = false;
            var thiscategory = category as IPhraseCategory;
            if (thiscategory != null)
            {
                isA = thiscategory.phrType == checkPhraseCategory;
            }
            return isA;
        }


        /**
         * Retrieves the children for this element. This method needs to be
         * overridden for each specific type of element. Each type of element will
         * have its own way of determining the child elements.
         * 
         * @return a <code>List</code> of <code>NLGElement</code>s representing the
         *         children of this element.
         */
        public abstract List<INLGElement> getChildren();

        /**
         * Retrieves the set of features currently contained in the feature map.
         * 
         * @return a <code>Set</code> of <code>string</code>s representing the
         *         feature names. The set is unordered.
         */

        public virtual List<string> getAllFeatureNames()
        {
            return _features.Keys.ToList();
        }

        public virtual string printTree(string indent)
        {
            var thisIndent = indent == null ? " |-" : indent + " |-";  
            var childIndent = indent == null ? " |-" : indent + " |-";  
            var print = new StringBuilder();
            print.Append("NLGElement: ").Append(ToString()).Append('\n'); 

            var children = getChildren();

            if (children != null)
            {
                foreach (var eachChild in getChildren())
                {
                    print.Append(thisIndent).Append(
                        eachChild.printTree(childIndent));
                }
            }
            return print.ToString();
        }

        /**
         * Determines if this element has its realisation equal to the given string.
         * 
         * @param elementRealisation
         *            the string to check against.
         * @return <code>true</code> if the string matches the element's
         *         realisation, <code>false</code> otherwise.
         */

        /**
         * Sets the number agreement on this element. This method is added for
         * convenience and not all element types will make use of the number
         * agreement feature. The method is identical to calling {@code
         * setFeature(Feature.NUMBER, NumberAgreement.PLURAL)} for plurals or
         * {@code setFeature(Feature.NUMBER, NumberAgreement.SINGULAR)} for the
         * singular.
         * 
         * @param isPlural
         *            <code>true</code> if this element is to be treated as a
         *            plural, <code>false</code> otherwise.
         */

        public virtual void setPlural(bool isPlural)
        {
            if (isPlural)
            {
                setFeature(Feature.NUMBER.ToString(), NumberAgreement.PLURAL);
            }
            else
            {
                setFeature(Feature.NUMBER.ToString(), NumberAgreement.SINGULAR);
            }
        }

        /**
         * Determines if this element is to be treated as a plural. This is a
         * convenience method and not all element types make use of number
         * agreement.
         * 
         * @return <code>true</code> if the <code>Feature.NUMBER</code> feature has
         *         the value <code>NumberAgreement.PLURAL</code>, <code>false</code>
         *         otherwise.
         */

        public virtual bool isPlural()
        {
            return NumberAgreement.PLURAL.Equals(getFeature(Feature.NUMBER.ToString()));
        }
        /**
 * @return the NLG factory
 */
        public NLGFactory getFactory()
        {
            return factory;
        }

        /**
         * @param factory
         *            the NLG factory to set
         */
        public void setFactory(NLGFactory factory)
        {
            this.factory = factory;
        }
        /**
         * An NLG element is equal to some object if the object is an NLGElement,
         * they have the same category and the same features.
         */

  

    }
}