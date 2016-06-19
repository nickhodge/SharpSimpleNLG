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
    public abstract class AggregationRule
    {

        protected NLGFactory factory;

        /**
         * Creates a new instance of AggregationRule
         */

        public AggregationRule()
        {
            factory = new NLGFactory();
        }

        /**
         * Set the factory that the rule should use to create phrases.
         * 
         * @param factory
         *            the factory
         */

        public void setFactory(NLGFactory factory)
        {
            this.factory = factory;
        }

        /**
         * 
         * @return the factory being used by this rule to create phrases
         */

        public NLGFactory getFactory()
        {
            return factory;
        }

        /**
         * Performs aggregation on an arbitrary number of elements in a list. This
         * method calls {{@link #apply(NLGElement, NLGElement)} on all pairs of
         * elements in the list, recursively aggregating whenever it can.
         * 
         * @param phrases
         *            the sentences
         * @return a list containing the phrases, such that, for any two phrases s1
         *         and s2, if {@link #apply(NLGElement, NLGElement)} succeeds on s1
         *         and s2, the list contains the result; otherwise, the list
         *         contains s1 and s2.
         */

        public List<INLGElement> apply(List<INLGElement> phrases)
        {
            var results = new List<INLGElement>();
            ;

            if (phrases.Count >= 2)
            {
                var removed = new List<INLGElement>();

                for (var i = 0; i < phrases.Count; i++)
                {
                    var current = phrases[i];

                    if (removed.Contains(current))
                    {
                        continue;
                    }

                    for (var j = i + 1; j < phrases.Count; j++)
                    {
                        var next = phrases[j];
                        var aggregated = apply(current, next);

                        if (aggregated != null)
                        {
                            current = aggregated;
                            removed.Add(next);
                        }
                    }

                    results.Add(current);
                }

            }
            else if (phrases.Count == 1)
            {
                results.Add(apply(phrases[0]));
            }

            return results;
        }

        /**
         * Perform aggregation on a single phrase. This method only works on a
         * {@link simplenlg.framework.CoordinatedPhraseElement}, in which case it
         * calls {@link #apply(List)} on the children of the coordinated phrase,
         * returning a coordinated phrase whose children are the result.
         * 
         * @param phrase
         * @return aggregated result
         */

        public INLGElement apply(INLGElement phrase)
        {
            INLGElement result = null;

            var element = phrase as CoordinatedPhraseElement;
            if (element != null)
            {
                var children = element.getChildren();
                var aggregated = apply(children);

                if (aggregated.Count == 1)
                {
                    result = aggregated[0];

                }
                else
                {
                    result = factory.createCoordinatedPhrase();

                    foreach (var agg in aggregated)
                    {
                        ((CoordinatedPhraseElement) result).addCoordinate(agg);
                    }
                }
            }


            if (result != null)
            {
                foreach (var feature in phrase.getAllFeatureNames())
                {
                    if (result is SPhraseSpec)
                        ((SPhraseSpec)result).setFeature(feature, phrase.getFeature(feature));
                    else
                        result.setFeature(feature, phrase.getFeature(feature));

                }
            }

            return result;
        }

        /**
         * Performs aggregation on a pair of sentences. This is the only method that
         * extensions of <code>AggregationRule</code> need to implement.
         * 
         * @param sentence1
         *            the first sentence
         * @param sentence2
         *            the second sentence
         * @return an aggregated sentence, if the method succeeds, <code>null</code>
         *         otherwise
         */
        public abstract INLGElement apply(INLGElement sentence1, INLGElement sentence2);

    }
}