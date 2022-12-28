using System.Runtime.InteropServices;

namespace BurnOutSharp.Models.MoPaQ
{
    /// <summary>
    /// MPQ User Data are optional, and is commonly used in custom maps for
    /// Starcraft II. If MPQ User Data header is present, it contains an offset,
    /// from where the MPQ header should be searched.
    /// </summary>
    /// <see href="http://zezula.net/en/mpq/mpqformat.html"/>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class UserData
    {
        /// <summary>
        /// The user data signature
        /// </summary>
        /// <see cref="SignatureValue"/>
        public string Signature;

        /// <summary>
        /// Maximum size of the user data
        /// </summary>
        public uint UserDataSize;

        /// <summary>
        /// Offset of the MPQ header, relative to the beginning of this header
        /// </summary>
        public uint HeaderOffset;

        /// <summary>
        /// Appears to be size of user data header (Starcraft II maps)
        /// </summary>
        public uint UserDataHeaderSize;

        // TODO: Does this area contain extra data that should be read in?
    }
}
