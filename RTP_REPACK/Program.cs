using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace RTP_REPACK
{
    class Program
    {
        public static string Version = "B.1.2.1 (2024-09-28)";

        public static string headerText()
        {
            return "# RE4_RTP_REPACK" + Environment.NewLine +
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
            else if (args.Length >= 2)
            {
                if (File.Exists(args[0]))
                {
                    bool isUHD = false;
                    bool isPS2 = false;
                    bool isPS4NS = false;

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

                    if (isUHD || isPS2 || isPS4NS)
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

                            if (fileInfo.Extension.ToUpperInvariant() == ".IDXRTP")
                            {
                                string baseDirectory = Path.GetDirectoryName(fileInfo.FullName);
                                string baseFileName = Path.GetFileNameWithoutExtension(fileInfo.FullName);

                                string baseFilePath = Path.Combine(baseDirectory, baseFileName);

                                string pattern = "^(00)([0-9]{2})$";
                                System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(pattern, System.Text.RegularExpressions.RegexOptions.CultureInvariant);

                                if (regex.IsMatch(baseFileName))
                                {
                                    baseFilePath = Path.Combine(baseDirectory, baseFileName + "_RTP");
                                }

                                string objFile = baseFilePath + ".obj";
                                string rtpFile = Path.Combine(baseDirectory, baseFileName + ".RTP");
                                string tx2File = baseFilePath + ".Repack.txt2";

                                if (File.Exists(objFile))
                                {
                                    try
                                    {
                                        RTPrepack.Repack(rtpFile, objFile, tx2File, isPS2, isPS4NS, createDebugFile);
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine("Error: " + ex);
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("The .obj file does not exist.");
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
            }
            else
            {
                Console.WriteLine("The second argument is required.");
                Console.WriteLine("Press any key to close the console.");
                Console.ReadKey();
            }

            Console.WriteLine("Finished!!!");
        }
    }
}
