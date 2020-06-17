namespace BurnOutSharp.ProtectionType
{
    public class CDCheck
    {
        public static string CheckContents(string fileContent)
        {
            string check = "GetDriveType";
            if (fileContent.Contains(check))
                return $"CD Check (Index {fileContent.IndexOf(check)})";

            check = "GetVolumeInformation";
            if (fileContent.Contains(check))
                return $"CD Check (Index {fileContent.IndexOf(check)})";

            return null;
        }
    }
}
