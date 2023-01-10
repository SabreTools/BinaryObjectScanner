using System;

namespace BurnOutSharp.Models.CFB
{
    public static class Constants
    {
        public static readonly byte[] SignatureBytes = new byte[] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1 };
    
        public const ulong SignatureUInt64 = 0xE11AB1A1E011CFD0;

        #region Class IDs

        // <remarks>Shares a value with Merge Module and Patch Creation Properties</remarks>
        public static readonly Guid InstallerPackage = new Guid("000c1084-0000-0000-c000-000000000046");

        public static readonly Guid PatchPackage = new Guid("000C1086-0000-0000-C000-000000000046");

        public static readonly Guid Transform = new Guid("000C1082-0000-0000-C000-000000000046");

        #endregion
    }
}