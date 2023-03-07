namespace BinaryObjectScanner.Models.PortableExecutable
{
    /// <summary>
    /// Contains the error message or message box display text for a message table resource.
    /// </summary>
    /// <see href="https://learn.microsoft.com/en-us/windows/win32/api/winnt/ns-winnt-message_resource_entry"/>
    public sealed class MessageResourceEntry
    {
        /// <summary>
        /// The length, in bytes, of the MESSAGE_RESOURCE_ENTRY structure.
        /// </summary>
        public ushort Length;

        /// <summary>
        /// Indicates that the string is encoded in Unicode, if equal to the value 0x0001.
        /// Indicates that the string is encoded in ANSI, if equal to the value 0x0000.
        /// </summary>
        public ushort Flags;

        /// <summary>
        /// Pointer to an array that contains the error message or message box display text.
        /// </summary>
        public string Text;
    }
}
