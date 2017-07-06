using SimpleNLG;
using System;
using System.Collections.Generic;

namespace SharpSimpleNLGConsoleNetCore
{
    class Program
    {
        public static void Main(string[] args)
        {
            var ss = new XMLLexicon();
            var Factory = new NLGFactory(ss);
            var Realiser = new Realiser(ss);

            // Instructions will be given to you by the director.

            var verbp = Factory.createVerbPhrase("be given");
            verbp.setFeature(Feature.TENSE.ToString(), Tense.FUTURE);
            var subj = Factory.createNounPhrase("The Director");
            var oobj = Factory.createNounPhrase("Instruction");
            var ioobj = Factory.createNounPhrase("you");
            subj.setPlural(false);
            oobj.setPlural(true);

            var s = new List<INLGElement>() { verbp, subj, oobj, ioobj };

            var clause = Factory.createClause();

            clause.setVerb(verbp);
            clause.setSubject(subj);
            clause.setObject(oobj);
            clause.setIndirectObject(ioobj);


            var sentence = Factory.createSentence(clause);
            sentence.setFeature(Feature.TENSE.ToString(), Tense.FUTURE);

            var active = Realiser.realise(sentence).ToString();

            Console.WriteLine($"{active}");

            Console.WriteLine("done");
            Console.ReadLine();
        }
    }
}