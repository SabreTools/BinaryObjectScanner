using System;
using System.IO;
using System.Linq;
using BurnOutSharp;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var p = new Progress<FileProtection>();
            p.ProgressChanged += Changed;
            foreach (string arg in args)
            {
                string protections = String.Join("\r\n", ProtectionFind.Scan(arg, p).Select(kvp => kvp.Key + ": " + kvp.Value.TrimEnd()));
                Console.WriteLine(protections);
                using (StreamWriter sw = new StreamWriter(File.OpenWrite($"{DateTime.Now:yyyy-MM-dd_HHmmss}.txt")))
                {
                    sw.WriteLine(protections);
                }
            }

            Console.WriteLine("Press any button to close...");
            Console.ReadLine();

            //ProtectionFind.ScanSectors('D', 2048);
        }

        private static void Changed(object source, FileProtection value)
        {
            Console.WriteLine($"{value.Percentage * 100:N2}%: {value.Filename} - {value.Protection}");
        }
    }
}
