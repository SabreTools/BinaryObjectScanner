using System.Runtime.InteropServices;

namespace BurnOutSharp.Models.NewExecutable
{
    /// <summary>
    /// Resource type and name strings are stored at the end of the
    /// resource table. Note that these strings are NOT null terminated and
    /// are case sensitive.
    /// </summary>
    /// <see href="http://bytepointer.com/resources/win16_ne_exe_format_win3.0.htm"/>
    [StructLayout(LayoutKind.Sequential)]
    public class ResourceTypeAndNameString
    {
        /// <summary>
        /// Length of the type or name string that follows. A zero value
        /// indicates the end of the resource type and name string, also
        /// the end of the resource table.
        /// </summary>
        public byte Length;

        /// <summary>
        /// ASCII text of the type or name string.
        /// </summary>
        public byte[] Text;
    }
}
