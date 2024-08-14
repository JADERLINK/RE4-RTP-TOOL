using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace RTP_EXTRACT
{
    class Program
    {
        public static string Version = "B.1.1.2 (2024-08-13)";

        public static string headerText()
        {
            return "# RE4_RTP_EXTRACT" + Environment.NewLine +
                   "# by: JADERLINK" + Environment.NewLine +
                   "# youtube.com/@JADERLINK" + Environment.NewLine +
                   "# Thanks to \"mariokart64n\" and \"zatarita\"" + Environment.NewLine +
                  $"# Version {Version}";
        }

        static void Main(string[] args)
        {
            System.Globalization.CultureInfo.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

            Console.WriteLine(headerText());

            if (args.Length == 0)
            {
                Console.WriteLine("For more information read:");
                Console.WriteLine("https://github.com/JADERLINK/RE4-RTP-TOOL");
                Console.WriteLine("Press any key to close the console.");
                Console.ReadKey();
            }
            else if (args.Length >= 1 && File.Exists(args[0]))
            {
                bool isPS2 = false;

                bool createDebugFile = false;

                if (args.Length >= 2 && args[1].ToUpper().Contains("TRUE"))
                {
                    isPS2 = true;
                }

                if (args.Length >= 3 && args[2].ToUpper().Contains("TRUE"))
                {
                    createDebugFile = true;
                }

                FileInfo fileInfo = new FileInfo(args[0]);

                if (fileInfo.Extension.ToUpperInvariant() == ".RTP")
                {
                    try
                    {
                        Console.WriteLine(fileInfo.Name);
                        RTPextract.extract(fileInfo.FullName, isPS2, createDebugFile);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: " + ex);
                    }
                }
                else
                {
                    Console.WriteLine("The extension is not valid: " + fileInfo.Extension);
                }

            }
            else
            {
                Console.WriteLine("File specified does not exist.");
            }

            Console.WriteLine("Finished!!!");
        }


    }


 

}
