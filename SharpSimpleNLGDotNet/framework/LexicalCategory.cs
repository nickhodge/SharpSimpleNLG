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

namespace SimpleNLG
{
    public interface ILexicalCategory: IElementCategory
    {
        LexicalCategoryEnum lexType { get; }
    }

    public class LexicalCategory_ANY : ILexicalCategory
    {
        public LexicalCategoryEnum lexType => LexicalCategoryEnum.ANY;
        public int enumType => (int)lexType;
    }

    public class LexicalCategory_SYMBOL : ILexicalCategory
    {
        public LexicalCategoryEnum lexType => LexicalCategoryEnum.SYMBOL;
        public int enumType => (int)lexType;
    }
    public class LexicalCategory_NOUN : ILexicalCategory
    {
        public LexicalCategoryEnum lexType => LexicalCategoryEnum.NOUN;
        public int enumType => (int)lexType;
    }
    public class LexicalCategory_ADJECTIVE : ILexicalCategory
    {
        public LexicalCategoryEnum lexType => LexicalCategoryEnum.ADJECTIVE;
        public int enumType => (int)lexType;
    }
    public class LexicalCategory_ADVERB : ILexicalCategory
    {
        public LexicalCategoryEnum lexType => LexicalCategoryEnum.ADVERB;
        public int enumType => (int)lexType;
    }
    public class LexicalCategory_VERB : ILexicalCategory
    {
        public LexicalCategoryEnum lexType => LexicalCategoryEnum.VERB;
        public int enumType => (int)lexType;
    }
    public class LexicalCategory_DETERMINER : ILexicalCategory
    {
        public LexicalCategoryEnum lexType => LexicalCategoryEnum.DETERMINER;
        public int enumType => (int)lexType;
    }
    public class LexicalCategory_PRONOUN : ILexicalCategory
    {
        public LexicalCategoryEnum lexType => LexicalCategoryEnum.PRONOUN;
        public int enumType => (int)lexType;
    }
    public class LexicalCategory_CONJUNCTION : ILexicalCategory
    {
        public LexicalCategoryEnum lexType => LexicalCategoryEnum.CONJUNCTION;
        public int enumType => (int)lexType;
    }
    public class LexicalCategory_PREPOSITION : ILexicalCategory
    {
        public LexicalCategoryEnum lexType => LexicalCategoryEnum.PREPOSITION;
        public int enumType => (int)lexType;
    }
    public class LexicalCategory_COMPLEMENTISER : ILexicalCategory
    {
        public LexicalCategoryEnum lexType => LexicalCategoryEnum.COMPLEMENTISER;
        public int enumType => (int)lexType;
    }
    public class LexicalCategory_MODAL : ILexicalCategory
    {
        public LexicalCategoryEnum lexType => LexicalCategoryEnum.MODAL;
        public int enumType => (int)lexType;
    }
    public class LexicalCategory_AUXILIARY : ILexicalCategory
    {
        public LexicalCategoryEnum lexType => LexicalCategoryEnum.AUXILIARY;
        public int enumType => (int)lexType;
    }
    public enum LexicalCategoryEnum //implements ElementCategory
    {
        _BASE = 4000,

        /** A default value, indicating an unspecified category. */
        ANY = 4001,

        /** The element represents a symbol. */
        SYMBOL = 4002,
        
         /** A noun element. */
        NOUN = 4003,

        /** An adjective element. */
        ADJECTIVE = 4004,

        /** An adverb element. */
        ADVERB = 4005,

        /** A verb element. */
        VERB = 4006,

        /** A determiner element often referred to as a specifier. */
        DETERMINER = 4007,

        /** A pronoun element. */
        PRONOUN = 4008,

        /** A conjunction element. */
        CONJUNCTION = 4009,

        /** A preposition element. */
        PREPOSITION = 4010,

        /** A complementiser element. */
        COMPLEMENTISER = 4011,

        /** A modal element. */
        MODAL = 4012,

        /** An auxiliary verb element. */
        AUXILIARY =4013,
    }

    public static class LexicalCategoryExtensions
    {
        public static IElementCategory valueOf(string s)
        {
            var lc = (LexicalCategoryEnum)Enum.Parse(typeof(LexicalCategoryEnum), s);
            switch (lc)
            {
                 case LexicalCategoryEnum.SYMBOL:
                    return new LexicalCategory_SYMBOL();
                case LexicalCategoryEnum.NOUN:
                    return new LexicalCategory_NOUN();
                case LexicalCategoryEnum.ADJECTIVE:
                    return new LexicalCategory_ADJECTIVE();
                case LexicalCategoryEnum.ADVERB:
                    return new LexicalCategory_ADVERB();
                case LexicalCategoryEnum.VERB:
                    return new LexicalCategory_VERB();
                case LexicalCategoryEnum.DETERMINER:
                    return new LexicalCategory_DETERMINER();
                case LexicalCategoryEnum.PRONOUN:
                    return new LexicalCategory_PRONOUN();
                case LexicalCategoryEnum.CONJUNCTION:
                    return new LexicalCategory_CONJUNCTION();
                case LexicalCategoryEnum.PREPOSITION:
                    return new LexicalCategory_PREPOSITION();
                case LexicalCategoryEnum.COMPLEMENTISER:
                    return new LexicalCategory_COMPLEMENTISER();
                case LexicalCategoryEnum.MODAL:
                    return new LexicalCategory_MODAL();
                case LexicalCategoryEnum.AUXILIARY:
                    return new LexicalCategory_AUXILIARY();
                default:
                    return new LexicalCategory_ANY();
            }

        }
    }

}
