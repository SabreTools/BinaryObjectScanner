using System.IO;
using System.Text;
using BurnOutSharp.Tools;
 
namespace BurnOutSharp.ExecutableType.Microsoft.Entries
{
    /// <summary>
    /// These name strings are case-sensitive and are not null-terminated
    /// </summary>
    public class ResidentNameTableEntry
    {
        /// <summary>
        /// Length of the name string that follows.
        /// A zero value indicates the end of the name table.
        /// </summary>
        public byte Length;

        /// <summary>
        /// ASCII text of the name string.
        /// </summary>
        public byte[] Data;

        /// <summary>
        /// Ordinal number (index into entry table).
        /// This value is ignored for the module name.
        /// </summary>
        public ushort OrdinalNumber;

        /// <summary>
        /// ASCII text of the name string
        /// </summary>
        public string DataAsString
        {
            get
            {
                if (Data == null)
                    return string.Empty;

                // Try to read direct as ASCII
                try
                {
                    return Encoding.ASCII.GetString(Data);
                }
                catch { }

                // If ASCII encoding fails, then just return an empty string
                return string.Empty;
            }
        }

        public static ResidentNameTableEntry Deserialize(Stream stream)
        {
            var rnte = new ResidentNameTableEntry();

            rnte.Length = stream.ReadByteValue();
            if (rnte.Length == 0)
                return rnte;

            rnte.Data = stream.ReadBytes(rnte.Length);
            rnte.OrdinalNumber = stream.ReadUInt16();

            return rnte;
        }

        public static ResidentNameTableEntry Deserialize(byte[] content, ref int offset)
        {
            var rnte = new ResidentNameTableEntry();

            rnte.Length = content.ReadByte(ref offset);
            if (rnte.Length == 0)
                return rnte;

            rnte.Data = content.ReadBytes(ref offset, rnte.Length);
            rnte.OrdinalNumber = content.ReadUInt16(ref offset);

            return rnte;
        }
    }
}