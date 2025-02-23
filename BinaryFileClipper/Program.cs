using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinaryFileClipper
{
    internal class Program
    {

        const int BUFFER_SIZE = 4096;
        static void Main(string[] args)
        {
            Console.WriteLine("BinaryFileClipper 1.0.0.0 by Randerion(HaoJun0823) https://www.haojun0823.xyz/");
            Console.WriteLine("Copyright 2025 Randerion(HaoJun0823) Licensed By Apache 2.0");
            Console.WriteLine("This tool can cut out a specific part of a file and save it.");
            Console.WriteLine("Github:https://github.com/haojun0823/BinaryFileClipper");
            if (args.Length != 4)
            {
               
                Console.WriteLine("Usage: BinaryFileClipper <source file> <destination file> <start position[HEX]> <clip size[HEX]>");
                return;
            }
            
            DoClip(args[0], args[1], long.Parse(args[2], NumberStyles.HexNumber), long.Parse(args[3], NumberStyles.HexNumber));

        }


        static void DoClip(String source, String dest, long start, long size)
        {
            Console.WriteLine($"Clip file from ${source} to {dest}, position at 0x{start:X} and cilp size is 0x{size:X}.");

            if (start <= 0 || size <= 0)
            {
                Console.WriteLine($"Start:0x{start:X} Size:0x{size}:X");
                throw new ArgumentException("Start and size must be greater than 0!");
            }




            if (!File.Exists(source))
            {
                Console.WriteLine($"Source file:{source}");
                throw new ArgumentException("Source file does not exist!");
            }

            if (File.Exists(dest))
            {
                Console.WriteLine($"Destination file:{dest}");
                throw new ArgumentException("Destination file already exists!");
            }




            long read_size = size;

            //using (FileStream fs = File.OpenRead(source))
            //{
            //    if(fs.Length < start + size)
            //    {
            //        Console.WriteLine($"Source file size:{fs.Length} Start:{start} Size:{size}");
            //        throw new ArgumentException("Start position and size exceed the file size!");
            //    }


            //    fs.Seek(start, SeekOrigin.Begin);

            //    using(BinaryReader br = new BinaryReader(fs))
            //    {
            //        using (FileStream fs2 = File.OpenWrite(dest))
            //        {
            //            while(size > 0)
            //            {
            //                byte[] buffer;
            //                if (size - 1024 >= 0)
            //                {
            //                    buffer = br.ReadBytes(1024);
            //                }
            //                else
            //                {
            //                    buffer = br.ReadBytes((int)size);
            //                }


            //                fs2.Write(buffer, 0, buffer.Length);
            //                size -= 1024;
            //            }
            //        }
            //    }


            //}

            using (FileStream s_fs = new FileStream(source, FileMode.Open, FileAccess.Read, FileShare.Read))
            {

                s_fs.Seek(start, SeekOrigin.Begin);

                using (FileStream d_fs = new FileStream(dest, FileMode.CreateNew, FileAccess.Write, FileShare.Write))
                {
                    while (size > 0)
                    {
                        byte[] buffer;
                        if (size - BUFFER_SIZE >= 0)
                        {
                            buffer = new byte[BUFFER_SIZE];
                            s_fs.Read(buffer, 0, BUFFER_SIZE);
                            
                            d_fs.Write(buffer, 0, BUFFER_SIZE);
                            
                        }
                        else
                        {
                            buffer = new byte[size];
                            s_fs.Read(buffer, 0, (int)size);
                            d_fs.Write(buffer, 0, (int)size);
                        }
                        size -= BUFFER_SIZE;
                        Console.WriteLine($"Writed {buffer.Length} bytes from {source} to {dest}, remaining:{(size<0?0:size)} bytes.");
                        //Console.WriteLine($"Progress:{(size < 0 ? 0 : size) * 100 / read_size}%");
                        Console.WriteLine($"Current Position:{source}::0x{s_fs.Position:X},{dest}::0x{d_fs.Position:X}");

                    }

                }

            }

            
            Console.WriteLine($"Clip success:{dest}");
            Console.WriteLine($"Done!");
        }

        //static long CalculateSize(long start,long end)
        //{

        //    if(end < start)
        //    {
        //        throw new ArgumentException("End must be greater than start!");
        //    }   

        //    return end - start;
        //}
    }
}
