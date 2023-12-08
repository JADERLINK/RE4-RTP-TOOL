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
        public static string Version = "B.1.1.0.0 (2023-12-08)";

        public static string headerText()
        {
            return "# RE4_RTP_REPACK" + Environment.NewLine +
                   "# by: JADERLINK" + Environment.NewLine +
                   "# Thanks to \"mariokart64n\" and \"zatarita\"" + Environment.NewLine +
                  $"# Version {Version}";
        }

        static void Main(string[] args)
        {
            Console.WriteLine(headerText());

            if (args.Length == 0)
            {
                Console.WriteLine("For more information read:");
                Console.WriteLine("https://github.com/JADERLINK/RE4-RTP-TOOL");
            }
            else if (args.Length >= 1 && File.Exists(args[0]))
            {
                Console.WriteLine(args[0]);

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

                if (fileInfo.Extension.ToUpperInvariant() == ".IDXRTP")
                {
                    string baseName = fileInfo.FullName.Remove(fileInfo.FullName.Length - fileInfo.Extension.Length, fileInfo.Extension.Length);
                    string objFile = baseName + ".obj";
                    string rtpFile = baseName + ".RTP";
                    string tx2File = baseName + ".Repack.txt2";

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

                        Console.WriteLine("The .obj file does not exist");
                    }

                }
                else
                {
                    Console.WriteLine("Wrong file");
                }

            }
            else
            {
                Console.WriteLine("The file does not exist");
            }

            Console.WriteLine("End");

        }
    }
}
