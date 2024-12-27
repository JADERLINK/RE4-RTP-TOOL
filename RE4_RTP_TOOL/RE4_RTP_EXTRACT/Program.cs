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
        public static string Version = "B.1.3.0 (2024-12-27)";

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
            Console.OutputEncoding = Encoding.UTF8;

            Console.WriteLine(headerText());

            if (args.Length == 0)
            {
                Console.WriteLine("For more information read:");
                Console.WriteLine("https://github.com/JADERLINK/RE4-RTP-TOOL");
                Console.WriteLine("Press any key to close the console.");
                Console.ReadKey();
            }
            else if (args.Length >= 2)
            {
                if (File.Exists(args[0]))
                {
                    bool isUHD = false;
                    bool isPS2 = false;
                    bool isPS4NS = false;
                    bool isBig = false;

                    string arg = args[1].ToUpper();
                    if (arg.Contains("UHD"))
                    {
                        isUHD = true;
                    }
                    else if (arg.Contains("2007PS2") || arg.Contains("PS2") || arg.Contains("2007"))
                    {
                        isPS2 = true;
                    }
                    else if (arg.Contains("PS4NS") || arg.Contains("PS4") || arg.Contains("NS"))
                    {
                        isPS4NS = true;
                    }
                    else if (arg.Contains("BIG"))
                    {
                        isBig = true;
                    }


                    if (isUHD || isPS2 || isPS4NS || isBig)
                    {
                        bool createDebugFile = false;
                        if (args.Length >= 3 && args[2].ToUpper().Contains("TRUE"))
                        {
                            createDebugFile = true;
                        }

                        FileInfo fileInfo = null;

                        try
                        {
                            fileInfo = new FileInfo(args[0]);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error in the path: " + Environment.NewLine + ex);
                        }

                        if (fileInfo != null)
                        {
                            Console.WriteLine(fileInfo.Name);

                            if (fileInfo.Extension.ToUpperInvariant() == ".RTP")
                            {
                                try
                                {
                                    RTPextract.Extract(fileInfo.FullName, isPS2, isPS4NS, isBig, createDebugFile);
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
                    }
                    else
                    {
                        Console.WriteLine("The second argument is invalid.");
                    }
                }
                else 
                {
                    Console.WriteLine("File specified does not exist.");
                }

                Console.WriteLine("Finished!!!");
            }
            else
            {
                Console.WriteLine("The second argument is required.");
                Console.WriteLine("Press any key to close the console.");
                Console.ReadKey();
            }

        }


    }


 

}
