using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace RE4_2007_GCA_TOOL
{
    internal static class Extract
    {
        public static void ExtractFile(string file)
        {
            FileInfo fileInfo = new FileInfo(file);
            string baseName = fileInfo.Name.Substring(0, fileInfo.Name.Length - fileInfo.Extension.Length);
            string baseDiretory = fileInfo.DirectoryName + '\\';

            var gca = new BinaryReader(fileInfo.OpenRead());

            uint SignatureGCAX = gca.ReadUInt32();

            if (SignatureGCAX != 0x58414347) //GCAX
            {
                gca.Close();
                Console.WriteLine("Invalid GCA file.");
                return;
            }

            uint padding = gca.ReadUInt32();
            long offset = gca.ReadInt64();

            gca.BaseStream.Position = offset;

            uint SignatureGCA3 = gca.ReadUInt32();

            if (SignatureGCA3 != 0x33414347) //GCA3
            {
                gca.Close();
                Console.WriteLine("Invalid GCA file.");
                return;
            }

            uint Unknown = gca.ReadUInt32();
            ulong BlockSize = gca.ReadUInt64();
            uint Padding = gca.ReadUInt32();
            uint Checksum = gca.ReadUInt32();
            ulong Amount = gca.ReadUInt64();

            Console.WriteLine("Amount: " + Amount);

            FileTableHeader[] headers = new FileTableHeader[Amount];

            for (ulong i = 0; i < Amount; i++)
            {
                headers[i] = new FileTableHeader();
                headers[i].Checksum = gca.ReadUInt32();
                headers[i].Attributes = gca.ReadUInt32();
                headers[i].Filetime = gca.ReadInt64();
                headers[i].UncompressedDataSize = gca.ReadInt64();
                headers[i].CompressedDataSize = gca.ReadInt64();
            }

            for (ulong i = 0; i < Amount; i++)
            {
                ushort length = gca.ReadUInt16();
                byte[] name = gca.ReadBytes(length);
                headers[i].Name = Encoding.UTF8.GetString(name);
            }

            gca.BaseStream.Position = 0x10;

            Console.WriteLine("Extracting files:");

            for (ulong i = 0; i < Amount; i++)
            {
                Console.WriteLine(headers[i].Name);

                byte[] arr = gca.ReadBytes((int)headers[i].CompressedDataSize);
                string path = baseDiretory + headers[i].Name;

                if ((headers[i].Attributes & 0x10) != 0x10) // esse é o atributo que representa que se refere a uma pasta
                {
                    try
                    {
                        FileInfo info = new FileInfo(path);
                        Directory.CreateDirectory(info.DirectoryName);

                        var myfile = info.Create();
                        myfile.Write(arr, 0, arr.Length);
                        myfile.Close();

                        info.LastAccessTimeUtc = DateTime.FromFileTimeUtc(headers[i].Filetime);
                        info.LastWriteTimeUtc = DateTime.FromFileTimeUtc(headers[i].Filetime);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: " + ex);
                    }
                }

            }

            gca.Close();

            Console.WriteLine("Creating IDXGCA file.");

            var idx = new FileInfo(baseDiretory + "\\" + baseName + ".idxgca").CreateText();
            idx.WriteLine("# github.com/JADERLINK/RE4-2007-GCA-TOOL");
            idx.WriteLine("# youtube.com/@JADERLINK");
            idx.WriteLine("# RE4 2007 GCA TOOL By JADERLINK");
            idx.WriteLine("# Thanks to \"zatarita\"");
            idx.WriteLine("# VERSION 1.0.0 (2024-06-09)");
            idx.WriteLine();

            for (ulong i = 0; i < Amount; i++)
            {
                idx.WriteLine(headers[i].Name);
            }
            
            idx.Close();
        }


        class FileTableHeader 
        {
            public uint Checksum = 0;
            public uint Attributes = 0;
            public long Filetime = 0;
            public long UncompressedDataSize = 0;
            public long CompressedDataSize = 0;
            public string Name = "";
        }

    }
}
