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
     * <p>
     * This is a helper class containing the main methods for realising the syntax
     * of clauses. It is used exclusively by the <code>SyntaxProcessor</code>.
     * </p>
     * 
     * @author D. Westwater, University of Aberdeen.
     * @version 4.0
     * 
     */

    public abstract class ClauseHelper
    {

        /**
         * The main method for controlling the syntax realisation of clauses.
         * 
         * @param parent
         *            the parent <code>SyntaxProcessor</code> that called this
         *            method.
         * @param phrase
         *            the <code>PhraseElement</code> representation of the clause.
         * @return the <code>NLGElement</code> representing the realised clause.
         */

        public static INLGElement realise(SyntaxProcessor parent, PhraseElement phrase)
        {
            ListElement realisedElement = null;
            var phraseFactory = phrase.getFactory();
            INLGElement splitVerb = null;
            var interrogObj = false;

            if (phrase != null)
            {
                realisedElement = new ListElement();
                var verbElement = phrase.getFeatureAsElement(InternalFeature.VERB_PHRASE.ToString());

                if (verbElement == null)
                {
                    verbElement = phrase.getHead();
                }

                checkSubjectNumberPerson(phrase, verbElement);
                checkDiscourseFunction(phrase);
                copyFrontModifiers(phrase, verbElement);
                addComplementiser(phrase, parent, realisedElement);
                addCuePhrase(phrase, parent, realisedElement);

                if (phrase.hasFeature(Feature.INTERROGATIVE_TYPE.ToString()))
                {
                    var inter = phrase.getFeature(Feature.INTERROGATIVE_TYPE.ToString());
                    interrogObj = (InterrogativeType.WHAT_OBJECT.Equals(inter)
                                   || InterrogativeType.WHO_OBJECT.Equals(inter)
                                   || InterrogativeType.HOW_PREDICATE.Equals(inter) ||
                                   InterrogativeType.HOW.Equals(inter)
                                   || InterrogativeType.WHY.Equals(inter) || InterrogativeType.WHERE.Equals(inter));
                    splitVerb = realiseInterrogative(phrase, parent, realisedElement, phraseFactory, verbElement);
                }
                else
                {
                    PhraseHelper.realiseList(parent,
                        realisedElement,
                        phrase.getFeatureAsElementList(InternalFeature.FRONT_MODIFIERS.ToString()),
                        DiscourseFunction.FRONT_MODIFIER);
                }

                addSubjectsToFront(phrase, parent, realisedElement, splitVerb);

                var passiveSplitVerb = addPassiveComplementsNumberPerson(phrase,
                    parent,
                    realisedElement,
                    verbElement);

                if (passiveSplitVerb != null)
                {
                    splitVerb = passiveSplitVerb;
                }

                // realise verb needs to know if clause is object interrogative
                realiseVerb(phrase, parent, realisedElement, splitVerb, verbElement, interrogObj);
                addPassiveSubjects(phrase, parent, realisedElement, phraseFactory);
                addInterrogativeFrontModifiers(phrase, parent, realisedElement);
                addEndingTo(phrase, parent, realisedElement, phraseFactory);
            }
            return realisedElement;
        }

        /**
         * Adds <em>to</em> to the end of interrogatives concerning indirect
         * objects. For example, <em>who did John give the flower <b>to</b></em>.
         * 
         * @param phrase
         *            the <code>PhraseElement</code> representing this clause.
         * @param parent
         *            the parent <code>SyntaxProcessor</code> that will do the
         *            realisation of the complementiser.
         * @param realisedElement
         *            the current realisation of the clause.
         * @param phraseFactory
         *            the phrase factory to be used.
         */

        private static void addEndingTo(PhraseElement phrase,
            SyntaxProcessor parent,
            ListElement realisedElement,
            NLGFactory phraseFactory)
        {

            if (InterrogativeType.WHO_INDIRECT_OBJECT.Equals(phrase.getFeature(Feature.INTERROGATIVE_TYPE.ToString())))
            {
                var word = phraseFactory.createWord("to", new LexicalCategory_PREPOSITION()); 
                realisedElement.addComponent(parent.realise(word));
            }
        }

        /**
         * Adds the front modifiers to the end of the clause when dealing with
         * interrogatives.
         * 
         * @param phrase
         *            the <code>PhraseElement</code> representing this clause.
         * @param parent
         *            the parent <code>SyntaxProcessor</code> that will do the
         *            realisation of the complementiser.
         * @param realisedElement
         *            the current realisation of the clause.
         */

        private static void addInterrogativeFrontModifiers(PhraseElement phrase,
            SyntaxProcessor parent,
            ListElement realisedElement)
        {
            INLGElement currentElement = null;
            if (phrase.hasFeature(Feature.INTERROGATIVE_TYPE.ToString()))
            {
                foreach (var subject in phrase.getFeatureAsElementList(InternalFeature.FRONT_MODIFIERS.ToString()))
                {
                    currentElement = parent.realise(subject);
                    if (currentElement != null)
                    {
                        currentElement.setFeature(InternalFeature.DISCOURSE_FUNCTION.ToString(), DiscourseFunction.FRONT_MODIFIER);

                        realisedElement.addComponent(currentElement);
                    }
                }
            }
        }

        /**
         * Realises the subjects of a passive clause.
         * 
         * @param phrase
         *            the <code>PhraseElement</code> representing this clause.
         * @param parent
         *            the parent <code>SyntaxProcessor</code> that will do the
         *            realisation of the complementiser.
         * @param realisedElement
         *            the current realisation of the clause.
         * @param phraseFactory
         *            the phrase factory to be used.
         */

        private static void addPassiveSubjects(PhraseElement phrase,
            SyntaxProcessor parent,
            ListElement realisedElement,
            NLGFactory phraseFactory)
        {
            INLGElement currentElement = null;

            if (phrase.getFeatureAsBoolean(Feature.PASSIVE.ToString()))
            {
                var allSubjects = phrase.getFeatureAsElementList(InternalFeature.SUBJECTS.ToString());

                if (allSubjects.size() > 0 || phrase.hasFeature(Feature.INTERROGATIVE_TYPE.ToString()))
                {
                    realisedElement.addComponent(parent.realise(phraseFactory.createPrepositionPhrase("by")));
                        
                }

                foreach (var subject in allSubjects)
                {

                    subject.setFeature(Feature.PASSIVE.ToString(), true);
                    if (subject.isA(PhraseCategoryEnum.NOUN_PHRASE) || subject is CoordinatedPhraseElement)
                    {
                        currentElement = parent.realise(subject);
                        if (currentElement != null)
                        {
                            currentElement.setFeature(InternalFeature.DISCOURSE_FUNCTION.ToString(), DiscourseFunction.SUBJECT);
                            realisedElement.addComponent(currentElement);
                        }
                    }
                }
            }
        }

        /**
         * Realises the verb part of the clause.
         * 
         * @param phrase
         *            the <code>PhraseElement</code> representing this clause.
         * @param parent
         *            the parent <code>SyntaxProcessor</code> that will do the
         *            realisation of the complementiser.
         * @param realisedElement
         *            the current realisation of the clause.
         * @param splitVerb
         *            an <code>NLGElement</code> representing the subjects that
         *            should split the verb
         * @param verbElement
         *            the <code>NLGElement</code> representing the verb phrase for
         *            this clause.
         * @param whObj
         *            whether the VP is part of an object WH-interrogative
         */

        private static void realiseVerb(PhraseElement phrase,
            SyntaxProcessor parent,
            ListElement realisedElement,
            INLGElement splitVerb,
            INLGElement verbElement,
            bool whObj)
        {

            setVerbFeatures(phrase, verbElement);

            var currentElement = parent.realise(verbElement);
            if (currentElement != null)
            {
                if (splitVerb == null)
                {
                    currentElement.setFeature(InternalFeature.DISCOURSE_FUNCTION.ToString(), DiscourseFunction.VERB_PHRASE);

                    realisedElement.addComponent(currentElement);

                }
                else
                {
                    if (currentElement is ListElement)
                    {
                        var children = currentElement.getChildren();
                        currentElement = children[0];
                        currentElement.setFeature(InternalFeature.DISCOURSE_FUNCTION.ToString(), DiscourseFunction.VERB_PHRASE);
                        realisedElement.addComponent(currentElement);
                        realisedElement.addComponent(splitVerb);

                        for (var eachChild = 1; eachChild < children.Count; eachChild++)
                        {
                            currentElement = children[eachChild];
                            currentElement.setFeature(InternalFeature.DISCOURSE_FUNCTION.ToString(), DiscourseFunction.VERB_PHRASE);
                            realisedElement.addComponent(currentElement);
                        }
                    }
                    else
                    {
                        currentElement.setFeature(InternalFeature.DISCOURSE_FUNCTION.ToString(), DiscourseFunction.VERB_PHRASE);

                        if (whObj)
                        {
                            realisedElement.addComponent(currentElement);
                            realisedElement.addComponent(splitVerb);
                        }
                        else
                        {
                            realisedElement.addComponent(splitVerb);
                            realisedElement.addComponent(currentElement);
                        }
                    }
                }
            }
        }

        /**
         * Ensures that the verb inherits the features from the clause.
         * 
         * @param phrase
         *            the <code>PhraseElement</code> representing this clause.
         * @param verbElement
         *            the <code>NLGElement</code> representing the verb phrase for
         *            this clause.
         */

        private static void setVerbFeatures(PhraseElement phrase, INLGElement verbElement)
        {
            // this routine copies features from the clause to the VP.
            // it is disabled, as this copying is now done automatically
            // when features are set in SPhraseSpec
            // if (verbElement != null) {
            // verbElement.setFeature(Feature.INTERROGATIVE_TYPE, phrase
            // .getFeature(Feature.INTERROGATIVE_TYPE));
            // verbElement.setFeature(InternalFeature.COMPLEMENTS, phrase
            // .getFeature(InternalFeature.COMPLEMENTS));
            // verbElement.setFeature(InternalFeature.PREMODIFIERS, phrase
            // .getFeature(InternalFeature.PREMODIFIERS));
            // verbElement.setFeature(Feature.FORM, phrase
            // .getFeature(Feature.FORM));
            // verbElement.setFeature(Feature.MODAL, phrase
            // .getFeature(Feature.MODAL));
            // verbElement.setNegated(phrase.isNegated());
            // verbElement.setFeature(Feature.PASSIVE, phrase
            // .getFeature(Feature.PASSIVE));
            // verbElement.setFeature(Feature.PERFECT, phrase
            // .getFeature(Feature.PERFECT));
            // verbElement.setFeature(Feature.PROGRESSIVE, phrase
            // .getFeature(Feature.PROGRESSIVE));
            // verbElement.setTense(phrase.getTense());
            // verbElement.setFeature(Feature.FORM, phrase
            // .getFeature(Feature.FORM));
            // verbElement.setFeature(LexicalFeature.GENDER, phrase
            // .getFeature(LexicalFeature.GENDER));
            // }
        }

        /**
         * Realises the complements of passive clauses; also sets number, person for
         * passive
         * 
         * @param phrase
         *            the <code>PhraseElement</code> representing this clause.
         * @param parent
         *            the parent <code>SyntaxProcessor</code> that will do the
         *            realisation of the complementiser.
         * @param realisedElement
         *            the current realisation of the clause.
         * @param verbElement
         *            the <code>NLGElement</code> representing the verb phrase for
         *            this clause.
         */

        private static INLGElement addPassiveComplementsNumberPerson(PhraseElement phrase,
            SyntaxProcessor parent,
            ListElement realisedElement,
            INLGElement verbElement)
        {
            object passiveNumber = null;
            object passivePerson = null;
            INLGElement currentElement = null;
            INLGElement splitVerb = null;
            var verbPhrase = phrase.getFeatureAsElement(InternalFeature.VERB_PHRASE.ToString());

            // count complements to set plural feature if more than one
            var numComps = 0;
            var coordSubj = false;

            if (phrase.getFeatureAsBoolean(Feature.PASSIVE.ToString()) && verbPhrase != null
                && !InterrogativeType.WHAT_OBJECT.Equals(phrase.getFeature(Feature.INTERROGATIVE_TYPE.ToString())))
            {

                // complements of a clause are stored in the VPPhraseSpec
                foreach (var subject in verbPhrase.getFeatureAsElementList(InternalFeature.COMPLEMENTS.ToString()))
                {

                    // AG: complement needn't be an NP
                    // subject.isA(PhraseCategory.NOUN_PHRASE) &&
                    if (DiscourseFunction.OBJECT.Equals(subject.getFeature(InternalFeature.DISCOURSE_FUNCTION.ToString())))
                    {
                        subject.setFeature(Feature.PASSIVE.ToString(), true);
                        numComps++;
                        currentElement = parent.realise(subject);

                        if (currentElement != null)
                        {
                            currentElement.setFeature(InternalFeature.DISCOURSE_FUNCTION.ToString(), DiscourseFunction.OBJECT);

                            if (phrase.hasFeature(Feature.INTERROGATIVE_TYPE.ToString()))
                            {
                                splitVerb = currentElement;
                            }
                            else
                            {
                                realisedElement.addComponent(currentElement);
                            }
                        }

                        // flag if passive subject is coordinated with an "and"
                        if (!coordSubj && subject is CoordinatedPhraseElement)
                        {
                            var conj = ((CoordinatedPhraseElement) subject).getConjunction();
                            coordSubj = (conj != null && conj.Equals("and"));
                        }

                        if (passiveNumber == null)
                        {
                            passiveNumber = subject.getFeature(Feature.NUMBER.ToString());
                        }
                        else
                        {
                            passiveNumber = NumberAgreement.PLURAL;
                        }

                        if (Person.FIRST.Equals(subject.getFeature(Feature.PERSON.ToString())))
                        {
                            passivePerson = Person.FIRST;
                        }
                        else if (Person.SECOND.Equals(subject.getFeature(Feature.PERSON.ToString()))
                                 && !Person.FIRST.Equals(passivePerson))
                        {
                            passivePerson = Person.SECOND;
                        }
                        else if (passivePerson == null)
                        {
                            passivePerson = Person.THIRD;
                        }

                        if (Form.GERUND.Equals(phrase.getFeature(Feature.FORM.ToString()))
                            && !phrase.getFeatureAsBoolean(Feature.SUPPRESS_GENITIVE_IN_GERUND.ToString()))
                        {
                            subject.setFeature(Feature.POSSESSIVE.ToString(), true);
                        }
                    }
                }
            }

            if (verbElement != null)
            {
                if (passivePerson != null)
                {
                    verbElement.setFeature(Feature.PERSON.ToString(), passivePerson);
                    // below commented out. for non-passive, number and person set
                    // by checkSubjectNumberPerson
                    // } else {
                    // verbElement.setFeature(Feature.PERSON, phrase
                    // .getFeature(Feature.PERSON));
                }

                if (numComps > 1 || coordSubj)
                {
                    verbElement.setFeature(Feature.NUMBER.ToString(), NumberAgreement.PLURAL);
                }
                else if (passiveNumber != null)
                {
                    verbElement.setFeature(Feature.NUMBER.ToString(), passiveNumber);
                }
            }
            return splitVerb;
        }

        /**
         * Adds the subjects to the beginning of the clause unless the clause is
         * infinitive, imperative or passive, or the subjects split the verb.
         * 
         * @param phrase
         *            the <code>PhraseElement</code> representing this clause.
         * @param parent
         *            the parent <code>SyntaxProcessor</code> that will do the
         *            realisation of the complementiser.
         * @param realisedElement
         *            the current realisation of the clause.
         * @param splitVerb
         *            an <code>NLGElement</code> representing the subjects that
         *            should split the verb
         */

        private static void addSubjectsToFront(PhraseElement phrase,
            SyntaxProcessor parent,
            ListElement realisedElement,
            INLGElement splitVerb)
        {
            if (!Form.INFINITIVE.Equals(phrase.getFeature(Feature.FORM.ToString()))
                && !Form.IMPERATIVE.Equals(phrase.getFeature(Feature.FORM.ToString()))
                && !phrase.getFeatureAsBoolean(Feature.PASSIVE.ToString()) && splitVerb == null)
            {
                realisedElement.addComponents(realiseSubjects(phrase, parent).getChildren());
            }
        }

        /**
         * Realises the subjects for the clause.
         * 
         * @param phrase
         *            the <code>PhraseElement</code> representing this clause.
         * @param parent
         *            the parent <code>SyntaxProcessor</code> that will do the
         *            realisation of the complementiser.
         * @param realisedElement
         *            the current realisation of the clause.
         */

        private static ListElement realiseSubjects(PhraseElement phrase, SyntaxProcessor parent)
        {

            INLGElement currentElement = null;
            var realisedElement = new ListElement();

            foreach (INLGElement subject in phrase.getFeatureAsElementList(InternalFeature.SUBJECTS.ToString()))
            {

                subject.setFeature(InternalFeature.DISCOURSE_FUNCTION.ToString(), DiscourseFunction.SUBJECT);
                if (Form.GERUND.Equals(phrase.getFeature(Feature.FORM.ToString()))
                    && !phrase.getFeatureAsBoolean(Feature.SUPPRESS_GENITIVE_IN_GERUND.ToString()))
                {
                    subject.setFeature(Feature.POSSESSIVE.ToString(), true);
                }
                currentElement = parent.realise(subject);
                if (currentElement != null)
                {
                    realisedElement.addComponent(currentElement);
                }
            }
            return realisedElement;
        }

        /**
         * This is the main controlling method for handling interrogative clauses.
         * The actual steps taken are dependent on the type of question being asked.
         * The method also determines if there is a subject that will split the verb
         * group of the clause. For example, the clause
         * <em>the man <b>should give</b> the woman the flower</em> has the verb
         * group indicated in <b>bold</b>. The phrase is rearranged as yes/no
         * question as
         * <em><b>should</b> the man <b>give</b> the woman the flower</em> with the
         * subject <em>the man</em> splitting the verb group.
         * 
         * @param phrase
         *            the <code>PhraseElement</code> representing this clause.
         * @param parent
         *            the parent <code>SyntaxProcessor</code> that will do the
         *            realisation of the complementiser.
         * @param realisedElement
         *            the current realisation of the clause.
         * @param phraseFactory
         *            the phrase factory to be used.
         * @param verbElement
         *            the <code>NLGElement</code> representing the verb phrase for
         *            this clause.
         * @return an <code>NLGElement</code> representing a subject that should
         *         split the verb
         */

        private static INLGElement realiseInterrogative(PhraseElement phrase,
            SyntaxProcessor parent,
            ListElement realisedElement,
            NLGFactory phraseFactory,
            INLGElement verbElement)
        {
            INLGElement splitVerb = null;

            if (phrase.getParent() != null)
            {
                phrase.getParent().setFeature(InternalFeature.INTERROGATIVE.ToString(), true);
            }

            var type = phrase.getFeature(Feature.INTERROGATIVE_TYPE.ToString());

            if (type is InterrogativeType)
            {
                switch ((InterrogativeType) type)
                {
                    case InterrogativeType.YES_NO:
                        splitVerb = realiseYesNo(phrase, parent, verbElement, phraseFactory, realisedElement);
                        break;

                    case InterrogativeType.WHO_SUBJECT:
                    case InterrogativeType.WHAT_SUBJECT:
                        realiseInterrogativeKeyWord(((InterrogativeType) type).getString(),
                             new LexicalCategory_PRONOUN(),
                            parent,
                            realisedElement, 
                            phraseFactory);
                        phrase.removeFeature(InternalFeature.SUBJECTS.ToString());
                        break;

                    case InterrogativeType.HOW_MANY:
                        realiseInterrogativeKeyWord("how", new LexicalCategory_PRONOUN(),  parent, realisedElement,
                            
                            phraseFactory);
                        realiseInterrogativeKeyWord("many", new LexicalCategory_ADVERB(), parent, realisedElement,
                            
                            phraseFactory);
                        break;

                    case InterrogativeType.HOW:
                    case InterrogativeType.WHY:
                    case InterrogativeType.WHERE:
                    case InterrogativeType.WHO_OBJECT:
                    case InterrogativeType.WHO_INDIRECT_OBJECT:
                    case InterrogativeType.WHAT_OBJECT:
                        splitVerb = realiseObjectWHInterrogative(((InterrogativeType) type).getString(),
                            phrase,
                            parent,
                            realisedElement,
                            phraseFactory);
                        break;

                    case InterrogativeType.HOW_PREDICATE:
                        splitVerb = realiseObjectWHInterrogative("how", phrase, parent, realisedElement, phraseFactory);
                        break;

                    default:
                        break;
                }
            }

            return splitVerb;
        }

        /*
         * Check if a sentence has an auxiliary (needed to relise questions
         * correctly)
         */

        private static bool hasAuxiliary(PhraseElement phrase)
        {
            return phrase.hasFeature(Feature.MODAL.ToString()) || phrase.getFeatureAsBoolean(Feature.PERFECT.ToString())
                   || phrase.getFeatureAsBoolean(Feature.PROGRESSIVE.ToString())
                   || Tense.FUTURE.Equals(phrase.getFeatureTense(Feature.TENSE.ToString()));
        }

        /**
         * Controls the realisation of <em>wh</em> object questions.
         * 
         * @param keyword
         *            the wh word
         * @param phrase
         *            the <code>PhraseElement</code> representing this clause.
         * @param parent
         *            the parent <code>SyntaxProcessor</code> that will do the
         *            realisation of the complementiser.
         * @param realisedElement
         *            the current realisation of the clause.
         * @param phraseFactory
         *            the phrase factory to be used.
         * @param subjects
         *            the <code>List</code> of subjects in the clause.
         * @return an <code>NLGElement</code> representing a subject that should
         *         split the verb
         */

        private static INLGElement realiseObjectWHInterrogative(string keyword,
            PhraseElement phrase,
            SyntaxProcessor parent,
            ListElement realisedElement,
            NLGFactory phraseFactory)
        {
            INLGElement splitVerb = null;
            realiseInterrogativeKeyWord(keyword, new LexicalCategory_PRONOUN(), parent, realisedElement, 
                phraseFactory);

            // if (!Tense.FUTURE.Equals(phrase.getFeature(Feature.TENSE)) &&
            // !copular) {
            if (!hasAuxiliary(phrase) && !VerbPhraseHelper.isCopular(phrase))
            {
                addDoAuxiliary(phrase, parent, phraseFactory, realisedElement);

            }
            else if (!phrase.getFeatureAsBoolean(Feature.PASSIVE.ToString()))
            {
                splitVerb = realiseSubjects(phrase, parent);
            }

            return splitVerb;
        }

        /**
         * Adds a <em>do</em> verb to the realisation of this clause.
         * 
         * @param phrase
         *            the <code>PhraseElement</code> representing this clause.
         * @param parent
         *            the parent <code>SyntaxProcessor</code> that will do the
         *            realisation of the complementiser.
         * @param realisedElement
         *            the current realisation of the clause.
         * @param phraseFactory
         *            the phrase factory to be used.
         */

        private static void addDoAuxiliary(PhraseElement phrase,
            SyntaxProcessor parent,
            NLGFactory phraseFactory,
            ListElement realisedElement)
        {

            PhraseElement doPhrase = phraseFactory.createVerbPhrase("do"); 
            doPhrase.setFeature(Feature.TENSE.ToString(), phrase.getFeatureTense(Feature.TENSE.ToString()));
            doPhrase.setFeature(Feature.PERSON.ToString(), phrase.getFeature(Feature.PERSON.ToString()));
            doPhrase.setFeature(Feature.NUMBER.ToString(), phrase.getFeature(Feature.NUMBER.ToString()));
            realisedElement.addComponent(parent.realise(doPhrase));
        }

        /**
         * Realises the key word of the interrogative. For example, <em>who</em>,
         * <em>what</em>
         * 
         * @param keyWord
         *            the key word of the interrogative.
         * @param cat
         *            the category (usually pronoun, but not in the case of
         *            "how many")
         * @param parent
         *            the parent <code>SyntaxProcessor</code> that will do the
         *            realisation of the complementiser.
         * @param realisedElement
         *            the current realisation of the clause.
         * @param phraseFactory
         *            the phrase factory to be used.
         */

        private static void realiseInterrogativeKeyWord(string keyWord,
            ILexicalCategory cat,
            SyntaxProcessor parent,
            ListElement realisedElement,
            NLGFactory phraseFactory)
        {

            if (keyWord != null)
            {
                var question = phraseFactory.createWord(keyWord, cat);
                var currentElement = parent.realise(question);

                if (currentElement != null)
                {
                    realisedElement.addComponent(currentElement);
                }
            }
        }

        /**
         * Performs the realisation for YES/NO types of questions. This may involve
         * adding an optional <em>do</em> auxiliary verb to the beginning of the
         * clause. The method also determines if there is a subject that will split
         * the verb group of the clause. For example, the clause
         * <em>the man <b>should give</b> the woman the flower</em> has the verb
         * group indicated in <b>bold</b>. The phrase is rearranged as yes/no
         * question as
         * <em><b>should</b> the man <b>give</b> the woman the flower</em> with the
         * subject <em>the man</em> splitting the verb group.
         * 
         * 
         * @param phrase
         *            the <code>PhraseElement</code> representing this clause.
         * @param parent
         *            the parent <code>SyntaxProcessor</code> that will do the
         *            realisation of the complementiser.
         * @param realisedElement
         *            the current realisation of the clause.
         * @param phraseFactory
         *            the phrase factory to be used.
         * @param verbElement
         *            the <code>NLGElement</code> representing the verb phrase for
         *            this clause.
         * @param subjects
         *            the <code>List</code> of subjects in the clause.
         * @return an <code>NLGElement</code> representing a subject that should
         *         split the verb
         */

        private static INLGElement realiseYesNo(PhraseElement phrase,
            SyntaxProcessor parent,
            INLGElement verbElement,
            NLGFactory phraseFactory,
            ListElement realisedElement)
        {

            INLGElement splitVerb = null;

            if (!(verbElement is VPPhraseSpec && VerbPhraseHelper.isCopular(((VPPhraseSpec) verbElement).getVerb()))
                && !phrase.getFeatureAsBoolean(Feature.PROGRESSIVE.ToString()) && !phrase.hasFeature(Feature.MODAL.ToString())
                && !Tense.FUTURE.Equals(phrase.getFeatureTense(Feature.TENSE.ToString()))
                && !phrase.getFeatureAsBoolean(Feature.NEGATED.ToString())
                && !phrase.getFeatureAsBoolean(Feature.PASSIVE.ToString()))
            {
                addDoAuxiliary(phrase, parent, phraseFactory, realisedElement);
            }
            else
            {
                splitVerb = realiseSubjects(phrase, parent);
            }
            return splitVerb;
        }

        /**
         * Realises the cue phrase for the clause if it exists.
         * 
         * @param phrase
         *            the <code>PhraseElement</code> representing this clause.
         * @param parent
         *            the parent <code>SyntaxProcessor</code> that will do the
         *            realisation of the complementiser.
         * @param realisedElement
         *            the current realisation of the clause.
         */

        private static void addCuePhrase(PhraseElement phrase, SyntaxProcessor parent, ListElement realisedElement)
        {

            var currentElement = parent.realise(phrase.getFeatureAsElement(Feature.CUE_PHRASE.ToString()));

            if (currentElement != null)
            {
                currentElement.setFeature(InternalFeature.DISCOURSE_FUNCTION.ToString(), DiscourseFunction.CUE_PHRASE);
                realisedElement.addComponent(currentElement);
            }
        }

        /**
         * Checks to see if this clause is a subordinate clause. If it is then the
         * complementiser is added as a component to the realised element
         * <b>unless</b> the complementiser has been suppressed.
         * 
         * @param phrase
         *            the <code>PhraseElement</code> representing this clause.
         * @param parent
         *            the parent <code>SyntaxProcessor</code> that will do the
         *            realisation of the complementiser.
         * @param realisedElement
         *            the current realisation of the clause.
         */

        private static void addComplementiser(PhraseElement phrase, SyntaxProcessor parent, ListElement realisedElement)
        {

            INLGElement currentElement;

            if (ClauseStatus.SUBORDINATE.Equals(phrase.getFeature(InternalFeature.CLAUSE_STATUS.ToString()))
                && !phrase.getFeatureAsBoolean(Feature.SUPRESSED_COMPLEMENTISER.ToString()))
            {

                currentElement = parent.realise(phrase.getFeatureAsElement(Feature.COMPLEMENTISER.ToString()));

                if (currentElement != null)
                {
                    realisedElement.addComponent(currentElement);
                }
            }
        }

        /**
         * Copies the front modifiers of the clause to the list of post-modifiers of
         * the verb only if the phrase has infinitive form.
         * 
         * @param phrase
         *            the <code>PhraseElement</code> representing this clause.
         * @param verbElement
         *            the <code>NLGElement</code> representing the verb phrase for
         *            this clause.
         */

        private static void copyFrontModifiers(PhraseElement phrase, INLGElement verbElement)
        {
            var frontModifiers = phrase.getFeatureAsElementList(InternalFeature.FRONT_MODIFIERS.ToString());
            var clauseForm = phrase.getFeature(Feature.FORM.ToString());

            // bug fix by Chris Howell (Agfa) -- do not overwrite existing post-mods
            // in the VP
            if (verbElement != null)
            {
                List<INLGElement> phrasePostModifiers = phrase.getFeatureAsElementList(InternalFeature.POSTMODIFIERS.ToString());

                if (verbElement is PhraseElement)
                {
                    List<INLGElement> verbPostModifiers =
                        verbElement.getFeatureAsElementList(InternalFeature.POSTMODIFIERS.ToString());

                    foreach (var eachModifier in phrasePostModifiers)
                    {

                        // need to check that VP doesn't already contain the
                        // post-modifier
                        // this only happens if the phrase has already been realised
                        // and later modified, with realiser called again. In that
                        // case, postmods will be copied over twice
                        if (!verbPostModifiers.Contains(eachModifier))
                        {
                            ((PhraseElement) verbElement).addPostModifier(eachModifier);
                        }
                    }
                }
            }

            // if (verbElement != null) {
            // verbElement.setFeature(InternalFeature.POSTMODIFIERS, phrase
            // .getFeature(InternalFeature.POSTMODIFIERS));
            // }

            if (Form.INFINITIVE.Equals(clauseForm))
            {
                phrase.setFeature(Feature.SUPRESSED_COMPLEMENTISER.ToString(), true);

                foreach (var eachModifier in frontModifiers)
                {
                    if (verbElement is PhraseElement)
                    {
                        ((PhraseElement) verbElement).addPostModifier(eachModifier);
                    }
                }
                phrase.removeFeature(InternalFeature.FRONT_MODIFIERS.ToString());
                if (verbElement != null)
                {
                    verbElement.setFeature(InternalFeature.NON_MORPH.ToString(), true);
                }
            }
        }

        /**
         * Checks the discourse function of the clause and alters the form of the
         * clause as necessary. The following algorithm is used: <br>
         * 
         * <pre>
         * If the clause represents a direct or indirect object then 
         *      If form is currently Imperative then
         *           Set form to Infinitive
         *           Suppress the complementiser
         *      If form is currently Gerund and there are no subjects
         *      	 Suppress the complementiser
         * If the clause represents a subject then
         *      Set the form to be Gerund
         *      Suppress the complementiser
         * </pre>
         * 
         * @param phrase
         *            the <code>PhraseElement</code> representing this clause.
         */

        private static void checkDiscourseFunction(PhraseElement phrase)
        {
            List<INLGElement> subjects = phrase.getFeatureAsElementList(InternalFeature.SUBJECTS.ToString());
            var clauseForm = phrase.getFeature(Feature.FORM.ToString());
            var discourseValue = phrase.getFeature(InternalFeature.DISCOURSE_FUNCTION.ToString());

            if (DiscourseFunction.OBJECT.Equals(discourseValue) ||
                DiscourseFunction.INDIRECT_OBJECT.Equals(discourseValue))
            {

                if (Form.IMPERATIVE.Equals(clauseForm))
                {
                    phrase.setFeature(Feature.SUPRESSED_COMPLEMENTISER.ToString(), true);
                    phrase.setFeature(Feature.FORM.ToString(), Form.INFINITIVE.ToString());
                }
                else if (Form.GERUND.Equals(clauseForm) && subjects.Count == 0)
                {
                    phrase.setFeature(Feature.SUPRESSED_COMPLEMENTISER.ToString(), true);
                }
            }
            else if (DiscourseFunction.SUBJECT.Equals(discourseValue))
            {
                phrase.setFeature(Feature.FORM.ToString(), Form.GERUND);
                phrase.setFeature(Feature.SUPRESSED_COMPLEMENTISER.ToString(), true);
            }
        }

        /**
         * Checks the subjects of the phrase to determine if there is more than one
         * subject. This ensures that the verb phrase is correctly set. Also set
         * person correctly
         * 
         * @param phrase
         *            the <code>PhraseElement</code> representing this clause.
         * @param verbElement
         *            the <code>NLGElement</code> representing the verb phrase for
         *            this clause.
         */

        private static void checkSubjectNumberPerson(PhraseElement phrase, INLGElement verbElement)
        {
            INLGElement currentElement = null;
            List<INLGElement> subjects = phrase.getFeatureAsElementList(InternalFeature.SUBJECTS.ToString());
            var pluralSubjects = false;
            var person = Person.FIRST;
            var personSet = false;

            if (subjects != null)
            {
                switch (subjects.size())
                {
                    case 0:
                        break;

                    case 1:
                        currentElement = subjects.get(0);
                        // coordinated NP with "and" are plural (not coordinated NP with
                        // "or")
                        if (currentElement is CoordinatedPhraseElement
                            && ((CoordinatedPhraseElement) currentElement).checkIfPlural())
                            pluralSubjects = true;
                        else if ((currentElement.getFeature(Feature.NUMBER.ToString())?.ToString() == NumberAgreement.PLURAL.ToString())
                                 && !(currentElement is SPhraseSpec)) // ER mod-
                            // clauses
                            // are
                            // singular
                            // as
                            // NPs,
                            // even
                            // if
                            // they
                            // are
                            // plural
                            // internally
                            pluralSubjects = true;
                        else if (currentElement.isA(PhraseCategoryEnum.NOUN_PHRASE))
                        {
                            INLGElement currentHead = currentElement.getFeatureAsElement(InternalFeature.HEAD.ToString());

                            var p = currentElement.getFeature(Feature.PERSON.ToString());
                            if (p != null)
                            {
                                personSet = true;
                                person = p.ToPerson();
                            }

                            if (currentHead == null)
                            {
                                // subject is null and therefore is not gonna be plural
                                pluralSubjects = false;
                            }
                            else if (currentHead.getFeature(Feature.NUMBER.ToString()) == NumberAgreement.PLURAL.ToString())
                                pluralSubjects = true;
                            else if (currentHead is ListElement)
                            {
                                pluralSubjects = true;
                                /*
                                 * } else if (currentElement is
                                 * CoordinatedPhraseElement &&
                                 * "and".Equals(currentElement.getFeatureAsString(
                                 *  Feature.CONJUNCTION))) { pluralSubjects
                                 * = true;
                                 */
                            }
                        }
                        break;

                    default:
                        pluralSubjects = true;
                        break;
                }
            }
            if (verbElement != null)
            {
                verbElement.setFeature(Feature.NUMBER.ToString(), pluralSubjects
                    ? NumberAgreement.PLURAL
                    : phrase.getFeature(Feature.NUMBER.ToString()));    
                if (personSet)    
                    verbElement.setFeature(Feature.PERSON.ToString(), person);
            }
        }
    }
}