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

namespace SimpleNLGTests.morphology
{

    /**
     * This tests that two sentences are realised as a list.
     * @author Rodrigo de Oliveira - Data2Text Ltd
     *
     */

    public class EnumeratedListTest
    {

        [Test]
        public void bulletList()
        {
            var lexicon = Lexicon.getDefaultLexicon();
            var nlgFactory = new NLGFactory(lexicon);
            var realiser = new Realiser(lexicon);
            realiser.setFormatter(new HTMLFormatter());
            var document = nlgFactory.createDocument("Document");
            var paragraph = nlgFactory.createParagraph();
            var list = nlgFactory.createList();
            var item1 = nlgFactory.createListItem();
            var item2 = nlgFactory.createListItem();
            // NB: a list item employs orthographical operations only until sentence level;
            // nest clauses within a sentence to generate more than 1 clause per list item. 
            var sentence1 = nlgFactory.createSentence("this", "be", "the first sentence");
            var sentence2 = nlgFactory.createSentence("this", "be", "the second sentence");
            item1.addComponent(sentence1);
            item2.addComponent(sentence2);
            list.addComponent(item1);
            list.addComponent(item2);
            paragraph.addComponent(list);
            document.addComponent(paragraph);
            var expectedOutput = "<h1>Document</h1>" + "<p>" + "<ul>" + "<li>This is the first sentence.</li>"
                                    + "<li>This is the second sentence.</li>" + "</ul>" + "</p>";

            var realisedOutput = realiser.realise(document).getRealisation();
            Assert.AreEqual(expectedOutput, realisedOutput);
        }

        [Test]
        public void enumeratedList()
        {
            var lexicon = Lexicon.getDefaultLexicon();
            var nlgFactory = new NLGFactory(lexicon);
            var realiser = new Realiser(lexicon);
            realiser.setFormatter(new HTMLFormatter());
            var document = nlgFactory.createDocument("Document");
            var paragraph = nlgFactory.createParagraph();
            var list = nlgFactory.createEnumeratedList();
            var item1 = nlgFactory.createListItem();
            var item2 = nlgFactory.createListItem();
            // NB: a list item employs orthographical operations only until sentence level;
            // nest clauses within a sentence to generate more than 1 clause per list item. 
            var sentence1 = nlgFactory.createSentence("this", "be", "the first sentence");
            var sentence2 = nlgFactory.createSentence("this", "be", "the second sentence");
            item1.addComponent(sentence1);
            item2.addComponent(sentence2);
            list.addComponent(item1);
            list.addComponent(item2);
            paragraph.addComponent(list);
            document.addComponent(paragraph);
            var expectedOutput = "<h1>Document</h1>" + "<p>" + "<ol>" + "<li>This is the first sentence.</li>"
                                    + "<li>This is the second sentence.</li>" + "</ol>" + "</p>";

            var realisedOutput = realiser.realise(document).getRealisation();
            Assert.AreEqual(expectedOutput, realisedOutput);
        }

    }
}