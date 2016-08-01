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
using System.Diagnostics;

namespace SimpleNLG
{
    public class NIHLexiconXMLDumpUtil
    {
        // filenames
        private static string DB_FILENAME; // DB location
        private static string WORDLIST_FILENAME; // word list
        private static string XML_FILENAME; // word list

        /**
         * This main method reads a list of CSV words and POS tags and looks up against 
         * the NIHDB Lexicon for a corresponding entry. If found the baseform is written out into a XML 
         * file, which can be used in SimpleNLG or elsewhere. 
         * 
         * @param args : List of Arguments that this command line application must be provided with in order:
         * <ol>
         * 		<li>The full path to the NIHDB Lexicon database file e.g. C:\\NIHDB\\lexAccess2009</li>
         * 		<li>The full path to the list of baseforms and POS tags to include in the written out XML Lexicon file</li>
         * 		<li>The full path to the XML file that the XML Lexicon will be written out to.</li>
         * </ol>
         * 
         *<p>Example usage: 
         *   java simplenlg.lexicon.util.NIHLexiconXMLDumpUtil C:\\NIHDB\\lexAccess2009 C:\\NIHDB\\wordlist.csv C:\\NIHDB\\default-lexicon.xml
         *   
         *   You will need to have the HSQLDB driver (org.hsqldb.jdbc.JDBCDriver) on your Java classpath before running this application.
         *</p>
         */

        public static void main(string[] args)
        {
            Lexicon lex = null;

            if (args.Length == 3)
            {

                DB_FILENAME = args[0];
                WORDLIST_FILENAME = args[1];
                XML_FILENAME = args[2];

                // Check to see if the HSQLDB driver is available on the classpath:
                var dbDriverAvaliable = false;
                try
                {
                    Class < ? >
                    driverClass = Class.forName("org.hsqldb.jdbc.JDBCDriver", false, NIHLexiconXMLDumpUtil.class.
                    getClassLoader())
                    ;
                    if (null != driverClass)
                    {
                        dbDriverAvaliable = true;
                    }
                }
                catch (ClassNotFoundException cnfe)
                {
                    Debug.WriteLine("*** Please add the HSQLDB JDBCDriver to your Java classpath and try again.");
                }

                if ((null != DB_FILENAME && !DB_FILENAME.isEmpty()) &&
                    (null != WORDLIST_FILENAME && !WORDLIST_FILENAME.isEmpty()) &&
                    (null != XML_FILENAME && !XML_FILENAME.isEmpty()) && dbDriverAvaliable)
                {
                    lex = new NIHDBLexicon(DB_FILENAME);

                    try
                    {
                        LineNumberReader wordListFile = new LineNumberReader(new FileReader(WORDLIST_FILENAME));
                        FileWriter xmlFile = new FileWriter(XML_FILENAME);
                        xmlFile.write(String.format("<lexicon>%n"));
                        string line = wordListFile.readLine();
                        while (line != null)
                        {
                            var cols = line.Split(',');
                            var basef = cols[0];
                            var cat = cols[1];
                            WordElement word = null;
                            if (cat.equalsIgnoreCase("noun"))
                                word = lex.getWord(basef, LexicalCategory.NOUN);
                            else if (cat.equalsIgnoreCase("verb"))
                                word = lex.getWord(basef, LexicalCategory.VERB);
                            else if (cat.equalsIgnoreCase("adv"))
                                word = lex.getWord(basef, LexicalCategory.ADVERB);
                            else if (cat.equalsIgnoreCase("adj"))
                                word = lex.getWord(basef, LexicalCategory.ADJECTIVE);
                            else if (cat.equalsIgnoreCase("det"))
                                word = lex.getWord(basef, LexicalCategory.DETERMINER);
                            else if (cat.equalsIgnoreCase("prep"))
                                word = lex.getWord(basef, LexicalCategory.PREPOSITION);
                            else if (cat.equalsIgnoreCase("pron"))
                                word = lex.getWord(basef, LexicalCategory.PRONOUN);
                            else if (cat.equalsIgnoreCase("conj"))
                                word = lex.getWord(basef, LexicalCategory.CONJUNCTION);
                            else if (cat.equalsIgnoreCase("modal"))
                                word = lex.getWord(basef, LexicalCategory.MODAL);
                            else if (cat.equalsIgnoreCase("interjection"))
                                word = lex.getWord(basef, LexicalCategory.NOUN);
                                    // Kilgarriff;s interjections are mostly nouns in the lexicon

                            if (word == null)
                                Console.WriteLine(("*** The following baseform and POS tag is not found: " + base + ":" + cat);
                            else
                            xmlFile.write(word.toXML());
                            line = wordListFile.readLine();
                        }
                        xmlFile.write(String.format("</lexicon>%n"));
                        wordListFile.close();
                        xmlFile.close();

                        lex.close();

                        Console.WriteLine(("*** XML Lexicon Export Completed.");

                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine("*** An Error occured during the export. The Exception message is below: ");
                        Debug.WriteLine(e.getMessage());
                        Debug.WriteLine("************************");
                        Debug.WriteLine("Please make sure you have the correct application arguments: ");
                        printArgumentsMessage();
                    }
                }
                else
                {
                    printErrorArgumentMessage();
                }
            }
            else
            {
                printErrorArgumentMessage();
            }
        }

        /**
         * Prints Arguments Error Messages if incorrect or not enough parameters have been supplied. 
         */

        private void printErrorArgumentMessage()
        {
            Debug.WriteLine("Insuffient number of arguments supplied. Please supply the following Arguments: \n");
            printArgumentsMessage();
        }

        /**
         * Prints this utility applications arguments requirements. 
         */

        private void printArgumentsMessage()
        {
            Debug.WriteLine("\t\t 1. The full path to the NIHDB Lexicon database file e.g. C:\\NIHDB\\lexAccess2009 ");
            Debug.WriteLine("\t\t 2. The full path to the list of baseforms and POS tags to include in the written out XML Lexicon file");
            Debug.WriteLine("\t\t 3. The full path to the XML file that the XML Lexicon will be written out to.");
        }

    }
}
