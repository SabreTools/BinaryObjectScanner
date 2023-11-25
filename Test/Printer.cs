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
#if NET20 || NET35
                foreach (string file in Directory.GetFiles(path, "*", SearchOption.AllDirectories))
#else
                foreach (string file in Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories))
#endif
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

            using Stream stream = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read);

            // Read the first 8 bytes
            byte[]? magic = stream.ReadBytes(8);
            stream.Seek(0, SeekOrigin.Begin);

            // Get the file type
            SupportedFileType ft = FileTypes.GetFileType(magic ?? []);
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
                    using var jsw = new StreamWriter(File.OpenWrite($"{filenameBase}.json"));
                    jsw.WriteLine(serializedData);
                }
#endif
            // Create the output data
            var builder = wrapper.PrettyPrint();
            Console.WriteLine(builder);

            // Write the output data
            using var sw = new StreamWriter(File.OpenWrite($"{filenameBase}.txt"));
            sw.WriteLine(builder.ToString());
        }

        #region Printing Implementations

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this IWrapper wrapper)
        {
            return wrapper switch
            {
                AACSMediaKeyBlock item => item.PrettyPrint(),
                BDPlusSVM item => item.PrettyPrint(),
                BFPK item => item.PrettyPrint(),
                BSP item => item.PrettyPrint(),
                CFB item => item.PrettyPrint(),
                CIA item => item.PrettyPrint(),
                GCF item => item.PrettyPrint(),
                InstallShieldCabinet item => item.PrettyPrint(),
                IRD item => item.PrettyPrint(),
                LinearExecutable item => item.PrettyPrint(),
                MicrosoftCabinet item => item.PrettyPrint(),
                MSDOS item => item.PrettyPrint(),
                N3DS item => item.PrettyPrint(),
                NCF item => item.PrettyPrint(),
                NewExecutable item => item.PrettyPrint(),
                Nitro item => item.PrettyPrint(),
                PAK item => item.PrettyPrint(),
                PFF item => item.PrettyPrint(),
                PlayJAudioFile item => item.PrettyPrint(),
                PortableExecutable item => item.PrettyPrint(),
                Quantum item => item.PrettyPrint(),
                SGA item => item.PrettyPrint(),
                VBSP item => item.PrettyPrint(),
                VPK item => item.PrettyPrint(),
                WAD item => item.PrettyPrint(),
                XeMID item => item.PrettyPrint(),
                XMID item => item.PrettyPrint(),
                XZP item => item.PrettyPrint(),
                _ => new StringBuilder(),
            };
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this AACSMediaKeyBlock item)
        {
            var builder = new StringBuilder();
            SabreTools.Printing.AACSMediaKeyBlock.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this BDPlusSVM item)
        {
            var builder = new StringBuilder();
            SabreTools.Printing.BDPlusSVM.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this BFPK item)
        {
            var builder = new StringBuilder();
            SabreTools.Printing.BFPK.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this BSP item)
        {
            var builder = new StringBuilder();
            SabreTools.Printing.BSP.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this CFB item)
        {
            var builder = new StringBuilder();
            SabreTools.Printing.CFB.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this CIA item)
        {
            var builder = new StringBuilder();
            SabreTools.Printing.CIA.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this GCF item)
        {
            var builder = new StringBuilder();
            SabreTools.Printing.GCF.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this InstallShieldCabinet item)
        {
            var builder = new StringBuilder();
            SabreTools.Printing.InstallShieldCabinet.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this IRD item)
        {
            var builder = new StringBuilder();
            SabreTools.Printing.IRD.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this LinearExecutable item)
        {
            var builder = new StringBuilder();
            SabreTools.Printing.LinearExecutable.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this MicrosoftCabinet item)
        {
            var builder = new StringBuilder();
            SabreTools.Printing.MicrosoftCabinet.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this MSDOS item)
        {
            var builder = new StringBuilder();
            SabreTools.Printing.MSDOS.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this N3DS item)
        {
            var builder = new StringBuilder();
            SabreTools.Printing.N3DS.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this NCF item)
        {
            var builder = new StringBuilder();
            SabreTools.Printing.NCF.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this NewExecutable item)
        {
            var builder = new StringBuilder();
            SabreTools.Printing.NewExecutable.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this Nitro item)
        {
            var builder = new StringBuilder();
            SabreTools.Printing.Nitro.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this PAK item)
        {
            var builder = new StringBuilder();
            SabreTools.Printing.PAK.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this PFF item)
        {
            var builder = new StringBuilder();
            SabreTools.Printing.PFF.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this PlayJAudioFile item)
        {
            var builder = new StringBuilder();
            SabreTools.Printing.PlayJAudioFile.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this PortableExecutable item)
        {
            var builder = new StringBuilder();
            SabreTools.Printing.PortableExecutable.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this Quantum item)
        {
            var builder = new StringBuilder();
            SabreTools.Printing.Quantum.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this SGA item)
        {
            var builder = new StringBuilder();
            SabreTools.Printing.SGA.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this VBSP item)
        {
            var builder = new StringBuilder();
            SabreTools.Printing.VBSP.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this VPK item)
        {
            var builder = new StringBuilder();
            SabreTools.Printing.VPK.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this WAD item)
        {
            var builder = new StringBuilder();
            SabreTools.Printing.WAD.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this XeMID item)
        {
            var builder = new StringBuilder();
            SabreTools.Printing.XeMID.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this XMID item)
        {
            var builder = new StringBuilder();
            SabreTools.Printing.XMID.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this XZP item)
        {
            var builder = new StringBuilder();
            SabreTools.Printing.XZP.Print(builder, item.Model);
            return builder;
        }

        #endregion
    }
}