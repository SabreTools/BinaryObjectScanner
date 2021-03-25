﻿using System;
using System.IO;
using System.Linq;
using BurnOutSharp;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create progress indicator
            var p = new Progress<ProtectionProgress>();
            p.ProgressChanged += Changed;

            // Create scanner to be shared
            var scanner = new Scanner(p)
            {
                IncludePosition = true,
                ScanAllFiles = false,
                ScanArchives = true,
                ScanPackers = true,
            };

            foreach (string arg in args)
            {
                try
                {
                    var protections = scanner.GetProtections(arg);
                    if (protections != null)
                    {
                        using (StreamWriter sw = new StreamWriter(File.OpenWrite($"{DateTime.Now:yyyy-MM-dd_HHmmss}.txt")))
                        {
                            foreach (string key in protections.Keys)
                            {
                                // Skip over files with no protection
                                if (protections[key] == null || !protections[key].Any())
                                    continue;

                                string line = $"{key}: {string.Join(", ", protections[key])}";
                                Console.WriteLine(line);
                                sw.WriteLine(line);
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine($"No protections found for {arg}");
                    }
                }
                catch (Exception ex)
                {
                    using (StreamWriter sw = new StreamWriter(File.OpenWrite($"{DateTime.Now:yyyy-MM-dd_HHmmss}-exception.txt")))
                    {
                        sw.WriteLine(ex.Message);
                    }
                }
            }

            Console.WriteLine("Press Enter to close...");
            Console.ReadLine();
        }

        private static void Changed(object source, ProtectionProgress value)
        {
            Console.WriteLine($"{value.Percentage * 100:N2}%: {value.Filename} - {value.Protection}");
        }
    }
}
