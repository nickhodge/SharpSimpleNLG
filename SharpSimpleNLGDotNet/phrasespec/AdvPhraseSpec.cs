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

namespace SimpleNLG
{
    public class AdvPhraseSpec : PhraseElement
    {
        /**
         * <p>
         * This class defines a adverbial phrase.  It is essentially
         * a wrapper around the <code>PhraseElement</code> class, with methods
         * for setting common constituents such as preModifier.
         * For example, the <code>setAdverb</code> method in this class sets
         * the head of the element to be the specified adverb
         *
         * From an API perspective, this class is a simplified version of the AdvPhraseSpec
         * class in simplenlg V3.  It provides an alternative way for creating syntactic
         * structures, compared to directly manipulating a V4 <code>PhraseElement</code>.
         * 
         * Methods are provided for setting and getting the following constituents:
         * <UL>
         * <LI>PreModifier		(eg, "very")
         * <LI>Adverb        (eg, "quickly")
         * </UL>
         * 
         * NOTE: AdvPhraseSpec do not usually have (user-set) features
         * 
         * <code>AdvPhraseSpec</code> are produced by the <code>createAdverbPhrase</code>
         * method of a <code>PhraseFactory</code>
         * </p>
         * 
         * @author E. Reiter, University of Aberdeen.
         * @version 4.1
         * 
         */

        public AdvPhraseSpec(NLGFactory phraseFactory) : base (new PhraseCategory_ADVERB_PHRASE())
        {

            // set default feature value
            setFeature(Feature.ELIDED.ToString(), false);
            this.setFactory(phraseFactory);
        }

        /** sets the adverb (head) of the phrase
         * @param adverb
         */

        public void setAdverb(object adverb)
        {
            if (adverb is INLGElement)
                setHead(adverb);
            else
            {
                // create noun as word
                var adverbElement = getFactory().createWord(adverb, new LexicalCategory_ADVERB());

                // set head of NP to nounElement
                setHead(adverbElement);
            }
        }

        /**
         * @return adverb (head) of  phrase
         */

        public INLGElement getAdverb()
        {
            return getHead();
        }

        // inherit usual modifier routines


    }
}