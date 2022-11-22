namespace BurnOutSharp.Builder
{
    /// <summary>
    /// Methods related to Object Identifiers (OID) and dot notation
    /// </summary>
    public static partial class ObjectIdentifier
    {
        /// <summary>
        /// Parse an OID in separated-value notation into dot notation
        /// </summary>
        /// <param name="values">List of values to check against</param>
        /// <returns>List of values representing the dot notation</returns>
        public static string ParseOIDToDotNotation(ulong[] values)
        {
            return string.Join(".", values);
        }
    }
}