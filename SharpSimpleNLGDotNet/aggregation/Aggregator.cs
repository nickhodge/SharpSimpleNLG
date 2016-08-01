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
    public class Aggregator : NLGModule
    {

        public List<AggregationRule> _rules { get; set; }
        public NLGFactory _factory { get; set; }

        /**
         * Creates an instance of Aggregator
         */

        public override void initialise()
        {
            _rules = new List<AggregationRule>();
            _factory = new NLGFactory();
        }

        /**
         * Set the factory that this aggregator should use to create phrases. The
         * factory will be passed on to all the component rules.
         * 
         * @param factory
         *            the phrase factory
         */

        public void setFactory(NLGFactory factory)
        {
            _factory = factory;

            foreach (var rule in _rules)
            {
                rule.setFactory(_factory);
            }
        }

        /**
         * Add a rule to this aggregator. Aggregation rules are applied in the order
         * in which they are supplied.
         * 
         * @param rule
         *            the rule
         */

        public void addRule(AggregationRule rule)
        {
            rule.setFactory(_factory);
            _rules.Add(rule);
        }

        /**
         * Get the rules in this aggregator.
         * 
         * @return the rules
         */

        public List<AggregationRule> getRules()
        {
            return _rules;
        }

        /**
         * Apply aggregation to a single phrase. This will only work if the phrase
         * is a coordinated phrase, whose children can be further aggregated.
         * 
         */

        public override INLGElement realise(INLGElement element)
        {
            var result = element;

            foreach (var rule in _rules)
            {
                var intermediate = rule.apply(result);

                if (intermediate != null)
                {
                    result = intermediate;
                }
            }

            return result;
        }

        /**
         * Apply aggregation to a list of elements. This method iterates through the
         * rules supplied via {@link #addRule(AggregationRule)} and applies them to
         * the elements.
         * 
         * @param elements
         *            the list of elements to aggregate
         * @return a list of the elements that remain after the aggregation rules
         *         have been applied
         */


        public override List<INLGElement> realise(List<INLGElement> elements)
        {
            foreach (var rule in _rules)
            {
                elements = rule.apply(elements);
            }

            return elements;
        }

    }
}