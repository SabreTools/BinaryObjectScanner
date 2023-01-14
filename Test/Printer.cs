using System;
using System.IO;
using System.Text;
using BurnOutSharp;
using BurnOutSharp.Matching;
using BurnOutSharp.Utilities;
using BurnOutSharp.Wrappers;

namespace Test
{
    internal static class Printer
    {
        /// <summary>
        /// Wrapper to print information for a single path
        /// </summary>
        /// <param name="path">File or directory path</param>
        /// <param name="json">Enable JSON output, if supported</param>
        /// <param name="debug">Enable debug output</param>
        public static void PrintPathInfo(string path, bool json, bool debug)
        {
            Console.WriteLine($"Checking possible path: {path}");

            // Check if the file or directory exists
            if (File.Exists(path))
            {
                PrintFileInfo(path, json, debug);
            }
            else if (Directory.Exists(path))
            {
                foreach (string file in Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories))
                {
                    PrintFileInfo(file, json, debug);
                }
            }
            else
            {
                Console.WriteLine($"{path} does not exist, skipping...");
            }
        }

        /// <summary>
        /// Print information for a single file, if possible
        /// </summary>
        private static void PrintFileInfo(string file, bool json, bool debug)
        {
            Console.WriteLine($"Attempting to print info for {file}");

            using (Stream stream = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                // Read the first 8 bytes
                byte[] magic = stream.ReadBytes(8);
                stream.Seek(0, SeekOrigin.Begin);

                // Get the file type
                SupportedFileType ft = BurnOutSharp.Tools.Utilities.GetFileType(magic);
                if (ft == SupportedFileType.UNKNOWN)
                {
                    string extension = Path.GetExtension(file).TrimStart('.');
                    ft = BurnOutSharp.Tools.Utilities.GetFileType(extension);
                }

                // Print out the file format
                Console.WriteLine($"File format found: {ft}");

                // Setup the wrapper to print
                string wrapperName = null;
                WrapperBase wrapper = null;

                // Assign the correct wrapper
                switch (ft)
                {
                    // AACS Media Key Block
                    case SupportedFileType.AACSMediaKeyBlock:
                        wrapperName = "AACS media key block";
                        wrapper = AACSMediaKeyBlock.Create(stream);
                        break;

                    // BD+ SVM
                    case SupportedFileType.BDPlusSVM:
                        wrapperName = "BD+ SVM";
                        wrapper = BDPlusSVM.Create(stream);
                        break;

                    // BFPK archive
                    case SupportedFileType.BFPK:
                        wrapperName = "BFPK archive";
                        wrapper = BFPK.Create(stream);
                        break;

                    // BSP
                    case SupportedFileType.BSP:
                        wrapperName = "BSP";
                        wrapper = BSP.Create(stream);
                        break;

                    // CFB
                    case SupportedFileType.CFB:
                        wrapperName = "Compact File Binary";
                        wrapper = CFB.Create(stream);
                        break;

                    // CIA
                    case SupportedFileType.CIA:
                        wrapperName = "CIA";
                        wrapper = CIA.Create(stream);
                        break;

                    // MS-DOS executable and decendents
                    case SupportedFileType.Executable:
                        wrapperName = "MS-DOS executable";
                        wrapper = MSDOS.Create(stream);
                        if (wrapper != null)
                        {
                            // Check for a valid new executable address
                            if ((wrapper as MSDOS).NewExeHeaderAddr >= stream.Length)
                                break;

                            // Try to read the executable info
                            stream.Seek((wrapper as MSDOS).NewExeHeaderAddr, SeekOrigin.Begin);
                            magic = stream.ReadBytes(4);

                            // New Executable
                            if (magic.StartsWith(BurnOutSharp.Models.NewExecutable.Constants.SignatureBytes))
                            {
                                stream.Seek(0, SeekOrigin.Begin);
                                wrapperName = "New Executable";
                                wrapper = NewExecutable.Create(stream);
                            }

                            // Linear Executable
                            else if (magic.StartsWith(BurnOutSharp.Models.LinearExecutable.Constants.LESignatureBytes)
                                || magic.StartsWith(BurnOutSharp.Models.LinearExecutable.Constants.LXSignatureBytes))
                            {
                                stream.Seek(0, SeekOrigin.Begin);
                                wrapperName = "Linear Executable";
                                wrapper = LinearExecutable.Create(stream);
                            }

                            // Portable Executable
                            else if (magic.StartsWith(BurnOutSharp.Models.PortableExecutable.Constants.SignatureBytes))
                            {
                                stream.Seek(0, SeekOrigin.Begin);
                                wrapperName = "Portable Executable";
                                wrapper = PortableExecutable.Create(stream);
                            }
                        }

                        break;

                    // GCF
                    case SupportedFileType.GCF:
                        wrapperName = "GCF";
                        wrapper = GCF.Create(stream);
                        break;

                    // IS-CAB archive
                    case SupportedFileType.InstallShieldCAB:
                        wrapperName = "InstallShield Cabinet";
                        wrapper = InstallShieldCabinet.Create(stream);
                        break;

                    // MoPaQ (MPQ) archive
                    case SupportedFileType.MPQ:
                        wrapperName = "MoPaQ archive";
                        //wrapper = MPQ.Create(stream);
                        break;

                    // MS-CAB archive
                    case SupportedFileType.MicrosoftCAB:
                        wrapperName = "Microsoft Cabinet";
                        wrapper = MicrosoftCabinet.Create(stream);
                        break;

                    // N3DS
                    case SupportedFileType.N3DS:
                        wrapperName = "Nintendo 3DS";
                        wrapper = N3DS.Create(stream);
                        break;

                    // NCF
                    case SupportedFileType.NCF:
                        wrapperName = "NCF";
                        wrapper = NCF.Create(stream);
                        break;

                    // Nitro
                    case SupportedFileType.Nitro:
                        wrapperName = "Nintendo DS/DSi";
                        wrapper = Nitro.Create(stream);
                        break;

                    // PAK
                    case SupportedFileType.PAK:
                        wrapperName = "Nintendo DS/DSi";
                        wrapper = PAK.Create(stream);
                        break;

                    // Quantum
                    case SupportedFileType.Quantum:
                        wrapperName = "Quantum archive";
                        wrapper = Quantum.Create(stream);
                        break;

                    // SGA
                    case SupportedFileType.SGA:
                        wrapperName = "SGA";
                        wrapper = SGA.Create(stream);
                        break;

                    // VBSP
                    case SupportedFileType.VBSP:
                        wrapperName = "VBSP";
                        wrapper = VBSP.Create(stream);
                        break;

                    // VPK
                    case SupportedFileType.VPK:
                        wrapperName = "VPK";
                        wrapper = VPK.Create(stream);
                        break;

                    // WAD
                    case SupportedFileType.WAD:
                        wrapperName = "Valve WAD";
                        wrapper = WAD.Create(stream);
                        break;

                    // XZP
                    case SupportedFileType.XZP:
                        wrapperName = "XZP";
                        wrapper = XZP.Create(stream);
                        break;

                    default:
                        Console.WriteLine($"{ft} cannot have information printed yet!");
                        Console.WriteLine();
                        return;
                }

                // If we don't have a wrapper
                if (wrapper == null)
                {
                    Console.WriteLine($"Something went wrong parsing {wrapperName}!");
                    Console.WriteLine();
                    return;
                }

                // Print the wrapper name
                Console.WriteLine($"{wrapperName} wrapper created successfully!");

#if NET6_0_OR_GREATER
                // If we have the JSON flag
                if (json)
                {
                    // Create the output data
                    string serializedData = wrapper.ExportJSON();
                    Console.WriteLine(serializedData);

                    // Write the output data
                    using (var sw = new StreamWriter(File.OpenWrite($"info-{DateTime.Now:yyyy-MM-dd_HHmmss}.json")))
                    {
                        sw.WriteLine(serializedData);
                    }
                }
#endif
                // If we don't have the JSON flag
                if (!json)
                {
                    // Create the output data
                    StringBuilder builder = wrapper.PrettyPrint();
                    Console.WriteLine(builder);

                    // Write the output data
                    using (var sw = new StreamWriter(File.OpenWrite($"info-{DateTime.Now:yyyy-MM-dd_HHmmss}.txt")))
                    {
                        sw.WriteLine(builder.ToString());
                    }
                }
            }
        }
    }
}