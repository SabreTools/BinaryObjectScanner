using System;
using System.IO;
using System.Text;
using BinaryObjectScanner.Utilities;
using SabreTools.IO;
using SabreTools.Serialization.Interfaces;
using SabreTools.Serialization.Wrappers;

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
                SupportedFileType ft = FileTypes.GetFileType(magic);
                if (ft == SupportedFileType.UNKNOWN)
                {
                    string extension = Path.GetExtension(file).TrimStart('.');
                    ft = FileTypes.GetFileType(extension);
                }

                // Print out the file format
                Console.WriteLine($"File format found: {ft}");

                // Setup the wrapper to print
                var wrapper = WrapperFactory.CreateWrapper(ft, stream);

                // If we don't have a wrapper
                if (wrapper == null)
                {
                    Console.WriteLine($"Either {ft} is not supported or something went wrong during parsing!");
                    Console.WriteLine();
                    return;
                }

                // Print the wrapper name
                Console.WriteLine($"{wrapper.Description()} wrapper created successfully!");

                // Get the base info output name
                string filenameBase = $"info-{DateTime.Now:yyyy-MM-dd_HHmmss.ffff}";

#if NET6_0_OR_GREATER
                // If we have the JSON flag
                if (json)
                {
                    // Create the output data
                    string serializedData = wrapper.ExportJSON();
                    Console.WriteLine(serializedData);

                    // Write the output data
                    using (var sw = new StreamWriter(File.OpenWrite($"{filenameBase}.json")))
                    {
                        sw.WriteLine(serializedData);
                    }
                }
#endif
                // Create the output data
                StringBuilder builder = wrapper.PrettyPrint();
                Console.WriteLine(builder);

                // Write the output data
                using (var sw = new StreamWriter(File.OpenWrite($"{filenameBase}.txt")))
                {
                    sw.WriteLine(builder.ToString());
                }
            }
        }

        #region Printing Implementations

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this IWrapper wrapper)
        {
            switch (wrapper)
            {
                case AACSMediaKeyBlock item: return item.PrettyPrint();
                case BDPlusSVM item: return item.PrettyPrint();
                case BFPK item: return item.PrettyPrint();
                case BSP item: return item.PrettyPrint();
                case CFB item: return item.PrettyPrint();
                case CIA item: return item.PrettyPrint();
                case GCF item: return item.PrettyPrint();
                case InstallShieldCabinet item: return item.PrettyPrint();
                case LinearExecutable item: return item.PrettyPrint();
                case MicrosoftCabinet item: return item.PrettyPrint();
                case MSDOS item: return item.PrettyPrint();
                case N3DS item: return item.PrettyPrint();
                case NCF item: return item.PrettyPrint();
                case NewExecutable item: return item.PrettyPrint();
                case Nitro item: return item.PrettyPrint();
                case PAK item: return item.PrettyPrint();
                case PFF item: return item.PrettyPrint();
                case PlayJAudioFile item: return item.PrettyPrint();
                case PortableExecutable item: return item.PrettyPrint();
                case Quantum item: return item.PrettyPrint();
                case SGA item: return item.PrettyPrint();
                case VBSP item: return item.PrettyPrint();
                case VPK item: return item.PrettyPrint();
                case WAD item: return item.PrettyPrint();
                case XZP item: return item.PrettyPrint();
                default: return new StringBuilder();
            }
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this AACSMediaKeyBlock item)
        {
            StringBuilder builder = new StringBuilder();
            SabreTools.Printing.AACSMediaKeyBlock.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this BDPlusSVM item)
        {
            StringBuilder builder = new StringBuilder();
            SabreTools.Printing.BDPlusSVM.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this BFPK item)
        {
            StringBuilder builder = new StringBuilder();
            SabreTools.Printing.BFPK.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this BSP item)
        {
            StringBuilder builder = new StringBuilder();
            SabreTools.Printing.BSP.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this CFB item)
        {
            StringBuilder builder = new StringBuilder();
            SabreTools.Printing.CFB.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this CIA item)
        {
            StringBuilder builder = new StringBuilder();
            SabreTools.Printing.CIA.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this GCF item)
        {
            StringBuilder builder = new StringBuilder();
            SabreTools.Printing.GCF.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this InstallShieldCabinet item)
        {
            StringBuilder builder = new StringBuilder();
            SabreTools.Printing.InstallShieldCabinet.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this LinearExecutable item)
        {
            StringBuilder builder = new StringBuilder();
            SabreTools.Printing.LinearExecutable.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this MicrosoftCabinet item)
        {
            StringBuilder builder = new StringBuilder();
            SabreTools.Printing.MicrosoftCabinet.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this MSDOS item)
        {
            StringBuilder builder = new StringBuilder();
            SabreTools.Printing.MSDOS.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this N3DS item)
        {
            StringBuilder builder = new StringBuilder();
            SabreTools.Printing.N3DS.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this NCF item)
        {
            StringBuilder builder = new StringBuilder();
            SabreTools.Printing.NCF.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this NewExecutable item)
        {
            StringBuilder builder = new StringBuilder();
            SabreTools.Printing.NewExecutable.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this Nitro item)
        {
            StringBuilder builder = new StringBuilder();
            SabreTools.Printing.Nitro.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this PAK item)
        {
            StringBuilder builder = new StringBuilder();
            SabreTools.Printing.PAK.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this PFF item)
        {
            StringBuilder builder = new StringBuilder();
            SabreTools.Printing.PFF.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this PlayJAudioFile item)
        {
            StringBuilder builder = new StringBuilder();
            SabreTools.Printing.PlayJAudioFile.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this PortableExecutable item)
        {
            StringBuilder builder = new StringBuilder();
            SabreTools.Printing.PortableExecutable.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this Quantum item)
        {
            StringBuilder builder = new StringBuilder();
            SabreTools.Printing.Quantum.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this SGA item)
        {
            StringBuilder builder = new StringBuilder();
            SabreTools.Printing.SGA.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this VBSP item)
        {
            StringBuilder builder = new StringBuilder();
            SabreTools.Printing.VBSP.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this VPK item)
        {
            StringBuilder builder = new StringBuilder();
            SabreTools.Printing.VPK.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this WAD item)
        {
            StringBuilder builder = new StringBuilder();
            SabreTools.Printing.WAD.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this XZP item)
        {
            StringBuilder builder = new StringBuilder();
            SabreTools.Printing.XZP.Print(builder, item.Model);
            return builder;
        }

        #endregion
    }
}