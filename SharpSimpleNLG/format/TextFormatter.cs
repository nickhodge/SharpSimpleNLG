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
     * This processing module adds some simple plain text formatting to the
     * SimpleNLG output. This includes the following:
     * <ul>
     * <li>Adding the document title to the beginning of the text.</li>
     * <li>Adding section titles in the relevant places.</li>
     * <li>Adding appropriate new line breaks for ease-of-reading.</li>
     * <li>Adding list items with ' * '.</li>
     * <li>Adding numbers for enumerated lists (e.g., "1.1 - ", "1.2 - ", etc.)</li>
     * </ul>
     * </p>
     * 
     * @author D. Westwater, University of Aberdeen.
     * @version 4.0
     * 
     */

    public class TextFormatter : NLGModule
    {

        private static NumberedPrefix numberedPrefix = new NumberedPrefix();

        public override void initialise()
        {
            // Do nothing
        }

        public override INLGElement realise(INLGElement element)
        {
            INLGElement realisedComponent = null;
            var realisation = new StringBuilder();

            if (element != null)
            {
                var category = element.getCategory();
                List<INLGElement> components = element.getChildren();

                //NB: The order of the if-statements below is important!

                // check if this is a canned text first
                if (element is StringElement)
                {
                    realisation.append(element.getRealisation());

                }
                else if (category is IDocumentCategory)
                {
                    var documentElement = element as DocumentElement;
                    var title = documentElement != null ? documentElement.getTitle() : null;
 
                    switch ((DocumentCategoryEnum)category.enumType)
                    {

                        case DocumentCategoryEnum.DOCUMENT:
                            appendTitle(realisation, title, 2);
                            realiseSubComponents(realisation, components);
                            break;
                        case DocumentCategoryEnum.SECTION:
                            appendTitle(realisation, title, 1);
                            realiseSubComponents(realisation, components);
                            break;
                        case DocumentCategoryEnum.LIST:
                            realiseSubComponents(realisation, components);
                            break;

                        case DocumentCategoryEnum.ENUMERATED_LIST:
                            numberedPrefix.upALevel();
                            if (title != null)
                            {
                                realisation.append(title).append('\n');
                            }

                            if (null != components && 0 < components.size())
                            {

                                realisedComponent = realise(components.get(0));
                                if (realisedComponent != null)
                                {
                                    realisation.append(realisedComponent.getRealisation());
                                }
                                for (var i = 1; i < components.size(); i++)
                                {
                                    if (realisedComponent != null && !realisedComponent.getRealisation().endsWith("\n"))
                                    {
                                        realisation.append(' ');
                                    }
                                    if (components.get(i).getParent().getCategory().enumType == (int)DocumentCategoryEnum.ENUMERATED_LIST)
                                    {
                                        numberedPrefix.increment();
                                    }
                                    realisedComponent = realise(components.get(i));
                                    if (realisedComponent != null)
                                    {
                                        realisation.append(realisedComponent.getRealisation());
                                    }
                                }
                            }

                            numberedPrefix.downALevel();
                            break;

                        case DocumentCategoryEnum.PARAGRAPH:
                            if (null != components && 0 < components.size())
                            {
                                realisedComponent = realise(components.get(0));
                                if (realisedComponent != null)
                                {
                                    realisation.append(realisedComponent.getRealisation());
                                }
                                for (var i = 1; i < components.size(); i++)
                                {
                                    if (realisedComponent != null)
                                    {
                                        realisation.append(' ');
                                    }
                                    realisedComponent = realise(components.get(i));
                                    if (realisedComponent != null)
                                    {
                                        realisation.append(realisedComponent.getRealisation());
                                    }
                                }
                            }
                            realisation.append("\n\n");
                            break;

                        case DocumentCategoryEnum.SENTENCE:
                            realisation.append(element.getRealisation());
                            break;

                        case DocumentCategoryEnum.LIST_ITEM:
                            if (element.getParent() != null)
                            {
                                if (element.getParent().getCategory().enumType == (int)DocumentCategoryEnum.LIST)
                                {
                                    realisation.append(" * ");
                                }
                                else if (element.getParent().getCategory().enumType == (int)DocumentCategoryEnum.ENUMERATED_LIST)
                                {
                                    realisation.append(numberedPrefix.getPrefix() + " - ");
                                }
                            }

                            foreach (var eachComponent in components)
                        {
                            realisedComponent = realise(eachComponent);

                            if (realisedComponent != null)
                            {
                                realisation.append(realisedComponent
                                    .getRealisation());

                                if (components.indexOf(eachComponent) < components.size() - 1)
                                {
                                    realisation.append(' ');
                                }
                            }
                        }
                            //finally, append newline
                            realisation.append("\n");
                            break;
                    }

                    // also need to check if element is a ListElement (items can
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
        }

        /**
         * realiseSubComponents -- Realises subcomponents iteratively.
         * @param realisation -- The current realisation StringBuilder.
         * @param components -- The components to realise.
         */

        private void realiseSubComponents(StringBuilder realisation, List<INLGElement> components)
        {
            INLGElement realisedComponent;
            foreach (var eachComponent in components)
            {
                realisedComponent = realise(eachComponent);
                if (realisedComponent != null)
                {
                    realisation.append(realisedComponent
                        .getRealisation());
                }
            }
        }

        /**
         * appendTitle -- Appends document or section title to the realised document.
         * @param realisation -- The current realisation StringBuilder.
         * @param title -- The title to append.
         * @param numberOfLineBreaksAfterTitle -- Number of line breaks to append.
         */

        private void appendTitle(StringBuilder realisation, string title, int numberOfLineBreaksAfterTitle)
        {
            if (title != null && !title.isEmpty())
            {
                realisation.append(title);
                for (var i = 0; i < numberOfLineBreaksAfterTitle; i++)
                {
                    realisation.append("\n");
                }
            }
        }

        public override List<INLGElement> realise(List<INLGElement> elements)
        {
            var realisedList = new List<INLGElement>();

            if (elements != null)
            {
                foreach (var eachElement in elements)
                {
                    realisedList.add(realise(eachElement));
                }
            }
            return realisedList;
        }
    }
}
