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
 * The Original Code is "Simplenlg".
 *
 * The Initial Developer of the Original Code is Ehud Reiter, Albert Gatt and Dave Westwater.
 * Portions created by Ehud Reiter, Albert Gatt and Dave Westwater are Copyright (C) 2010-11 The University of Aberdeen. All Rights Reserved.
 *
 * Contributor(s): Ehud Reiter, Albert Gatt, Dave Wewstwater, Roman Kutlak, Margaret Mitchell, Saad Mahamood.
 */
package simplenlg.lexicon.english;

import junit.framework.Assert;
import simplenlg.features.Inflection;
import simplenlg.features.LexicalFeature;
import simplenlg.framework.LexicalCategory;
import simplenlg.framework.WordElement;
import simplenlg.lexicon.Lexicon;

/**
 * @author Dave Westwater, Data2Text Ltd
 * 
 */
public class SharedLexiconTests {

	public void doBasicTests(Lexicon lexicon) {
		// test getWords. Should be 2 "can" (of any cat), 1 noun tree, 0 adj
		// trees
		Assert.AreEqual(3, lexicon.getWords("can").size());
		Assert.AreEqual(1, lexicon.getWords("can", LexicalCategory.NOUN)
				.size());
		Assert.AreEqual(0, lexicon.getWords("can",
				LexicalCategory.ADJECTIVE).size());

		// below test removed as standard morph variants no longer recorded in
		// lexicon
		// WordElement early = lexicon.getWord("early",
		// LexicalCategory.ADJECTIVE);
		// Assert.AreEqual("earlier",
		// early.getFeatureAsString(Feature.COMPARATIVE));

		// test getWord. Comparative of ADJ "good" is "better", superlative is
		// "best", this is a qualitative and predicative adjective
		WordElement good = lexicon.getWord("good", LexicalCategory.ADJECTIVE);
		Assert.AreEqual("better", good
				.getFeatureAsString(LexicalFeature.COMPARATIVE));
		Assert.AreEqual("best", good
				.getFeatureAsString(LexicalFeature.SUPERLATIVE));
		Assert.AreEqual(true, good.getFeatureAsBoolean(
				LexicalFeature.QUALITATIVE).booleanValue());
		Assert.AreEqual(true, good.getFeatureAsBoolean(
				LexicalFeature.PREDICATIVE).booleanValue());
		Assert.AreEqual(false, good.getFeatureAsBoolean(
				LexicalFeature.COLOUR).booleanValue());
		Assert.AreEqual(false, good.getFeatureAsBoolean(
				LexicalFeature.CLASSIFYING).booleanValue());

		// test getWord. There is only one "woman", and its plural is "women".
		// It is not an acronym, not proper, and countable
		WordElement woman = lexicon.getWord("woman");

		Assert.AreEqual("women", woman
				.getFeatureAsString(LexicalFeature.PLURAL));
		Assert.AreEqual(null, woman
				.getFeatureAsString(LexicalFeature.ACRONYM_OF));
		Assert.AreEqual(false, woman.getFeatureAsBoolean(
				LexicalFeature.PROPER).booleanValue());
		Assert.assertFalse(woman.hasInflectionalVariant(Inflection.UNCOUNT));

		// NB: This fails if the lexicon is XMLLexicon. No idea why.
		// Assert.AreEqual("irreg",
		// woman.getFeatureAsString(LexicalFeature.DEFAULT_INFL));

		// test getWord. Noun "sand" is non-count
		WordElement sand = lexicon.getWord("sand", LexicalCategory.NOUN);
		Assert.AreEqual(true, sand.hasInflectionalVariant(Inflection.UNCOUNT));
		Assert.AreEqual(Inflection.UNCOUNT, sand.getDefaultInflectionalVariant());

		// test hasWord
		Assert.AreEqual(true, lexicon.hasWord("tree")); // "tree" exists
		Assert.AreEqual(false, lexicon.hasWord("tree",
				LexicalCategory.ADVERB)); // but not as an adverb

		// test getWordByID; quickly, also check that this is a verb_modifier
		WordElement quickly = lexicon.getWordByID("E0051632");
		Assert.AreEqual("quickly", quickly.getBaseForm());
		Assert.AreEqual(LexicalCategory.ADVERB, quickly.getCategory());
		Assert.AreEqual(true, quickly.getFeatureAsBoolean(
				LexicalFeature.VERB_MODIFIER).booleanValue());
		Assert.AreEqual(false, quickly.getFeatureAsBoolean(
				LexicalFeature.SENTENCE_MODIFIER).booleanValue());
		Assert.AreEqual(false, quickly.getFeatureAsBoolean(
				LexicalFeature.INTENSIFIER).booleanValue());

		// test getWordFromVariant, verb type (tran or intran, not ditran)
		WordElement eat = lexicon.getWordFromVariant("eating");
		Assert.AreEqual("eat", eat.getBaseForm());
		Assert.AreEqual(LexicalCategory.VERB, eat.getCategory());
		Assert.AreEqual(true, eat.getFeatureAsBoolean(
				LexicalFeature.INTRANSITIVE).booleanValue());
		Assert.AreEqual(true, eat.getFeatureAsBoolean(
				LexicalFeature.TRANSITIVE).booleanValue());
		Assert.AreEqual(false, eat.getFeatureAsBoolean(
				LexicalFeature.DITRANSITIVE).booleanValue());

		// test BE is handled OK
		Assert.AreEqual("been", lexicon.getWordFromVariant("is",
				LexicalCategory.VERB).getFeatureAsString(
				LexicalFeature.PAST_PARTICIPLE));

		// test modal
		WordElement can = lexicon.getWord("can", LexicalCategory.MODAL);
		Assert.AreEqual("could", can
				.getFeatureAsString(LexicalFeature.PAST));

		// test non-existent word
		Assert.AreEqual(0, lexicon.getWords("akjmchsgk").size());

		// test lookup word method
		Assert.AreEqual(lexicon.lookupWord("say", LexicalCategory.VERB)
				.getBaseForm(), "say");
		Assert.AreEqual(lexicon.lookupWord("said", LexicalCategory.VERB)
				.getBaseForm(), "say");
		Assert.AreEqual(lexicon
				.lookupWord("E0054448", LexicalCategory.VERB).getBaseForm(),
				"say");
	}

}
