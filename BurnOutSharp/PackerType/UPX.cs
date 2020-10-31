using System.Text;

namespace BurnOutSharp.PackerType
{
    public class UPX
    {
        public static string CheckContents(byte[] fileContent, bool includePosition = false)
        {
            // UPX!
            byte[] check = new byte[] { 0x55, 0x50, 0x58, 0x21 };
            if (fileContent.Contains(check, out int position))
            {
                string version = GetVersion(fileContent, position);
                return $"UPX {version}" + (includePosition ? $" (Index {position})" : string.Empty);
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