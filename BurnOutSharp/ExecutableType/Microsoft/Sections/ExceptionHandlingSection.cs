using BurnOutSharp.ExecutableType.Microsoft.Tables;

namespace BurnOutSharp.ExecutableType.Microsoft.Sections
{
    /// <summary>
    /// The .pdata section contains an array of function table entries that are used for exception handling.
    /// It is pointed to by the exception table entry in the image data directory.
    /// The entries must be sorted according to the function addresses (the first field in each structure) before being emitted into the final image.
    /// The target platform determines which of the three function table entry format variations described below is used.
    /// </summary>
    /// <remarks>https://docs.microsoft.com/en-us/windows/win32/debug/pe-format#the-edata-section-image-only</remarks>
    internal class ExceptionHandlingSection
    {
        /// <summary>
        /// Array of function table entries that are used for exception handling
        /// </summary>
        public FunctionTable FunctionTable;
    }
}