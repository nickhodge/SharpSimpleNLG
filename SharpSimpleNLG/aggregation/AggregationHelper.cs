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
    public class AggregationHelper
    {

        public static List<DiscourseFunction> FUNCTIONS = new List<DiscourseFunction>()
        {
            DiscourseFunction.SUBJECT,
            DiscourseFunction.HEAD,
            DiscourseFunction.COMPLEMENT,
            DiscourseFunction.PRE_MODIFIER,
            DiscourseFunction.POST_MODIFIER,
            DiscourseFunction.VERB_PHRASE
        };

        public static List<DiscourseFunction> RECURSIVE = new List<DiscourseFunction>()
        { DiscourseFunction.VERB_PHRASE};

        public static List<FunctionalSet> collectFunctionalPairs(
            INLGElement phrase1, INLGElement phrase2)
        {
            var children1 = getAllChildren(phrase1);
            var children2 = getAllChildren(phrase2);
            var pairs = new List<FunctionalSet>();

            if (children1.Count == children2.Count)
            {
                var periph = Periphery.LEFT;

                for (var i = 0; i < children1.Count; i++)
                {
                    var child1 = children1[i];
                    var child2 = children2[i];
                    var cat1 = child1.getCategory();
                    var cat2 = child2.getCategory();
                    var func1 = (DiscourseFunction) child1
                        .getFeature(InternalFeature.DISCOURSE_FUNCTION.ToString());
                    var func2 = (DiscourseFunction) child2
                        .getFeature(InternalFeature.DISCOURSE_FUNCTION.ToString());

                    if (cat1 == cat2 && func1 == func2)
                    {
                        pairs.Add(FunctionalSet.newInstance(func1, cat1, periph, new List<INLGElement> { child1, child2 }));

                        if (((ILexicalCategory)cat1).lexType == LexicalCategoryEnum.VERB)
                        {
                            periph = Periphery.RIGHT;
                        }

                    }
                    else
                    {
                        pairs.Clear();
                        break;
                    }
                }
            }

            return pairs;
        }

        private static List<INLGElement> getAllChildren(INLGElement element)
        {
            var children = new List<INLGElement>();
            List<INLGElement> components = element is
            ListElement
                ? element
                    .getFeatureAsElementList(InternalFeature.COMPONENTS.ToString())
                : element
                    .getChildren();

            foreach (var child in components)
            {
                children.Add(child);

                if (((IPhraseCategory)child.getCategory()).phrType == PhraseCategoryEnum.VERB_PHRASE
                    || child.getFeature(InternalFeature.DISCOURSE_FUNCTION.ToString()).ToString() == DiscourseFunction.VERB_PHRASE.ToString())
                {
                    children.AddRange(getAllChildren(child));
                }
            }

            return children;
        }

    }
}
