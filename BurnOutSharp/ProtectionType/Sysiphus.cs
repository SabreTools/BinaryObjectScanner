namespace BurnOutSharp.ProtectionType
{
    public class Sysiphus : IContentCheck
    {
        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includePosition = false)
        {
            // "V SUHPISYSDVD"
            byte?[] check = new byte?[] { 0x56, 0x20, 0x53, 0x55, 0x48, 0x50, 0x49, 0x53, 0x59, 0x53, 0x44, 0x56, 0x44 };
            if (fileContent.FirstPosition(check, out int position))
                return $"Sysiphus DVD {GetVersion(fileContent, position)}" + (includePosition ? $" (Index {position})" : string.Empty);

            // "V SUHPISYS"
            check = new byte?[] { 0x56, 0x20, 0x53, 0x55, 0x48, 0x50, 0x49, 0x53, 0x59, 0x53 };
            if (fileContent.FirstPosition(check, out position))
                return $"Sysiphus {GetVersion(fileContent, position)}" + (includePosition ? $" (Index {position})" : string.Empty);

            return null;
        }

        private static string GetVersion(byte[] fileContent, int position)
        {
            int index = position - 3;
            char subVersion = (char)fileContent[index];
            index++;
            index++;
            char version = (char)fileContent[index];

            if (char.IsNumber(version) && char.IsNumber(subVersion))
                return $"{version}.{subVersion}";
            else
                return "";
        }
    }
}
