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
        public static string Version = "B.1.1.2 (2024-08-13)";

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
                            RTPrepack.Repack(rtpFile, objFile, tx2File, isPS2, createDebugFile);
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
            else
            {
                Console.WriteLine("File specified does not exist.");
            }

            Console.WriteLine("Finished!!!");
        }
    }
}
