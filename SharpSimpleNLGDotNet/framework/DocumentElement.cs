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
    public class DocumentElement  : NLGElement
    {

    /** The feature relating to the title or heading of this element. */
    private static string FEATURE_TITLE = "textTitle"; 

    /** The feature relating to the components (or child nodes) of this element. */
    private static string FEATURE_COMPONENTS = "textComponents"; 

    /**
     * The blank constructor. Using this constructor will require manual setting
     * of the element's category and title.
     */

    public DocumentElement()
    {
        // Do nothing
    }

    /**
     * Creates a new DocumentElement with the given category and title.
     * 
     * @param category
     *            the category for this element.
     * @param textTitle
     *            the title of this element, predominantly used with DOCUMENT
     *            and SECTION types.
     */

    public DocumentElement(IDocumentCategory category, string textTitle)
    {
        this.setCategory(category);
        setTitle(textTitle);
    }

    /**
     * Sets the title of this element. Titles are specifically used with
     * documents (the document name) and sections (headings).
     * 
     * @param textTitle
     *            the new title for this element.
     */

    public void setTitle(string textTitle)
    {
        this.setFeature(FEATURE_TITLE, textTitle);
    }

    /**
     * Retrieves the title of this element.
     * 
     * @return the title of this element as a string.
     */

    public string getTitle()
    {
        return this.getFeatureAsString(FEATURE_TITLE);
    }

    /**
     * Retrieves the child components of this element.
     * 
     * @return a <code>List</code> of <code>NLGElement</code>s representing the
     *         child components.
     */

    public List<INLGElement> getComponents()
    {
        return this.getFeatureAsElementList(FEATURE_COMPONENTS);
    }

    /**
     * <p>
     * Add a single child component to the current list of child components. If
     * there are no existing child components a new list is created.
     * </p>
     * <p>
     * Note that there are restrictions on which child types can be added to
     * which parent types.  Intermediate nodes are added if necessary; eg,
     * if a sentence is added to a document, the sentence will be embedded
     * in a paragraph before it is added
     * See <code>
     * DocumentCategory</code> for further information.
     * </p>
     * 
     * @param element
     *            the <code>NLGElement</code> to be added. If this is
     *            <code>NULL</code> the method does nothing.
     */

    public void addComponent(INLGElement element)
    {
        if (element != null)
        {
            var thisCategory = this.getCategory();
            var category = element.getCategory();
            if (category != null && thisCategory is IDocumentCategory)
            {
                if (((IDocumentCategory) thisCategory).hasSubPart(category))
                {
                    addElementToComponents(element);
                }
                else
                {
                    var promotedElement = promote(element);
                    if (promotedElement != null)
                        addElementToComponents(promotedElement);
                    else // error condition - add original element so something is visible
                        addElementToComponents(element);
                }
            }
            else
            {
                addElementToComponents(element);
            }
        }
    }

    /** add an element to a components list
     * @param element
     */

    private void addElementToComponents(INLGElement element)
    {
        var components = getComponents();
        components.Add(element);
        element.setParent(this);
        setComponents(components);
    }


    /** promote an NLGElement so that it is at the right level to be added to a DocumentElement/
     * Promotion means adding surrounding nodes at higher doc levels
     * @param element
     * @return
     */

    private INLGElement promote(INLGElement element)
    {
        // check if promotion needed
        if (((IDocumentCategory) this.getCategory()).hasSubPart(element.getCategory()))
        {
            return element;
        }
        // if element is not a DocumentElement, embed it in a sentence and recurse
        if (!(element is DocumentElement))
        {
            var sentence = new DocumentElement(new DocumentCategory_SENTENCE(), null);
            sentence.addElementToComponents(element);
            return promote(sentence);
        }

            // if element is a Sentence, promote it to a paragraph
        if (!(element is IDocumentCategory))
        {
            if (element.getCategory().enumType == (int)DocumentCategoryEnum.SENTENCE)
            {
                var paragraph = new DocumentElement(new DocumentCategory_PARAGRAPH(), null);
                paragraph.addElementToComponents(element);
                return promote(paragraph);
            }
        }
        // otherwise can't do anything
        return null;
    }

    /**
     * <p>
     * Adds a collection of <code>NLGElements</code> to the list of child
     * components. If there are no existing child components, then a new list is
     * created.
     * </p>
     * <p>
     * As with <code>addComponents(...)</code> only legitimate child types are
     * added to the list.
     * </p>
     * 
     * @param textComponents
     *            the <code>List</code> of <code>NLGElement</code>s to be added.
     *            If this is <code>NULL</code> the method does nothing.
     */

    public void addComponents(List<INLGElement> textComponents)
    {
        if (textComponents != null)
        {
            var thisCategory = this.getCategory();
            var elementsToAdd = new List<INLGElement>();
            IElementCategory category = null;

            foreach (var eachElement in textComponents)
            {
                category = ((INLGElement) eachElement).getCategory();
                if (category != null && thisCategory is IDocumentCategory)
                {
                    if (((IDocumentCategory) thisCategory).hasSubPart(category))
                    {
                        elementsToAdd.add((INLGElement) eachElement);
                        ((INLGElement) eachElement).setParent(this);
                    }
                }
            }
            if (elementsToAdd.Count > 0)
            {
                var components = getComponents();
                if (components == null)
                {
                    components = new List<INLGElement>();
                }
                components.AddRange(elementsToAdd);
                this.setFeature(FEATURE_COMPONENTS, components);
            }
        }
    }

    /**
     * Removes the specified component from the list of child components.
     * 
     * @param textComponent
     *            the component to be removed.
     * @return <code>true</code> if the element was removed, or
     *         <code>false</code> if the element did not exist, there is no
     *         component list or the the given component to remove is
     *         <code>NULL</code>.
     */

    public bool removeComponent(INLGElement textComponent)
    {
        var removed = false;

        if (textComponent != null)
        {
            var components = getComponents();
            if (components != null)
            {
                removed = components.remove(textComponent);
            }
        }
        return removed;
    }

    /**
     * Removes all the child components from this element.
     */

    public void clearComponents()
    {
        var components = getComponents();
        if (components != null)
        {
            components.clear();
        }
    }

    /**
     * Child elements of a <code>DocumentElement</code> are the components. This
     * method is the same as calling <code>getComponents()</code>.
     */


    public override List<INLGElement> getChildren()
    {
        return getComponents();
    }

    /**
     * Replaces the existing components with the supplied list of components.
     * This is identical to calling:<br>
     * <code><pre>
     *     clearComponents();
     *     addComponents(components);
     * </pre></code>
     * 
     * @param components
     */

    public void setComponents(List<INLGElement> components)
    {
        this.setFeature(FEATURE_COMPONENTS, components);
    }

    public override string printTree(string indent)
    {
        var thisIndent = indent == null ? " |-" : indent + " |-";  
        var childIndent = indent == null ? " | " : indent + " | ";  
        var lastIndent = indent == null ? " \\-" : indent + " \\-";  
        var lastChildIndent = indent == null ? "   " : indent + "   ";  
        var print = new StringBuilder();
        print.Append("DocumentElement: category=").Append(  
            getCategory().ToString());

        var realisation = getRealisation();
        if (realisation != null)
        {
            print.Append(" realisation=").Append(realisation); 
        }
        print.Append('\n');

        List<INLGElement> children = getChildren();
        var length = children.Count - 1;
        var index = 0;

        if (children.Count > 0)
        {
            for (index = 0; index < length; index++)
            {
                print.Append(thisIndent).Append(
                    children[index].printTree(childIndent));
            }
            print.Append(lastIndent).Append(
                children[index].printTree(lastChildIndent));
        }
        return print.ToString();
    }
    }
}