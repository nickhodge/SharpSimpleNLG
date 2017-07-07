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
using System.Collections.Generic;
using NUnit.Framework;
using SimpleNLG;
using SimpleNLG.Extensions;

namespace SimpleNLGTests.external
{
    /**
     * Tests from third parties
     * @author ereiter
     * 
     */
	
	
    public class ExternalTest
    {
        [SetUpFixture]
        public class SetupForExternalTest
        {
            public static Lexicon lexicon = Lexicon.getDefaultLexicon();
            public static NLGFactory phraseFactory = new NLGFactory(lexicon);
            public static Realiser realiser = new Realiser(lexicon);
         }

        [Test]
        public void forcherTest()
        {
            // Bjorn Forcher's tests
            SetupForExternalTest.phraseFactory.setLexicon(SetupForExternalTest.lexicon);
            SPhraseSpec s1 = SetupForExternalTest.phraseFactory.createClause(null, "associate", "Marie");
            s1.setFeature(Feature.PASSIVE.ToString(), true);
            PPPhraseSpec pp1 = SetupForExternalTest.phraseFactory.createPrepositionPhrase("with"); 
            pp1.addComplement("Peter"); 
            pp1.addComplement("Paul"); 
            s1.addPostModifier(pp1);


            SetupForExternalTest.realiser.setDebugMode(true);
            var r = SetupForExternalTest.realiser.realise(s1).getRealisation();

            Assert.AreEqual("Marie is associated with Peter and Paul", r); 


            SPhraseSpec s2 = SetupForExternalTest.phraseFactory.createClause();
            s2.setSubject((NPPhraseSpec)SetupForExternalTest.phraseFactory.createNounPhrase("Peter")); 
            s2.setVerb("have"); 
            s2.setObject("something to do"); 
            s2.addPostModifier((PPPhraseSpec)SetupForExternalTest.phraseFactory.createPrepositionPhrase(
                "with", "Paul"));  


            Assert.AreEqual("Peter has something to do with Paul", 
                SetupForExternalTest.realiser.realise(s2).getRealisation());
        }

        [Test]
        public void luTest()
        {
            // Xin Lu's test
            SetupForExternalTest.phraseFactory.setLexicon(SetupForExternalTest.lexicon);
            SPhraseSpec s1 = SetupForExternalTest.phraseFactory.createClause("we", 
                "consider", 
                "John"); 
            s1.addPostModifier("a friend"); 

            Assert.AreEqual("we consider John a friend", SetupForExternalTest.realiser 
                .realise(s1).getRealisation());
        }


        [Test]
        public void dwightTest()
        {
            // Rachel Dwight's test
            SetupForExternalTest.phraseFactory.setLexicon(SetupForExternalTest.lexicon);

            NPPhraseSpec noun4 = SetupForExternalTest.phraseFactory
                .createNounPhrase("FGFR3 gene in every cell"); 

            noun4.setSpecifier("the");

            PPPhraseSpec prep1 = SetupForExternalTest.phraseFactory.createPrepositionPhrase(
                "of", noun4); 

            NPPhraseSpec noun1 = SetupForExternalTest.phraseFactory.createNounPhrase(
                "the", "patient's mother");  

            NPPhraseSpec noun2 = SetupForExternalTest.phraseFactory.createNounPhrase(
                "the", "patient's father");  

            NPPhraseSpec noun3 = SetupForExternalTest.phraseFactory
                .createNounPhrase("changed copy"); 
            noun3.addPreModifier("one"); 
            noun3.addComplement(prep1);

            var coordNoun1 = new CoordinatedPhraseElement(
                noun1, noun2);
            coordNoun1.setConjunction("or"); 

            VPPhraseSpec verbPhrase1 = SetupForExternalTest.phraseFactory.createVerbPhrase("have"); 
            verbPhrase1.setFeature(Feature.TENSE.ToString(), Tense.PRESENT);

            SPhraseSpec sentence1 = SetupForExternalTest.phraseFactory.createClause(coordNoun1,
                verbPhrase1, noun3);

             Assert
                .AreEqual(
                    "the patient's mother or the patient's father has one changed copy of the FGFR3 gene in every cell",
                    
                    SetupForExternalTest.realiser.realise(sentence1).getRealisation());

            // Rachel's second test
            noun3 = SetupForExternalTest.phraseFactory.createNounPhrase("a", "gene test");  
            noun2 = SetupForExternalTest.phraseFactory.createNounPhrase("an", "LDL test");  
            noun1 = SetupForExternalTest.phraseFactory.createNounPhrase("the", "clinic");  
            verbPhrase1 = SetupForExternalTest.phraseFactory.createVerbPhrase("perform"); 

            var coord1 = new CoordinatedPhraseElement(noun2,
                noun3);
            sentence1 = SetupForExternalTest.phraseFactory.createClause(noun1, verbPhrase1, coord1);
            sentence1.setFeature(Feature.TENSE.ToString(), Tense.PAST);

            Assert
                .AreEqual(
                    "the clinic performed an LDL test and a gene test", SetupForExternalTest.realiser 
                        .realise(sentence1).getRealisation());
        }


        [Test]
        public void novelliTest()
        {
            // Nicole Novelli's test
            SPhraseSpec p = SetupForExternalTest.phraseFactory.createClause(
                "Mary", "chase", "George");   

            PPPhraseSpec pp = SetupForExternalTest.phraseFactory.createPrepositionPhrase(
                "in", "the park");  
            p.addPostModifier(pp);

            Assert.AreEqual("Mary chases George in the park", SetupForExternalTest.realiser 
                .realise(p).getRealisation());

            // another question from Nicole
            SPhraseSpec run = SetupForExternalTest.phraseFactory.createClause(
                "you", "go", "running");   
            run.setFeature(Feature.MODAL.ToString(), "should"); 
            run.addPreModifier("really"); 
            SPhraseSpec think = SetupForExternalTest.phraseFactory.createClause("I", "think");  
            think.setObject(run);
            run.setFeature(Feature.SUPRESSED_COMPLEMENTISER.ToString(), true);

            var text = SetupForExternalTest.realiser.realise(think).getRealisation();
            Assert.AreEqual("I think you should really go running", text); 
        }


        [Test]
        public void piotrekTest()
        {
            // Piotrek Smulikowski's test
            SetupForExternalTest.phraseFactory.setLexicon(SetupForExternalTest.lexicon);
            SPhraseSpec sent = SetupForExternalTest.phraseFactory.createClause(
                "I", "shoot", "the duck");   
            sent.setFeature(Feature.TENSE.ToString(), Tense.PAST);

            PPPhraseSpec loc = SetupForExternalTest.phraseFactory.createPrepositionPhrase(
                "at", "the Shooting Range");  
            sent.addPostModifier(loc);
            sent.setFeature(Feature.CUE_PHRASE.ToString(), "then"); 

            Assert.AreEqual("then I shot the duck at the Shooting Range", 
                SetupForExternalTest.realiser.realise(sent).getRealisation());
        }


        [Test]
        public void prescottTest()
        {
            // Michael Prescott's test
            SetupForExternalTest.phraseFactory.setLexicon(SetupForExternalTest.lexicon);
            var embedded = SetupForExternalTest.phraseFactory.createClause(
                "Jill", "prod", "Spot");   
            var sent = SetupForExternalTest.phraseFactory.createClause(
                "Jack", "see", embedded);  
            embedded.setFeature(Feature.SUPRESSED_COMPLEMENTISER.ToString(), true);
            embedded.setFeature(Feature.FORM.ToString(), Form.BARE_INFINITIVE.ToString());

            Assert.AreEqual("Jack sees Jill prod Spot", SetupForExternalTest.realiser 
                .realise(sent).getRealisation());
        }


        [Test]
        public void wissnerTest()
        {
            // Michael Wissner's text
            var p = SetupForExternalTest.phraseFactory.createClause("a wolf", "eat");  
            p.setFeature(Feature.INTERROGATIVE_TYPE.ToString(), InterrogativeType.WHAT_OBJECT);
            Assert.AreEqual("what does a wolf eat", SetupForExternalTest.realiser.realise(p) 
                .getRealisation());

        }


        [Test]
        public void phanTest()
        {
            // Thomas Phan's text
            var subjectElement = SetupForExternalTest.phraseFactory.createNounPhrase("I");
            var verbElement = SetupForExternalTest.phraseFactory.createVerbPhrase("run");

            var prepPhrase = SetupForExternalTest.phraseFactory.createPrepositionPhrase("from");
            prepPhrase.addComplement("home");

            verbElement.addComplement(prepPhrase);
            var newSentence = SetupForExternalTest.phraseFactory.createClause();
            newSentence.setSubject(subjectElement);
            newSentence.setVerbPhrase(verbElement);

            Assert.AreEqual("I run from home", SetupForExternalTest.realiser.realise(newSentence) 
                .getRealisation());

        }


        [Test]
        public void kerberTest()
        {
            // Frederic Kerber's tests
            var sp = SetupForExternalTest.phraseFactory.createClause("he", "need");
            var secondSp = SetupForExternalTest.phraseFactory.createClause();
            secondSp.setVerb("build");
            secondSp.setObject("a house");
            secondSp.setFeature(Feature.FORM.ToString(), Form.INFINITIVE);
            sp.setObject("stone");
            sp.addComplement(secondSp);
            Assert.AreEqual("he needs stone to build a house", SetupForExternalTest.realiser.realise(sp).getRealisation());

            var sp2 = SetupForExternalTest.phraseFactory.createClause("he", "give");
            sp2.setIndirectObject("I");
            sp2.setObject("the book");
            Assert.AreEqual("he gives me the book", SetupForExternalTest.realiser.realise(sp2).getRealisation());

        }


        [Test]
        public void stephensonTest()
        {
            // Bruce Stephenson's test
            var qs2 = SetupForExternalTest.phraseFactory.createClause();
            qs2 = SetupForExternalTest.phraseFactory.createClause();
            qs2.setSubject("moles of Gold");
            qs2.setVerb("are");
            qs2.setFeature(Feature.NUMBER.ToString(), NumberAgreement.PLURAL);
            qs2.setFeature(Feature.PASSIVE.ToString(), false);
            qs2.setFeature(Feature.INTERROGATIVE_TYPE.ToString(), InterrogativeType.HOW_MANY);
            qs2.setObject("in a 2.50 g sample of pure Gold");
            var sentence = SetupForExternalTest.phraseFactory.createSentence(qs2);
            Assert.AreEqual("How many moles of Gold are in a 2.50 g sample of pure Gold?",
                SetupForExternalTest.realiser.realise(sentence).getRealisation());
        }


        [Test]
        public void pierreTest()
        {
            // John Pierre's test
            var p = SetupForExternalTest.phraseFactory.createClause("Mary", "chase", "George");
            p.setFeature(Feature.INTERROGATIVE_TYPE.ToString(), InterrogativeType.WHAT_OBJECT);
            Assert.AreEqual("What does Mary chase?", SetupForExternalTest.realiser.realiseSentence(p));

            p = SetupForExternalTest.phraseFactory.createClause("Mary", "chase", "George");
            p.setFeature(Feature.INTERROGATIVE_TYPE.ToString(), InterrogativeType.YES_NO);
            Assert.AreEqual("Does Mary chase George?", SetupForExternalTest.realiser.realiseSentence(p));

            p = SetupForExternalTest.phraseFactory.createClause("Mary", "chase", "George");
            p.setFeature(Feature.INTERROGATIVE_TYPE.ToString(), InterrogativeType.WHERE);
            Assert.AreEqual("Where does Mary chase George?", SetupForExternalTest.realiser.realiseSentence(p));

            p = SetupForExternalTest.phraseFactory.createClause("Mary", "chase", "George");
            p.setFeature(Feature.INTERROGATIVE_TYPE.ToString(), InterrogativeType.WHY);
            Assert.AreEqual("Why does Mary chase George?", SetupForExternalTest.realiser.realiseSentence(p));

            p = SetupForExternalTest.phraseFactory.createClause("Mary", "chase", "George");
            p.setFeature(Feature.INTERROGATIVE_TYPE.ToString(), InterrogativeType.HOW);
            Assert.AreEqual("How does Mary chase George?", SetupForExternalTest.realiser.realiseSentence(p));


        }

        [Test]
        public void data2TextTest()
        {
            // Data2Text tests
            // test OK to have number at end of sentence
            SPhraseSpec p = SetupForExternalTest.phraseFactory.createClause("the dog", "weigh", "12");
            Assert.AreEqual("The dog weighs 12.", SetupForExternalTest.realiser.realiseSentence(p));

            // test OK to have "there be" sentence with "there" as a StringElement
            var dataDropout2 = SetupForExternalTest.phraseFactory.createNLGElement("data dropouts");
            dataDropout2.setPlural(true);
            var sentence2 = SetupForExternalTest.phraseFactory.createClause();
            sentence2.setSubject(SetupForExternalTest.phraseFactory.createStringElement("there"));
            sentence2.setVerb("be");
            sentence2.setObject(dataDropout2);
            Assert.AreEqual("There are data dropouts.", SetupForExternalTest.realiser.realiseSentence(sentence2));

            // test OK to have gerund form verb
            SPhraseSpec weather1 = SetupForExternalTest.phraseFactory.createClause("SE 10-15", "veer", "S 15-20");
            weather1.setFeature(Feature.FORM.ToString(), Form.GERUND);
            Assert.AreEqual("SE 10-15 veering S 15-20.", SetupForExternalTest.realiser.realiseSentence(weather1));

            // test OK to have subject only
            SPhraseSpec weather2 = SetupForExternalTest.phraseFactory.createClause("cloudy and misty", "be", "XXX");
            weather2.getVerbPhrase().setFeature(Feature.ELIDED.ToString(), true);
            Assert.AreEqual("Cloudy and misty.", SetupForExternalTest.realiser.realiseSentence(weather2));

            // test OK to have VP only
            SPhraseSpec weather3 = SetupForExternalTest.phraseFactory.createClause("S 15-20", "increase", "20-25");
            weather3.setFeature(Feature.FORM.ToString(), Form.GERUND);
            weather3.getSubject().setFeature(Feature.ELIDED.ToString(), true);
            Assert.AreEqual("Increasing 20-25.", SetupForExternalTest.realiser.realiseSentence(weather3));

            // conjoined test
            SPhraseSpec weather4 = SetupForExternalTest.phraseFactory.createClause("S 20-25", "back", "SSE");
            weather4.setFeature(Feature.FORM.ToString(), Form.GERUND);
            weather4.getSubject().setFeature(Feature.ELIDED.ToString(), true);

            var coord = new CoordinatedPhraseElement();
            coord.addCoordinate(weather1);
            coord.addCoordinate(weather3);
            coord.addCoordinate(weather4);
            coord.setConjunction("then");
            Assert.AreEqual("SE 10-15 veering S 15-20, increasing 20-25 then backing SSE.",
                SetupForExternalTest.realiser.realiseSentence(coord));


            // no verb
            SPhraseSpec weather5 = SetupForExternalTest.phraseFactory.createClause("rain", null, "likely");
            Assert.AreEqual("Rain likely.", SetupForExternalTest.realiser.realiseSentence(weather5));

        }


        /*[Test]
        public void rafaelTest()
        {
            // Rafael Valle's tests
            List<NLGElement> ss = new List<NLGElement>();
            ClauseCoordinationRule coord = new ClauseCoordinationRule();
            coord.setFactory(SetupForPassive.phraseFactory);

            ss.add(SetupForPassive.agreePhrase("John Lennon")); // john lennon agreed with it  
            ss.add(SetupForPassive.disagreePhrase("Geri Halliwell")); // Geri Halliwell disagreed with it
            ss.add(SetupForPassive.commentPhrase("Melanie B")); // Mealnie B commented on it
            ss.add(SetupForPassive.agreePhrase("you")); // you agreed with it
            ss.add(SetupForPassive.commentPhrase("Emma Bunton")); //Emma Bunton commented on it

            List<NLGElement> results = coord.apply(ss);
            List<String> ret = SetupForPassive.realizeAll(results);
            Assert.AreEqual(
                "[John Lennon and you agreed with it, Geri Halliwell disagreed with it, Melanie B and Emma Bunton commented on it]",
                ret.toString());
        }
		*/
        private NLGElement commentPhrase(String name)
        {
            // used by testRafael
            var s = SetupForExternalTest.phraseFactory.createClause();
            s.setSubject(SetupForExternalTest.phraseFactory.createNounPhrase(name));
            s.setVerbPhrase(SetupForExternalTest.phraseFactory.createVerbPhrase("comment on"));
            s.setObject("it");
            s.setFeature(Feature.TENSE.ToString(), Tense.PAST);
            return s;
        }

        private NLGElement agreePhrase(String name)
        {
            // used by testRafael
            var s = SetupForExternalTest.phraseFactory.createClause();
            s.setSubject(SetupForExternalTest.phraseFactory.createNounPhrase(name));
            s.setVerbPhrase(SetupForExternalTest.phraseFactory.createVerbPhrase("agree with"));
            s.setObject("it");
            s.setFeature(Feature.TENSE.ToString(), Tense.PAST);
            return s;
        }

        private NLGElement disagreePhrase(String name)
        {
            // used by testRafael
            var s = SetupForExternalTest.phraseFactory.createClause();
            s.setSubject(SetupForExternalTest.phraseFactory.createNounPhrase(name));
            s.setVerbPhrase(SetupForExternalTest.phraseFactory.createVerbPhrase("disagree with"));
            s.setObject("it");
            s.setFeature(Feature.TENSE.ToString(), Tense.PAST);
            return s;
        }

        private List<String> realizeAll(List<NLGElement> results)
        {
            // used by testRafael
            var ret = new List<String>();
            foreach (var e in results)
            {
                var r = SetupForExternalTest.realiser.realise(e).getRealisation();
                ret.add(r);
            }
            return ret;
        }

		[Test]
        public void wikipediaTest()
        {
            // test code fragments in wikipedia
            // realisation
            var subject = SetupForExternalTest.phraseFactory.createNounPhrase("the", "woman");
            subject.setPlural(true);
            var sentence = SetupForExternalTest.phraseFactory.createClause(subject, "smoke");
            sentence.setFeature(Feature.NEGATED.ToString(), true);
            Assert.AreEqual("The women do not smoke.", SetupForExternalTest.realiser.realiseSentence(sentence));

            // aggregation
            var s1 = SetupForExternalTest.phraseFactory.createClause("the man", "be", "hungry");
            var s2 = SetupForExternalTest.phraseFactory.createClause("the man", "buy", "an apple");
            var subject2 = SetupForExternalTest.phraseFactory.createCoordinatedPhrase(s1, s2);
            Assert.AreEqual("The man is hungry and buys an apple.", SetupForExternalTest.realiser.realiseSentence(subject2));

        }

        [Test]
        public void leanTest()
        {
            // A Lean's test
            var sentence = SetupForExternalTest.phraseFactory.createClause();
            sentence.setVerb("be");
            sentence.setObject("a ball");
            sentence.setFeature(Feature.INTERROGATIVE_TYPE.ToString(), InterrogativeType.WHAT_SUBJECT);
            Assert.AreEqual("What is a ball?", SetupForExternalTest.realiser.realiseSentence(sentence));

            sentence = SetupForExternalTest.phraseFactory.createClause();
            sentence.setVerb("be");
            var o = SetupForExternalTest.phraseFactory.createNounPhrase("example");
            o.setPlural(true);
            o.addModifier("of jobs");
            sentence.setFeature(Feature.INTERROGATIVE_TYPE.ToString(), InterrogativeType.WHAT_SUBJECT);
            sentence.setObject(o);
            Assert.AreEqual("What are examples of jobs?", SetupForExternalTest.realiser.realiseSentence(sentence));

            var p = SetupForExternalTest.phraseFactory.createClause();
            var sub1 = SetupForExternalTest.phraseFactory.createNounPhrase("Mary");

            sub1.setFeature(LexicalFeature.GENDER, Gender.FEMININE);
            sub1.setFeature(Feature.PRONOMINAL.ToString(), true);
            sub1.setFeature(Feature.PERSON.ToString(), Person.FIRST);
            p.setSubject(sub1);
            p.setVerb("chase");
            p.setObject("the monkey");


            var output2 = SetupForExternalTest.realiser.realiseSentence(p); // Realiser created earlier. 
            Assert.AreEqual("I chase the monkey.", output2);

            var test = SetupForExternalTest.phraseFactory.createClause();
            var subject = SetupForExternalTest.phraseFactory.createNounPhrase("Mary");

            subject.setFeature(Feature.PRONOMINAL.ToString(), true);
            subject.setFeature(Feature.PERSON.ToString(), Person.SECOND);
            test.setSubject(subject);
            test.setVerb("cry");

            test.setFeature(Feature.INTERROGATIVE_TYPE.ToString(), InterrogativeType.WHY);
            test.setFeature(Feature.TENSE.ToString(), Tense.PRESENT);
            Assert.AreEqual("Why do you cry?", SetupForExternalTest.realiser.realiseSentence(test));

            test = SetupForExternalTest.phraseFactory.createClause();
            subject = SetupForExternalTest.phraseFactory.createNounPhrase("Mary");

            subject.setFeature(Feature.PRONOMINAL.ToString(), true);
            subject.setFeature(Feature.PERSON.ToString(), Person.SECOND);
            test.setSubject(subject);
            test.setVerb("be");
            test.setObject("crying");

            test.setFeature(Feature.INTERROGATIVE_TYPE.ToString(), InterrogativeType.WHY);
            test.setFeature(Feature.TENSE.ToString(), Tense.PRESENT);
            Assert.AreEqual("Why are you crying?", SetupForExternalTest.realiser.realiseSentence(test));


        }

        [Test]
        public void kalijurandTest()
        {
            // K Kalijurand's test
            var lemma = "walk";


            var word = SetupForExternalTest.lexicon.lookupWord(lemma, new LexicalCategory_VERB());
            var inflectedWord = new InflectedWordElement(word);

            inflectedWord.setFeature(Feature.FORM.ToString(), Form.PAST_PARTICIPLE);
            var form = SetupForExternalTest.realiser.realise(inflectedWord).getRealisation();
            Assert.AreEqual("walked", form);


            inflectedWord = new InflectedWordElement(word);

            inflectedWord.setFeature(Feature.PERSON.ToString(), Person.THIRD);
            form = SetupForExternalTest.realiser.realise(inflectedWord).getRealisation();
            Assert.AreEqual("walks", form);

        }

        [Test]
        public void layTest()
        {
            // Richard Lay's test
            var lemma = "slap";

            var word = SetupForExternalTest.lexicon.lookupWord(lemma,new LexicalCategory_VERB());
            var inflectedWord = new InflectedWordElement(word);
            inflectedWord.setFeature(Feature.FORM.ToString(), Form.PRESENT_PARTICIPLE);
            var form = SetupForExternalTest.realiser.realise(inflectedWord).getRealisation();
            Assert.AreEqual("slapping", form);


            var v = SetupForExternalTest.phraseFactory.createVerbPhrase("slap");
            v.setFeature(Feature.PROGRESSIVE.ToString(), true);
            var progressive = SetupForExternalTest.realiser.realise(v).getRealisation();
            Assert.AreEqual("is slapping", progressive);
        }
    }
}