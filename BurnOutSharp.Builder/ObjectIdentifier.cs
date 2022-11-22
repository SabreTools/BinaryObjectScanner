using System;
using System.Collections.Generic;

namespace BurnOutSharp.Builder
{
    /// <summary>
    /// Methods related to Object Identifiers (OID)
    /// </summary>
    public static partial class ObjectIdentifier
    {
        // TODO: ulong[] isn't going to work. If we can use .NET 7, we can use UInt128
        // We might want to look into storing all values as GUID? I don't remember if
        // you can do value comparisions between an integral value and a GUID, though.

        /// <summary>
        /// Parse an OID in DER-encoded byte notation into a list of values
        /// </summary>
        /// <param name="data">Byte array representing the data to read</param>
        /// <param name="length">Total length of the data according to the DER TLV</param>
        /// <returns>Array of values representing the OID</returns>
        public static ulong[] ParseDERIntoArray(byte[] data, ulong length)
        {
            // The first byte contains nodes 1 and 2
            int firstNode = Math.DivRem(data[0], 40, out int secondNode);

            // Create a list for all nodes
            List<ulong> nodes = new List<ulong> { (ulong)firstNode, (ulong)secondNode };

            // All other nodes are encoded uniquely
            int offset = 1;
            while (offset < (long)length)
            {
                // If bit 7 is not set
                if ((data[offset] & 0x80) == 0)
                {
                    nodes.Add(data[offset]);
                    offset++;
                    continue;
                }

                // Otherwise, read the encoded value in a loop
                ulong dotValue = 0;
                bool doneProcessing = false;

                do
                {
                    // Shift the current encoded value
                    dotValue <<= 7;

                    // If we have a leading zero byte, we're at the end
                    if ((data[offset] & 0x80) == 0)
                        doneProcessing = true;

                    // Clear the top byte
                    unchecked { data[offset] &= (byte)~0x80; }

                    // Add the new value to the result
                    dotValue |= data[offset];

                    // Increment the offset
                    offset++;
                } while (offset < data.Length && !doneProcessing);

                // Add the parsed value to the output
                nodes.Add(dotValue);
            }

            return nodes.ToArray();
        }
    }
}
