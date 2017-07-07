using NUnit.Framework;
using SimpleNLG;

namespace SimpleNLGTests.syntax
{

    /**
     * test suite for simple XXXPhraseSpec classes
     * @author ereiter
     * 
     */

    public class PhraseSpecTest : SimpleNLG4TestBase
    {



        /**
         * Check that empty phrases are not realised as "null"
         */

        [Test]
        public void emptyPhraseRealisationTest()
        {
            var emptyClause = this.phraseFactory.createClause();
            Assert.AreEqual("", this.realiser.realise(emptyClause)
                .getRealisation());
        }


        /**
         * Test SPhraseSpec
         */

        [Test]
        public void testSPhraseSpec()
        {

            // simple test of methods
            var c1 = (SPhraseSpec) this.phraseFactory.createClause();
            c1.setVerb("give");
            c1.setSubject("John");
            c1.setObject("an apple");
            c1.setIndirectObject("Mary");
            c1.setFeature(Feature.TENSE.ToString(), Tense.PAST);
            c1.setFeature(Feature.NEGATED.ToString(), true);

            // check getXXX methods
            Assert.AreEqual("give", getBaseForm(c1.getVerb()));
            Assert.AreEqual("John", getBaseForm(c1.getSubject()));
            Assert.AreEqual("an apple", getBaseForm(c1.getObject()));
            Assert.AreEqual("Mary", getBaseForm(c1.getIndirectObject()));

            Assert.AreEqual("John did not give Mary an apple", this.realiser 
             .realise(c1).getRealisation());



            // test modifier placement
            var c2 = this.phraseFactory.createClause();
            c2.setVerb("see");
            c2.setSubject("the man");
            c2.setObject("me");
            c2.addModifier("fortunately");
            c2.addModifier("quickly");
            c2.addModifier("in the park");
            // try setting tense directly as a feature
            c2.setFeature(Feature.TENSE.ToString(), Tense.PAST);
            this.realiser.setDebugMode(true);
            Assert.AreEqual("fortunately the man quickly saw me in the park",
                this.realiser 
                    .realise(c2).getRealisation());
        }

        // get string for head of constituent
        private string getBaseForm(INLGElement constituent)
        {
            if (constituent == null)
                return null;
            else if (constituent is StringElement)
                return constituent.getRealisation();
            else if (constituent is WordElement)
                return ((WordElement) constituent).getBaseForm();
            else if (constituent is InflectedWordElement)
                return getBaseForm(((InflectedWordElement) constituent).getBaseWord());
            else if (constituent is PhraseElement)
                return getBaseForm(((PhraseElement) constituent).getHead());
            else
                return null;
        }
    }
}
