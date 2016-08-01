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
    public class StringElement : NLGElement
    {

    public StringElement(string value)
    {
        setCategory(new PhraseCategory_CANNED_TEXT());
        setFeature(Feature.ELIDED.ToString(), false);
        setRealisation(value);
    }

  
    public override List<INLGElement> getChildren()
    {
        return new List<INLGElement>();
    }

  
    public override string ToString()
    {
        return getRealisation();
    }

    public override string printTree(string indent)
    {
        var print = new StringBuilder();
        print
            .Append("StringElement: content=\"").Append(getRealisation()).append('\"'); 
        var features = this.getAllFeatures();

        if (features != null)
        {
            print.Append(", features=").append(features.ToString()); 
        }
        print.Append('\n');
        return print.ToString();
    }
    }
}