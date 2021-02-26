using System.Text;

namespace BurnOutSharp.PackerType
{
    public class UPX : IContentCheck
    {
        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includePosition = false)
        {
            // UPX!
            byte[] check = new byte[] { 0x55, 0x50, 0x58, 0x21 };
            if (fileContent.Contains(check, out int position))
            {
                string version = GetVersion(fileContent, position);
                return $"UPX {version}" + (includePosition ? $" (Index {position})" : string.Empty);
            }

            // NOS 
            check = new byte[] { 0x55, 0x50, 0x58, 0x21 };
            if (fileContent.Contains(check, out position))
            {
                string version = GetVersion(fileContent, position);
                return $"UPX (NOS Variant) {version}" + (includePosition ? $" (Index {position})" : string.Empty);
            }

            // UPX0
            check = new byte[] { 0x55, 0x50, 0x58, 0x30 };
            if (fileContent.Contains(check, out position))
            {
                // UPX1
                byte[] check2 = new byte[] { 0x55, 0x50, 0x58, 0x31 };
                if (fileContent.Contains(check2, out int position2))
                {
                    return $"UPX (Unknown Version)" + (includePosition ? $" (Index {position}, {position2})" : string.Empty);
                }                
            }

            // NOS0
            check = new byte[] { 0x4E, 0x4F, 0x53, 0x30 };
            if (fileContent.Contains(check, out position))
            {
                // NOS1
                byte[] check2 = new byte[] { 0x4E, 0x4F, 0x53, 0x31 };
                if (fileContent.Contains(check2, out int position2))
                {
                    return $"UPX (NOS Variant) (Unknown Version)" + (includePosition ? $" (Index {position}, {position2})" : string.Empty);
                }
            }

            return null;
        }

        private static string GetVersion(byte[] fileContent, int index)
        {
            try
            {
                index -= 5;
                string versionString = Encoding.ASCII.GetString(fileContent, index, 4);
                if (!char.IsNumber(versionString[0]))
                    return "(Unknown Version)";

                return versionString;
            }
            catch
            {
                return "(Unknown Version)";
            }
        }
    }
}