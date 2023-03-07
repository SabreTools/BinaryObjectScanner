namespace BinaryObjectScanner.Models.CFB
{
    /// <summary>
    /// VARIANT is a container for a union that can hold many types of data.
    /// </summary>
    /// <see href="https://learn.microsoft.com/en-us/openspecs/windows_protocols/ms-oaut/b2ee2b50-665e-43e6-a92c-8f2a29fd7add"/>
    public sealed class Variant
    {
        /// <summary>
        /// MUST be set to the size, in quad words (64 bits), of the structure.
        /// </summary>
        public uint Size;

        /// <summary>
        /// MUST be set to 0 and MUST be ignored by the recipient.
        /// </summary>
        public uint RpcReserved;

        /// <summary>
        /// MUST be set to one of the values specified with a "V".
        /// </summary>
        public VariantType VariantType;

        /// <summary>
        /// MAY be set to 0 and MUST be ignored by the recipient.
        /// </summary>
        public ushort Reserved1;

        /// <summary>
        /// MAY be set to 0 and MUST be ignored by the recipient.
        /// </summary>
        public ushort Reserved2;

        /// <summary>
        /// MAY be set to 0 and MUST be ignored by the recipient.
        /// </summary>
        public ushort Reserved3;

        /// <summary>
        /// MUST contain an instance of the type, according to the value
        /// in the <see cref="VariantType"/> field.
        /// </summary>
        public object Union;
    }
}