using System;
using System.IO;
using System.Linq;
using BinaryObjectScanner.Interfaces;
using SabreTools.Matching;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Packer
{
    /// <summary>
    /// Though not technically a packer, this detection is for any executables that include
    /// others in their resources in some uncompressed manner to be used at runtime.
    /// </summary>
    public class EmbeddedExecutable : IExtractablePortableExecutable, IPortableExecutableCheck
    {
        /// <inheritdoc/>
        public string? CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex.Model.SectionTable;
            if (sections == null)
                return null;

            // Get the resources that have an executable signature
            if (pex.ResourceData?.Any(kvp => kvp.Value is byte[] ba && ba.StartsWith(SabreTools.Models.MSDOS.Constants.SignatureBytes)) == true)
                return "Embedded Executable";

            return null;
        }

        /// <inheritdoc/>
        public string? Extract(string file, PortableExecutable pex, bool includeDebug)
        {
            try
            {
                // Get the resources that have an executable signature
                var resources = pex.ResourceData
                    .Where(kvp => kvp.Value != null && kvp.Value is byte[])
                    .Select(kvp => kvp.Value as byte[])
                    .Where(b => b != null && b.StartsWith(SabreTools.Models.MSDOS.Constants.SignatureBytes))
                    .ToList();

                string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                Directory.CreateDirectory(tempPath);

                for (int i = 0; i < resources.Count; i++)
                {
                    try
                    {
                        // Get the resource data
                        var data = resources[i];
                        if (data == null)
                            continue;

                        // Create the temp filename
                        string tempFile = $"embedded_resource_{i}.bin";
                        tempFile = Path.Combine(tempPath, tempFile);

                        // Write the resource data to a temp file
                        using (var tempStream = File.Open(tempFile, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                        {
                            if (tempStream != null)
                                tempStream.Write(data, 0, data.Length);
                        }
                    }
                    catch (Exception ex)
                    {
                        if (includeDebug) Console.WriteLine(ex);
                    }
                }

                return tempPath;
            }
            catch (Exception ex)
            {
                if (includeDebug) Console.WriteLine(ex);
                return null;
            }
        }
    }
}
