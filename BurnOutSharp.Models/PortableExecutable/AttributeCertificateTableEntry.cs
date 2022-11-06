namespace BurnOutSharp.Models.PortableExecutable
{
    /// <summary>
    /// Attribute certificates can be associated with an image by adding an attribute
    /// certificate table. The attribute certificate table is composed of a set of
    /// contiguous, quadword-aligned attribute certificate entries. Zero padding is
    /// inserted between the original end of the file and the beginning of the attribute
    /// certificate table to achieve this alignment.
    /// 
    /// The virtual address value from the Certificate Table entry in the Optional
    /// Header Data Directory is a file offset to the first attribute certificate
    /// entry. Subsequent entries are accessed by advancing that entry's dwLength
    /// bytes, rounded up to an 8-byte multiple, from the start of the current
    /// attribute certificate entry. This continues until the sum of the rounded dwLength
    /// values equals the Size value from the Certificates Table entry in the Optional
    /// Header Data Directory. If the sum of the rounded dwLength values does not equal
    /// the Size value, then either the attribute certificate table or the Size field
    /// is corrupted.
    /// 
    /// The first certificate starts at offset 0x5000 from the start of the file on disk.
    /// To advance through all the attribute certificate entries:
    /// 
    /// 1. Add the first attribute certificate's dwLength value to the starting offset.
    /// 2. Round the value from step 1 up to the nearest 8-byte multiple to find the offset
    ///    of the second attribute certificate entry.
    /// 3. Add the offset value from step 2 to the second attribute certificate entry's
    ///    dwLength value and round up to the nearest 8-byte multiple to determine the offset
    ///    of the third attribute certificate entry.
    /// 4. Repeat step 3 for each successive certificate until the calculated offset equals
    ///    0x6000 (0x5000 start + 0x1000 total size), which indicates that you've walked
    ///    the entire table.
    /// 
    /// Attribute certificate table entries can contain any certificate type, as long as
    /// the entry has the correct dwLength value, a unique wRevision value, and a unique
    /// wCertificateType value. The most common type of certificate table entry is a
    /// WIN_CERTIFICATE structure, which is documented in Wintrust.h and discussed in
    /// the remainder of this section.
    /// </summary>
    /// <see href="https://learn.microsoft.com/en-us/windows/win32/debug/pe-format"/>
    public class AttributeCertificateTableEntry
    {
        /// <summary>
        /// Specifies the length of the attribute certificate entry.
        /// </summary>
        public uint Length;

        /// <summary>
        /// Contains the certificate version number.
        /// </summary>
        public WindowsCertificateRevision Revision;

        /// <summary>
        /// Specifies the type of content in Certificate.
        /// </summary>
        public WindowsCertificateType CertificateType;

        /// <summary>
        /// Contains a certificate, such as an Authenticode signature.
        /// </summary>
        /// <see href="https://learn.microsoft.com/en-us/windows/win32/debug/pe-format#certificate-data"/>
        public byte[] Certificate;
    }
}
