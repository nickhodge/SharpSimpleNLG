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

using System;
using System.Diagnostics;
using FluentAssertions;
using NUnit.Framework;
using SimpleNLG;

namespace SimpleNLGTests.lexicon
{

    /**
     * @author D. Westwater, Data2Text Ltd
     * 
     */

    public class XMLLexiconTest
    {

        // lexicon object -- an instance of Lexicon
        XMLLexicon lexicon = null;


        [Test]
        public void setUp_from_EmbeddedResource()
        {
            var timer = new Stopwatch();
            timer.Start();
            this.lexicon = new XMLLexicon();
            timer.Stop();
            Console.WriteLine($"Loading Lexicon took: {timer.ElapsedMilliseconds}ms");
        }


        [Test]
        public void setUp_from_Filepath()
        {
            var timer = new Stopwatch();
            timer.Start();
            this.lexicon = new XMLLexicon(@"C:\work\SharpSimpleNLG\SharpSimpleNLGDotNetCore\lexicon\default-lexicon.xml");
            timer.Stop();
            Console.WriteLine($"Loading Lexicon took: {timer.ElapsedMilliseconds}ms");
        }


        /**
         * Tests the immutability of the XMLLexicon by checking to make sure features 
         * are not inadvertently propagated to the canonical XMLLexicon WordElement object.
         */

        [Test]
        public void xmlLexiconImmutabilityTest()
        {
            this.lexicon = new XMLLexicon();
            NLGFactory factory = new NLGFactory(lexicon);
            Realiser realiser = new Realiser(lexicon);

            // "wall" is singular.
            NPPhraseSpec wall = factory.createNounPhrase("the", "wall");
            Assert.AreEqual(NumberAgreement.SINGULAR, wall.getFeature(Feature.NUMBER.ToString()));

            // Realise a sentence with plural form of "wall"
            wall.setPlural(true);
            SPhraseSpec sentence = factory.createClause("motion", "observe");
            sentence.setFeature(Feature.TENSE.ToString(), Tense.PAST);
            PPPhraseSpec pp = factory.createPrepositionPhrase("in", wall);
            sentence.addPostModifier(pp);
            var r = realiser.realiseSentence(sentence);

            Assert.AreEqual(r, "Motion observed in the walls.");

            // Create a new 'the wall' NP and check to make sure that the syntax processor has
            // not propagated plurality to the canonical XMLLexicon WordElement object.
            wall = factory.createNounPhrase("the", "wall");
            Assert.AreEqual(NumberAgreement.SINGULAR, wall.getFeature(Feature.NUMBER.ToString()));
        }

        [Test]
        public void xmlLexiconLookupTests()
        {
            this.lexicon = new XMLLexicon();

            var w = lexicon.getWords("man");
            w.Count.ShouldBeEquivalentTo(1);
            w[0].getBaseForm().ShouldBeEquivalentTo("man");
            w[0].features.Count.ShouldBeEquivalentTo(2);
            w[0].getId().ShouldBeEquivalentTo("E0038767");
        }

        [Test]
        public void xmlLexiconLookupWord()
        {
            this.lexicon = new XMLLexicon();
            var w = lexicon.lookupWord("man", new LexicalCategory_NOUN());
            var id = w.getId();
            Assert.IsNotNull(id);
            Assert.IsNotEmpty(id);
            id.ShouldBeEquivalentTo("E0038767");
        }

        [Test]
        public void xmlLexiconLookupWord_plurals()
        {
            this.lexicon = new XMLLexicon();
            var w1 = lexicon.lookupWord("man", new LexicalCategory_NOUN());
            var w2 = w1.getFeature(LexicalFeature.PLURAL.ToString());
            w2.ShouldBeEquivalentTo("men");
        }


        [Test]
        public void xmlLexiconGetWordVariants()
        {
            this.lexicon = new XMLLexicon();
            var w = lexicon.getWordsFromVariant("did");
            var resw = w[0];
            Assert.IsNotNull(resw);
            resw.getAllFeatures().Count.ShouldBeEquivalentTo(8);
            var form1 = resw.getFeature(LexicalFeature.PAST);
            form1.ShouldBeEquivalentTo("did");
            var form2 = resw.getFeature(LexicalFeature.PRESENT3S);
            form2.ShouldBeEquivalentTo("does");
            var form3 = resw.getFeature(LexicalFeature.PAST_PARTICIPLE);
            form3.ShouldBeEquivalentTo("done");
            var form4 = resw.getFeature(LexicalFeature.PRESENT_PARTICIPLE);
            form4.ShouldBeEquivalentTo("doing");
        }
        
    }

}