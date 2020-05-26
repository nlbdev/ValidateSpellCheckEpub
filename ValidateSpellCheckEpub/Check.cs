using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Globalization;
using System.Threading;
using System.Text.RegularExpressions;
using System.Xml.XPath;
using System.Net;

using System.IO.Compression;
using System.Diagnostics;
using System.Drawing;

using NHunspell;
using EpubSharp;
using EpubSharp.Format;
using EpubSharp.Format.Writers;
using EpubSharp.Format.Readers;




namespace ValidateSpellCheckEpub
{
    class Check
    {
        XNamespace xhtml = "http://www.w3.org/1999/xhtml";
        XNamespace xml = XNamespace.Xml;
        XNamespace nsSmil = "http://www.w3.org/TR/REC-smil/SMIL10.dtd";
        XNamespace nsEpubSmil = "http://www.idpf.org/2007/ops";

        List<string> FinalReportlist = new List<string>();

        HashSet<string> UniqueWord_with_language_tag_list = new HashSet<string>();
        HashSet<string> BookLanguagesTag = new HashSet<string>();
        long BookTotalWords = 0;
        bool bookHas_errors = false;


        /*
 @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
 @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
 @@@@                              Method                                                                @@@@
 @@@@****************************Make_Dictionary  ******************************************************@@@@
 @@@@                                                                                                    @@@@
 @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
 @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
 */

        public void Make_Dictionary()
        {
            try
            {
                UniqueWord_with_language_tag_list.Clear();

                string myWordFile = "not_Dictionary.txt";
                string onlyFileName = Path.GetFileNameWithoutExtension(myWordFile);
                

                var WordsArray = File.ReadAllLines(myWordFile, Encoding.Default    );
                //var logList = new List<string>(logFile);
                char tabchar = '\t';

                foreach (string items in WordsArray)
                {
                   
                    char[] delimiterChars = { ' ',   '\t' };                    
                    string[] words = items.Split(delimiterChars);

                    string trimmedWord = words[0];
                    
                   //UniqueWord_with_language_tag_list.Add(trimmedWord + Program.WordSplitterString + "no");
                    UniqueWord_with_language_tag_list.Add(trimmedWord );
                }

                //spellCheck(UniqueWord_with_language_tag_list);

                /*
                TextWriter TxtFile = new StreamWriter(onlyFileName +"_processed.txt", false);
                foreach (string allWords in UniqueWord_with_language_tag_list)
                {
                    //Console.WriteLine(allreport);
                    TxtFile.WriteLine(allWords);
                }                
                TxtFile.Close();
                */
                // writing report for each book 
                TextWriter TxtFile_report = new StreamWriter(onlyFileName + "_q_Not_in_Dictionary.txt", false, Encoding.UTF8);               

                foreach (string allreport in FinalReportlist)
                {
                    TxtFile_report.WriteLine(allreport);
                }
                TxtFile_report.Close();
            }// try end
            catch (Exception e)
            {
                string error = "\n \nError in Make_Dictionary";
                Console.WriteLine(error + e);
                Program.Global_error = Program.Global_error + error + e;
            }// catch end
        }

        /*
  @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
  @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
  @@@@                              Method                                                                @@@@
  @@@@****************************GetFiles  ******************************************************@@@@
  @@@@                                                                                                    @@@@
  @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
  @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
  */

        public void DownloadFiles()
        {
            try
            {
                string GitFilesFolderUrl = "https://github.com/nlbdev/ValidateSpellCheckEpub/raw/master/ValidateSpellCheckEpub/custom_files/";                

                string[] FilesForWords = { "CustomWords-en_US.txt", "CustomWords-nb_NO.txt", "unicode_Signs.txt" };

                foreach (string Filename in FilesForWords)
                {
                    string fileName = Filename, myStringWebResource = null;
                    // Create a new WebClient instance.
                    WebClient myWebClient = new WebClient();
                    // Concatenate the domain with the Web resource filename.
                    myStringWebResource = GitFilesFolderUrl + fileName;
                    myWebClient.DownloadFile(myStringWebResource, @"custom_files\" + fileName);
                    Console.WriteLine("Successfully Downloaded File \"{0}\" from \"{1}\"", fileName, myStringWebResource);
                }

               
                
            }// try end
            catch (Exception e)
            {
                string error = "\n \nError in download files";
                Console.WriteLine(error + e);
                Program.Global_error = Program.Global_error + error + e;
            }// catch end
        }




        /*
   @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
   @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
   @@@@                              Method                                                                @@@@
   @@@@****************************GetFiles  ******************************************************@@@@
   @@@@                                                                                                    @@@@
   @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
   @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
   */

        public void GetFiles()
        {
            try
            {
                string bookFolderName = "";
                int i = 0;
                string[] fileArray = Directory.GetFiles(Program.BooksFolderPath, "*.epub", SearchOption.AllDirectories);
                
                Console.WriteLine("The number of books in folder is {0}.", fileArray.Length);
                foreach (string filePath in fileArray)
                {
                    UniqueWord_with_language_tag_list.Clear();
                    BookLanguagesTag.Clear();
                    BookTotalWords = 0;

                    i = i + 1;
                    bookFolderName = Path.GetFileNameWithoutExtension(filePath);
                    
                    string BookReportFolder = Program.ReportOutputFolderPath + "\\" + bookFolderName;
                    string BookReportFile = BookReportFolder + "\\" + bookFolderName + ".txt";
                   
                    FinalReportlist.Add("\n************** Bok nummer = (" + bookFolderName + ")*************");
                    Console.WriteLine("\n"+i + " (" + bookFolderName + ") start");


                    ProcessEpub(filePath);
                    CheckTag(filePath);             // code for structure check              
                    CheckCharacter(filePath);        // code for character check
                    ProcessHtmlFiles(filePath);     //code for spell check

                    if (!Directory.Exists(BookReportFolder))
                    {
                        DirectoryInfo di = Directory.CreateDirectory(BookReportFolder);
                    }

                    // writing report for each book 
                    TextWriter TxtFile_report = new StreamWriter(BookReportFile, false, Encoding.UTF8);

                    if (bookHas_errors == true)
                    {
                        TxtFile_report.WriteLine("********book has xml errors and report is not complete******");
                    }

                    foreach (string allreport in FinalReportlist)
                    {
                        //Console.WriteLine(allreport);
                        TxtFile_report.WriteLine(allreport);
                    }
                    TxtFile_report.Close();
                    FinalReportlist.Clear();
                    Console.WriteLine("finished");
                }// each die end

                TextWriter TxtFile = new StreamWriter(Program.ReportFile, false);
                TxtFile.WriteLine(Program.Global_error);
                TxtFile.Close();               
            }// try end
            catch (Exception e)
            {
                string error = "\n \nError in GetFiles";
                Console.WriteLine(error + e);
                Program.Global_error = Program.Global_error + error + e;
            }// catch end
        }


        /*
     @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
     @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
     @@@@                              Method                                                                @@@@
     @@@@****************************Read Epub      ******************************************************@@@@
     @@@@                                                                                                    @@@@
     @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
     @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
     */
        //------------method geeting smil fil -----------
        public void ProcessEpub(string currentFile)
        {
            try
            {
               
                string EpubNummerOnly = Path.GetFileNameWithoutExtension(currentFile);

                // Read an epub file
                EpubBook book = EpubReader.Read(currentFile);

                // Read metadata
                string title = book.Title;
                FinalReportlist.Add("Title of book = " + title);

                //******Author******
                var authors = book.Authors;
                int Authors_count = 0;
                string Authors_Metadata = "";
                foreach (var eachData in authors)
                {
                    Authors_count++;
                    Authors_Metadata = Authors_Metadata +" (" + eachData + "), ";
                }
                FinalReportlist.Add( "Author of book= (" + Authors_count + ")" + Authors_Metadata);

                //******images***********
                ICollection<EpubByteFile> images = book.Resources.Images;
                FinalReportlist.Add("Images in  book= (" + images.Count + ")");

                //******other metadata***********
                // Access internal EPUB format specific data structures.
                EpubFormat format = book.Format;
                OcfDocument ocf = format.Ocf;
                OpfDocument opf = format.Opf;
                NcxDocument ncx = format.Ncx;
                NavDocument nav = format.Nav;
                var bookVersion = format.Opf.EpubVersion;
                var bookIsbnID = format.Opf.Metadata.Identifiers;

                var allMetadata = opf.Metadata.Languages;
                string language_Metadata = "";
                foreach (var eachData in allMetadata)
                {
                    language_Metadata = language_Metadata + eachData + ", ";
                }
                FinalReportlist.Add("Language of of book= (" + allMetadata.Count + ")" + language_Metadata);
                FinalReportlist.Add("version of book= " + bookVersion);


                // Get table of contents
                FinalReportlist.Add("\n*****Table of Content*****");
                ICollection<EpubChapter> chapters = book.TableOfContents;
                foreach (var eachH1chapter in chapters)
                {
                    FinalReportlist.Add(eachH1chapter.Title);
                    //*******writing h2 Chapter****
                    if (eachH1chapter.SubChapters.Count >= 1)
                    {
                        var h2chapter = eachH1chapter.SubChapters;
                        foreach (var eachH2chapter in h2chapter)
                        {
                            FinalReportlist.Add("\t"+ eachH2chapter.Title);
                            //*******writing h3 Chapter****
                            if (eachH2chapter.SubChapters.Count >= 1)
                            {
                                var h3chapter = eachH2chapter.SubChapters;
                                foreach (var eachH3chapter in h3chapter)
                                {
                                    FinalReportlist.Add("\t\t" + eachH3chapter.Title);
                                }
                            }

                        }
                    }
                }
            }// try end
            catch (Exception e)
            {
                string error = "\n Error in syncStart " + Program.Current_bookPath;
                Console.WriteLine(error + e);
                Program.Global_error = Program.Global_error + error;

            }// catch end

        }//syncStart end



        /*
     @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
     @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
     @@@@                              Method                                                                @@@@
     @@@@****************************ProcessHtmlFiles      ******************************************************@@@@
     @@@@                                                                                                    @@@@
     @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
     @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
     */
        //------------method geeting ProcessHtmlFiles -----------
        public void ProcessHtmlFiles(string filePath )

        {
            string EpubNummerOnly = Path.GetFileNameWithoutExtension(filePath);
            // Read an epub file
            EpubBook book = EpubReader.Read(filePath);
            try
            {
                ICollection<EpubTextFile> html = book.Resources.Html;
                    foreach (var eachFile in html)
                    {
                        // HTML of current text content file
                        string htmlContent = eachFile.TextContent;
                        try
                        {
                            XDocument xmlFile = XDocument.Parse(eachFile.TextContent);
                            readContent(xmlFile, EpubNummerOnly, eachFile.FileName);
                        }
                        catch 
                        {
                            var ll = htmlContent[0];
                            if (ll != '<')
                            {
                                htmlContent = htmlContent.Remove(0, 1);
                            }
                            try
                            {
                                //string ChangedHtml = DecodeHtmlSymbols(htmlContent);
                                XDocument xmlFile = XDocument.Parse(htmlContent);
                                readContent(xmlFile, EpubNummerOnly, eachFile.FileName);
                            }
                            catch (Exception j)
                            {
                                string error = "\n Error in xml praser  " + EpubNummerOnly + "\t " + eachFile.FileName;
                                Console.WriteLine(error);
                                Program.Global_error = Program.Global_error + error;
                                break;
                            }

                        }
                    }//loop all files end
                //***********saving file*********

                string languageTag = String.Join(" , ", BookLanguagesTag.ToArray());
                FinalReportlist.Add("\n**************Spell Check*************");
                FinalReportlist.Add("Whole File Treated;  Total words in book=  " + BookTotalWords + "\t total Unique words " + UniqueWord_with_language_tag_list.Count + "  languages= " + BookLanguagesTag.Count + " " + languageTag);
                
                spellCheck(UniqueWord_with_language_tag_list);   // **************call spell check program****************

                Console.WriteLine("Stage 3: Spell Check Completed");
            }// try end
            catch (Exception e)
            {
                string error = "\n Error in ProcessHtmlFiles " + Program.Current_bookPath;
                Console.WriteLine(error + e);
                Program.Global_error = Program.Global_error + error;

            }// catch end
        }//ProcessFiles



        /*
                    @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
                    @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
                    @@@@                              Method                                                                @@@@
                    @@@@****************************readContent      ******************************************************@@@@
                    @@@@                                                                                                    @@@@
                    @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
                    @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
                    */
        //------------method reading content file -----------
        public void readContent(XDocument xmlFile, string bookFolderName ,string filePath)
        //public void downloadavis(int avisNavn)
        {
            try
            {
                // Create and load the XML document.
                XmlDocument doc = new XmlDocument();
                doc.Load(xmlFile.CreateReader());

                XmlNodeReader node = new XmlNodeReader(doc);
                while (node.Read())
                {
                    /*
                    if ((node.NodeType == XmlNodeType.Element)  || (node.NodeType == XmlNodeType.EndElement))
                    {
                        Console.WriteLine("{0}: <{1}>", node.XmlLang, node.Name);
                        Console.ReadKey();
                    }
                    */
                    //***************************treating only text element*****************
                    if ((node.NodeType == XmlNodeType.Text))
                    {
                        string mytext = node.Value.ToString();
                       
                        Program.AllBookLanguages.Add(bookFolderName + "---" + node.XmlLang);
                        //Console.WriteLine("---"+mytext +"---");
                        mytext = mytext.Replace("\n", "");


                        char[] delimiterChars = { ' ' };
                        string[] words = mytext.Split(delimiterChars);
                        foreach (string word in words)
                        {
                            var trimmedWord = word.TrimEnd(new char[] { ' ', ',', '.', ':', '\t', '?', '»', '«', ';', '!', '(', ')', '\'', '‘', '’', '“', '“', '"', '[', ']', '/', ']' });

                            trimmedWord = trimmedWord.TrimStart (new char[] { ' ', ',', '.', ':', '\t', '?', '»', '«', ';', '!', '(', ')', '\'', '‘', '’', '“', '“', '"', '[', ']', '/', ']' });

                            BookTotalWords = BookTotalWords + 1;                            
                            UniqueWord_with_language_tag_list.Add(trimmedWord + Program.WordSplitterString + node.XmlLang);
                            BookLanguagesTag.Add(node.XmlLang.ToString());
                            
                        }
                        //Console.ReadKey(); 
                    }
                }
            }// try end
            catch (Exception e)
            {
                string error = "\n readContent" + bookFolderName;
                Console.WriteLine(error + e);
                Program.Global_error = Program.Global_error + error + e;
                // Console.ReadKey();
            }// catch end

        }//method end





        /*
     @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
     @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
     @@@@                              Method                                                                @@@@
     @@@@****************************CheckTag    ******************************************************@@@@
     @@@@                                                                                                    @@@@
     @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
     @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
     */

        public void CheckTag( string filePath)
        {
            string EpubNummerOnly = Path.GetFileNameWithoutExtension(filePath);
            try
            {
                
                // Read an epub file
                EpubBook book = EpubReader.Read(filePath);
                XElement Mybook = new XElement(xhtml + "html");
                int file_Counter = 0;
                ICollection<EpubTextFile> html = book.Resources.Html;
                foreach (var eachFile in html)
                {
                    
                    string htmlContent = eachFile.TextContent;
                    try
                    {
                        XDocument xmlFile = XDocument.Parse(eachFile.TextContent);
                        foreach (var singleLinkElement in xmlFile.Descendants(xhtml + "body"))
                        {
                            Mybook.Add(singleLinkElement);
                        }
                    }
                    catch
                    {
                        var ll = htmlContent[0];
                        if (ll != '<')
                        {
                            htmlContent = htmlContent.Remove(0, 1);
                        }
                        try
                        {
                            //string ChangedHtml = DecodeHtmlSymbols(htmlContent);
                            XDocument xmlFile = XDocument.Parse(htmlContent);
                            foreach (var singleLinkElement in xmlFile.Descendants(xhtml + "body"))
                            {
                                Mybook.Add(singleLinkElement);
                            }

                        }
                        catch (Exception j)
                        {
                            bookHas_errors = true;
                            file_Counter++;
                            if (file_Counter == 1)
                            {
                                string error = "\n\n" + EpubNummerOnly + "\n Error in xml praser check tag  " + "\t " + eachFile.FileName;
                                Console.WriteLine(error);
                                Program.Global_error = Program.Global_error + error;

                            }
                            else 
                            {
                                string error =  "\n Error in xml praser check tag  " + "\t " + eachFile.FileName;
                                Console.WriteLine(error);
                                Program.Global_error = Program.Global_error + error;
                                

                            }
                            
                        }

                    }
                }// all files end
                   
                //***********************for checking Element properties****************
                var queryResult = from c in Mybook.Descendants()
                                  select (c.Name);

                List<string> Elementlist = new List<string>();
                foreach (var item in queryResult.Distinct())
                {
                    //Elementlist.Add(item.LocalName);
                    var queryResult2 = from c2 in Mybook.Descendants(item)
                                       select (c2.Name);
                    if ((item.LocalName.ToString().ToLower() == "img") || (item.LocalName.ToString().ToLower() == "table") || (item.LocalName.ToString().ToLower() == "math"))
                    {
                        Elementlist.Add(item.LocalName + "\t\t\t" + queryResult2.Count() + "\t\t Warning ............" + item.LocalName + " in book");
                    }
                    else
                    {
                        Elementlist.Add(item.LocalName + "\t\t\t" + queryResult2.Count() + "\t");
                    }
                }

                Elementlist.Sort();
                foreach (string UniqueElement in Elementlist)
                {
                    //Console.WriteLine(UniqueElement);
                }
                //***********************for checking Element properties end****************

                //***********************Attribute Check ****************
                List<string> Attributelist = new List<string>();
                var Attributeresult = (from ele in Mybook.Descendants().Attributes()
                                       select ele).ToList();


                //****************finding different attributes**************
                var AttributeresultName = (from ele2 in Mybook.Descendants().Attributes()
                                           select (ele2.Name));
                foreach (var item in AttributeresultName.Distinct())
                {
                    var queryResult2 = (from ele2 in Mybook.Descendants().Attributes(item)
                                        select (ele2.Name));
                    Attributelist.Add(item.LocalName + "\t\t\t" + queryResult2.Count() + "\t");
                }
                Attributelist.Sort();
                //Console.WriteLine("\n\n**************Attribute*************");
                foreach (string UniqueAttribute in Attributelist)
                {
                    //Console.WriteLine(UniqueAttribute);
                }
                //****************finding different attributes end**************
                //List<string> AttributeClasslist = new List<string>();
                var AttributeClasslist = new HashSet<string>();
                foreach (var attribute in Attributeresult)
                {
                    // UniqueAttributeList.Add(attribute.Name.ToString());
                    if ((attribute.Value.ToLower() == "asciimath"))
                    {
                        AttributeClasslist.Add(attribute.Value + "\tWarning  ............asciimath in book");
                    }
                    if ((attribute.Name.ToString().ToLower() == "class"))
                    {
                        if ((attribute.Value.ToString().ToLower() == "asciimath") || (attribute.Value.ToString().ToLower() == "page-normal"))
                        {
                            AttributeClasslist.Add(attribute.Value + "\tWarning ............ in book");
                        }
                        else
                        {
                            AttributeClasslist.Add(attribute.Value);
                        }
                    }
                }


                FinalReportlist.Add("\n**************All Unique Element in Book*************");
                FinalReportlist.Add(String.Join("\n", Elementlist.ToArray()));
                FinalReportlist.Add("\n**************All Unique Attribute*************");
                FinalReportlist.Add(String.Join("\n", Attributelist.ToArray()));
                FinalReportlist.Add("\n**************Class attribute*************");
                FinalReportlist.Add(String.Join("\n", AttributeClasslist.ToArray()));
                //FinalReportlist.Add("\n**************Class attribute*************");
                Console.WriteLine("Stage 1: CheckTag finished");
                //***********************Attribute Check  end****************
            }// try end
            catch (Exception e)
            {
                string error = "\n \nError in CheckTag" + EpubNummerOnly + e;
                Console.WriteLine(error);
                Program.Global_error = Program.Global_error + error;

            }// catch end
        }



        /*
    @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
    @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
    @@@@                              Method                                                                @@@@
    @@@@****************************CheckCharacter    ******************************************************@@@@
    @@@@                                                                                                    @@@@
    @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
    @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
    */

        public void CheckCharacter(string filePath)
        {
            string EpubNummerOnly = Path.GetFileNameWithoutExtension(filePath);
            try
            {


                // Read an epub file
                EpubBook book = EpubReader.Read(filePath);
                XElement Mybook = new XElement(xhtml + "html");

                ICollection<EpubTextFile> html = book.Resources.Html;
                foreach (var eachFile in html)
                {


                    string htmlContent = eachFile.TextContent;
                    try
                    {
                        XDocument xmlFile = XDocument.Parse(eachFile.TextContent);
                        foreach (var singleLinkElement in xmlFile.Descendants(xhtml + "body"))
                        {
                            Mybook.Add(singleLinkElement);
                        }
                    }
                    catch
                    {
                        var ll = htmlContent[0];
                        if (ll != '<')
                        {
                            htmlContent = htmlContent.Remove(0, 1);
                        }
                        try
                        {
                            //string ChangedHtml = DecodeHtmlSymbols(htmlContent);
                            XDocument xmlFile = XDocument.Parse(htmlContent);
                            foreach (var singleLinkElement in xmlFile.Descendants(xhtml + "body"))
                            {
                                Mybook.Add(singleLinkElement);
                            }

                        }
                        catch (Exception j)
                        {
                            string error = "\n Error in xml praser CheckCharacter  " + EpubNummerOnly +"\t " + eachFile.FileName ;
                            Console.WriteLine(error);
                            Program.Global_error = Program.Global_error + error;
                            break;
                        }
                    }

                }// all files end


                FinalReportlist.Add("\n**************UTF Sigh Check*************");
                var myList = new HashSet<string>();
                long BookTotalCharacters = 0;

                
                //**************readingFile************
                string CustomSignFile = @"custom_files\unicode_Signs.txt";
                string readContents = "";

                

                try
                {
                    
                    using (StreamReader streamReader = new StreamReader(CustomSignFile, Encoding.UTF8))
                    {
                        readContents = streamReader.ReadToEnd();
                    }
                    readContents = Regex.Replace(readContents, @"\t|\n|\r", "");
                }
                catch
                {
                    string error = "\n Error in xml reading file " + CustomSignFile;
                    Console.WriteLine(error);
                    Program.Global_error = Program.Global_error + error;
                }

                char[] charArr = readContents.ToCharArray();               



                XmlDocument doc = new XmlDocument();
                doc.Load(Mybook.CreateReader());
                XmlNodeReader node = new XmlNodeReader(doc);
                while (node.Read())
                {
                    if ((node.NodeType == XmlNodeType.Text))
                    {
                        string mytext = node.Value;
                        //Console.WriteLine(mytext);
                        BookTotalCharacters = BookTotalCharacters + mytext.Length;

                        
                        // Display only letters.
                        mytext = Regex.Replace(mytext, "[0-9]", "");
                        mytext = Regex.Replace(mytext, "[A-Za-z ]", "");
                        
                        mytext = Regex.Replace(mytext, "[øæåØÆÅ(),?.-]", "");

                        foreach (char ch in charArr)
                        {

                            mytext = mytext.Replace(ch, 'l');
                        }
                        mytext = Regex.Replace(mytext, "[A-Za-z ]", "");
                        mytext = Regex.Replace(mytext, @"\t|\n|\r", "");


                      
                        
                        char[] array = mytext.ToCharArray();
                        foreach (char item in array)
                        {

                            //***********this is for show whole book value**********
                           // myList.Add(item.ToString() + "\t" + "\\u" + ((int)item).ToString("X4")+ "\t(" + node.Value+")");                            
                            myList.Add(item.ToString() + "\t" + "\\u" + ((int)item).ToString("X4"));
                           // myList.Add(item.ToString() );


                            //Console.WriteLine(item.ToString() + "\t" + "\\u" + ((int)item).ToString("X4")+"\t" +node.Value);
                        }
                    }
                }



                FinalReportlist.Add("\nbok har total tegn= " + BookTotalCharacters + "\tbok har total ukjenttegn = " + myList.Count);

                foreach (string value in myList)
                {
                    FinalReportlist.Add(value);
                }
                Console.WriteLine("stage 2: Sign Check  finished");


            }// try end
            catch (Exception e)
            {


                string error = "\n \nError in CheckCharacter";
                Console.WriteLine(error + e);
                Program.Global_error = Program.Global_error + error + e;
            }// catch end
        }



        /*
             @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
             @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
             @@@@                              Method                                                                @@@@
             @@@@****************************spellCheck    ******************************************************@@@@
             @@@@                                                                                                    @@@@
             @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
             @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
             */

        
        public void spellCheck(HashSet<string> UniqueWord_with_language_tag_list)

        {
            var BookWrongWordsTag = new HashSet<string>();
            var BookWrongWordsTag_tested_english = new HashSet<string>();
            int remaining = UniqueWord_with_language_tag_list.Count;
            try
            {

                //************loading norwegian dictionary************
                string Language_aff_no = @"language\nb\nb_NO.aff";
                string Language_dic_no = @"language\nb\nb_NO.dic";
                string Language_dic_custom_no = @"custom_files\CustomWords-nb_NO.txt";

                Hunspell hunspell_no = new Hunspell(Language_aff_no, Language_dic_no);
                string[] lines_no = System.IO.File.ReadAllLines(Language_dic_custom_no);
                foreach (var line_no in lines_no)
                {
                    hunspell_no.Add(line_no);
                }

                //************loading English dictionary************
                string Language_aff_en = @"language\en\en_US.aff";
                string Language_dic_en = @"language\en\en_US.dic";
                string Language_dic_custom_en = @"custom_files\CustomWords-en_US.txt";

                Hunspell hunspell_en = new Hunspell(Language_aff_en, Language_dic_en);
                string[] lines_en = System.IO.File.ReadAllLines(Language_dic_custom_en);
                foreach (var line_en in lines_en)
                {
                    hunspell_en.Add(line_en);
                }

                //******************main process for hunspell******************

                {

                    foreach (var word_with_language in UniqueWord_with_language_tag_list)
                    {
                        string Language = "";
                        //*********************split word from language************
                        var word_with_language_array = word_with_language.Split(new string[] { Program.WordSplitterString }, StringSplitOptions.None);
                        int i = 0;
                        string myWordForCheck = "";
                        foreach (var splited_word in word_with_language_array)
                        {
                            if (i == 0)
                            {
                                myWordForCheck = splited_word;
                            }
                            if (i == 1)
                            {
                                Language = splited_word;
                            }
                            i = i + 1;
                        }
                        //*********************split word from language End************
                        remaining = remaining - 1;

                        if (Language == "no")
                        {
                            bool correct_no = hunspell_no.Spell(myWordForCheck);
                            if (correct_no == true)
                            {
                                //Console.WriteLine("correct "+ myWordForCheck);
                            }
                            else
                            {

                                bool correct_en = hunspell_en.Spell(myWordForCheck);
                                if (correct_en == true)
                                {
                                    BookWrongWordsTag_tested_english.Add(myWordForCheck + "\t\t\t" + Language);
                                    //Console.WriteLine("correct "+ myWordForCheck);
                                }

                                //Console.WriteLine("wrong word \t" + myWordForCheck);
                                BookWrongWordsTag.Add(myWordForCheck + "\t\t\t" + Language);
                                //Console.ReadKey();
                                /*
                                List<string> suggestions = hunspell.Suggest(myWordForCheck);
                                Console.WriteLine("There are " + suggestions.Count.ToString() + " suggestions");
                                foreach (string suggestion in suggestions)
                                {
                                    Console.WriteLine("Suggestion is: " + suggestion);
                                }
                                */
                            }
                        }


                        else if (Language == "en")
                        {
                            bool correct_en = hunspell_en.Spell(myWordForCheck);
                            if (correct_en == true)
                            {
                                //Console.WriteLine("correct "+ myWordForCheck);
                            }
                            else
                            {
                                //Console.WriteLine("wrong word \t" + myWordForCheck);
                                BookWrongWordsTag.Add(myWordForCheck + "\t\t\t" + Language);
                                //Console.ReadKey();
                                /*
                                List<string> suggestions = hunspell.Suggest(myWordForCheck);
                                Console.WriteLine("There are " + suggestions.Count.ToString() + " suggestions");
                                foreach (string suggestion in suggestions)
                                {
                                    Console.WriteLine("Suggestion is: " + suggestion);
                                }
                                */
                            }
                        }

                        else
                        {
                            BookWrongWordsTag.Add(myWordForCheck + "\t" + "unable to check " + Language);
                        }

                    }//for loop end
                    FinalReportlist.Add("\n**************Error words = " + BookWrongWordsTag.Count + "*************");
                    foreach (var wrongWord in BookWrongWordsTag)
                    {
                        FinalReportlist.Add(wrongWord);
                    }

                    //**********************add suggestion for english words*************
                    FinalReportlist.Add("\n**************Error words should be english = " + BookWrongWordsTag_tested_english.Count + "*************");
                    foreach (var wrongWord_suggestion in BookWrongWordsTag_tested_english)
                    {
                        FinalReportlist.Add(wrongWord_suggestion);
                    }


                    /*

                    //*************************stem***********************
                    Console.WriteLine("Find the word stem of the word " + myWordForCheck);
                    List<string> stems = hunspell.Stem(myWordForCheck);
                    foreach (string stem in stems)
                    {
                        Console.WriteLine("Word Stem is: " + stem);
                    }



                    //*************************hyphen***********************

                    Console.WriteLine("Hyph - Hyphenation Functions");
                    Console.WriteLine("¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯");

                    // Important: Due to the fact Hyphen will use unmanaged memory you have to serve the IDisposable pattern
                    // In this block of code this is be done by a using block. But you can also call hyphen.Dispose()
                    using (Hyphen hyphen = new Hyphen("language\\nb\\hyph_nb_NO.dic"))
                    {
                        Console.WriteLine("Get the hyphenation of the word "+ myWordForCheck);
                        HyphenResult hyphenated = hyphen.Hyphenate(myWordForCheck);
                        Console.WriteLine("'Recommendation' is hyphenated as: " + hyphenated.HyphenatedWord);
                    }


                    //*************************MyThes - Thesaurus/Synonym Functions***********************
                    Console.WriteLine("MyThes - Thesaurus/Synonym Functions");
                    Console.WriteLine("¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯");

                    MyThes thes = new MyThes("language\\nb\\th_nb_NO_v2.dat");


                    ThesResult tr = thes.Lookup(myWordForCheck, hunspell);

                    if (tr.IsGenerated)
                        Console.WriteLine("Generated over stem (The original word form wasn't in the thesaurus)");
                    foreach (ThesMeaning meaning in tr.Meanings)
                    {
                        Console.WriteLine();
                        Console.WriteLine("  Meaning: " + meaning.Description);

                        foreach (string synonym in meaning.Synonyms)
                        {
                            Console.WriteLine("    Synonym: " + synonym);

                        }
                    }
                */


                    /*
                    Console.WriteLine("");
                    Console.WriteLine("Generate the plural of 'girl' by providing sample 'boys'");
                    List<string> generated = hunspell.Generate("girl","boys");
                    foreach (string stem in generated)
                    {
                        Console.WriteLine("Generated word is: " + stem);
                    }

                    Console.WriteLine("");
                    Console.WriteLine("Analyze the word "+ myWordForCheck);
                    List<string> morphs = hunspell.Analyze(myWordForCheck);
                    foreach (string morph in morphs)
                    {
                        Console.WriteLine("Morph is: " + morph);
                    }
                    */

                }

                //Console.WriteLine("finished");
                //Console.ReadKey();



            }// try end
            catch (Exception e)
            {
                string error = "\n \nError in spellCheck";
                Console.WriteLine(error + e);
                Program.Global_error = Program.Global_error + error + e;


            }// catch end
        }



/*
    @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
    @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
    @@@@                              Method                                                                @@@@
    @@@@****************************RegexOptions words ******************************************************@@@@
    @@@@                                                                                                    @@@@
    @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
    @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
    */

private static readonly RegexOptions RegexOptions = RegexOptions.CultureInvariant | RegexOptions.IgnorePatternWhitespace | RegexOptions.ExplicitCapture;
        private static readonly RegexOptions RegexOptionsIgnoreCase = RegexOptions.IgnoreCase | RegexOptions;
        private static readonly RegexOptions RegexOptionsIgnoreCaseSingleLine = RegexOptions.Singleline | RegexOptionsIgnoreCase;
        private static readonly RegexOptions RegexOptionsIgnoreCaseMultiLine = RegexOptions.Multiline | RegexOptionsIgnoreCase;

        private static string DecodeHtmlSymbols(string text)
        {
            if (text == null) return null;
            var regex = new Regex(@"(?<defined>(&nbsp|&quot|&mdash|&ldquo|&rdquo|\&\#8211|\&\#8212|&\#8230|\&\#171|&laquo|&raquo|&amp);?)|(?<other>\&\#\d+;?)", RegexOptionsIgnoreCase);
            text = Regex.Replace(regex.Replace(text, SpecialSymbolsEvaluator), @"\ {2,}", " ", RegexOptions);
            text = WebUtility.HtmlDecode(text);
            return text;
        }

        private static string SpecialSymbolsEvaluator(Match m)
        {
            if (!m.Groups["defined"].Success) return " ";
            switch (m.Groups["defined"].Value.ToLower())
            {
                case "&nbsp;": return " ";
                case "&nbsp": return " ";
                case "&quot;": return "\"";
                case "&quot": return "\"";
                case "&mdash;": return " ";
                case "&mdash": return " ";
                case "&ldquo;": return "\"";
                case "&ldquo": return "\"";
                case "&rdquo;": return "\"";
                case "&rdquo": return "\"";
                case "&#8211;": return "-";
                case "&#8211": return "-";
                case "&#8212;": return "-";
                case "&#8212": return "-";
                case "&#8230": return "...";
                case "&#171;": return "\"";
                case "&#171": return "\"";
                case "&laquo;": return "\"";
                case "&laquo": return "\"";
                case "&raquo;": return "\"";
                case "&raquo": return "\"";
                case "&amp;": return "and";
                case "&amp": return "and";
                default: return " ";
            }
        }


    }// class check end
}// name space end


//var xmldoc = new XmlDocument();
//xmldoc.Load(xelement.CreateReader());
// XDocument to XElement  ------   XElement kk = xmldoc.Root; 