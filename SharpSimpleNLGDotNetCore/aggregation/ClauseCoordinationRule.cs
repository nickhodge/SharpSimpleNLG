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
    public class ClauseCoordinationRule : AggregationRule
    {

        /**
         * Constructs an instance of the ClauseCoordinationRule
         */

        public ClauseCoordinationRule()
        {
            
        }

        /**
         * Applies aggregation to two NLGElements e1 and e2, succeeding only if they
         * are clauses (that is, e1.getCategory() == e2.getCategory ==
         * {@link simplenlg.framework.PhraseCategory#CLAUSE}).
         */

        public override INLGElement apply(INLGElement previous, INLGElement next)
        {
            INLGElement aggregated = null;

            if (previous.getCategory().enumType == (int)PhraseCategoryEnum.CLAUSE
                && next.getCategory().enumType == (int)PhraseCategoryEnum.CLAUSE
                && PhraseChecker.nonePassive(new List<INLGElement> { previous, next})
                && !PhraseChecker.expletiveSubjects(new List<INLGElement> { previous, next }))
            {

                // case 1: identical sentences: remove the current
                if (PhraseChecker.sameSentences(new List<INLGElement> { previous, next }))
                {
                    aggregated = previous;

                    // case 2: subjects identical: coordinate VPs
                }
                else if (PhraseChecker.sameFrontMods(new List<INLGElement> { previous, next })
                         && PhraseChecker.sameSubjects(new List<INLGElement> { previous, next })
                         && PhraseChecker.samePostMods(new List<INLGElement> { previous, next }))
                {
                    aggregated = factory.createClause();
                    aggregated.setFeature(InternalFeature.SUBJECTS.ToString(), previous
                        .getFeatureAsElementList(InternalFeature.SUBJECTS.ToString()));
                    aggregated.setFeature(InternalFeature.FRONT_MODIFIERS.ToString(), previous
                        .getFeatureAsElement(InternalFeature.FRONT_MODIFIERS.ToString()));
                    aggregated.setFeature(Feature.CUE_PHRASE.ToString(), previous
                        .getFeatureAsElement(Feature.CUE_PHRASE.ToString()));
                    aggregated
                        .setFeature(
                            InternalFeature.POSTMODIFIERS.ToString(),
                            previous
                                .getFeatureAsElementList(InternalFeature.POSTMODIFIERS.ToString()));
                    INLGElement vp;

                    // case 2.1: VPs have different arguments but same
                    // head & mods
                    if (!PhraseChecker.sameVPArgs(new List<INLGElement> { previous, next })
                        && PhraseChecker.sameVPHead(new List<INLGElement> { previous, next })
                        && PhraseChecker.sameVPModifiers(new List<INLGElement> { previous, next }))
                    {

                        var vp1 = previous
                            .getFeatureAsElement(InternalFeature.VERB_PHRASE.ToString());
                        vp = factory.createVerbPhrase();
                        vp.setFeature(InternalFeature.HEAD.ToString(), vp1
                            .getFeatureAsElement(InternalFeature.HEAD.ToString()));
                        vp.setFeature(InternalFeature.COMPLEMENTS.ToString(),
                                vp1.getFeatureAsElementList(InternalFeature.COMPLEMENTS.ToString()));
                        vp.setFeature(
                                InternalFeature.PREMODIFIERS.ToString(),
                                vp1
                                    .getFeatureAsElementList(InternalFeature.PREMODIFIERS.ToString()));
                        vp.setFeature(
                                InternalFeature.POSTMODIFIERS.ToString(),
                                vp1
                                    .getFeatureAsElementList(InternalFeature.POSTMODIFIERS.ToString()));

                        // case 2.2: just create a coordinate vP
                    }
                    else
                    {
                        var vp1 = previous
                            .getFeatureAsElement(InternalFeature.VERB_PHRASE.ToString());
                        var vp2 = next
                            .getFeatureAsElement(InternalFeature.VERB_PHRASE.ToString());
                        vp = factory.createCoordinatedPhrase(vp1, vp2);

                        // case 2.3: expletive subjects
                    }

                    aggregated.setFeature(InternalFeature.VERB_PHRASE.ToString(), vp);

                    // case 3: identical VPs: conjoin subjects and front
                    // modifiers
                }
                else if (PhraseChecker.sameFrontMods(new List<INLGElement> { previous, next })
                         && PhraseChecker.sameVP(new List<INLGElement> { previous, next })
                         && PhraseChecker.samePostMods(new List<INLGElement> { previous, next }))
                {
                    aggregated = factory.createClause();
                    aggregated
                        .setFeature(
                            InternalFeature.FRONT_MODIFIERS.ToString(),
                            previous
                                .getFeatureAsElementList(InternalFeature.FRONT_MODIFIERS.ToString()));
                    var subjects = factory
                        .createCoordinatedPhrase();
                    subjects.setCategory(new PhraseCategory_NOUN_PHRASE());
                    var allSubjects = previous
                        .getFeatureAsElementList(InternalFeature.SUBJECTS.ToString());
                    allSubjects.addAll(next
                        .getFeatureAsElementList(InternalFeature.SUBJECTS.ToString()));

                    foreach (var subj in allSubjects)
                    {
                        subjects.addCoordinate(subj);
                    }

                    aggregated.setFeature(InternalFeature.SUBJECTS.ToString(), subjects);
                    aggregated
                        .setFeature(
                            InternalFeature.POSTMODIFIERS.ToString(),
                            previous
                                .getFeatureAsElementList(InternalFeature.POSTMODIFIERS.ToString()));
                    aggregated.setFeature(InternalFeature.VERB_PHRASE.ToString(), previous
                        .getFeature(InternalFeature.VERB_PHRASE.ToString()));
                }
            }

            return aggregated;
        }
    }
}
