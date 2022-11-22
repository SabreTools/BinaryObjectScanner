using System.Linq;
using System.Text;

namespace BurnOutSharp.Builder
{
    /// <summary>
    /// Methods related to Object Identifiers (OID) and OID-IRI formatting
    /// </summary>
    public static partial class ObjectIdentifier
    {
        /// <summary>
        /// Parse an OID in separated-value notation into OID-IRI notation
        /// </summary>
        /// <param name="values">List of values to check against</param>
        /// <param name="index">Current index into the list</param>
        /// <returns>OID-IRI formatted string, if possible</returns>
        /// <see href="http://www.oid-info.com/index.htm"/>
        public static string ParseOIDToOIDIRINotation(ulong[] values)
        {
            // If we have an invalid set of values, we can't do anything
            if (values == null || values.Length == 0)
                return null;

            // Set the initial index
            int index = 0;

            // Get a string builder for the path
            var nameBuilder = new StringBuilder();

            // Try to parse the standard value
            string standard = ParseOIDToOIDIRINotation(values, ref index);
            if (standard == null)
                return null;

            // Add the standard value to the output
            nameBuilder.Append(standard);

            // If we have no more items
            if (index == values.Length)
                return nameBuilder.ToString();

            // Add trailing items as just values
            nameBuilder.Append("/");
            nameBuilder.Append(string.Join("/", values.Skip(index)));

            // Create and return the string
            return nameBuilder.ToString();
        }

        /// <summary>
        /// Parse an OID in separated-value notation into OID-IRI notation
        /// </summary>
        /// <param name="values">List of values to check against</param>
        /// <param name="index">Current index into the list</param>
        /// <returns>OID-IRI formatted string, if possible</returns>
        /// <see href="http://www.oid-info.com/index.htm"/>
        private static string ParseOIDToOIDIRINotation(ulong[] values, ref int index)
        {
            // TODO: Once the modified OID-IRI formatting is done, make a compliant version
            return null;
        }
    }
}