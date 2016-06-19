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
     * TextFormatterTest -- Test's the TextFormatter.
     * @author Daniel Paiva -- Arria NLG plc.
     */
    public class TextFormatterTest {

    [Test]
    public void testEnumeratedList() {
        var lexicon = Lexicon.getDefaultLexicon();
        var nlgFactory = new NLGFactory(lexicon);
        var realiser = new Realiser(lexicon);
        realiser.setFormatter(new TextFormatter());
        var document = nlgFactory.createDocument("Document");
        var paragraph = nlgFactory.createParagraph();


        var subListItem1 = nlgFactory.createListItem();
        var subListSentence1 = nlgFactory.createSentence("this", "be", "sub-list sentence 1");
        subListItem1.addComponent(subListSentence1);

        var subListItem2 = nlgFactory.createListItem();
        var subListSentence2 = nlgFactory.createSentence("this", "be", "sub-list sentence 2");
        subListItem2.addComponent(subListSentence2);

        var subList = nlgFactory.createEnumeratedList();
        subList.addComponent(subListItem1);
        subList.addComponent(subListItem2);

        var item1 = nlgFactory.createListItem();
        var sentence1 = nlgFactory.createSentence("this", "be", "the first sentence");
        item1.addComponent(sentence1);

        var item2 = nlgFactory.createListItem();
        var sentence2 = nlgFactory.createSentence("this", "be", "the second sentence");
        item2.addComponent(sentence2);

        var list = nlgFactory.createEnumeratedList();
        list.addComponent(subList);
        list.addComponent(item1);
        list.addComponent(item2);
        paragraph.addComponent(list);
        document.addComponent(paragraph);
        var expectedOutput = "Document\n" +
                                "\n" +
                                "1.1 - This is sub-list sentence 1.\n" +
                                "1.2 - This is sub-list sentence 2.\n"+
                                "2 - This is the first sentence.\n" +
                                "3 - This is the second sentence.\n" +
                                "\n\n"; // for the end of a paragraph

        var realisedOutput = realiser.realise(document).getRealisation();
        Assert.AreEqual(expectedOutput, realisedOutput);
    }

        [Test]
        public void testEnumeratedListWithSeveralLevelsOfNesting() {
        var lexicon = Lexicon.getDefaultLexicon();
        var nlgFactory = new NLGFactory(lexicon);
        var realiser = new Realiser(lexicon);
        realiser.setFormatter(new TextFormatter());
        var document = nlgFactory.createDocument("Document");
        var paragraph = nlgFactory.createParagraph();

        // sub item 1
        var subList1Item1 = nlgFactory.createListItem();
        var subList1Sentence1 = nlgFactory.createSentence("sub-list item 1");
        subList1Item1.addComponent(subList1Sentence1);

        // sub sub item 1
        var subSubList1Item1 = nlgFactory.createListItem();
        var subSubList1Sentence1 = nlgFactory.createSentence("sub-sub-list item 1");
        subSubList1Item1.addComponent(subSubList1Sentence1);

        // sub sub item 2
        var subSubList1Item2 = nlgFactory.createListItem();
        var subSubList1Sentence2 = nlgFactory.createSentence("sub-sub-list item 2");
        subSubList1Item2.addComponent(subSubList1Sentence2);

        // sub sub list
        var subSubList1 = nlgFactory.createEnumeratedList();
        subSubList1.addComponent(subSubList1Item1);
        subSubList1.addComponent(subSubList1Item2);

        // sub item 2
        var subList1Item2 = nlgFactory.createListItem();
        var subList1Sentence2 = nlgFactory.createSentence("sub-list item 3");
        subList1Item2.addComponent(subList1Sentence2);

        // sub list 1
        var subList1 = nlgFactory.createEnumeratedList();
        subList1.addComponent(subList1Item1);
        subList1.addComponent(subSubList1);
        subList1.addComponent(subList1Item2);

        // item 2
        var item2 = nlgFactory.createListItem();
        var sentence2 = nlgFactory.createSentence("item 2");
        item2.addComponent(sentence2);

        // item 3
        var item3 = nlgFactory.createListItem();
        var sentence3 = nlgFactory.createSentence("item 3");
        item3.addComponent(sentence3);

        // list
        var list = nlgFactory.createEnumeratedList();
        list.addComponent(subList1);
        list.addComponent(item2);
        list.addComponent(item3);

        paragraph.addComponent(list);

        document.addComponent(paragraph);

        var expectedOutput = "Document\n" +
                                "\n" +
                                "1.1 - Sub-list item 1.\n" +
                                "1.2.1 - Sub-sub-list item 1.\n" +
                                "1.2.2 - Sub-sub-list item 2.\n" +
                                "1.3 - Sub-list item 3.\n"+
                                "2 - Item 2.\n" +
                                "3 - Item 3.\n" +
                                "\n\n";

        var realisedOutput = realiser.realise(document).getRealisation();
        Assert.AreEqual(expectedOutput, realisedOutput);
    }

}
}