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
    public class NewAggregator : NLGModule
    {
        private SyntaxProcessor _syntax;
        private NLGFactory _factory;


        public override void initialise()
        {
            _syntax = new SyntaxProcessor();
            _factory = new NLGFactory();
        }

        public override List<INLGElement> realise(List<INLGElement> elements)
        {
            return null;
        }

        public override INLGElement realise(INLGElement element)
        {
            return null;
        }

        public INLGElement realise(INLGElement phrase1, INLGElement phrase2)
        {
            INLGElement result = null;

            if (phrase1 is PhraseElement
                && phrase2 is  PhraseElement
                && phrase1.getCategory().enumType == (int)PhraseCategoryEnum.CLAUSE
                && phrase2.getCategory().enumType == (int)PhraseCategoryEnum.CLAUSE)
            {

                var funcSets = AggregationHelper.collectFunctionalPairs(_syntax.realise(phrase1), _syntax.realise(phrase2));

                applyForwardConjunctionReduction(funcSets);
                applyBackwardConjunctionReduction(funcSets);
                result = _factory.createCoordinatedPhrase(phrase1, phrase2);
            }

            return result;
        }

        private void applyForwardConjunctionReduction(List<FunctionalSet> funcSets)
        {
            foreach (var pair in funcSets)
            {
                if (pair.getPeriphery() == Periphery.LEFT && pair.formIdentical())
                {
                    pair.elideRightMost();
                }
            }

        }

        private void applyBackwardConjunctionReduction(List<FunctionalSet> funcSets)
        {
            foreach (var pair in funcSets)
            {
                if (pair.getPeriphery() == Periphery.RIGHT && pair.formIdentical())
                {
                    pair.elideLeftMost();
                }
            }
        }
    }
}