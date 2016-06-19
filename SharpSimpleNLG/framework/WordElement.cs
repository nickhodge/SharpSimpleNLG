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
using System.Collections.Generic;
using System.Text;
using SimpleNLG.Extensions;

namespace SimpleNLG
{
    public class WordElement : NLGElement
    {

   

         // Words have baseForm, category, id, and features
        // features are inherited from NLGElement

        public string baseForm { get; set; } // base form, eg "dog". currently also in NLG Element, but
        // will be removed from there

        public string id { get; set; } // id in lexicon (may be null);

        Dictionary<Inflection, InflectionSet> inflVars; // the inflectional variants

        Inflection defaultInfl; // the default inflectional variant

        // LexicalCategory category; // type of word

        /**********************************************************/
        // constructors
        /**********************************************************/


        /**
         * create a WordElement with the specified baseForm, category, ID
         * 
         * @param baseForm
         *            - base form of WordElement
         * @param category
         *            - category of WordElement
         * @param id
         *            - ID of word in lexicon
         */

        public WordElement(string baseForm = null, ILexicalCategory category = null, string id = null)
        {
            this.baseForm = baseForm;
            if (category == null)
                setCategory(new LexicalCategory_ANY());
            else
                setCategory(category);
            this.id = id;
            inflVars = new Dictionary<Inflection, InflectionSet>();
        }

        /**
         * creates a duplicate WordElement from an existing WordElement
         * 
         * @param currentWord
         *            - An existing WordElement
         */

        public WordElement(WordElement currentWord)
        {
            baseForm = currentWord.getBaseForm();
            setCategory(currentWord.getCategory());
            id = currentWord.getId();
            inflVars = currentWord.getInflectionalVariants();
            defaultInfl = (Inflection) currentWord.getDefaultInflectionalVariant();
            setFeatures(currentWord);
        }




        /**********************************************************/
        // getters and setters
        /**********************************************************/

        /**
         * @return the baseForm
         */

        public string getBaseForm()
        {
            return baseForm;
        }

        /**
         * @return the id
         */

        public string getId()
        {
            return id;
        }

        /**
         * @param baseForm
         *            the baseForm to set
         */

        public void setBaseForm(string baseForm)
        {
            this.baseForm = baseForm;
        }

        /**
         * @param id
         *            the id to set
         */

        public void setId(string id)
        {
            this.id = id;
        }

        /**
         * Set the default inflectional variant of a word. This is mostly relevant
         * if the word has more than one possible inflectional variant (for example,
         * it can be inflected in both a regular and irregular way).
         * 
         * <P>
         * If the default inflectional variant is set, the inflectional forms of the
         * word may change as a result. This depends on whether inflectional forms
         * have been specifically associated with this variant, via
         * {@link #addInflectionalVariant(Inflection, string, string)}.
         * 
         * <P>
         * The <code>NIHDBLexicon</code> associates different inflectional variants
         * with words, if they are so specified, and adds the correct forms.
         * 
         * @param variant
         *            The variant
         */

        public void setDefaultInflectionalVariant(Inflection variant)
        {
            if (getFeature(LexicalFeature.DEFAULT_INFL) != null)
            {
                removeFeature(LexicalFeature.DEFAULT_INFL);
            }
            setFeature(LexicalFeature.DEFAULT_INFL, variant);
            defaultInfl = variant;

            if (inflVars.containsKey(variant))
            {
                var set = inflVars[variant];
                var forms = LexicalFeature.getInflectionalFeatures(getCategory());

                if (forms != null)
                {
                    foreach (var f in forms)
                    {
                        setFeature(f, set.getForm(f));
                    }
                }
            }
        }

        /**
         * @return the default inflectional variant
         */

        public object getDefaultInflectionalVariant()
        {
            // return getFeature(LexicalFeature.DEFAULT_INFL);
            return defaultInfl;
        }

        /**
         * Convenience method to get all the inflectional forms of the word.
         * 
         * @return the HashMap of inflectional variants
         */

        public Dictionary<Inflection, InflectionSet> getInflectionalVariants()
        {
            return inflVars;
        }


        /**
         * Convenience method to set the default spelling variant of a word.
         * Equivalent to
         * <code>setFeature(LexicalFeature.DEFAULT_SPELL, variant)</code>.
         * 
         * <P>
         * By default, the spelling variant used is the base form. If otherwise set,
         * this forces the realiser to always use the spelling variant specified.
         * 
         * @param variant
         *            The spelling variant
         */

        public void setDefaultSpellingVariant(string variant)
        {
            setFeature(LexicalFeature.DEFAULT_SPELL, variant);
        }

        /**
         * Convenience method, equivalent to
         * <code>getFeatureAsString(LexicalFeature.DEFAULT_SPELL)</code>. If this
         * feature is not set, the baseform is returned.
         * 
         * @return the default inflectional variant
         */

        public string getDefaultSpellingVariant()
        {
            var defSpell = getFeatureAsString(LexicalFeature.DEFAULT_SPELL);
            return defSpell == null ? getBaseForm() : defSpell;
        }


        /**
         * Add an inflectional variant to this word element. This method is intended
         * for use by a <code>Lexicon</code>. The idea is that words which have more
         * than one inflectional variant (for example, a regular and an irregular
         * form of the past tense), can have a default variant (for example, the
         * regular), but also store information about the other variants. This comes
         * in useful in case the default inflectional variant is reset to a new one.
         * In that case, the stored forms for the new variant are used to inflect
         * the word.
         * 
         * <P>
         * <strong>An example:</strong> The verb <i>lie</i> has both a regular form
         * (<I>lies, lied, lying</I>) and an irregular form (<I>lay, lain,</I> etc).
         * Assume that the <code>Lexicon</code> provides this information and treats
         * this as variant information of the same word (as does the
         * <code>NIHDBLexicon</code>, for example). Typically, the default
         * inflectional variant is the <code>Inflection.REGULAR</code>. This means
         * that morphology proceeds to inflect the verb as <I>lies, lying</I> and so
         * on. If the default inflectional variant is reset to
         * <code>Inflection.IRREGULAR</code>, the stored irregular forms will be
         * used instead.
         * 
         * @param infl
         *            the Inflection pattern with which this form is associated
         * @param lexicalFeature
         *            the actual inflectional feature being set, for example
         *            <code>LexicalFeature.PRESENT_3S</code>
         * @param form
         *            the actual inflected word form
         */

        public void addInflectionalVariant(Inflection infl, string lexicalFeature,
            string form)
        {
            if (inflVars.containsKey(infl))
            {
                inflVars[infl].addForm(lexicalFeature, form);
            }
            else
            {
                var set = new InflectionSet(infl);
                set.addForm(lexicalFeature, form);
                inflVars.put(infl, set);
            }
        }

        /**
         * Specify that this word has an inflectional variant (e.g. irregular)
         * 
         * @param infl
         *            the variant
         */

        public void addInflectionalVariant(Inflection infl)
        {
            inflVars.put(infl, new InflectionSet(infl));
        }

        /**
         * Check whether this word has a particular inflectional variant
         * 
         * @param infl
         *            the variant
         * @return <code>true</code> if this word has the variant
         */

        public bool hasInflectionalVariant(Inflection infl)
        {
            return inflVars.containsKey(infl);
        }

        /**
         * Sets Features from another existing WordElement into this WordElement.
         * 
         * @param currentWord
         * 				the WordElement to copy features from
         */

        public void setFeatures(WordElement currentWord)
        {
            if (null != currentWord && null != currentWord.getAllFeatures())
            {
                foreach (var feature in currentWord.getAllFeatureNames())
                {
                    setFeature(feature, currentWord.getFeature(feature));
                }
            }
        }

 
        public override string ToString()
        {
            var _category = getCategory();
            var buffer = new StringBuilder("WordElement["); 
            buffer.Append(getBaseForm()).Append(':');
            if (_category != null)
            {
                buffer.Append(_category.ToString());
            }
            else
            {
                buffer.Append("no category"); 
            }
            buffer.Append(']');
            return buffer.ToString();
        }

        public string toXML()
        {
            var xml = "<word>\n"; 
            if (getBaseForm() != null)
                xml = xml + $"  <base>{getBaseForm()}</base>\n"; 
            if (getCategory().enumType == (int)LexicalCategoryEnum.ANY)
                xml = xml + $"  <category>{getCategory().ToString().toLowerCase()}</category>\n"; 
            if (getId() != null)
                xml = xml + $"  <id>{getId()}</id>\n"; 

            var featureNames = new SortedSet<string>(getAllFeatureNames()); // list features in alpha order
            foreach (var feature in featureNames)
            {
                var value = getFeature(feature);
                if (value != null)
                {
                    // ignore null features
                    if (value is bool)
                    {
                        // booleans ignored if false,
                        // shown as <XX/> if true
                        var bvalue = ((Boolean) value);
                        if (bvalue)
                            xml = xml + $"  <{feature}/>\n"; 
                    }
                    else
                    {
                        // otherwise include feature and value
                        xml = xml + $"  <{feature}>{value}</{feature}>\n";
                    }
                }

            }
            xml = xml + "</word>\n"; 
            return xml;
        }

        /**
         * This method returns an empty <code>List</code> as words do not have child
         * elements.
         */
 
        public override List<INLGElement> getChildren()
        {
            return new List<INLGElement>();
        }

  
        public override string printTree(string indent)
        {
            var print = new StringBuilder();
            print.Append("WordElement: base=").Append(getBaseForm()) 
                .Append(", category=").Append(getCategory().ToString()) 
                .Append(", ").Append(base.ToString()).Append('\n'); 
            return print.ToString();
        }

     
    }
}