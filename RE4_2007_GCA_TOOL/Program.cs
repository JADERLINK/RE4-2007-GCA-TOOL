using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace RE4_2007_GCA_TOOL
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("# RE4 2007 GCA TOOL");
            Console.WriteLine("# By JADERLINK");
            Console.WriteLine("# Thanks to \"zatarita\"");
            Console.WriteLine("# VERSION 1.0.0 (2024-06-09)");

            if (args.Length == 0)
            {
                Console.WriteLine("For more information read:");
                Console.WriteLine("https://github.com/JADERLINK/RE4-2007-GCA-TOOL");
                Console.WriteLine("Press any key to close the console.");
                Console.ReadKey();
            }
            else if (args.Length >= 1 && File.Exists(args[0]))
            {
                string file = args[0];
                FileInfo info = null;

                try
                {
                    info = new FileInfo(file);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error in the path: " + Environment.NewLine + ex);
                }

                if (info != null)
                {
                    Console.WriteLine("File: " + info.Name);

                    if (info.Extension.ToUpperInvariant() == ".GCA"
                        || info.Extension.ToUpperInvariant() == ".DAT")
                    {
                        try
                        {
                            Extract.ExtractFile(file);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error: " + Environment.NewLine + ex);
                        }

                    }
                    else if (info.Extension.ToUpperInvariant() == ".IDXGCA")
                    {
                        try
                        {
                            Repack.RepackFile(file);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error: " + Environment.NewLine + ex);
                        }
                    }
                    else
                    {
                        Console.WriteLine("The extension is not valid: " + info.Extension);
                    }
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
