using System.IO;
using BurnOutSharp.ExecutableType.Microsoft.PE.Headers;
using BurnOutSharp.ExecutableType.Microsoft.PE.Tables;

namespace BurnOutSharp.ExecutableType.Microsoft.PE.Sections
{
    /// <summary>
    /// The .debug section is used in object files to contain compiler-generated debug information and in image files to contain
    /// all of the debug information that is generated.
    /// This section describes the packaging of debug information in object and image files.
    /// </summary>
    /// <remarks>https://docs.microsoft.com/en-us/windows/win32/debug/pe-format#the-debug-section</remarks>
    public class DebugSection
    {
        /// <summary>
        /// Image files contain an optional debug directory that indicates what form of debug information is present and where it is.
        /// This directory consists of an array of debug directory entries whose location and size are indicated in the image optional header.
        /// </summary>
        public DebugDirectory DebugDirectory;

        public static DebugSection Deserialize(Stream stream, SectionHeader[] sections)
        {
            long originalPosition = stream.Position;
            var ds = new DebugSection();

            ds.DebugDirectory = DebugDirectory.Deserialize(stream);
            
            // TODO: Read in raw debug data

            return ds;
        }

        public static DebugSection Deserialize(byte[] content, ref int offset, SectionHeader[] sections)
        {
            int originalPosition = offset;
            var ds = new DebugSection();

            ds.DebugDirectory = DebugDirectory.Deserialize(content, ref offset);

            // TODO: Read in raw debug data

            return ds;
        }
    }
}