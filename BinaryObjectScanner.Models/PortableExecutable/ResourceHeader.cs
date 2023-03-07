namespace BinaryObjectScanner.Models.PortableExecutable
{
    /// <summary>
    /// Contains information about the resource header itself and the data specific to
    /// this resource. This structure is not a true C-language structure, because it
    /// contains variable-length members. The structure definition provided here is for
    /// explanation only; it is not present in any standard header file.
    /// </summary>
    /// <see href="https://learn.microsoft.com/en-us/windows/win32/menurc/resourceheader"/>
    public sealed class ResourceHeader
    {
        /// <summary>
        /// The size, in bytes, of the data that follows the resource header for this
        /// particular resource. It does not include any file padding between this
        /// resource and any resource that follows it in the resource file.
        /// </summary>
        public uint DataSize;

        /// <summary>
        /// The size, in bytes, of the resource header data that follows.
        /// </summary>
        public uint HeaderSize;

        /// <summary>
        /// The resource type. The TYPE member can either be a numeric value or a
        /// null-terminated Unicode string that specifies the name of the type. See the
        /// following Remarks section for a description of Name or Ordinal type members.
        /// 
        /// If the TYPE member is a numeric value, it can specify either a standard or a
        /// user-defined resource type. If the member is a string, then it is a
        /// user-defined resource type.
        /// 
        /// Values less than 256 are reserved for system use.
        /// </summary>
        public ResourceType ResourceType;

        /// <summary>
        /// A name that identifies the particular resource. The NAME member, like the TYPE
        /// member, can either be a numeric value or a null-terminated Unicode string.
        /// See the following Remarks section for a description of Name or Ordinal type
        /// members.
        /// 
        /// You do not need to add padding for DWORD alignment between the TYPE and NAME
        /// members because they contain WORD data. However, you may need to add a WORD of
        /// padding after the NAME member to align the rest of the header on DWORD boundaries.
        /// </summary>
        public uint Name;

        /// <summary>
        /// A predefined resource data version. This will determine which version of the
        /// resource data the application should use.
        /// </summary>
        public uint DataVersion;

        /// <summary>
        /// A set of attribute flags that can describe the state of the resource. Modifiers
        /// in the .RC script file assign these attributes to the resource. The script
        /// identifiers can assign the following flag values.
        /// 
        /// Applications do not use any of these attributes. The attributes are permitted
        /// in the script for backward compatibility with existing scripts, but they are
        /// ignored. Resources are loaded when the corresponding module is loaded, and are
        /// freed when the module is unloaded.
        /// </summary>
        public MemoryFlags MemoryFlags;

        /// <summary>
        /// The language for the resource or set of resources. Set the value for this member
        /// with the optional LANGUAGE resource definition statement. The parameters are
        /// constants from the Winnt.h file.
        /// 
        /// Each resource includes a language identifier so the system or application can
        /// select a language appropriate for the current locale of the system. If there are
        /// multiple resources of the same type and name that differ only in the language of
        /// the strings within the resources, you will need to specify a LanguageId for each
        /// one.
        /// </summary>
        public ushort LanguageId;

        /// <summary>
        /// A user-defined version number for the resource data that tools can use to read and
        /// write resource files. Set this value with the optional VERSION resource definition
        /// statement.
        /// </summary>
        public uint Version;

        /// <summary>
        /// Specifies user-defined information about the resource that tools can use to read and
        /// write resource files. Set this value with the optional CHARACTERISTICS resource
        /// definition statement.
        /// </summary>
        public uint Characteristics;
    }
}
