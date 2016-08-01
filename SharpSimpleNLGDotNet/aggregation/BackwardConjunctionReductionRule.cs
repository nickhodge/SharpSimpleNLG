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
    public class BackwardConjunctionReductionRule : AggregationRule
    {

    //private SyntaxProcessor _syntaxProcessor;

    /**
     * Creates a new <code>BackwardConjunctionReduction</code>.
     */

    public BackwardConjunctionReductionRule() : base()
    {
        //this._syntaxProcessor = new SyntaxProcessor();
    }

    /**
     * Applies backward conjunction reduction to two NLGElements e1 and e2,
     * succeeding only if they are clauses (that is, e1.getCategory() ==
     * e2.getCategory == {@link simplenlg.framework.PhraseCategory#CLAUSE}).
     * 
     * @param previous
     *            the first phrase
     * @param next
     *            the second phrase
     * @return a coordinate phrase if aggregation is successful,
     *         <code>null</code> otherwise
     */

    public override INLGElement apply(INLGElement previous, INLGElement next)
    {
        var success = false;

        if (previous.getCategory().enumType == (int)PhraseCategoryEnum.CLAUSE
            && next.getCategory().enumType == (int)PhraseCategoryEnum.CLAUSE
            && PhraseChecker.nonePassive(new List<INLGElement> { previous, next }))
        {

            var rightPeriphery = PhraseChecker.rightPeriphery(new List<INLGElement> { previous, next});

            foreach (var pair in rightPeriphery)
            {
                if (pair.lemmaIdentical())
                {
                    pair.elideLeftmost();
                    success = true;
                }
            }
        }

        return success
            ? factory.createCoordinatedPhrase(previous, next)
            : null;
    }

    }
}