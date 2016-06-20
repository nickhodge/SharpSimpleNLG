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
    public class SyntaxProcessor : NLGModule
    {
        public override void initialise()
        {
            
        }

        public override INLGElement realise(INLGElement element)
        {
            //Debug.WriteLine($"realise single element {element}");
            INLGElement realisedElement = null;

            if (element != null && !element.getFeatureAsBoolean(Feature.ELIDED.ToString()))
            {

                if (element is DocumentElement)
                {
                    List<INLGElement> children = element.getChildren();
                    ((DocumentElement) element).setComponents(realise(children));
                    realisedElement = element;
                }
                else
                if (element is PhraseElement)
                {
                    realisedElement = realisePhraseElement((PhraseElement) element);

                }
                else
                if (element is ListElement)
                {
                    realisedElement = new ListElement();
                    ((ListElement) realisedElement).addComponents(realise(element
                        .getChildren()));
                }
                else
                if (element is InflectedWordElement)
                {
                    var baseForm = ((InflectedWordElement) element).getBaseForm();
                    var category = element.getCategory();

                    if (lexicon != null && baseForm != null)
                    {
                        var word = ((InflectedWordElement) element).getBaseWord();

                        if (word == null)
                        {
                            if (category is ILexicalCategory)
                            {
                                word = this.lexicon.lookupWord(baseForm,
                                    (ILexicalCategory) category);
                            }
                            else
                            {
                                word = this.lexicon.lookupWord(baseForm);
                            }
                        }

                        if (word != null)
                        {
                            ((InflectedWordElement) element).setBaseWord(word);
                        }
                    }

                    realisedElement = element;

                }
                else
                if (element is WordElement)
                {
                    // AG: need to check if it's a word element, in which case it
                    // needs to be marked for inflection
                    var infl = new InflectedWordElement(
                        (WordElement) element);

                    // // the inflected word inherits all features from the base
                    // word
                    foreach (var feature in element.getAllFeatureNames())
                    {
                        infl.setFeature(feature, element.getFeature(feature));
                    }

                    realisedElement = realise(infl);

                }
                else
                if (element is CoordinatedPhraseElement)
                {
                    realisedElement = CoordinatedPhraseHelper.realise(this,
                        (CoordinatedPhraseElement) element);

                }
                else
                {
                    realisedElement = element;
                }
            }

            // Remove the spurious ListElements that have only one element.
            if (realisedElement is ListElement)
            {
                if (((ListElement) realisedElement).size() == 1)
                {
                    realisedElement = ((ListElement) realisedElement).getFirst();
                }
            }

            return realisedElement;
        }

  
        public override List<INLGElement> realise(List<INLGElement> elements)
        {
            //Debug.WriteLine($"realise list elements {elements.Count}");
            var realisedList = new List<INLGElement>();
            INLGElement childRealisation;

            if (elements != null)
            {
                foreach (var eachElement in elements)
                {
                    if (eachElement != null)
                    {
                        childRealisation = realise(eachElement);
                        if (childRealisation != null)
                        {
                            if (childRealisation is ListElement)
                            {
                                realisedList
                                    .addAll(((ListElement) childRealisation)
                                        .getChildren());
                            }
                            else
                            {
                                realisedList.add(childRealisation);
                            }
                        }
                    }
                }
            }
            return realisedList;
        }

        /**
         * Realises a phrase element.
         * 
         * @param phrase
         *            the element to be realised
         * @return the realised element.
         */

        private INLGElement realisePhraseElement(PhraseElement phrase)
        {
            //Debug.WriteLine($"realise phrase element {phrase}");
            INLGElement realisedElement = null;

            if (phrase != null)
            {
                var category = phrase.getCategory();
                realisedElement = phrase;
                if (category is IPhraseCategory)
                {
                    switch ((PhraseCategoryEnum)category.enumType)
                    {

                        case PhraseCategoryEnum.CLAUSE:
                            realisedElement = ClauseHelper.realise(this, phrase);
                            break;

                        case PhraseCategoryEnum.NOUN_PHRASE:
                            realisedElement = NounPhraseHelper.realise(this, phrase);
                            break;

                        case PhraseCategoryEnum.VERB_PHRASE:
                            realisedElement = VerbPhraseHelper.realise(this, phrase);
                            break;

                        case PhraseCategoryEnum.PREPOSITIONAL_PHRASE:
                        case PhraseCategoryEnum.ADJECTIVE_PHRASE:
                        case PhraseCategoryEnum.ADVERB_PHRASE:
                            realisedElement = PhraseHelper.realise(this, phrase);
                            break;

                    }
                }
            }

            return realisedElement;
        }
    }
}