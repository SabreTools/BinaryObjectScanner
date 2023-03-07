using System;

namespace BinaryObjectScanner.Models.CFB
{
    public static class Constants
    {
        public static readonly byte[] SignatureBytes = new byte[] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1 };
    
        public const ulong SignatureUInt64 = 0xE11AB1A1E011CFD0;

        
        /// <see href="https://devblogs.microsoft.com/setup/identifying-windows-installer-file-types/"/>
        #region Class IDs

        /// <summary>
        /// Installer Package (msi), Merge Module (msm), Patch Creation Properties (pcp)
        /// </summary>
        public static readonly Guid InstallerPackage = new Guid("000c1084-0000-0000-c000-000000000046");

        /// <summary>
        /// Patch Package (msp)
        /// </summary>
        public static readonly Guid PatchPackage = new Guid("000C1086-0000-0000-C000-000000000046");

        /// <summary>
        /// Transform (mst)
        /// </summary>
        public static readonly Guid Transform = new Guid("000C1082-0000-0000-C000-000000000046");

        #endregion

        /// <see href="https://learn.microsoft.com/en-us/windows/win32/stg/predefined-property-set-format-identifiers"/>
        #region Property Set Format IDs

        /// <summary>
        /// The Summary Information Property Set
        /// </summary>
        public static readonly Guid FMTID_SummaryInformation = new Guid("F29F85E0-4FF9-1068-AB91-08002B27B3D9");
        
        /// <summary>
        /// The DocumentSummaryInformation and UserDefined Property Sets
        /// </summary>
        public static readonly Guid FMTID_DocSummaryInformation = new Guid("D5CDD502-2E9C-101B-9397-08002B2CF9AE");
        
        /// <summary>
        /// The DocumentSummaryInformation and UserDefined Property Sets
        /// </summary>
        public static readonly Guid FMTID_UserDefinedProperties = new Guid("D5CDD505-2E9C-101B-9397-08002B2CF9AE");

        #endregion
    }
}