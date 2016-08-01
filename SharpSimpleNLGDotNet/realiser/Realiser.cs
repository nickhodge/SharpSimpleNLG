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

using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using SimpleNLG.Extensions;

namespace SimpleNLG
{
    /**
     * @author D. Westwater, Data2Text Ltd
     * 
     */

    public class Realiser : NLGModule
    {

        private MorphologyProcessor morphology = new MorphologyProcessor();
        private OrthographyProcessor orthography = new OrthographyProcessor();
        private SyntaxProcessor syntax = new SyntaxProcessor();
        private NLGModule formatter = new TextFormatter();
        private bool debug = false;

        /**
         * create a realiser (no lexicon)
         */

        public Realiser()
        {
            initialise();
        }

        /**
         * Create a realiser with a lexicon (should match lexicon used for
         * NLGFactory)
         * 
         * @param lexicon
         */

        public Realiser(Lexicon lexicon)
        {
            initialise();
            setLexicon(lexicon);
        }

        /**
         * Check whether this processor separates premodifiers using a comma.
         * 
         * <br/>
         * <strong>Implementation note:</strong> this method checks whether the
         * {@link simplenlg.orthography.english.OrthographyProcessor} has the
         * parameter set.
         * 
         * @return <code>true</code> if premodifiers in the noun phrase are
         *         comma-separated.
         */

        public bool isCommaSepPremodifiers()
        {
            return orthography == null ? false : orthography.isCommaSepPremodifiers();
        }

        /**
         * Set whether to separate premodifiers using a comma. If <code>true</code>,
         * premodifiers will be comma-separated, as in <i>the long, dark road</i>.
         * If <code>false</code>, they won't. <br/>
         * <strong>Implementation note:</strong>: this method sets the relevant
         * parameter in the
         * {@link simplenlg.orthography.english.OrthographyProcessor}.
         * 
         * @param commaSepPremodifiers
         *            the commaSepPremodifiers to set
         */

        public void setCommaSepPremodifiers(bool commaSepPremodifiers)
        {
            if (orthography != null)
            {
                orthography.setCommaSepPremodifiers(commaSepPremodifiers);
            }
        }

        /**
         * Check whether this processor separates cue phrases from the matrix clause using a comma.
         * 
         * <br/>
         * <strong>Implementation note:</strong> this method checks whether the
         * {@link simplenlg.orthography.english.OrthographyProcessor} has the
         * parameter set.
         * 
         * @return <code>true</code> if cue phrases have a comma before the remainder of the host phrase
         */

        public bool isCommaSepCuephrase()
        {
            return orthography == null ? false : orthography.isCommaSepCuephrase();
        }

        /**
         * Set whether to separate cue phrases from the host phrase using a comma. If <code>true</code>,
         * a comma will be inserted, as in <i>however, Bill arrived late</i>.
         * If <code>false</code>, they won't. <br/>
         * <strong>Implementation note:</strong>: this method sets the relevant
         * parameter in the
         * {@link simplenlg.orthography.english.OrthographyProcessor}.
         * 
         * @param commaSepcuephrase
         */

        public void setCommaSepCuephrase(bool commaSepCuephrase)
        {
            if (orthography != null)
            {
                orthography.setCommaSepCuephrase(commaSepCuephrase);
            }
        }

        public override void initialise()
        {
            morphology.initialise();
            orthography.initialise();
            syntax.initialise();
            // AG: added call to initialise for formatter
            formatter.initialise();
        }

        public override INLGElement realise(INLGElement element)
        {

            var debug = new StringBuilder();

            if (this.debug)
            {
                Debug.WriteLine("INITIAL TREE\n"); 
                Debug.WriteLine(element.printTree(null));
                debug.append("INITIAL TREE<br/>");
                debug.append(element.printTree("&nbsp;&nbsp;").replaceAll(@"\n", "<br/>"));
            }

            var postSyntax = syntax.realise(element);
            if (this.debug)
            {
                Debug.WriteLine("<br/>POST-SYNTAX TREE<br/>"); 
                Debug.WriteLine(postSyntax.printTree(null));
                debug.append("<br/>POST-SYNTAX TREE<br/>");
                debug.append(postSyntax.printTree("&nbsp;&nbsp;").replaceAll(@"\n", "<br/>"));
            }

            var postMorphology = morphology.realise(postSyntax);
            if (this.debug)
            {
                Debug.WriteLine("\nPOST-MORPHOLOGY TREE\n"); 
                Debug.WriteLine(postMorphology.printTree(null));
                debug.append("<br/>POST-MORPHOLOGY TREE<br/>");
                debug.append(postMorphology.printTree("&nbsp;&nbsp;").replaceAll(@"\n", "<br/>"));
            }

            var postOrthography = orthography.realise(postMorphology);
            if (this.debug)
            {
                Debug.WriteLine("\nPOST-ORTHOGRAPHY TREE\n"); 
                Debug.WriteLine(postOrthography.printTree(null));
                debug.append("<br/>POST-ORTHOGRAPHY TREE<br/>");
                debug.append(postOrthography.printTree("&nbsp;&nbsp;").replaceAll(@"\n", "<br/>"));
            }

            INLGElement postFormatter = null;
            if (formatter != null)
            {
                postFormatter = formatter.realise(postOrthography);
                if (this.debug)
                {
                    Debug.WriteLine("\nPOST-FORMATTER TREE\n"); 
                    Debug.WriteLine(postFormatter.printTree(null));
                    debug.append("<br/>POST-FORMATTER TREE<br/>");
                    debug.append(postFormatter.printTree("&nbsp;&nbsp;").replaceAll(@"\n", "<br/>"));
                }

            }
            else
            {
                postFormatter = postOrthography;
            }

            if (this.debug)
            {
                postFormatter.setFeature("debug", debug.ToString());
            }

            return postFormatter;
        }

        /**
         * Convenience class to realise any NLGElement as a sentence
         * 
         * @param element
         * @return string realisation of the NLGElement
         */

        public string realiseSentence(INLGElement element)
        {
            INLGElement realised = null;
            if (element is DocumentElement)
                realised = realise(element);
            else
            {
                var sentence = new DocumentElement(new DocumentCategory_SENTENCE(), null);
                sentence.addComponent(element);
                realised = realise(sentence);
            }

            if (realised == null)
                return null;
            else
                return realised.getRealisation();
        }


        public override List<INLGElement> realise(List<INLGElement> elements)
        {
            var realisedElements = new List<INLGElement>();
            if (null != elements)
            {
                foreach (var element in elements)
                {
                    var realisedElement = realise(element);
                    realisedElements.add(realisedElement);
                }
            }
            return realisedElements;
        }

        public void setLexicon(Lexicon newLexicon)
        {
            syntax.setLexicon(newLexicon);
            morphology.setLexicon(newLexicon);
            orthography.setLexicon(newLexicon);
        }

        public void setFormatter(NLGModule formatter)
        {
            this.formatter = formatter;
        }

        public void setDebugMode(bool debugOn)
        {
            debug = debugOn;
        }
    }
}
