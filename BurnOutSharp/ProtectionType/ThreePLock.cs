namespace BurnOutSharp.ProtectionType
{
    public class ThreePLock : IContentCheck
    {
        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includePosition = false)
        {
            // .ldr
            byte?[] check = new byte?[] { 0x2E, 0x6C, 0x64, 0x72 };
            if (fileContent.FirstPosition(check, out int position))
            {
                // .ldt
                byte?[] check2 = new byte?[] { 0x2E, 0x6C, 0x64, 0x74 };
                if (fileContent.FirstPosition(check2, out int position2))
                    return "3PLock" + (includePosition ? $" (Index {position}, {position2})" : string.Empty);
            }

            // This produced false positives in some DirectX 9.0c installer files
            // "Y" + (char)0xC3 + "U" + (char)0x8B + (char)0xEC + (char)0x83 + (char)0xEC + "0SVW"
            //check = new byte?[] { 0x59, 0xC3, 0x55, 0x8B, 0xEC, 0x83, 0xEC, 0x30, 0x53, 0x56, 0x57 };
            //if (fileContent.Contains(check, out position))
            //    return "3PLock" + (includePosition ? $" (Index {position})" : string.Empty);

            return null;
        }
    }
}
