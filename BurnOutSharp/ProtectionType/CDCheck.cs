namespace BurnOutSharp.ProtectionType
{
    public class CDCheck
    {
        public static string CheckContents(string fileContent)
        {
            if (fileContent.Contains("GetDriveType")
                || fileContent.Contains("GetVolumeInformation"))
                return "CD Check";

            return null;
        }
    }
}
