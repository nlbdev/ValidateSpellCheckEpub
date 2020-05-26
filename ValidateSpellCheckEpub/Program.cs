using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ValidateSpellCheckEpub
{
    class Program
    {
        public static string Global_error = "";//---use for global error
        public static string ReportFile = "Report.txt";//---use for global error

        //public static string BooksFolderPath = @"D:\NLBPUB\";// \\128.39.251.205\Arkiv\master\NLBPUB
        public static string fullPath = Directory.GetCurrentDirectory();
        //public static string fullPath = @"C:\script\bokbasen\";
        public static string BooksFolderPath = fullPath + @"\Epub\";
        public static string ReportOutputFolderPath = fullPath + @"\report\";
        //public static string ReportOutputFolderPath = @"\\nlb-script\report\";
        public static HashSet<string> AllBookLanguages = new HashSet<string>();

        public static string Current_bookInformasjon = "";
        public static string Current_bookPath = "";


        public static long AllBooksTotalWords = 0;
        public static long AllBooksTotalCharacters = 0;
        public static long AllBooksUniqueWords = 0;
        public static long AllBooksUniqueCharacters = 0;

        public static string AllBooksNewUniqueWords = "";
        public static string AllBooksNewUniqueCharacters = "";
        public static string WordSplitterString = "qzjxwq";

        static void Main(string[] args)
        {
            Check CheckObject = new Check(); // declearing  Check class object 
            try
            {
              // CheckObject.Make_Dictionary();

               CheckObject.GetFiles();
               

            }
            catch (Exception e)
            {
                Console.WriteLine("Error" + e);
            }
            Console.WriteLine("Whole program finished");
            Console.ReadKey();
        }//Main End
    }
}
