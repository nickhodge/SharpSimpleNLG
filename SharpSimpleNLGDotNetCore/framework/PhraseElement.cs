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
    public abstract class PhraseElement : NLGElement
    {

        /**
         * Creates a new phrase of the given type.
         * 
         * @param newCategory
         *            the <code>PhraseCategory</code> type for this phrase.
         */

        protected PhraseElement(IPhraseCategory newCategory)
        {
            base.setCategory(newCategory);

            // set default feature value
            base.setFeature(Feature.ELIDED.ToString(), false);
        }

        /**
         * <p>
         * This method retrieves the child components of this phrase. The list
         * returned will depend on the category of the element.<br>
         * <ul>
         * <li>Clauses consist of cue phrases, front modifiers, pre-modifiers,
         * subjects, verb phrases and complements.</li>
         * <li>Noun phrases consist of the specifier, pre-modifiers, the noun
         * subjects, complements and post-modifiers.</li>
         * <li>Verb phrases consist of pre-modifiers, the verb group, complements
         * and post-modifiers.</li>
         * <li>Canned text phrases have no children thus an empty list will be
         * returned.</li>
         * <li>All the other phrases consist of pre-modifiers, the main phrase
         * element, complements and post-modifiers.</li>
         * </ul>
         * </p>
         * 
         * @return a <code>List</code> of <code>NLGElement</code>s representing the
         *         child elements of this phrase.
         */

        public override List<INLGElement> getChildren()
        {
            var children = new List<INLGElement>();
            var category = getCategory();
            INLGElement currentElement = null;

            if (category is IPhraseCategory)
            {
                switch ((PhraseCategoryEnum) category.enumType)
                {
                    case PhraseCategoryEnum.CLAUSE:
                        currentElement = getFeatureAsElement(Feature.CUE_PHRASE.ToString());
                        if (currentElement != null)
                        {
                            children.add(currentElement);
                        }
                        children
                            .addAll(getFeatureAsElementList(InternalFeature.FRONT_MODIFIERS.ToString()));
                        children
                            .addAll(getFeatureAsElementList(InternalFeature.PREMODIFIERS.ToString()));
                        children
                            .addAll(getFeatureAsElementList(InternalFeature.SUBJECTS.ToString()));
                        children
                            .addAll(getFeatureAsElementList(InternalFeature.VERB_PHRASE.ToString()));
                        children
                            .addAll(getFeatureAsElementList(InternalFeature.COMPLEMENTS.ToString()));
                        break;

                    case PhraseCategoryEnum.NOUN_PHRASE:
                        currentElement = getFeatureAsElement(InternalFeature.SPECIFIER.ToString());
                        if (currentElement != null)
                        {
                            children.add(currentElement);
                        }
                        children
                            .addAll(getFeatureAsElementList(InternalFeature.PREMODIFIERS.ToString()));
                        currentElement = getHead();
                        if (currentElement != null)
                        {
                            children.add(currentElement);
                        }
                        children
                            .addAll(getFeatureAsElementList(InternalFeature.COMPLEMENTS.ToString()));
                        children
                            .addAll(getFeatureAsElementList(InternalFeature.POSTMODIFIERS.ToString()));
                        break;

                    case PhraseCategoryEnum.VERB_PHRASE:
                        children
                            .addAll(getFeatureAsElementList(InternalFeature.PREMODIFIERS.ToString()));
                        currentElement = getHead();
                        if (currentElement != null)
                        {
                            children.add(currentElement);
                        }
                        children
                            .addAll(getFeatureAsElementList(InternalFeature.COMPLEMENTS.ToString()));
                        children
                            .addAll(getFeatureAsElementList(InternalFeature.POSTMODIFIERS.ToString()));
                        break;

                    case PhraseCategoryEnum.CANNED_TEXT:
                        // Do nothing
                        break;

                    default:
                        children
                            .addAll(getFeatureAsElementList(InternalFeature.PREMODIFIERS.ToString()));
                        currentElement = getHead();
                        if (currentElement != null)
                        {
                            children.add(currentElement);
                        }
                        children
                            .addAll(getFeatureAsElementList(InternalFeature.COMPLEMENTS.ToString()));
                        children
                            .addAll(getFeatureAsElementList(InternalFeature.POSTMODIFIERS.ToString()));
                        break;
                }
            }
            return children;
        }

        /**
         * Sets the head, or main component, of this current phrase. For example,
         * the head for a verb phrase should be a verb while the head of a noun
         * phrase should be a noun. <code>string</code>s are converted to
         * <code>StringElement</code>s. If <code>null</code> is passed in as the new
         * head then the head feature is removed.
         * 
         * @param newHead
         *            the new value for the head of this phrase.
         */

        public virtual void setHead(object newHead)
        {
            if (newHead == null)
            {
                removeFeature(InternalFeature.HEAD.ToString());
                return;
            }
            INLGElement headElement;
            if (newHead is INLGElement)
                headElement = (INLGElement) newHead;
            else
                headElement = new StringElement(newHead.ToString());

            setFeature(InternalFeature.HEAD.ToString(), headElement);

        }

        /**
         * Retrieves the current head of this phrase.
         * 
         * @return the <code>NLGElement</code> representing the head.
         */

        public virtual INLGElement getHead()
        {
            return getFeatureAsElement(InternalFeature.HEAD.ToString());
        }

        /**
         * <p>
         * Adds a new complement to the phrase element. Complements will be realised
         * in the syntax after the head element of the phrase. Complements differ
         * from post-modifiers in that complements are crucial to the understanding
         * of a phrase whereas post-modifiers are optional.
         * </p>
         * 
         * <p>
         * If the new complement being added is a <em>clause</em> or a
         * <code>CoordinatedPhraseElement</code> then its clause status feature is
         * set to <code>ClauseStatus.SUBORDINATE</code> and it's discourse function
         * is set to <code>DiscourseFunction.OBJECT</code> by default unless an
         * existing discourse function exists on the complement.
         * </p>
         * 
         * <p>
         * Complements can have different functions. For example, the phrase <I>John
         * gave Mary a flower</I> has two complements, one a direct object and one
         * indirect. If a complement is not specified for its discourse function,
         * then this is automatically set to <code>DiscourseFunction.OBJECT</code>.
         * </p>
         * 
         * @param newComplement
         *            the new complement as an <code>NLGElement</code>.
         */

        public virtual void addComplement(INLGElement newComplement)
        {
            var complements = getFeatureAsElementList(InternalFeature.COMPLEMENTS.ToString());
            if (complements == null)
            {
                complements = new List<INLGElement>();
            }

            // check if the new complement has a discourse function; if not, assume
            // object
            if (!newComplement.hasFeature(InternalFeature.DISCOURSE_FUNCTION.ToString()))
            {
                newComplement.setFeature(InternalFeature.DISCOURSE_FUNCTION.ToString(), DiscourseFunction.OBJECT);
            }

            complements.add(newComplement);
            setFeature(InternalFeature.COMPLEMENTS.ToString(), complements);
            if (newComplement.isA(PhraseCategoryEnum.CLAUSE) || newComplement is CoordinatedPhraseElement)
            {
                if (newComplement is SPhraseSpec)
                {
                    ((SPhraseSpec) newComplement).setFeature(InternalFeature.CLAUSE_STATUS.ToString(),
                        ClauseStatus.SUBORDINATE);

                    if (!newComplement.hasFeature(InternalFeature.DISCOURSE_FUNCTION.ToString()))
                    {
                        ((SPhraseSpec) newComplement).setFeature(InternalFeature.DISCOURSE_FUNCTION.ToString(),
                            DiscourseFunction.OBJECT);
                    }
                }
                else
                {
                    newComplement.setFeature(InternalFeature.CLAUSE_STATUS.ToString(),
                        ClauseStatus.SUBORDINATE);

                    if (!newComplement.hasFeature(InternalFeature.DISCOURSE_FUNCTION.ToString()))
                    {
                        newComplement.setFeature(InternalFeature.DISCOURSE_FUNCTION.ToString(),
                            DiscourseFunction.OBJECT);
                    }
                }
            }
        }

        /**
         * <p>
         * Sets a complement of the phrase element. If a complement already exists
         * of the same DISCOURSE_FUNCTION, it is removed. This replaces complements
         * set earlier via {@link #addComplement(NLGElement)}
         * </p>
         * 
         * @param newComplement
         *            the new complement as an <code>NLGElement</code>.
         */

        public virtual void setComplement(INLGElement newComplement)
        {
            var f = newComplement.getFeature(InternalFeature.DISCOURSE_FUNCTION.ToString());
            if (f != null)
            {
                removeComplements((DiscourseFunction) f);
            }
            addComplement(newComplement);
        }




/**
 * remove complements of the specified DiscourseFunction
 * 
 * @param function
 */

        public virtual void removeComplements(DiscourseFunction function)
        {
            var complements = getFeatureAsElementList(InternalFeature.COMPLEMENTS.ToString());
            if (complements == null)
                return;
            var complementsToRemove = new List<INLGElement>();
            foreach (var complement in complements)
            {
                if ((int) function == (int) complement.getFeature(InternalFeature.DISCOURSE_FUNCTION.ToString()))
                    complementsToRemove.add(complement);
            }
            if (!complementsToRemove.isEmpty())
            {
                complements.removeAll(complementsToRemove);
                setFeature(InternalFeature.COMPLEMENTS.ToString(), complements);
            }

            return;
        }

        /**
         * <p>
         * Adds a new complement to the phrase element. Complements will be realised
         * in the syntax after the head element of the phrase. Complements differ
         * from post-modifiers in that complements are crucial to the understanding
         * of a phrase whereas post-modifiers are optional.
         * </p>
         * 
         * @param newComplement
         *            the new complement as a <code>string</code>. It is used to
         *            create a <code>StringElement</code>.
         */

        public virtual void addComplement(string newComplement)
        {
            var newElement = new StringElement(newComplement);
            var complements = getFeatureAsElementList(InternalFeature.COMPLEMENTS.ToString());
            if (complements == null)
            {
                complements = new List<INLGElement>();
            }
            complements.add(newElement);
            this.setFeature(InternalFeature.COMPLEMENTS.ToString(), complements);
        }

        /**
         * <p>
         * Sets the complement to the phrase element. This replaces any complements
         * set earlier.
         * </p>
         * 
         * @param newComplement
         *            the new complement as a <code>string</code>. It is used to
         *            create a <code>StringElement</code>.
         */

        public virtual void setComplement(string newComplement)
        {
            setFeature(InternalFeature.COMPLEMENTS.ToString(), null);
            addComplement(newComplement);
        }

        /**
         * Adds a new post-modifier to the phrase element. Post-modifiers will be
         * realised in the syntax after the complements.
         * 
         * @param newPostModifier
         *            the new post-modifier as an <code>NLGElement</code>.
         */

        public virtual void addPostModifier(INLGElement newPostModifier)
        {
            var postModifiers = getFeatureAsElementList(InternalFeature.POSTMODIFIERS.ToString());
            if (postModifiers == null)
            {
                postModifiers = new List<INLGElement>();
            }
            newPostModifier.setFeature(InternalFeature.DISCOURSE_FUNCTION.ToString(), DiscourseFunction.POST_MODIFIER);
            postModifiers.add(newPostModifier);
            setFeature(InternalFeature.POSTMODIFIERS.ToString(), postModifiers);
        }

        /**
         * Adds a new post-modifier to the phrase element. Post-modifiers will be
         * realised in the syntax after the complements.
         * 
         * @param newPostModifier
         *            the new post-modifier as a <code>string</code>. It is used to
         *            create a <code>StringElement</code>.
         */

        public virtual  void addPostModifier(string newPostModifier)
        {
            var postModifiers = getFeatureAsElementList(InternalFeature.POSTMODIFIERS.ToString());
            if (postModifiers == null)
            {
                postModifiers = new List<INLGElement>();
            }
            postModifiers.add(new StringElement(newPostModifier));
            setFeature(InternalFeature.POSTMODIFIERS.ToString(), postModifiers);
        }

        /**
         * Set the postmodifier for this phrase. This resets all previous
         * postmodifiers to <code>null</code> and replaces them with the given
         * string.
         * 
         * @param newPostModifier
         *            the postmodifier
         */

        public virtual void setPostModifier(string newPostModifier)
        {
            this.setFeature(InternalFeature.POSTMODIFIERS.ToString(), null);
            addPostModifier(newPostModifier);
        }

        /**
         * Set the postmodifier for this phrase. This resets all previous
         * postmodifiers to <code>null</code> and replaces them with the given
         * string.
         * 
         * @param newPostModifier
         *            the postmodifier
         */

        public virtual void setPostModifier(INLGElement newPostModifier)
        {
            this.setFeature(InternalFeature.POSTMODIFIERS.ToString(), null);
            addPostModifier(newPostModifier);
        }

        /**
         * Adds a new front modifier to the phrase element.
         * 
         * @param newFrontModifier
         *            the new front modifier as an <code>NLGElement</code>.
         */

        public virtual void addFrontModifier(INLGElement newFrontModifier)
        {
            var frontModifiers = getFeatureAsElementList(InternalFeature.FRONT_MODIFIERS.ToString());
            if (frontModifiers == null)
            {
                frontModifiers = new List<INLGElement>();
            }
            frontModifiers.add(newFrontModifier);
            setFeature(InternalFeature.FRONT_MODIFIERS.ToString(), frontModifiers);
        }

        /**
         * Adds a new front modifier to the phrase element.
         * 
         * @param newFrontModifier
         *            the new front modifier as a <code>string</code>. It is used to
         *            create a <code>StringElement</code>.
         */

        public virtual void addFrontModifier(string newFrontModifier)
        {
            var frontModifiers = getFeatureAsElementList(InternalFeature.FRONT_MODIFIERS.ToString());

            if (frontModifiers == null)
            {
                frontModifiers = new List<INLGElement>();
            }

            frontModifiers.add(new StringElement(newFrontModifier));
            setFeature(InternalFeature.FRONT_MODIFIERS.ToString(), frontModifiers);
        }

        /**
         * Set the frontmodifier for this phrase. This resets all previous front
         * modifiers to <code>null</code> and replaces them with the given string.
         * 
         * @param newFrontModifier
         *            the front modifier
         */

        public virtual void setFrontModifier(string newFrontModifier)
        {
            this.setFeature(InternalFeature.FRONT_MODIFIERS.ToString(), null);
            addFrontModifier(newFrontModifier);
        }

        /**
         * Set the front modifier for this phrase. This resets all previous front
         * modifiers to <code>null</code> and replaces them with the given string.
         * 
         * @param newFrontModifier
         *            the front modifier
         */

        public virtual void setFrontModifier(INLGElement newFrontModifier)
        {
            this.setFeature(InternalFeature.FRONT_MODIFIERS.ToString(), null);
            addFrontModifier(newFrontModifier);
        }

        /**
         * Adds a new pre-modifier to the phrase element.
         * 
         * @param newPreModifier
         *            the new pre-modifier as an <code>NLGElement</code>.
         */

        public virtual void addPreModifier(INLGElement newPreModifier)
        {
            var preModifiers = getFeatureAsElementList(InternalFeature.PREMODIFIERS.ToString());
            if (preModifiers == null)
            {
                preModifiers = new List<INLGElement>();
            }
            preModifiers.add(newPreModifier);
            setFeature(InternalFeature.PREMODIFIERS.ToString(), preModifiers);
        }

        /**
         * Adds a new pre-modifier to the phrase element.
         * 
         * @param newPreModifier
         *            the new pre-modifier as a <code>string</code>. It is used to
         *            create a <code>StringElement</code>.
         */

        public virtual void addPreModifier(string newPreModifier)
        {
            addPreModifier(new StringElement(newPreModifier));
        }

        /**
         * Set the premodifier for this phrase. This resets all previous
         * premodifiers to <code>null</code> and replaces them with the given
         * string.
         * 
         * @param newPreModifier
         *            the premodifier
         */

        public virtual void setPreModifier(string newPreModifier)
        {
            this.setFeature(InternalFeature.PREMODIFIERS.ToString(), null);
            addPreModifier(newPreModifier);
        }

        /**
         * Set the premodifier for this phrase. This resets all previous
         * premodifiers to <code>null</code> and replaces them with the given
         * string.
         * 
         * @param newPreModifier
         *            the premodifier
         */

        public virtual void setPreModifier(INLGElement newPreModifier)
        {
            this.setFeature(InternalFeature.PREMODIFIERS.ToString(), null);
            addPreModifier(newPreModifier);
        }

        /**
         * Add a modifier to a phrase Use heuristics to decide where it goes
         * 
         * @param modifier
         */

        public virtual void addModifier(object modifier)
        {
            // default addModifier - always make modifier a preModifier
            if (modifier == null)
                return;

            // assume is preModifier, add in appropriate form
            if (modifier is INLGElement)
            addPreModifier((INLGElement) modifier);
            else
            addPreModifier((string) modifier);
            return;
        }

        /**
         * Retrieves the current list of pre-modifiers for the phrase.
         * 
         * @return a <code>List</code> of <code>NLGElement</code>s.
         */

        public virtual List<INLGElement> getPreModifiers()
        {
            return getFeatureAsElementList(InternalFeature.PREMODIFIERS.ToString());
        }

        /**
         * Retrieves the current list of post modifiers for the phrase.
         * 
         * @return a <code>List</code> of <code>NLGElement</code>s.
         */

        public virtual List<INLGElement> getPostModifiers()
        {
            return getFeatureAsElementList(InternalFeature.POSTMODIFIERS.ToString());
        }

        /**
         * Retrieves the current list of frony modifiers for the phrase.
         * 
         * @return a <code>List</code> of <code>NLGElement</code>s.
         */

        public virtual List<INLGElement> getFrontModifiers()
        {
            return getFeatureAsElementList(InternalFeature.FRONT_MODIFIERS.ToString());
        }

        public override string printTree(string indent)
        {
            var thisIndent = indent == null ? " |-" : indent + " |-";  
            var childIndent = indent == null ? " | " : indent + " | ";  
            var lastIndent = indent == null ? " \\-" : indent + " \\-";  
            var lastChildIndent = indent == null ? "   " : indent + "   ";  
            var print = new StringBuilder();
            print.append("PhraseElement: category=") 
                .append(getCategory().ToString()).append(", features={"); 

            var features = getAllFeatures();
            foreach (var eachFeature in features.Keys)
            {
                if (features[eachFeature] != null)
                {
                    if (features[eachFeature] is IEnumerable<string>)
                    {
                        print.append(eachFeature).append('=').append(
                             ((IEnumerable<string>)features[eachFeature]).tostring()).append(' ');
                    }
                    else
                    {
                        print.append(eachFeature).append('=').append(
                            features[eachFeature].ToString()).append(' ');
                    }
                }
            }
            print.append("}\n"); 
            var children = getChildren();
            var length = children.size() - 1;
            var index = 0;

            for (index = 0; index < length; index++)
            {
                print.append(thisIndent).append(
                    children.get(index).printTree(childIndent));
            }
            if (length >= 0)
            {
                print.append(lastIndent).append(
                    children.get(length).printTree(lastChildIndent));
            }
            return print.ToString();
        }

        /**
         * Removes all existing complements on the phrase.
         */

        public virtual void clearComplements()
        {
            removeFeature(InternalFeature.COMPLEMENTS.ToString());
        }

        /**
         * Sets the determiner for the phrase. This only has real meaning on noun
         * phrases and is added here for convenience. Determiners are some times
         * referred to as specifiers.
         * 
         * @param newDeterminer
         *            the new determiner for the phrase.
         * @deprecated Use {@link NPPhraseSpec#setSpecifier(object)} directly
         */

        public virtual void setDeterminer(object newDeterminer)
        {
            var factory = new NLGFactory();
            var determinerElement = factory.createWord(newDeterminer, new LexicalCategory_DETERMINER());

            if (determinerElement != null)
            {
                determinerElement.setFeature(InternalFeature.DISCOURSE_FUNCTION.ToString(),
                    DiscourseFunction.SPECIFIER);
                setFeature(InternalFeature.SPECIFIER.ToString(), determinerElement);
                determinerElement.setParent(this);
            }
        }
    }
}