using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace RE4_2007_GCA_TOOL
{
    internal static class Repack
    {
        public static void RepackFile(string file)
        {
            StreamReader idx = null;
            BinaryWriter gca = null;
            FileInfo fileInfo = new FileInfo(file);
            string baseName = fileInfo.Name.Substring(0, fileInfo.Name.Length - fileInfo.Extension.Length);
            string baseDiretory = fileInfo.DirectoryName + '\\';

            try
            {
                idx = new FileInfo(file).OpenText();
                gca = new BinaryWriter(new FileInfo(baseDiretory + baseName + ".dat").Create(), Encoding.GetEncoding(1252));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + Environment.NewLine + ex);
            }

            if (idx != null && gca != null)
            {
                List<string> FileList = new List<string>();
                List<string> check = new List<string>();

                string endLine = "";
                while (endLine != null)
                {
                    endLine = idx.ReadLine();

                    if (endLine != null)
                    {
                        endLine = endLine.Trim();

                        if ( !(endLine.Length == 0 || endLine.StartsWith("#")) )
                        {
                            string validFile = ValidFileName(endLine);
                            string toUpper = validFile.ToUpperInvariant();
                            if (!check.Contains(toUpper) && toUpper.Length > 0)
                            {
                                FileList.Add(validFile);
                                check.Add(toUpper);
                            }
                        }

                    }

                }

                idx.Close();

                // arquivo GCA

                long GCA3_OFFSET = 0;
                long Checksum_Pos_Offset = 0;
                long BlockSize_Pos_Offset = 0;

                byte[] mainHeader = new byte[16] { 0x47, 0x43, 0x41, 0x58, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                gca.Write(mainHeader);

                FileTableHeader[] headers = new FileTableHeader[FileList.Count];

                Console.WriteLine("Inserting files into GCA file:");

                int index = 0;
                foreach (var item in FileList)
                {
                    Console.WriteLine(item);
                    string path = Path.Combine(baseDiretory, item);

                    headers[index] = new FileTableHeader();
                    headers[index].Attributes = 0x20; // atributo de arquivo morto.
                    headers[index].Filetime = DateTime.Now.ToFileTimeUtc();
                    headers[index].Name = item;
                    try
                    {
                        if (Directory.Exists(path))
                        {
                            headers[index].Attributes = 0x10; // atributo que representa que é uma pasta e não um arquivo
                        }
                        else if (File.Exists(path))
                        {
                            FileInfo info = new FileInfo(path);

                            var fileStream = info.OpenRead();
                            fileStream.CopyTo(gca.BaseStream);
                            fileStream.Close();

                            headers[index].DataSize = info.Length;
                            headers[index].Filetime = info.LastWriteTime.ToFileTimeUtc();

                            var crc32 = new DamienG.Security.Cryptography.Crc32();
                            uint hash = 0;
                            using (var fs = info.OpenRead())
                            {
                                hash = BitConverter.ToUInt32(crc32.ComputeHash(fs).Reverse().ToArray(), 0);
                            }
                            headers[index].Checksum = hash;

                        }
                        else 
                        {
                            Console.WriteLine("The above directory or file does not exist.");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: "+ ex);
                    }
                    index++;
                }

                GCA3_OFFSET = gca.BaseStream.Position;
                gca.Write((uint)0x33414347); //GCA3
                gca.Write((uint)0); //Unknown
                BlockSize_Pos_Offset = gca.BaseStream.Position;
                gca.Write((ulong)0); //BlockSize
                gca.Write((uint)0); //Padding
                Checksum_Pos_Offset = gca.BaseStream.Position;
                gca.Write((uint)0); //Checksum
                gca.Write((ulong)headers.Length);

                for (int i = 0; i < headers.Length; i++)
                {
                    gca.Write((uint)headers[i].Checksum);
                    gca.Write((uint)headers[i].Attributes);
                    gca.Write((ulong)headers[i].Filetime);
                    gca.Write((ulong)headers[i].DataSize); //UncompressedDataSize
                    gca.Write((ulong)headers[i].DataSize); //CompressedDataSize
                }

                for (int i = 0; i < headers.Length; i++)
                {
                    byte[] name = Encoding.UTF8.GetBytes(headers[i].Name);
                    gca.Write((ushort)name.Length);
                    gca.Write(name);

                }

                long end_offset = gca.BaseStream.Position;
                long BlockSize = end_offset - GCA3_OFFSET;

                gca.BaseStream.Position = BlockSize_Pos_Offset;
                gca.Write((ulong)BlockSize);

                gca.BaseStream.Position = 0x08;
                gca.Write((ulong)GCA3_OFFSET);

                gca.BaseStream.Position = GCA3_OFFSET;
                var _crc32 = new DamienG.Security.Cryptography.Crc32();
                uint _hash = BitConverter.ToUInt32(_crc32.ComputeHash(gca.BaseStream).Reverse().ToArray(), 0);

                gca.BaseStream.Position = Checksum_Pos_Offset;
                gca.Write((uint)_hash);

                gca.Close();
            }
        }

        private static string ValidFileName(string fileName)
        {
            string res = "";
            foreach (var c in fileName)
            {
                if (IsValidChar(c))
                {
                    res += c;
                }
            }
            return res;
        }

        private static bool IsValidChar(char c) 
        {
            foreach (var item in Path.GetInvalidPathChars())
            {
                if (item == c)
                {
                    return false;
                }
            }
           return  true;
        }

        class FileTableHeader
        {
            public uint Checksum = 0;
            public uint Attributes = 0;
            public long Filetime = 0;
            public long DataSize = 0;
            public string Name = "";
        }


    }
}
