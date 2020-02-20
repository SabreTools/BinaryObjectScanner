namespace BurnOutSharp.ProtectionType
{
    public class PSXAntiModchip
    {
        // TODO: Figure out PSX binary header so this can be checked explicitly
        public static string CheckContents(string file, string fileContent)
        {
            // TODO: Detect Red Hand protection
            if (fileContent.Contains("     SOFTWARE TERMINATED\nCONSOLE MAY HAVE BEEN MODIFIED\n     CALL 1-888-780-7690"))
                return "PlayStation Anti-modchip (English)";

            if (fileContent.Contains("強制終了しました。\n本体が改造されている\nおそれがあります。"))
                return "PlayStation Anti-modchip (Japanese)";

            return null;
        }
    }
}
