namespace BurnOutSharp.Models.PortableExecutable
{
    /// <summary>
    /// Contains information about message strings with identifiers in the range indicated
    /// by the LowId and HighId members.
    /// </summary>
    /// <see href="https://learn.microsoft.com/en-us/windows/win32/api/winnt/ns-winnt-message_resource_block"/>
    public class MessageResourceBlock
    {
        /// <summary>
        /// The lowest message identifier contained within this structure.
        /// </summary>
        public uint LowId;

        /// <summary>
        /// The highest message identifier contained within this structure.
        /// </summary>
        public uint HighId;

        /// <summary>
        /// The offset, in bytes, from the beginning of the MESSAGE_RESOURCE_DATA structure to the
        /// MESSAGE_RESOURCE_ENTRY structures in this MESSAGE_RESOURCE_BLOCK. The MESSAGE_RESOURCE_ENTRY
        /// structures contain the message strings.
        /// </summary>
        public uint OffsetToEntries;
    }
}
