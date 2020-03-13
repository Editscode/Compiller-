using System;
using System.IO;

namespace Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            
            for (int i = 1; i < 129; i++)
            {
                if(FileCompare($@"C:\out\{i}.txt", $@"C:\tru\{i}.txt"))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($@"{i}.txt");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($@"{i}.txt");
                }
            }

        }
        private static bool FileCompare(string file1, string file2)
        {
            int file1byte;
            int file2byte;
            FileStream fs1;
            FileStream fs2;

            if (file1 == file2)
            {

                return true;
            }

            fs1 = new FileStream(file1, FileMode.Open);
            fs2 = new FileStream(file2, FileMode.Open);
            if (fs1.Length != fs2.Length)
            {
                fs1.Close();
                fs2.Close();
                return false;
            }
            do
            {
                file1byte = fs1.ReadByte();
                file2byte = fs2.ReadByte();
            }
            while ((file1byte == file2byte) && (file1byte != -1));
            fs1.Close();
            fs2.Close();
            return ((file1byte - file2byte) == 0);
        }
    }
}
