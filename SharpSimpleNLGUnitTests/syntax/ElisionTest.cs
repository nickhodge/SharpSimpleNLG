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
    public class ElisionTest : SimpleNLG4TestBase
    {


        /**
         * Test elision of phrases in various places in the sentence
         */
//	public void testPhraseElision() {
//		SPhraseSpec s1 = this.phraseFactory.createClause();
//		s1.setSubject(this.np4); //the rock
//		this.kiss.setComplement(this.np5);//kiss the curtain
//		s1.setVerbPhrase(this.kiss);
//		
//		Assert.AreEqual("the rock kisses the curtain", this.realiser.realise(s1).getRealisation());
//		
//		//elide subject np
//		this.np4.setFeature(Feature.ELIDED, true);
//		Assert.AreEqual("kisses the curtain", this.realiser.realise(s1).getRealisation());
//		
//		//elide vp
//		this.np4.setFeature(Feature.ELIDED, false);
//		this.kiss.setFeature(Feature.ELIDED, true);
//		Assert.AreEqual("the rock", this.realiser.realise(s1).getRealisation());
//		
//		//elide complement only
//		this.kiss.setFeature(Feature.ELIDED, false);
//		this.np5.setFeature(Feature.ELIDED, true);
//		Assert.AreEqual("the rock kisses", this.realiser.realise(s1).getRealisation());
//	}

/* DISABLED here as this test doesnt pass in current build of Java SimpleNLG */
        /**
         * Test for elision of specific words rather than phrases
         */

        //[Test]
        public void wordElisionTest()
        {
            this.realiser.setDebugMode(true);
            var s1 = this.phraseFactory.createClause();
            s1.setSubject(this.np4); //the rock
            this.kiss.setComplement(this.np5); //kiss the curtain
            s1.setVerbPhrase(this.kiss);

            this.np5.setFeature(Feature.ELIDED.ToString(), true);
            Assert.AreEqual("the rock kisses", this.realiser.realise(s1).getRealisation());
        }



        /**
         * Test for elision of specific words rather than phrases
         *
        [Test]
        public void testWordElision() {
            this.realiser.setDebugMode(true);
            SPhraseSpec s1 = this.phraseFactory.createClause();
            s1.setSubject(this.np4); //the rock
            this.kiss.setComplement(this.np5);//kiss the curtain
            s1.setVerbPhrase(this.kiss);
            
            this.kiss.getHead().setFeature(Feature.ELIDED, true);
            Assert.AreEqual("the rock kisses the curtain", this.realiser.realise(s1).getRealisation());
        } */
    }
}
