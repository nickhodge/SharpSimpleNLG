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
    public abstract class PhraseChecker
    {

        /**
         * Check that the sentences supplied are identical
         * 
         * @param sentences
         *            the sentences
         * @return <code>true</code> if for every pair of sentences <code>s1</code>
         *         and <code>s2</code>, <code>s1.Equals(s2)</code>.
         */

        public static bool sameSentences(List<INLGElement> sentences)
        {
            var equal = false;

            if (sentences.Count >= 2)
            {
                for (var i = 1; i < sentences.Count; i++)
                {
                    equal = sentences[i - 1].equals(sentences[i]);
                }
            }

            return equal;
        }

        /**
         * Check whether these sentences have expletive subjects (there, it etc)
         * 
         * @param sentences
         *            the sentences
         * @return <code>true</code> if all the sentences have expletive subjects
         */

        public static bool expletiveSubjects(List<INLGElement> sentences)
        {
            var expl = true;

            for (var i = 1; i < sentences.Count && expl; i++)
            {
                expl = (sentences[i] is SPhraseSpec ? ((SPhraseSpec) sentences[i])
                        .getFeatureAsBoolean(LexicalFeature.EXPLETIVE_SUBJECT)
                    : false)
                ;

            }

            return expl;

        }

        /**
         * Check that the sentences supplied have identical front modifiers
         * 
         * @param sentences
         *            the sentences
         * @return <code>true</code> if for every pair of sentences <code>s1</code>
         *         and <code>s2</code>,
         *         <code>s1.getFrontModifiers().Equals(s2.getFrontModifiers())</code>
         *         .
         */

        public static bool sameFrontMods(List<INLGElement> sentences)
        {
            var equal = true;

            if (sentences.Count >= 2)
            {
                for (var i = 1; i < sentences.Count && equal; i++)
                {

                    if (!sentences[i - 1].hasFeature(Feature.CUE_PHRASE.ToString())
                        && !sentences[i].hasFeature(Feature.CUE_PHRASE.ToString()))
                    {
                        equal = sentences[i - 1]
                            .getFeatureAsElementList(
                                InternalFeature.FRONT_MODIFIERS.ToString())
                            .equals(
                                sentences[i]
                                    .getFeatureAsElementList(InternalFeature.FRONT_MODIFIERS.ToString()));

                    }
                    else if (sentences[i - 1].hasFeature(Feature.CUE_PHRASE.ToString())
                             && sentences[i].hasFeature(Feature.CUE_PHRASE.ToString()))
                    {
                        equal = sentences[i - 1]
                                    .getFeatureAsElementList(
                                        InternalFeature.FRONT_MODIFIERS.ToString())
                                    .equals(
                                        sentences[i]
                                            .getFeatureAsElementList(InternalFeature.FRONT_MODIFIERS.ToString()))
                                && sentences[i]
                                    .getFeatureAsElementList(Feature.CUE_PHRASE.ToString())
                                    .equals(
                                        sentences[i - 1]
                                            .getFeatureAsElementList(Feature.CUE_PHRASE.ToString()));

                    }
                    else
                    {
                        equal = false;
                    }
                }
            }

            return equal;
        }

        /**
         * Check that some phrases have the same postmodifiers
         * 
         * @param sentences
         *            the phrases
         * @return true if they have the same postmodifiers
         */

        public static bool samePostMods(List<INLGElement> sentences)
        {
            var equal = true;

            if (sentences.Count >= 2)
            {

                for (var i = 1; i < sentences.Count && equal; i++)
                {
                    equal = sentences[i - 1]
                        .getFeatureAsElementList(InternalFeature.POSTMODIFIERS.ToString())
                        .equals(
                            sentences[i]
                                .getFeatureAsElementList(InternalFeature.POSTMODIFIERS.ToString()));
                }
            }

            return equal;
        }

        /**
         * Check that the sentences supplied have identical subjects
         * 
         * @param sentences
         *            the sentences
         * @return <code>true</code> if for every pair of sentences <code>s1</code>
         *         and <code>s2</code>
         *         <code>s1.getSubjects().Equals(s2.getSubjects())</code>.
         */

        public static bool sameSubjects(List<INLGElement> sentences)
        {
            var equal = sentences.Count >= 2;

            for (var i = 1; i < sentences.Count && equal; i++)
            {
                equal = sentences[i - 1].getFeatureAsElementList(
                    InternalFeature.SUBJECTS.ToString()).equals(
                    sentences[i]
                        .getFeatureAsElementList(InternalFeature.SUBJECTS.ToString()));
            }

            return equal;
        }

        // /**
        // * Check that the sentences have the same complemts raised to subject
        // * position in the passive
        // *
        // * @param sentences
        // * the sentences
        // * @return <code>true</code> if the passive raising complements are the
        // same
        // */
        // public static bool samePassiveRaisingSubjects(SPhraseSpec...
        // sentences) {
        // bool samePassiveSubjects = sentences.length >= 2;
        //
        // for (int i = 1; i < sentences.length && samePassiveSubjects; i++) {
        // VPPhraseSpec vp1 = (VPPhraseSpec) sentences[i - 1].getVerbPhrase();
        // VPPhraseSpec vp2 = (VPPhraseSpec) sentences[i].getVerbPhrase();
        // samePassiveSubjects = vp1.getPassiveRaisingComplements().Equals(
        // vp2.getPassiveRaisingComplements());
        //
        // }
        //
        // return samePassiveSubjects;
        // }

        /**
         * Check whether all sentences are passive
         * 
         * @param sentences
         *            the sentences
         * @return <code>true</code> if for every sentence <code>s</code>,
         *         <code>s.isPassive() == true</code>.
         */

        public static bool allPassive(List<INLGElement> sentences)
        {
            var passive = true;

            for (var i = 0; i < sentences.Count && passive; i++)
            {
                passive = sentences[i].getFeatureAsBoolean(Feature.PASSIVE.ToString());
            }

            return passive;
        }

        /**
         * Check whether all sentences are active
         * 
         * @param sentences
         *            the sentences
         * @return <code>true</code> if for every sentence <code>s</code>,
         *         <code>s.isPassive() == false</code>.
         */

        public static bool allActive(List<INLGElement> sentences)
        {
            var active = true;

            for (var i = 0; i < sentences.Count && active; i++)
            {
                active = !sentences[i].getFeatureAsBoolean(Feature.PASSIVE.ToString());
            }

            return active;
        }

        /**
         * Check whether the sentences have the same <I>surface</I> subjects, that
         * is, they are either all active and have the same subjects, or all passive
         * and have the same passive raising subjects.
         * 
         * @param sentences
         *            the sentences
         * @return <code>true</code> if the sentences have the same surface subjects
         */

        public static bool sameSurfaceSubjects(List<INLGElement> sentences)
        {
            return allActive(sentences)
                   && sameSubjects(sentences)
                   || allPassive(sentences);
            // && PhraseChecker.samePassiveRaisingSubjects(sentences);
        }

        /**
         * Check that a list of sentences have the same verb
         * 
         * @param sentences
         *            the sentences
         * @return <code>true</code> if for every pair of sentences <code>s1</code>
         *         and <code>s2</code>
         *         <code>s1.getVerbPhrase().getHead().Equals(s2.getVerbPhrase().getHead())</code>
         */

        public static bool sameVPHead(List<INLGElement> sentences)
        {
            var equal = sentences.Count >= 2;

            for (var i = 1; i < sentences.Count && equal; i++)
            {
                var vp1 = sentences[i - 1]
                    .getFeatureAsElement(InternalFeature.VERB_PHRASE.ToString());
                var vp2 = sentences[i]
                    .getFeatureAsElement(InternalFeature.VERB_PHRASE.ToString());

                if (vp1 != null && vp2 != null)
                {
                    var h1 = vp1.getFeatureAsElement(InternalFeature.HEAD.ToString());
                    var h2 = vp2.getFeatureAsElement(InternalFeature.HEAD.ToString());
                    equal = h1 != null && h2 != null ? h1.equals(h2) : false;

                }
                else
                {
                    equal = false;
                }
            }

            return equal;
        }

        /**
         * Check that the sentences supplied are either all active or all passive.
         * 
         * @param sentences
         *            the sentences
         * @return <code>true</code> if the sentences have the same voice
         */

        public static bool haveSameVoice(List<INLGElement> sentences)
        {
            var samePassive = true;
            var prevIsPassive = false;

            if (sentences.Count > 1)
            {
                prevIsPassive = sentences[0].getFeatureAsBoolean(Feature.PASSIVE.ToString());

                for (var i = 1; i < sentences.Count && samePassive; i++)
                {
                    samePassive = sentences[i].getFeatureAsBoolean(Feature.PASSIVE.ToString()) == prevIsPassive;
                }
            }

            return samePassive;
        }

        // /**
        // * Check that the sentences supplied are not existential sentences (i.e.
        // of
        // * the form <I>there be...</I>)
        // *
        // * @param sentences
        // * the sentences
        // * @return <code>true</code> if none of the sentences is existential
        // */
        // public static bool areNotExistential(SPhraseSpec... sentences) {
        // bool notex = true;
        //
        // for (int i = 0; i < sentences.length && notex; i++) {
        // notex = !sentences[i].isExistential();
        // }
        //
        // return notex;
        // }

        /**
         * Check that the sentences supplied have identical verb phrases
         * 
         * @param sentences
         *            the sentences
         * @return <code>true</code> if for every pair of sentences <code>s1</code>
         *         and <code>s2</code>,
         *         <code>s1.getVerbPhrase().Equals(s2.getVerbPhrase())</code>.
         */

        public static bool sameVP(List<INLGElement> sentences)
        {
            var equal = sentences.Count >= 2;

            for (var i = 1; i < sentences.Count && equal; i++)
            {
                equal = sentences[i - 1].getFeatureAsElement(
                    InternalFeature.VERB_PHRASE.ToString()).equals(
                    sentences[i]
                        .getFeatureAsElement(InternalFeature.VERB_PHRASE.ToString()));
            }

            return equal;
        }

        /**
         * Check that the sentences supplied have the same complements at VP level.
         * 
         * @param sentences
         *            the sentences
         * @return <code>true</code> if for every pair of sentences <code>s1</code>
         *         and <code>s2</code>, their VPs have the same pre- and
         *         post-modifiers and complements.
         */

        public static bool sameVPArgs(List<INLGElement> sentences)
        {
            var equal = sentences.Count >= 2;

            for (var i = 1; i < sentences.Count && equal; i++)
            {
                var vp1 = sentences[i - 1]
                    .getFeatureAsElement(InternalFeature.VERB_PHRASE.ToString());
                var vp2 = sentences[i]
                    .getFeatureAsElement(InternalFeature.VERB_PHRASE.ToString());

                equal = vp1
                    .getFeatureAsElementList(InternalFeature.COMPLEMENTS.ToString())
                    .equals(
                        vp2
                            .getFeatureAsElementList(InternalFeature.COMPLEMENTS.ToString()));
            }

            return equal;
        }

        /**
         * check that the phrases supplied are sentences and have the same VP
         * premodifiers and postmodifiers
         * 
         * @param sentences
         *            the sentences
         * @return <code>true</code> if all pairs of sentences have VPs with the
         *         same pre and postmodifiers
         */

        public static bool sameVPModifiers(List<INLGElement> sentences)
        {
            var equal = sentences.Count >= 2;

            for (var i = 1; i < sentences.Count && equal; i++)
            {
                var vp1 = sentences[i - 1]
                    .getFeatureAsElement(InternalFeature.VERB_PHRASE.ToString());
                var vp2 = sentences[i]
                    .getFeatureAsElement(InternalFeature.VERB_PHRASE.ToString());

                equal = vp1
                            .getFeatureAsElementList(InternalFeature.POSTMODIFIERS.ToString())
                            .equals(
                                vp2
                                    .getFeatureAsElementList(InternalFeature.POSTMODIFIERS.ToString()))
                        && vp1
                            .getFeatureAsElementList(
                                InternalFeature.PREMODIFIERS.ToString())
                            .equals(
                                vp2
                                    .getFeatureAsElementList(InternalFeature.PREMODIFIERS.ToString()));
            }

            return equal;
        }

        /**
         * Collect a list of pairs of constituents with the same syntactic function
         * from the left periphery of two sentences. The left periphery encompasses
         * the subjects, front modifiers and cue phrases of the sentences.
         * 
         * @param sentences
         *            the list of sentences
         * @return a list of pairs of constituents with the same function, if any
         *         are found
         */

        public static List<PhraseSet> leftPeriphery(List<INLGElement> sentences)
        {
            var funcsets = new List<PhraseSet>();
            var cue = new PhraseSet(DiscourseFunction.CUE_PHRASE);
            var front = new PhraseSet(DiscourseFunction.FRONT_MODIFIER);
            var subj = new PhraseSet(DiscourseFunction.SUBJECT);

            foreach (var s in sentences)
            {
                if (s.hasFeature(Feature.CUE_PHRASE.ToString()))
                {
                    cue.addPhrases(s.getFeatureAsElementList(Feature.CUE_PHRASE.ToString()));
                }

                if (s.hasFeature(InternalFeature.FRONT_MODIFIERS.ToString()))
                {
                    front
                        .addPhrases(s
                            .getFeatureAsElementList(InternalFeature.FRONT_MODIFIERS.ToString()));
                }

                if (s.hasFeature(InternalFeature.SUBJECTS.ToString()))
                {
                    subj.addPhrases(s
                        .getFeatureAsElementList(InternalFeature.SUBJECTS.ToString()));
                }
            }

            funcsets.Add(cue);
            funcsets.Add(front);
            funcsets.Add(subj);
            return funcsets;
        }

        /**
         * Collect a list of pairs of constituents with the same syntactic function
         * from the right periphery of two sentences. The right periphery
         * encompasses the complements of the main verb, and its postmodifiers.
         * 
         * @param sentences
         *            the list of sentences
         * @return a list of pairs of constituents with the same function, if any
         *         are found
         */

        public static List<PhraseSet> rightPeriphery(List<INLGElement> sentences)
        {
            var funcsets = new List<PhraseSet>();
            var comps = new PhraseSet(DiscourseFunction.OBJECT);
            // new PhraseSet(DiscourseFunction.INDIRECT_OBJECT);
            var pmods = new PhraseSet(DiscourseFunction.POST_MODIFIER);

            foreach (var s in sentences)
            {
                var vp = s.getFeatureAsElement(InternalFeature.VERB_PHRASE.ToString());

                if (vp != null)
                {
                    if (vp.hasFeature(InternalFeature.COMPLEMENTS.ToString()))
                    {
                        comps
                            .addPhrases(vp
                                .getFeatureAsElementList(InternalFeature.COMPLEMENTS.ToString()));
                    }

                    if (vp.hasFeature(InternalFeature.POSTMODIFIERS.ToString()))
                    {
                        pmods
                            .addPhrases(vp
                                .getFeatureAsElementList(InternalFeature.POSTMODIFIERS.ToString()));
                    }
                }

                if (s.hasFeature(InternalFeature.POSTMODIFIERS.ToString()))
                {
                    pmods
                        .addPhrases(s
                            .getFeatureAsElementList(InternalFeature.POSTMODIFIERS.ToString()));
                }
            }

            funcsets.Add(comps);
            funcsets.Add(pmods);
            return funcsets;
        }

        /**
         * Check that no element of a give array of sentences is passive.
         * 
         * @param sentences
         *            the sentences
         * @return <code>true</code> if none of the sentences is passive
         */

        public static bool nonePassive(List<INLGElement> sentences)
        {
            var nopass = true;

            for (var i = 0; i < sentences.Count && nopass; i++)
            {
                nopass = !sentences[i].getFeatureAsBoolean(Feature.PASSIVE.ToString());
            }

            return nopass;
        }
    }
}