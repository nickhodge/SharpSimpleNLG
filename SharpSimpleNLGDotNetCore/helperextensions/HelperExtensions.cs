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
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SimpleNLG.Extensions
{
    /* These help with Java-style naming into C# style naming etc. etc.
     Ultimately, you would track back the uses of each of these and convert
     the source of the call to pure-er C# to reduce call depth/stack use
     Not a complete implementation of C# to Java - just for SimpleNLG
     Nick Hodge nhodge@mungr.com
    */
    
    public static class HelperExtensions
    {
        // for chars 
        public static bool isDigit(this char c)
        {
            return char.IsDigit(c);
        }

        // for Strings
        public static int length(this string s)
        {
            return s.Length;
        }

        public static string trim(this string s)
        {
            return s.Trim();
        }

        public static string toLowerCase(this string s)
        {
            return s.ToLowerInvariant();
        }

        public static string toUpperCase(this string s)
        {
            return s.ToUpperInvariant();
        }

        public static bool equalsIgnoreCase(this string s, string d)
        {
            return s.ToLowerInvariant().Equals(d.ToLowerInvariant());
        }

        public static int indexOf(this string s, string d)
        {
            return s.IndexOf(d);
        }

        public static int indexOf(this string s, char c)
        {
            return s.IndexOf(c);
        }

        public static char charAt(this string s, int i)
        {
            return s[i];
        }

        public static bool endsWith(this string s, string d)
        {
            return s.EndsWith(d);
        }

        public static bool contains(this string s, string d)
        {
            return s.Contains(d);
        }

        public static bool startsWith(this string s, string d)
        {
            return s.StartsWith(d);
        }

        // ref: https://docs.oracle.com/javase/7/docs/api/java/lang/String.html#substring(int,%20int)
        // and contrast: https://msdn.microsoft.com/en-us/library/aka44szs(v=vs.110).aspx
        //https://jamesmccaffrey.wordpress.com/2015/07/21/the-java-substring-function-vs-the-c-substring-function/
        public static string substring(this string s, int st, int end)
        {
            int len = end - st;
            return s.Substring(st, len);
        }

        public static bool isEmpty(this string s)
        {
            return s.Length == 0;
        }

        public static bool matches(this string s, string p)
        {
            return Regex.Match(s, p).Success;
        }

        public static string replaceAll(this string s, string p, string r)
        {
            return Regex.Replace(s, p, r);
        }

        // for Generic IEnumerables
        public static bool isEmpty<T>(this IEnumerable<T> s)
        {
            return !s.Any();
        }

        public static int size<T>(this IEnumerable<T> s)
        {
            return s.Count();
        }

        // for Generic Lists
        public static T get<T>(this List<T> l, int i)
        {
            return l[i];
        }

        public static int indexOf<T>(this List<T> l, T t)
        {
            return l.FindIndex(o => o.Equals(t));
        }

        public static bool contains<T>(this List<T> l, T i)
        {
            return l.Contains(i);
        }

        public static void clear<T>(this List<T> l)
        {
            l.Clear();
        }

        public static bool remove<T>(this List<T> l, T t)
        {
            return l.contains(t) && l.Remove(t);
        }

        public static bool removeAll<T>(this List<T> l, IEnumerable<T> lt)
        {
            var f = false;
            foreach (var lte in lt)
            {
                f = true;
                l.RemoveAll(x => x.Equals(lte));
            }
            return f;
        }

        public static bool add<T>(this List<T> l, T v)
        {
            l.Add(v);
            return true;
        }

        public static bool addAll<T>(this List<T> l, List<T> s)
        {
            l.AddRange(s);
            return true;
        }

        public static T set<T>(this List<T> l, int i, T v)
        {
            l[i] = v;
            return v;
        }


        public static string tostring(this IEnumerable<string> l)
        {
            var sb = new StringBuilder();
            sb.Append("{ ");
            foreach (var s in l)
            {
                sb.Append(s);
            }
            sb.Append(" }");
            return sb.ToString();
        }


        // for Generic HashSets
        public static bool add<T>(this HashSet<T> h, T v)
        {
            return h.Add(v);
        }

        // for Generic Stacks
        public static T push<T>(this Stack<T> s, T v)
        {
            s.Push(v);
            return v;
        }

        public static T pop<T>(this Stack<T> s)
        {
            return s.Pop();
        }

        public static bool empty<T>(this Stack<T> s)
        {
            return s.isEmpty();
        }

        // for Generic Dictionary
        public static V put<K,V>(this Dictionary<K,V> d, K k , V v)
        {
            if (!d.ContainsKey(k))
            {
                d.Add(k, v);
            }
            else
            {
                d[k] = v;
            }
            return v;
        }

        public static bool containsKey<K, V>(this Dictionary<K, V> d, K k)
        {
            return d.ContainsKey(k);
        }


        // for StringBuilder
        public static StringBuilder append(this StringBuilder sb, string s)
        {
            return sb.Append(s);
        }

        public static StringBuilder append(this StringBuilder sb, char c)
        {
            return sb.Append(c);
        }

        public static int length(this StringBuilder sb)
        {
            return sb.Length;
        }

        public static char charAt(this StringBuilder sb, int i)
        {
            return sb[i];
        }


        public static StringBuilder setLength(this StringBuilder sb, int i)
        {
            if (i >= sb.Length) return sb;
            return sb.Remove(i, (sb.Length - i));
        }


        public static StringBuilder deleteCharAt(this StringBuilder sb, int i)
        {
            if (i >= sb.Length) return sb;
            return sb.Remove(i,1);
        }

    }
}