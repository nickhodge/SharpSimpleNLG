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

using NUnit.Framework;
using SimpleNLG;

namespace SimpleNLGTests.syntax
{
    /**
     * Tests that check that realization of different Features against NLGElements.
     * 
     * @author François Portet
     */

    public class FeatureTest : SimpleNLG4TestBase
    {

        /**
         * Tests use of the Possessive Feature.
         */

        [Test]
        public void testPossessiveFeature_PastTense()
        {
            // Create the pronoun 'she'
            var she = this.phraseFactory.createWord("she", new LexicalCategory_PRONOUN());

            // Set possessive on the pronoun to make it 'her'
            she.setFeature(Feature.POSSESSIVE.ToString(), true);

            // Create a noun phrase with the subject lover and the determiner
            // as she
            var herLover = this.phraseFactory.createNounPhrase(she, "lover");

            // Create a clause to say 'he be her lover'
            var clause = this.phraseFactory.createClause("he", "be", herLover);

            // Add the cue phrase need the comma as orthography
            // currently doesn't handle this.
            // This could be expanded to be a noun phrase with determiner
            // 'two' and noun 'week', set to plural and with a premodifier of
            // 'after'
            clause.setFeature(Feature.CUE_PHRASE.ToString(), "after two weeks,");

            // Add the 'for a fortnight' as a post modifier. Alternatively
            // this could be added as a prepositional phrase 'for' with a
            // complement of a noun phrase ('a' 'fortnight')
            clause.addPostModifier("for a fortnight");

            // Set 'be' to 'was' as past tense
            clause.setFeature(Feature.TENSE.ToString(), Tense.PAST);

            // Add the clause to a sentence.
            var sentence1 = this.phraseFactory.createSentence(clause);

            // Realise the sentence
            var realised = this.realiser.realise(sentence1);

            Assert.AreEqual("After two weeks, he was her lover for a fortnight.",
                realised.getRealisation());
        }

        /**
         * Basic tests.
         */

        [Test]
        public void testTwoPossessiveFeature_PastTense()
        {
             // Create the pronoun 'she'
            var she = this.phraseFactory.createWord("she", new LexicalCategory_PRONOUN());

            // Set possessive on the pronoun to make it 'her'
            she.setFeature(Feature.POSSESSIVE.ToString(), true);

            // Create a noun phrase with the subject lover and the determiner
            // as she
            var herLover = this.phraseFactory.createNounPhrase(she, "lover");
            herLover.setPlural(true);

            // Create the pronoun 'he'
            NLGElement he = this.phraseFactory.createNounPhrase(new LexicalCategory_PRONOUN(), "he");
            he.setPlural(true);

            // Create a clause to say 'they be her lovers'
            var clause = this.phraseFactory.createClause(he, "be", herLover);
            clause.setFeature(Feature.POSSESSIVE.ToString(), true);

            // Add the cue phrase need the comma as orthography
            // currently doesn't handle this.
            // This could be expanded to be a noun phrase with determiner
            // 'two' and noun 'week', set to plural and with a premodifier of
            // 'after'
            clause.setFeature(Feature.CUE_PHRASE.ToString(), "after two weeks,");

            // Add the 'for a fortnight' as a post modifier. Alternatively
            // this could be added as a prepositional phrase 'for' with a
            // complement of a noun phrase ('a' 'fortnight')
            clause.addPostModifier("for a fortnight");

            // Set 'be' to 'was' as past tense
            clause.setFeature(Feature.TENSE.ToString(), Tense.PAST);

            // Add the clause to a sentence.
            var sentence1 = this.phraseFactory.createSentence(clause);

            // Realise the sentence
            var realised = this.realiser.realise(sentence1);

            Assert.AreEqual("After two weeks, they were her lovers for a fortnight.", 
                realised.getRealisation());
        }

        /**
         * Test use of the Complementiser feature by combining two S's using cue phrase and gerund.
         */

        [Test]
        public void testComplementiserFeature_PastTense()
        {
            var born = this.phraseFactory.createClause("Dave Bus", "be", "born");
            born.setFeature(Feature.TENSE.ToString(), Tense.PAST);
            born.addPostModifier("in");
            born.setFeature(Feature.COMPLEMENTISER.ToString(), "which");

            var theHouse = this.phraseFactory.createNounPhrase("the", "house");
            theHouse.addComplement(born);

            var clause = this.phraseFactory.createClause(theHouse, "be",
                 this.phraseFactory.createPrepositionPhrase("in", "Edinburgh"));
            var sentence = this.phraseFactory.createSentence(clause);
            var realised = this.realiser.realise(sentence);

            // Retrieve the realisation and dump it to the console
            Assert.AreEqual("The house which Dave Bus was born in is in Edinburgh.",
                realised.getRealisation());
        }

        /**
         * Test use of the Complementiser feature in a {@link CoordinatedPhraseElement} by combine two S's using cue phrase and gerund.
         */

        [Test]
        public void testComplementiserFeatureInACoordinatePhrase_PastTense()
        {
            var dave = this.phraseFactory.createWord("Dave Bus", new LexicalCategory_NOUN());
            var albert = this.phraseFactory.createWord("Albert", new LexicalCategory_NOUN());

            var coord1 = new CoordinatedPhraseElement(
                dave, albert);

            var born = this.phraseFactory.createClause(coord1, "be", "born");
            born.setFeature(Feature.TENSE.ToString(), Tense.PAST);
            born.addPostModifier("in");
            born.setFeature(Feature.COMPLEMENTISER.ToString(), "which");

            var theHouse = this.phraseFactory.createNounPhrase("the", "house");
            theHouse.addComplement(born);

            var clause = this.phraseFactory.createClause(theHouse, "be",
                 this.phraseFactory.createPrepositionPhrase("in", "Edinburgh"));
            var sentence = this.phraseFactory.createSentence(clause);

            var realised = this.realiser.realise(sentence);

            // Retrieve the realisation and dump it to the console
            Assert.AreEqual("The house which Dave Bus and Albert were born in is in Edinburgh.",
                realised.getRealisation());
        }

        /**
         * Test the use of the Progressive and Complementiser Features in future tense.
         */

        [Test]
        public void testProgressiveAndComplementiserFeatures_FutureTense()
        {
            // Inner clause is 'I' 'make' 'sentence' 'for'.
            var inner = this.phraseFactory.createClause("I", "make", "sentence for");
            // Inner clause set to progressive.
            inner.setFeature(Feature.PROGRESSIVE.ToString(), true);

            //Complementiser on inner clause is 'whom'
            inner.setFeature(Feature.COMPLEMENTISER.ToString(), "whom");

            // create the engineer and add the inner clause as post modifier 
            var engineer = this.phraseFactory.createNounPhrase("the engineer");
            engineer.addComplement(inner);

            // Outer clause is: 'the engineer' 'go' (preposition 'to' 'holidays')
            var outer = this.phraseFactory.createClause(engineer, "go",
                 this.phraseFactory.createPrepositionPhrase("to", "holidays"));

            // Outer clause tense is Future.
            outer.setFeature(Feature.TENSE.ToString(), Tense.FUTURE);

            // Possibly progressive as well not sure.
            outer.setFeature(Feature.PROGRESSIVE.ToString(), true);

            //Outer clause postmodifier would be 'tomorrow'
            outer.addPostModifier("tomorrow");
            var sentence = this.phraseFactory.createSentence(outer);
            var realised = this.realiser.realise(sentence);

            // Retrieve the realisation and dump it to the console
            Assert.AreEqual("The engineer whom I am making sentence for will be going to holidays tomorrow.",
                realised.getRealisation());
        }


        /**
         * Tests the use of the Complementiser, Passive, Perfect features in past tense.
         */

        [Test]
        public void testComplementiserPassivePerfectFeatures_PastTense()
        {
            var inner = this.phraseFactory.createClause("I", "play", "poker");
            inner.setFeature(Feature.TENSE.ToString(), Tense.PAST);
            inner.setFeature(Feature.COMPLEMENTISER.ToString(), "where");

            var house = this.phraseFactory.createNounPhrase("the", "house");
            house.addComplement(inner);

            var outer = this.phraseFactory.createClause(null, "abandon", house);

            outer.addPostModifier("since 1986");

            outer.setFeature(Feature.PASSIVE.ToString(), true);
            outer.setFeature(Feature.PERFECT.ToString(), true);

            var sentence = this.phraseFactory.createSentence(outer);
            var realised = this.realiser.realise(sentence);

            // Retrieve the realisation and dump it to the console
            Assert.AreEqual("The house where I played poker has been abandoned since 1986.",
                realised.getRealisation());
        }

        /**
         * Tests the user of the progressive and complementiser featuers in past tense.
         */

        [Test]
        public void testProgressiveComplementiserFeatures_PastTense()
        {
            var sandwich = this.phraseFactory.createNounPhrase(new LexicalCategory_NOUN(), "sandwich");
            sandwich.setPlural(true);
            // 
            var first = this.phraseFactory.createClause("I", "make", sandwich);
            first.setFeature(Feature.TENSE.ToString(), Tense.PAST);
            first.setFeature(Feature.PROGRESSIVE.ToString(), true);
            first.setPlural(false);

            var second = this.phraseFactory.createClause("the mayonnaise", "run out");
            second.setFeature(Feature.TENSE.ToString(), Tense.PAST);
            // 
            second.setFeature(Feature.COMPLEMENTISER.ToString(), "when");

            first.addComplement(second);

            var sentence = this.phraseFactory.createSentence(first);
            var realised = this.realiser.realise(sentence);

            // Retrieve the realisation and dump it to the console
            Assert.AreEqual("I was making sandwiches when the mayonnaise ran out.",
                realised.getRealisation());
        }

        /**
         * Test the use of Passive in creating a Passive sentence structure: <Object> + [be] + <verb> + [by] + [Subject].
         */

        [Test]
        public void testPassiveFeature()
        {
             var phrase = this.phraseFactory.createClause("recession", "affect", "value");
            phrase.setFeature(Feature.PASSIVE.ToString(), true);
            var sentence = this.phraseFactory.createSentence(phrase);
            var realised = this.realiser.realise(sentence);

            Assert.AreEqual("Value is affected by recession.", realised.getRealisation());
        }


        /**
         * Test for repetition of the future auxiliary "will", courtesy of Luxor
         * Vlonjati
         */

        [Test]
        public void testFutureTense()
        {
            var test = this.phraseFactory.createClause();

            var subj = this.phraseFactory.createNounPhrase("I");

            var verb = this.phraseFactory.createVerbPhrase("go");

            var adverb = this.phraseFactory
                .createAdverbPhrase("tomorrow");

            test.setSubject(subj);
            test.setVerbPhrase(verb);
            test.setFeature(Feature.TENSE.ToString(), Tense.FUTURE);
            test.addPostModifier(adverb);
            var sentence = this.realiser.realiseSentence(test);
            Assert.AreEqual("I will go tomorrow.", sentence);

            var test2 = this.phraseFactory.createClause();
            var vb = this.phraseFactory.createWord("go", new LexicalCategory_VERB());
            test2.setSubject(subj);
            test2.setVerb(vb);
            test2.setFeature(Feature.TENSE.ToString(), Tense.FUTURE);
            test2.addPostModifier(adverb);
            var sentence2 = this.realiser.realiseSentence(test);
            Assert.AreEqual("I will go tomorrow.", sentence2);

        }


    }
}