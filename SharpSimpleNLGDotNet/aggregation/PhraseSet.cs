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
using SimpleNLG.Extensions;

namespace SimpleNLG
{
    /**
     * This class wraps an ordered list of phrases which are constituents of two or
     * more (different) clauses and have the same discourse function in their parent
     * clause. FunctionPairs are used by {@link AggregationRule}s to collect candidate
     * phrase for elision.
     * 
     * @author agatt
     * 
     */

    public class PhraseSet
    {

        private DiscourseFunction function;
        private List<INLGElement> phrases = new List<INLGElement>();

        /**
         * Construct a set of compatible phrases and their function
         * 
         * @param function
         *            their function
         * @param phrases
         *            the list of constituent phrases for the function.
         */

        public PhraseSet(DiscourseFunction _function)
        {
            this.function = _function;
        }

        /**
         * Add a phrase
         * 
         * @param phrase
         *            the phrase to add
         */

        public void addPhrase(INLGElement phrase)
        {
            this.phrases.add(phrase);
        }

        /**
         * Add a collection of phrases.
         * 
         * @param phrases
         *            the phrases to add
         */

        public void addPhrases(List<INLGElement> phrases)
        {
            this.phrases.addAll(phrases);
        }

        /**
         * 
         * @return the function the pair of phrases have in their respective clauses
         */

        public DiscourseFunction getFunction()
        {
            return this.function;
        }

        /**
         * Elide the rightmost constituents in the phrase list, that is, all phrases
         * except the first.
         */

        public void elideRightmost()
        {
            for (var i = 1; i < this.phrases.size(); i++)
            {
                var phrase = this.phrases.get(i);

                if (phrase != null)
                {
                    phrase.setFeature(Feature.ELIDED.ToString(), true);
                }
            }
        }

        /**
         * Elide the leftmost consitutents in the phrase list, that is, all phrases
         * except the rightmost.
         */

        public void elideLeftmost()
        {
            for (var i = this.phrases.size() - 2; i >= 0; i--)
            {
                var phrase = this.phrases.get(i);

                if (phrase != null)
                {
                    phrase.setFeature(Feature.ELIDED.ToString(), true);
                }
            }
        }

        /**
         * Check whether the phrases are lemma identical. This method returns
         * <code>true</code> in the following cases:
         * 
         * <OL>
         * <LI>All phrases are {@link simplenlg.framework.NLGElement}s and they
         * have the same lexical head, irrespective of inflectional variations.</LI>
         * </OL>
         * 
         * @return <code>true</code> if the pair is lemma identical
         */

        public bool lemmaIdentical()
        {
            var ident = !this.phrases.isEmpty();

            for (var i = 1; i < this.phrases.size() && ident; i++)
            {
                var left = this.phrases.get(i - 1);
                var right = this.phrases.get(i);


                if (left != null && right != null)
                {
                    INLGElement leftHead = left.getFeatureAsElement(InternalFeature.HEAD.ToString());
                    INLGElement rightHead = right.getFeatureAsElement(InternalFeature.HEAD.ToString());
                    ident = (leftHead == rightHead || leftHead.equals(rightHead));
                }
            }

            return ident;
        }

        /**
         * Check whether the phrases in this set are identical in form. This method
         * returns true if for every pair of phrases <code>p1</code> and <p2>,
         * <code>p1.equals(p2)</code>.
         * 
         * @return <code>true</code> if all phrases in the set are form-identical
         */

        public bool formIdentical()
        {
            var ident = !this.phrases.isEmpty();

            for (var i = 1; i < this.phrases.size() && ident; i++)
            {
                var left = this.phrases.get(i - 1);
                var right = this.phrases.get(i);

                if (left != null && right != null)
                {
                    ident = left.equals(right);
                }
            }

            return ident;
        }
    }
}
