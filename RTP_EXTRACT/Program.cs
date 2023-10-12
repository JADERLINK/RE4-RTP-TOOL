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
        public static string Version = "A.1.0.0.0 (2023-10-12)";

        public static string headerText()
        {
            return "# RE4_RTP_EXTRACT" + Environment.NewLine +
                   "# by: JADERLINK" + Environment.NewLine +
                   "# Thanks to \"mariokart64n\" and \"zatarita\"" + Environment.NewLine +
                  $"# Version {Version}";
        }

        static void Main(string[] args)
        {
            Console.WriteLine(headerText());

            if (args.Length >= 1 && File.Exists(args[0]))
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
                        Console.WriteLine(args[0]);
                        RTPextract.extract(args[0], isPS2, createDebugFile);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: " + ex.Message);
                    }
                }
                else
                {
                    Console.WriteLine("Wrong file");
                }

            }
            else
            {
                Console.WriteLine("No file");
            }


            Console.WriteLine("end");
        }


    }


 

}
