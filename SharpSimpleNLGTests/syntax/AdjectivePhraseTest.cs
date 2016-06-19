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
     * JUnit test class for the {@link Realiser} class.
     * 
     * @author Saad Mahamood
     */

    public class AdjectivePhraseTest : SimpleNLG4TestBase
    { 


        /**
         * Test premodification & coordination of Adjective Phrases (Not much else
         * to simplenlg.test)
         */
     [Test]
        public void testAdj()
        {

            // form the adjphrase "incredibly salacious"
            this.salacious.addPreModifier(this.phraseFactory
                .createAdverbPhrase("incredibly")); 
            Assert.AreEqual("incredibly salacious", this.realiser 
                .realise(this.salacious).getRealisation());

            // form the adjphrase "incredibly beautiful"
            this.beautiful.addPreModifier("amazingly"); 
            Assert.AreEqual("amazingly beautiful", this.realiser 
                .realise(this.beautiful).getRealisation());

            // coordinate the two aps
            var coordap = new CoordinatedPhraseElement(
                this.salacious, this.beautiful);
            Assert.AreEqual("incredibly salacious and amazingly beautiful", 
                this.realiser.realise(coordap).getRealisation());

            // changing the inner conjunction
            coordap.setFeature(Feature.CONJUNCTION.ToString(), "or"); 
            Assert.AreEqual("incredibly salacious or amazingly beautiful", 
                this.realiser.realise(coordap).getRealisation());

            // coordinate this with a new AdjPhraseSpec
            var coord2 = new CoordinatedPhraseElement(coordap,
                this.stunning);
            //this.realiser.setDebugMode(true);
            Assert.AreEqual(
                "incredibly salacious or amazingly beautiful and stunning", 
                this.realiser.realise(coord2).getRealisation());

            // add a premodifier the coordinate phrase, yielding
            // "seriously and undeniably incredibly salacious or amazingly beautiful
            // and stunning"
            var preMod = new CoordinatedPhraseElement(
                new StringElement("seriously"), new StringElement("undeniably")); 

            coord2.addPreModifier(preMod);
            Assert.AreEqual(
                "seriously and undeniably incredibly salacious or amazingly beautiful and stunning", 
                this.realiser.realise(coord2).getRealisation());

            // adding a coordinate rather than coordinating should give a different
            // result
            coordap.addCoordinate(this.stunning);
            Assert.AreEqual(
                "incredibly salacious, amazingly beautiful or stunning", 
                this.realiser.realise(coordap).getRealisation());

        }

        /**
         * Simple test of adverbials
         */
        [Test]

        public void testAdv()
        {

            var sent = this.phraseFactory.createClause("John", "eat");  

            var adv = this.phraseFactory.createAdverbPhrase("quickly"); 

            sent.addPreModifier(adv);

            Assert.AreEqual("John quickly eats", this.realiser.realise(sent) 
                .getRealisation());

            adv.addPreModifier("very"); 

            Assert.AreEqual("John very quickly eats", this.realiser.realise( 
                sent).getRealisation());

        }

        /**
         * Test participles as adjectives
         */
        [Test]

        public void testParticipleAdj()
        {
            var ap = this.phraseFactory
                .createAdjectivePhrase(this.lexicon.getWord("associated",
                    new LexicalCategory_ADJECTIVE()));
            Assert.AreEqual("associated", this.realiser.realise(ap)
                .getRealisation());
        }

        /**
         * Test for multiple adjective modifiers with comma-separation. Example courtesy of William Bradshaw (Data2Text Ltd).
         */
        [Test]
        public void testMultipleModifiers()
        {
            var np = this.phraseFactory
                .createNounPhrase(this.lexicon.getWord("message",
                    new LexicalCategory_NOUN()));
            np.addPreModifier(this.lexicon.getWord("active",
                new LexicalCategory_ADJECTIVE()));
            np.addPreModifier(this.lexicon.getWord("temperature",
                new LexicalCategory_ADJECTIVE()));
            Assert.AreEqual("active, temperature message", this.realiser.realise(np).getRealisation());

            //now we set the realiser not to separate using commas
            this.realiser.setCommaSepPremodifiers(false);
            Assert.AreEqual("active temperature message", this.realiser.realise(np).getRealisation());

        }

    }
}