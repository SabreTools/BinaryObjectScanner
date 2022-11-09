namespace BurnOutSharp.Models.PortableExecutable
{
    /// <summary>
    /// Represents the organization of data in a file-version resource. It contains a string
    /// that describes a specific aspect of a file, for example, a file's version, its
    /// copyright notices, or its trademarks.
    /// </summary>
    /// <see href="https://learn.microsoft.com/en-us/windows/win32/menurc/string-str"/>
    public class StringData
    {
        /// <summary>
        /// The length, in bytes, of this String structure.
        /// </summary>
        public ushort Length;

        /// <summary>
        /// The size, in words, of the Value member.
        /// </summary>
        public ushort ValueLength;

        /// <summary>
        /// The type of data in the version resource.
        /// </summary>
        public VersionResourceType ResourceType;

        /// <summary>
        /// An arbitrary Unicode string. The Key member can be one or more of the following
        /// values. These values are guidelines only.
        /// - Comments: The Value member contains any additional information that should be
        ///     displayed for diagnostic purposes. This string can be an arbitrary length.
        /// - CompanyName: The Value member identifies the company that produced the file.
        ///     For example, "Microsoft Corporation" or "Standard Microsystems Corporation, Inc."
        /// - FileDescription: The Value member describes the file in such a way that it can be
        ///     presented to users. This string may be presented in a list box when the user is
        ///     choosing files to install. For example, "Keyboard driver for AT-style keyboards"
        ///     or "Microsoft Word for Windows".
        /// - FileVersion: The Value member identifies the version of this file. For example,
        ///     Value could be "3.00A" or "5.00.RC2".
        /// - InternalName: The Value member identifies the file's internal name, if one exists.
        ///     For example, this string could contain the module name for a DLL, a virtual device
        ///     name for a Windows virtual device, or a device name for a MS-DOS device driver.
        /// - LegalCopyright: The Value member describes all copyright notices, trademarks, and
        ///     registered trademarks that apply to the file. This should include the full text of
        ///     all notices, legal symbols, copyright dates, trademark numbers, and so on. In
        ///     English, this string should be in the format "Copyright Microsoft Corp. 1990 1994".
        /// - LegalTrademarks: The Value member describes all trademarks and registered trademarks
        ///     that apply to the file. This should include the full text of all notices, legal
        ///     symbols, trademark numbers, and so on. In English, this string should be in the
        ///     format "Windows is a trademark of Microsoft Corporation".
        /// - OriginalFilename: The Value member identifies the original name of the file, not
        ///     including a path. This enables an application to determine whether a file has been
        ///     renamed by a user. This name may not be MS-DOS 8.3-format if the file is specific
        ///     to a non-FAT file system.
        /// - PrivateBuild: The Value member describes by whom, where, and why this private version
        ///     of the file was built. This string should only be present if the VS_FF_PRIVATEBUILD
        ///     flag is set in the dwFileFlags member of the VS_FIXEDFILEINFO structure. For example,
        ///     Value could be "Built by OSCAR on \OSCAR2".
        /// - ProductName: The Value member identifies the name of the product with which this file is
        ///     distributed. For example, this string could be "Microsoft Windows".
        /// - ProductVersion: The Value member identifies the version of the product with which this
        ///     file is distributed. For example, Value could be "3.00A" or "5.00.RC2".
        /// - SpecialBuild: The Value member describes how this version of the file differs from the
        ///     normal version. This entry should only be present if the VS_FF_SPECIALBUILD flag is
        ///     set in the dwFileFlags member of the VS_FIXEDFILEINFO structure. For example, Value
        ///     could be "Private build for Olivetti solving mouse problems on M250 and M250E computers".
        /// </summary>
        public string Key;

        /// <summary>
        /// As many zero words as necessary to align the Value member on a 32-bit boundary.
        /// </summary>
        public ushort Padding;

        /// <summary>
        /// A zero-terminated string. See the szKey member description for more information.
        /// </summary>
        public string Value;
    }
}
