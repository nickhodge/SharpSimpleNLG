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

namespace SimpleNLG
{
    public interface IDocumentCategory: IElementCategory
    {
        DocumentCategoryEnum docEType { get; }
    }


    public static class DocumentCategoryExtensions {
        public static bool hasSubPart(this IElementCategory sourceElementCategory, IElementCategory elementCategory)
        {
            var subPart = false;
            if (elementCategory != null)
            {
                if (elementCategory is IDocumentCategory)
                {
                    switch (sourceElementCategory.enumType)
                    {
                        case (int) DocumentCategoryEnum.DOCUMENT:
                            subPart = !(elementCategory.enumType == (int)DocumentCategoryEnum.DOCUMENT
                            && elementCategory.enumType == (int)DocumentCategoryEnum.LIST_ITEM);
                            break;

                        case (int) DocumentCategoryEnum.SECTION:
                            subPart = elementCategory.enumType == (int)DocumentCategoryEnum.PARAGRAPH
                            || elementCategory.enumType == (int)DocumentCategoryEnum.SECTION;
                            break;

                        case (int) DocumentCategoryEnum.PARAGRAPH:
                            subPart = elementCategory.enumType == (int)DocumentCategoryEnum.SENTENCE
                            || elementCategory.enumType == (int)DocumentCategoryEnum.LIST;
                            break;

                        case (int) DocumentCategoryEnum.LIST:
                            subPart = elementCategory.enumType == (int)DocumentCategoryEnum.LIST_ITEM;
                            break;
                        case (int) DocumentCategoryEnum.ENUMERATED_LIST:
                            subPart = elementCategory.enumType == (int)DocumentCategoryEnum.LIST_ITEM;
                            break;

                    }
                }
                else
                {
                    subPart = elementCategory.enumType == (int)DocumentCategoryEnum.SENTENCE
                            || elementCategory.enumType == (int)DocumentCategoryEnum.LIST_ITEM;
                }
            }
            return subPart;
        }
    }

    public class DocumentCategory_DOCUMENT : IDocumentCategory
    {
        public DocumentCategoryEnum docEType => DocumentCategoryEnum.DOCUMENT;
        public int enumType => (int)docEType;
    }

    public class DocumentCategory_SECTION : IDocumentCategory
    {
        public DocumentCategoryEnum docEType => DocumentCategoryEnum.SECTION;
        public int enumType => (int)docEType;
    }

    public class DocumentCategory_PARAGRAPH : IDocumentCategory
    {
        public DocumentCategoryEnum docEType => DocumentCategoryEnum.PARAGRAPH;
        public int enumType => (int)docEType;
    }

    public class DocumentCategory_SENTENCE : IDocumentCategory
    {
        public DocumentCategoryEnum docEType => DocumentCategoryEnum.SENTENCE;
        public int enumType => (int)docEType;
    }

    public class DocumentCategory_LIST : IDocumentCategory
    {
        public DocumentCategoryEnum docEType => DocumentCategoryEnum.LIST;
        public int enumType => (int)docEType;
    }

    public class DocumentCategory_ENUMERATED_LIST : IDocumentCategory
    {
        public DocumentCategoryEnum docEType => DocumentCategoryEnum.ENUMERATED_LIST;
        public int enumType => (int)docEType;
    }

    public class DocumentCategory_LIST_ITEM : IDocumentCategory
    {
        public DocumentCategoryEnum docEType => DocumentCategoryEnum.LIST_ITEM;
        public int enumType => (int)docEType;
    }

    public enum DocumentCategoryEnum
    {
        _BASE = 3000,

        /** Definition for a document. */
        DOCUMENT = 3001,

        /** Definition for a section within a document. */
        SECTION = 3002,

        /** Definition for a paragraph. */
        PARAGRAPH = 3003,
        /** Definition for a sentence. */
        SENTENCE = 3004,

        /** Definition for creating a list of items. */
        LIST = 3005,

        /** Definition for creating a list of enumerated items. */
        ENUMERATED_LIST = 3006,

        /** Definition for an item in a list. */
        LIST_ITEM = 3007,
    }

 
}