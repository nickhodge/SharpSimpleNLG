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
using System.Linq;
using SimpleNLG.Extensions;

namespace SimpleNLG
{
    public abstract class VerbPhraseHelper
    {

        /**
         * The main method for realising verb phrases.
         * 
         * @param parent
         *            the <code>SyntaxProcessor</code> that called this method.
         * @param phrase
         *            the <code>PhraseElement</code> to be realised.
         * @return the realised <code>NLGElement</code>.
         */

        public static INLGElement realise(SyntaxProcessor parent, PhraseElement phrase)
        {
            ListElement realisedElement = null;
            var mainVerbRealisation = new Stack<INLGElement>();
            var auxiliaryRealisation = new Stack<INLGElement>();

            if (phrase != null)
            {
                var vgComponents = createVerbGroup(parent, phrase);
                splitVerbGroup(vgComponents, mainVerbRealisation,
                    auxiliaryRealisation);

                realisedElement = new ListElement();

                if (!phrase.hasFeature(InternalFeature.REALISE_AUXILIARY.ToString())
                    || phrase.getFeatureAsBoolean(
                        InternalFeature.REALISE_AUXILIARY.ToString()))
                {

                    realiseAuxiliaries(parent, realisedElement,
                        auxiliaryRealisation);

                    PhraseHelper.realiseList(parent, realisedElement, phrase
                        .getPreModifiers(), DiscourseFunction.PRE_MODIFIER);

                    realiseMainVerb(parent, phrase, mainVerbRealisation,
                        realisedElement);

                }
                else if (isCopular(phrase.getHead()))
                {
                    realiseMainVerb(parent, phrase, mainVerbRealisation,
                        realisedElement);
                    PhraseHelper.realiseList(parent, realisedElement, phrase
                        .getPreModifiers(), DiscourseFunction.PRE_MODIFIER);

                }
                else
                {
                    PhraseHelper.realiseList(parent, realisedElement, phrase
                        .getPreModifiers(), DiscourseFunction.PRE_MODIFIER);
                    realiseMainVerb(parent, phrase, mainVerbRealisation,
                        realisedElement);
                }
                realiseComplements(parent, phrase, realisedElement);
                PhraseHelper.realiseList(parent, realisedElement, phrase
                    .getPostModifiers(), DiscourseFunction.POST_MODIFIER);
            }

            return realisedElement;
        }

        /**
         * Realises the auxiliary verbs in the verb group.
         * 
         * @param parent
         *            the parent <code>SyntaxProcessor</code> that will do the
         *            realisation of the complementiser.
         * @param realisedElement
         *            the current realisation of the noun phrase.
         * @param auxiliaryRealisation
         *            the stack of auxiliary verbs.
         */

        private static void realiseAuxiliaries(SyntaxProcessor parent,
            ListElement realisedElement, Stack<INLGElement> auxiliaryRealisation)
        {
            while (!auxiliaryRealisation.isEmpty())
            {
                var aux = auxiliaryRealisation.pop();
                var currentElement = parent.realise(aux);
                if (currentElement != null)
                {
                    realisedElement.addComponent(currentElement);
                    currentElement.setFeature(InternalFeature.DISCOURSE_FUNCTION.ToString(),
                        DiscourseFunction.AUXILIARY);
                }
            }
        }

        /**
         * Realises the main group of verbs in the phrase.
         * 
         * @param parent
         *            the parent <code>SyntaxProcessor</code> that will do the
         *            realisation of the complementiser.
         * @param phrase
         *            the <code>PhraseElement</code> representing this noun phrase.
         * @param mainVerbRealisation
         *            the stack of the main verbs in the phrase.
         * @param realisedElement
         *            the current realisation of the noun phrase.
         */

        private static void realiseMainVerb(SyntaxProcessor parent,
            PhraseElement phrase, Stack<INLGElement> mainVerbRealisation,
            ListElement realisedElement)
        {
            while (!mainVerbRealisation.isEmpty())
            {
                var main = mainVerbRealisation.pop();
                main.setFeature(Feature.INTERROGATIVE_TYPE.ToString(), phrase
                    .getFeature(Feature.INTERROGATIVE_TYPE.ToString()));
                var currentElement = parent.realise(main);

                if (currentElement != null)
                {
                    realisedElement.addComponent(currentElement);
                }
            }
        }

        /**
         * Realises the complements of this phrase.
         * 
         * @param parent
         *            the parent <code>SyntaxProcessor</code> that will do the
         *            realisation of the complementiser.
         * @param phrase
         *            the <code>PhraseElement</code> representing this noun phrase.
         * @param realisedElement
         *            the current realisation of the noun phrase.
         */

        private static void realiseComplements(SyntaxProcessor parent,
            PhraseElement phrase, ListElement realisedElement)
        {

            var indirects = new ListElement();
            var directs = new ListElement();
            var unknowns = new ListElement();

            foreach (INLGElement complement in phrase.getFeatureAsElementList(InternalFeature.COMPLEMENTS.ToString()))
            {

                var discourseValue = complement
                    .getFeature(InternalFeature.DISCOURSE_FUNCTION.ToString());
                var currentElement = parent.realise(complement);
                if (currentElement != null)
                {
                    currentElement.setFeature(InternalFeature.DISCOURSE_FUNCTION.ToString(),
                        DiscourseFunction.COMPLEMENT);

                    if (DiscourseFunction.INDIRECT_OBJECT.Equals(discourseValue))
                    {
                        indirects.addComponent(currentElement);
                    }
                    else if (DiscourseFunction.OBJECT.Equals(discourseValue))
                    {
                        directs.addComponent(currentElement);
                    }
                    else
                    {
                        unknowns.addComponent(currentElement);
                    }
                }
            }
            if (!InterrogativeTypeExtensions.isIndirectObject(phrase
                .getFeature(Feature.INTERROGATIVE_TYPE.ToString())))
            {
                realisedElement.addComponents(indirects.getChildren());
            }
            if (!phrase.getFeatureAsBoolean(Feature.PASSIVE.ToString()))
            {
                if (!InterrogativeTypeExtensions.isAndObject(phrase
                    .getFeature(Feature.INTERROGATIVE_TYPE.ToString())))
                {
                    realisedElement.addComponents(directs.getChildren());
                }
                realisedElement.addComponents(unknowns.getChildren());
            }
        }

        /**
         * Splits the stack of verb components into two sections. One being the verb
         * associated with the main verb group, the other being associated with the
         * auxiliary verb group.
         * 
         * @param vgComponents
         *            the stack of verb components in the verb group.
         * @param mainVerbRealisation
         *            the main group of verbs.
         * @param auxiliaryRealisation
         *            the auxiliary group of verbs.
         */

        private static void splitVerbGroup(Stack<INLGElement> vgComponents,
            Stack<INLGElement> mainVerbRealisation,
            Stack<INLGElement> auxiliaryRealisation)
        {

            var mainVerbSeen = false;

            foreach (var word in vgComponents.Reverse())
            {
                if (!mainVerbSeen)
                {
                    mainVerbRealisation.push(word);
                    if (!word.Equals("not"))
                    {
                        
                        mainVerbSeen = true;
                    }
                }
                else
                {
                    auxiliaryRealisation.push(word);
                }
            }

        }

        /**
         * Creates a stack of verbs for the verb phrase. Additional auxiliary verbs
         * are added as required based on the features of the verb phrase.
         * 
         * @param parent
         *            the parent <code>SyntaxProcessor</code> that will do the
         *            realisation of the complementiser.
         * @param phrase
         *            the <code>PhraseElement</code> representing this noun phrase.
         * @return the verb group as a <code>Stack</code> of <code>NLGElement</code>
         *         s.
         */
  
        public static Stack<INLGElement> createVerbGroup(
            SyntaxProcessor parent, PhraseElement phrase)
        {

            string actualModal = null;
            var formValue = phrase.getFeature(Feature.FORM.ToString());
            Tense tenseValue = phrase.getFeatureTense(Feature.TENSE.ToString());
            var modal = phrase.getFeatureAsString(Feature.MODAL.ToString());
            var modalPast = false;
            var vgComponents = new Stack<INLGElement>();
            var interrogative = phrase.hasFeature(Feature.INTERROGATIVE_TYPE.ToString());

            if (Form.GERUND.Equals(formValue) || Form.INFINITIVE.Equals(formValue))
            {
                tenseValue = Tense.PRESENT;
            }

            if (Form.INFINITIVE.Equals(formValue))
            {
                actualModal = "to"; 

            }
            else if (formValue == null || Form.NORMAL.Equals(formValue))
            {
                if (Tense.FUTURE.Equals(tenseValue)
                    && modal == null
                    && ((!(phrase.getHead() is CoordinatedPhraseElement)) || (phrase
                                                                                  .getHead() is CoordinatedPhraseElement &&
                                                                              interrogative)))
                {

                    actualModal = "will"; 

                }
                else if (modal != null)
                {
                    actualModal = modal;

                    if (Tense.PAST.Equals(tenseValue))
                    {
                        modalPast = true;
                    }
                }
            }

            pushParticles(phrase, parent, vgComponents);
            var frontVG = grabHeadVerb(phrase, tenseValue, modal != null);
            checkImperativeInfinitive(formValue, frontVG);

            if (phrase.getFeatureAsBoolean(Feature.PASSIVE.ToString()))
            {
                frontVG = addBe(frontVG, vgComponents, Form.PAST_PARTICIPLE);
            }

            if (phrase.getFeatureAsBoolean(Feature.PROGRESSIVE.ToString()))
            {
                frontVG = addBe(frontVG, vgComponents, Form.PRESENT_PARTICIPLE);
            }

            if (phrase.getFeatureAsBoolean(Feature.PERFECT.ToString())
                || modalPast)
            {
                frontVG = addHave(frontVG, vgComponents, modal, tenseValue);
            }

            frontVG = pushIfModal(actualModal != null, phrase, frontVG,
                vgComponents);
            frontVG = createNot(phrase, vgComponents, frontVG, modal != null);

            if (frontVG != null)
            {
                pushFrontVerb(phrase, vgComponents, frontVG, formValue,
                    interrogative);
            }

            pushModal(actualModal, phrase, vgComponents);
            return vgComponents;
        }

        /**
         * Pushes the modal onto the stack of verb components.
         * 
         * @param actualModal
         *            the modal to be used.
         * @param phrase
         *            the <code>PhraseElement</code> representing this noun phrase.
         * @param vgComponents
         *            the stack of verb components in the verb group.
         */

        private static void pushModal(string actualModal, PhraseElement phrase,
            Stack<INLGElement> vgComponents)
        {
            if (actualModal != null
                && !phrase.getFeatureAsBoolean(InternalFeature.IGNORE_MODAL.ToString())
                    )
            {
                vgComponents.push(new InflectedWordElement(actualModal,
                     new LexicalCategory_MODAL()));
            }
        }

        /**
         * Pushes the front verb onto the stack of verb components.
         * 
         * @param phrase
         *            the <code>PhraseElement</code> representing this noun phrase.
         * @param vgComponents
         *            the stack of verb components in the verb group.
         * @param frontVG
         *            the first verb in the verb group.
         * @param formValue
         *            the <code>Form</code> of the phrase.
         * @param interrogative
         *            <code>true</code> if the phrase is interrogative.
         */

        private static void pushFrontVerb(PhraseElement phrase,
            Stack<INLGElement> vgComponents, INLGElement frontVG,
            object formValue, bool interrogative)
        {
            var interrogType = phrase.getFeature(Feature.INTERROGATIVE_TYPE.ToString());

            if (Form.GERUND.Equals(formValue))
            {
                frontVG.setFeature(Feature.FORM.ToString(), Form.PRESENT_PARTICIPLE);
                vgComponents.push(frontVG);

            }
            else if (Form.PAST_PARTICIPLE.Equals(formValue))
            {
                frontVG.setFeature(Feature.FORM.ToString(), Form.PAST_PARTICIPLE);
                vgComponents.push(frontVG);

            }
            else if (Form.PRESENT_PARTICIPLE.Equals(formValue))
            {
                frontVG.setFeature(Feature.FORM.ToString(), Form.PRESENT_PARTICIPLE);
                vgComponents.push(frontVG);

            }
            else if ((!(formValue == null || Form.NORMAL.Equals(formValue)) || interrogative)
                     && !isCopular(phrase.getHead()) && vgComponents.isEmpty())
            {

                // AG: fix below: if interrogative, only set non-morph feature in
                // case it's not WHO_SUBJECT OR WHAT_SUBJECT			
                if (!(InterrogativeType.WHO_SUBJECT.Equals(interrogType) || InterrogativeType.WHAT_SUBJECT
                          .Equals(interrogType)))
                {
                    frontVG.setFeature(InternalFeature.NON_MORPH.ToString(), true);
                }

                vgComponents.push(frontVG);

            }
            else
            {
                var numToUse = determineNumber(phrase.getParent(),
                    phrase);
                frontVG.setFeature(Feature.TENSE.ToString(), phrase.getFeatureTense(Feature.TENSE.ToString()));
                frontVG.setFeature(Feature.PERSON.ToString(), phrase
                    .getFeature(Feature.PERSON.ToString()));
                frontVG.setFeature(Feature.NUMBER.ToString(), numToUse);

                //don't push the front VG if it's a negated interrogative WH object question
                if (!(phrase.getFeatureAsBoolean(Feature.NEGATED.ToString()) && (InterrogativeType.WHO_OBJECT
                                                                                         .Equals(interrogType) ||
                                                                                     InterrogativeType.WHAT_OBJECT
                                                                                         .Equals(interrogType))))
                {
                    vgComponents.push(frontVG);
                }
            }
        }

        /**
         * Adds <em>not</em> to the stack if the phrase is negated.
         * 
         * @param phrase
         *            the <code>PhraseElement</code> representing this noun phrase.
         * @param vgComponents
         *            the stack of verb components in the verb group.
         * @param frontVG
         *            the first verb in the verb group.
         * @param hasModal
         *            the phrase has a modal
         * @return the new element for the front of the group.
         */
         
        private static INLGElement createNot(PhraseElement phrase,
            Stack<INLGElement> vgComponents, INLGElement frontVG, bool hasModal)
        {
            var newFront = frontVG;

            if (phrase.getFeatureAsBoolean(Feature.NEGATED.ToString()))
            {
                var factory = phrase.getFactory();

                // before adding "do", check if this is an object WH
                // interrogative
                // in which case, don't add anything as it's already done by
                // ClauseHelper
                var interrType = phrase.getFeature(Feature.INTERROGATIVE_TYPE.ToString());
                var addDo = !(InterrogativeType.WHAT_OBJECT.Equals(interrType) || InterrogativeType.WHO_OBJECT
                                   .Equals(interrType));

                if (!vgComponents.empty() || frontVG != null && isCopular(frontVG))
                {
                    vgComponents.push(new InflectedWordElement(
                        "not", new LexicalCategory_ADVERB())); 
                }
                else
                {
                    if (frontVG != null && !hasModal)
                    {
                        frontVG.setFeature(Feature.NEGATED.ToString(), true);
                        vgComponents.push(frontVG);
                    }

                    vgComponents.push(new InflectedWordElement(
                        "not", new LexicalCategory_ADVERB())); 

                    if (addDo)
                    {
                        if (factory != null)
                        {
                            newFront = factory.createInflectedWord("do",
                                new LexicalCategory_VERB());

                        }
                        else
                        {
                            newFront = new InflectedWordElement(
                                "do", new LexicalCategory_VERB()); 
                        }
                    }
                }
            }

            return newFront;
        }

        /**
         * Pushes the front verb on to the stack if the phrase has a modal.
         * 
         * @param hasModal
         *            the phrase has a modal
         * @param phrase
         *            the <code>PhraseElement</code> representing this noun phrase.
         * @param frontVG
         *            the first verb in the verb group.
         * @param vgComponents
         *            the stack of verb components in the verb group.
         * @return the new element for the front of the group.
         */

        private static INLGElement pushIfModal(bool hasModal,
            PhraseElement phrase, INLGElement frontVG,
            Stack<INLGElement> vgComponents)
        {

            var newFront = frontVG;
            if (hasModal
                && !phrase.getFeatureAsBoolean(InternalFeature.IGNORE_MODAL.ToString())
                    )
            {
                if (frontVG != null)
                {
                    frontVG.setFeature(InternalFeature.NON_MORPH.ToString(), true);
                    vgComponents.push(frontVG);
                }
                newFront = null;
            }
            return newFront;
        }

        /**
         * Adds <em>have</em> to the stack.
         * 
         * @param frontVG
         *            the first verb in the verb group.
         * @param vgComponents
         *            the stack of verb components in the verb group.
         * @param modal
         *            the modal to be used.
         * @param tenseValue
         *            the <code>Tense</code> of the phrase.
         * @return the new element for the front of the group.
         */

        private static INLGElement addHave(INLGElement frontVG,
            Stack<INLGElement> vgComponents, string modal, Tense tenseValue)
        {
            var newFront = frontVG;

            if (frontVG != null)
            {
                frontVG.setFeature(Feature.FORM.ToString(), Form.PAST_PARTICIPLE);
                vgComponents.push(frontVG);
            }
            newFront = new InflectedWordElement("have", new LexicalCategory_VERB()); 
            newFront.setFeature(Feature.TENSE.ToString(), tenseValue);
            if (modal != null)
            {
                newFront.setFeature(InternalFeature.NON_MORPH.ToString(), true);
            }
            return newFront;
        }

        /**
         * Adds the <em>be</em> verb to the front of the group.
         * 
         * @param frontVG
         *            the first verb in the verb group.
         * @param vgComponents
         *            the stack of verb components in the verb group.
         * @param frontForm
         *            the form the current front verb is to take.
         * @return the new element for the front of the group.
         */

        private static INLGElement addBe(INLGElement frontVG,
            Stack<INLGElement> vgComponents, Form frontForm)
        {

            if (frontVG != null)
            {
                frontVG.setFeature(Feature.FORM.ToString(), frontForm);
                vgComponents.push(frontVG);
            }
            return new InflectedWordElement("be", new LexicalCategory_VERB()); 
        }

        /**
         * Checks to see if the phrase is in imperative, infinitive or bare
         * infinitive form. If it is then no morphology is done on the main verb.
         * 
         * @param formValue
         *            the <code>Form</code> of the phrase.
         * @param frontVG
         *            the first verb in the verb group.
         */

        private static void checkImperativeInfinitive(object formValue,
            INLGElement frontVG)
        {

            if ((Form.IMPERATIVE.Equals(formValue)
                 || Form.INFINITIVE.Equals(formValue) || Form.BARE_INFINITIVE
                     .Equals(formValue))
                && frontVG != null)
            {
                frontVG.setFeature(InternalFeature.NON_MORPH.ToString(), true);
            }
        }

        /**
         * Grabs the head verb of the verb phrase and sets it to future tense if the
         * phrase is future tense. It also turns off negation if the group has a
         * modal.
         * 
         * @param phrase
         *            the <code>PhraseElement</code> representing this noun phrase.
         * @param tenseValue
         *            the <code>Tense</code> of the phrase.
         * @param hasModal
         *            <code>true</code> if the verb phrase has a modal.
         * @return the modified head element
         */

        private static INLGElement grabHeadVerb(PhraseElement phrase,
            Tense tenseValue, bool hasModal)
        {
            INLGElement frontVG = phrase.getHead();

            if (frontVG != null)
            {
                if (frontVG is WordElement)
                {
                    frontVG = new InflectedWordElement((WordElement) frontVG);
                }


                frontVG.setFeature(Feature.TENSE.ToString(), tenseValue);

                // if (Tense.FUTURE.Equals(tenseValue) && frontVG != null) {
                // frontVG.setFeature(Feature.TENSE, Tense.FUTURE);
                // }

                if (hasModal)
                {
                    frontVG.setFeature(Feature.NEGATED.ToString(), false);
                }
            }

            return frontVG;
        }

        /**
         * Pushes the particles of the main verb onto the verb group stack.
         * 
         * @param phrase
         *            the <code>PhraseElement</code> representing this noun phrase.
         * @param parent
         *            the parent <code>SyntaxProcessor</code> that will do the
         *            realisation of the complementiser.
         * @param vgComponents
         *            the stack of verb components in the verb group.
         */

        private static void pushParticles(PhraseElement phrase,
            SyntaxProcessor parent, Stack<INLGElement> vgComponents)
        {
            var particle = phrase.getFeature(Feature.PARTICLE.ToString());

            if (particle is string)
            {
                vgComponents.push(new StringElement((string) particle));

            }
            else if (particle is INLGElement)
            {
                vgComponents.push(parent.realise((INLGElement) particle));
            }
        }

        /**
         * Determines the number agreement for the phrase ensuring that any number
         * agreement on the parent element is inherited by the phrase.
         * 
         * @param parent
         *            the parent element of the phrase.
         * @param phrase
         *            the <code>PhraseElement</code> representing this noun phrase.
         * @return the <code>NumberAgreement</code> to be used for the phrase.
         */

        private static NumberAgreement determineNumber(INLGElement parent,
            PhraseElement phrase)
        {
            var numberValue = phrase.getFeature(Feature.NUMBER.ToString());
            NumberAgreement number;
            if (numberValue != null && numberValue is NumberAgreement)
            {
                number = (NumberAgreement) numberValue;
            }
            else
            {
                number = NumberAgreement.SINGULAR;
            }

            // Ehud Reiter = modified below to force number from VP for WHAT_SUBJECT
            // and WHO_SUBJECT interrogatuves
            if (parent is PhraseElement)
            {
                if (parent.isA(PhraseCategoryEnum.CLAUSE)
                    && (PhraseHelper.isExpletiveSubject((PhraseElement) parent)
                        || InterrogativeType.WHO_SUBJECT.Equals(parent
                            .getFeature(Feature.INTERROGATIVE_TYPE.ToString())) || InterrogativeType.WHAT_SUBJECT
                            .Equals(parent
                                .getFeature(Feature.INTERROGATIVE_TYPE.ToString())))
                    && isCopular(phrase.getHead()))
                {

                    if (hasPluralComplement(phrase
                        .getFeatureAsElementList(InternalFeature.COMPLEMENTS.ToString())))
                    {
                        number = NumberAgreement.PLURAL;
                    }
                    else
                    {
                        number = NumberAgreement.SINGULAR;
                    }
                }
            }
            return number;
        }

        /**
         * Checks to see if any of the complements to the phrase are plural.
         * 
         * @param complements
         *            the list of complements of the phrase.
         * @return <code>true</code> if any of the complements are plural.
         */

        private static bool hasPluralComplement(List<INLGElement> complements)
        {
            var plural = false;
            var complementIterator = complements.GetEnumerator();
            INLGElement eachComplement;
            object numberValue;

            while (complementIterator.MoveNext() && !plural)
            {
                eachComplement = complementIterator.Current;

                if (eachComplement != null && eachComplement.isA(PhraseCategoryEnum.NOUN_PHRASE))
                {

                    numberValue = eachComplement.getFeature(Feature.NUMBER.ToString());
                    if (numberValue != null && NumberAgreement.PLURAL.Equals(numberValue))
                    {
                        plural = true;
                    }
                }
            }
            return plural;
        }

        /**
         * Checks to see if the base form of the word is copular, i.e. <em>be</em>.
         * 
         * @param element
         *            the element to be checked
         * @return <code>true</code> if the element is copular.
         */

        public static bool isCopular(INLGElement element)
        {
            var copular = false;

            if (element is InflectedWordElement)
            {
                copular = "be".equalsIgnoreCase(((InflectedWordElement) element) 
                    .getBaseForm());

            }
            else if (element is WordElement)
            {
                copular = "be".equalsIgnoreCase(((WordElement) element) 
                    .getBaseForm());

            }
            else if (element is PhraseElement)
            {
                // get the head and check if it's "be"
                INLGElement head = element is SPhraseSpec
                    ? ((SPhraseSpec) element)
                        .getVerb()
                    : ((PhraseElement) element).getHead();

                if (head != null)
                {
                    copular = (head is WordElement && "be"
                                   .Equals(((WordElement) head).getBaseForm()));
                }
            }

            return copular;
        }
    }
}