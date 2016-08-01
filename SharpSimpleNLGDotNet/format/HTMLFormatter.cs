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
using System.Text;
using SimpleNLG.Extensions;

namespace SimpleNLG
{

    /**
     * <p>
     * This processing module adds some simple plain HTML formatting to the
     * SimpleNLG output. This includes the following:
     * <ul>
     * <li>Adding the document title to the beginning of the text.</li>
     * <li>Adding section titles in the relevant places.</li>
     * <li>Adding appropriate new line breaks for ease-of-reading.</li>
     * <li>Indenting list items with ' * '.</li>
     * </ul>
     * </p>
     * 
     * @author D. Westwater, University of Aberdeen ~ for the TextFormatter; 
     * 		   <br />J Christie, University of Aberdeen ~ for HTMLFormatter 
     * @version 4.0 original TextFormatter Version; <br />version 0.0 HTMLFormatter
     * 
     */

    //public class TextFormatter extends NLGModule {
    public class HTMLFormatter : NLGModule
    {

        // Modifications by James Christie to convert TextFormatter into a HTML Formatter


        public override void initialise()
        {

        } // constructor


        public override INLGElement realise(INLGElement element)
        {
            // realise a single element
            INLGElement realisedComponent = null;
            var realisation = new StringBuilder();

            if (element != null)
            {
                var category = element.getCategory();
                var components = element.getChildren();

                //NB: The order of the if-statements below is important!

                // check if this is a canned text first
                if (element is StringElement)
                {
                    realisation.append(element.getRealisation());

                }
                else if (category is IDocumentCategory)
                {
                    // && element instanceof DocumentElement

                    switch ((DocumentCategoryEnum) category.enumType)
                    {
                        case DocumentCategoryEnum.DOCUMENT:
                            var title = element is DocumentElement ? ((DocumentElement) element).getTitle() : null;
                            realisation.append("<h1>" + title + "</h1>");

                            foreach (var eachComponent in components)
                            {
                                realisedComponent = realise(eachComponent);
                                if (realisedComponent != null)
                                {
                                    realisation.append(realisedComponent.getRealisation());
                                }
                            }

                            break;

                        case DocumentCategoryEnum.SECTION:
                            title = element is DocumentElement ? ((DocumentElement) element).getTitle() : null;

                            if (title != null)
                            {
                                var sectionTitle = ((DocumentElement) element).getTitle();
                                realisation.append("<h2>" + sectionTitle + "</h2>");
                            }

                            foreach (var eachComponent in components)
                            {
                                realisedComponent = realise(eachComponent);
                                if (realisedComponent != null)
                                {
                                    realisation.append(realisedComponent.getRealisation());
                                }
                            }
                            break;

                        case DocumentCategoryEnum.LIST:
                            realisation.append("<ul>");
                            foreach (var eachComponent in components)
                            {
                                realisedComponent = realise(eachComponent);
                                if (realisedComponent != null)
                                {
                                    realisation.append(realisedComponent.getRealisation());
                                }
                            }
                            realisation.append("</ul>");
                            break;

                        case DocumentCategoryEnum.ENUMERATED_LIST:
                            realisation.append("<ol>");
                            foreach (var eachComponent in components)
                            {
                                realisedComponent = realise(eachComponent);
                                if (realisedComponent != null)
                                {
                                    realisation.append(realisedComponent.getRealisation());
                                }
                            }
                            realisation.append("</ol>");
                            break;

                        case DocumentCategoryEnum.PARAGRAPH:
                            if (null != components && 0 < components.size())
                            {
                                realisedComponent = realise(components.get(0));
                                if (realisedComponent != null)
                                {
                                    realisation.append("<p>");
                                    realisation.append(realisedComponent.getRealisation());
                                }
                                for (var i = 1; i < components.size(); i++)
                                {
                                    if (realisedComponent != null)
                                    {
                                        realisation.append(" ");
                                    }
                                    realisedComponent = realise(components.get(i));
                                    if (realisedComponent != null)
                                    {
                                        realisation.append(realisedComponent.getRealisation());
                                    }
                                }
                                realisation.append("</p>");
                            }

                            break;

                        case DocumentCategoryEnum.SENTENCE:
                            realisation.append(element.getRealisation());
                            break;

                        case DocumentCategoryEnum.LIST_ITEM:
                            realisation.append("<li>");

                            foreach (var eachComponent in components)
                            {
                                realisedComponent = realise(eachComponent);

                                if (realisedComponent != null)
                                {
                                    realisation.append(realisedComponent.getRealisation());

                                    if (components.indexOf(eachComponent) < components.size() - 1)
                                    {
                                        realisation.append(' ');
                                    }
                                }
                            }
                            realisation.append("</li>");

                            break;

                    }

                    // also need to check if element is a listelement (items can
                    // have embedded lists post-orthography) or a coordinate
                }
                else if (element is ListElement || element is CoordinatedPhraseElement)
                {

                    foreach (var eachComponent in components)
                    {
                        realisedComponent = realise(eachComponent);
                        if (realisedComponent != null)
                        {
                            realisation.append(realisedComponent.getRealisation()).append(' ');
                        }
                    }
                }
            }

            return new StringElement(realisation.ToString());
        } // realise ~ single element


        public override List<INLGElement> realise(List<INLGElement> elements)
        {
            // realise a list of elements
            var realisedList = new List<INLGElement>();

            if (elements != null)
            {
                foreach (var eachElement in elements)
                {
                    realisedList.add(realise(eachElement));
                }
            }
            return realisedList;
        } // realise ~ list of elements

    } // class
}