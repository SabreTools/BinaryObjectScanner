using System.Collections.Generic;

namespace BinaryObjectScanner.Models.PortableExecutable
{
    /// <summary>
    /// Contains information about formatted text for display as an error message or in a message
    /// box in a message table resource.
    /// </summary>
    /// <see href="https://learn.microsoft.com/en-us/windows/win32/api/winnt/ns-winnt-message_resource_data"/>
    public sealed class MessageResourceData
    {
        /// <summary>
        /// The number of MESSAGE_RESOURCE_BLOCK structures.
        /// </summary>
        public uint NumberOfBlocks;

        /// <summary>
        /// An array of structures. The array is the size indicated by the NumberOfBlocks member.
        /// </summary>
        public MessageResourceBlock[] Blocks;

        /// <summary>
        /// Message resource entries
        /// </summary>
        public Dictionary<uint, MessageResourceEntry> Entries;
    }
}
