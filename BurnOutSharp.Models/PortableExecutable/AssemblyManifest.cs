using System.Xml.Serialization;

namespace BurnOutSharp.Models.PortableExecutable
{
    /// <see href="https://learn.microsoft.com/en-us/windows/win32/sbscs/manifest-file-schema"/>
    [XmlRoot(ElementName = "assembly", Namespace = "urn:schemas-microsoft-com:asm.v1")]
    public sealed class AssemblyManifest
    {
        [XmlAttribute("manifestVersion")]
        public string ManifestVersion;

        #region Group

        [XmlElement("assemblyIdentity")]
        public AssemblyIdentity[] AssemblyIdentities;

        [XmlElement("noInheritable")]
        public AssemblyNoInheritable[] NoInheritables;

        #endregion

        #region Group

        [XmlElement("description")]
        public AssemblyDescription Description;

        [XmlElement("noInherit")]
        public AssemblyNoInherit NoInherit;

        //[XmlElement("noInheritable")]
        //public AssemblyNoInheritable NoInheritable;

        [XmlElement("comInterfaceExternalProxyStub")]
        public AssemblyCOMInterfaceExternalProxyStub[] COMInterfaceExternalProxyStub;

        [XmlElement("dependency")]
        public AssemblyDependency[] Dependency;

        [XmlElement("file")]
        public AssemblyFile[] File;

        [XmlElement("clrClass")]
        public AssemblyCommonLanguageRuntimeClass[] CLRClass;

        [XmlElement("clrSurrogate")]
        public AssemblyCommonLanguageSurrogateClass[] CLRSurrogate;

        #endregion

        [XmlAnyElement]
        public object[] EverythingElse;
    }

    /// <see href="https://learn.microsoft.com/en-us/windows/win32/sbscs/manifest-file-schema"/>
    public sealed class AssemblyActiveCodePage
    {
        [XmlText]
        public string Value;
    }

    /// <see href="https://learn.microsoft.com/en-us/windows/win32/sbscs/manifest-file-schema"/>
    public sealed class AssemblyAutoElevate
    {
        [XmlText]
        public string Value;
    }

    /// <see href="https://learn.microsoft.com/en-us/windows/win32/sbscs/manifest-file-schema"/>
    public sealed class AssemblyBindingRedirect
    {
        [XmlAttribute("oldVersion")]
        public string OldVersion;

        [XmlAttribute("newVersion")]
        public string NewVersion;
    }

    /// <see href="https://learn.microsoft.com/en-us/windows/win32/sbscs/manifest-file-schema"/>
    public sealed class AssemblyCOMClass
    {
        [XmlAttribute("clsid")]
        public string CLSID;

        [XmlAttribute("threadingModel")]
        public string ThreadingModel;

        [XmlAttribute("progid")]
        public string ProgID;

        [XmlAttribute("tlbid")]
        public string TLBID;

        [XmlAttribute("description")]
        public string Description;

        [XmlElement("progid")]
        public AssemblyProgID[] ProgIDs;
    }

    /// <see href="https://learn.microsoft.com/en-us/windows/win32/sbscs/manifest-file-schema"/>
    public sealed class AssemblyCOMInterfaceExternalProxyStub
    {
        [XmlAttribute("iid")]
        public string IID;

        [XmlAttribute("name")]
        public string Name;

        [XmlAttribute("tlbid")]
        public string TLBID;

        [XmlAttribute("numMethods")]
        public string NumMethods;

        [XmlAttribute("proxyStubClsid32")]
        public string ProxyStubClsid32;

        [XmlAttribute("baseInterface")]
        public string BaseInterface;
    }

    /// <see href="https://learn.microsoft.com/en-us/windows/win32/sbscs/manifest-file-schema"/>
    public sealed class AssemblyCOMInterfaceProxyStub
    {
        [XmlAttribute("iid")]
        public string IID;

        [XmlAttribute("name")]
        public string Name;

        [XmlAttribute("tlbid")]
        public string TLBID;

        [XmlAttribute("numMethods")]
        public string NumMethods;

        [XmlAttribute("proxyStubClsid32")]
        public string ProxyStubClsid32;

        [XmlAttribute("baseInterface")]
        public string BaseInterface;
    }

    /// <see href="https://learn.microsoft.com/en-us/windows/win32/sbscs/manifest-file-schema"/>
    public sealed class AssemblyCommonLanguageRuntimeClass
    {
        [XmlAttribute("name")]
        public string Name;

        [XmlAttribute("clsid")]
        public string CLSID;

        [XmlAttribute("progid")]
        public string ProgID;

        [XmlAttribute("tlbid")]
        public string TLBID;

        [XmlAttribute("description")]
        public string Description;

        [XmlAttribute("runtimeVersion")]
        public string RuntimeVersion;

        [XmlAttribute("threadingModel")]
        public string ThreadingModel;

        [XmlElement("progid")]
        public AssemblyProgID[] ProgIDs;
    }

    /// <see href="https://learn.microsoft.com/en-us/windows/win32/sbscs/manifest-file-schema"/>
    public sealed class AssemblyCommonLanguageSurrogateClass
    {
        [XmlAttribute("clsid")]
        public string CLSID;

        [XmlAttribute("name")]
        public string Name;

        [XmlAttribute("runtimeVersion")]
        public string RuntimeVersion;
    }

    /// <see href="https://learn.microsoft.com/en-us/windows/win32/sbscs/manifest-file-schema"/>
    public sealed class AssemblyDependency
    {
        [XmlElement("dependentAssembly")]
        public AssemblyDependentAssembly DependentAssembly;

        [XmlAttribute("optional")]
        public string Optional;
    }

    /// <see href="https://learn.microsoft.com/en-us/windows/win32/sbscs/manifest-file-schema"/>
    public sealed class AssemblyDependentAssembly
    {
        [XmlElement("assemblyIdentity")]
        public AssemblyIdentity AssemblyIdentity;

        [XmlElement("bindingRedirect")]
        public AssemblyBindingRedirect[] BindingRedirect;
    }

    /// <see href="https://learn.microsoft.com/en-us/windows/win32/sbscs/manifest-file-schema"/>
    public sealed class AssemblyDescription
    {
        [XmlText]
        public string Value;
    }

    /// <see href="https://learn.microsoft.com/en-us/windows/win32/sbscs/manifest-file-schema"/>
    public sealed class AssemblyDisableTheming
    {
        [XmlText]
        public string Value;
    }

    /// <see href="https://learn.microsoft.com/en-us/windows/win32/sbscs/manifest-file-schema"/>
    public sealed class AssemblyDisableWindowFiltering
    {
        [XmlText]
        public string Value;
    }

    /// <see href="https://learn.microsoft.com/en-us/windows/win32/sbscs/manifest-file-schema"/>
    public sealed class AssemblyDPIAware
    {
        [XmlText]
        public string Value;
    }

    /// <see href="https://learn.microsoft.com/en-us/windows/win32/sbscs/manifest-file-schema"/>
    public sealed class AssemblyDPIAwareness
    {
        [XmlText]
        public string Value;
    }

    /// <see href="https://learn.microsoft.com/en-us/windows/win32/sbscs/manifest-file-schema"/>
    public sealed class AssemblyFile
    {
        [XmlAttribute("name")]
        public string Name;

        [XmlAttribute("hash")]
        public string Hash;

        [XmlAttribute("hashalg")]
        public string HashAlgorithm;

        [XmlAttribute("size")]
        public string Size;

        #region Group

        [XmlElement("comClass")]
        public AssemblyCOMClass[] COMClass;

        [XmlElement("comInterfaceProxyStub")]
        public AssemblyCOMInterfaceProxyStub[] COMInterfaceProxyStub;

        [XmlElement("typelib")]
        public AssemblyTypeLib[] Typelib;

        [XmlElement("windowClass")]
        public AssemblyWindowClass[] WindowClass;

        #endregion
    }

    /// <see href="https://learn.microsoft.com/en-us/windows/win32/sbscs/manifest-file-schema"/>
    public sealed class AssemblyGDIScaling
    {
        [XmlText]
        public string Value;
    }

    /// <see href="https://learn.microsoft.com/en-us/windows/win32/sbscs/manifest-file-schema"/>
    public sealed class AssemblyHeapType
    {
        [XmlText]
        public string Value;
    }

    /// <see href="https://learn.microsoft.com/en-us/windows/win32/sbscs/manifest-file-schema"/>
    public sealed class AssemblyHighResolutionScrollingAware
    {
        [XmlText]
        public string Value;
    }

    /// <see href="https://learn.microsoft.com/en-us/windows/win32/sbscs/manifest-file-schema"/>
    public sealed class AssemblyIdentity
    {
        [XmlAttribute("name")]
        public string Name;

        [XmlAttribute("version")]
        public string Version;

        [XmlAttribute("type")]
        public string Type;

        [XmlAttribute("processorArchitecture")]
        public string ProcessorArchitecture;

        [XmlAttribute("publicKeyToken")]
        public string PublicKeyToken;

        [XmlAttribute("language")]
        public string Language;
    }

    /// <see href="https://learn.microsoft.com/en-us/windows/win32/sbscs/manifest-file-schema"/>
    public sealed class AssemblyLongPathAware
    {
        [XmlText]
        public string Value;
    }

    /// <see href="https://learn.microsoft.com/en-us/windows/win32/sbscs/manifest-file-schema"/>
    public sealed class AssemblyNoInherit
    {
    }

    /// <see href="https://learn.microsoft.com/en-us/windows/win32/sbscs/manifest-file-schema"/>
    public sealed class AssemblyNoInheritable
    {
    }

    /// <see href="https://learn.microsoft.com/en-us/windows/win32/sbscs/manifest-file-schema"/>
    public sealed class AssemblyPrinterDriverIsolation
    {
        [XmlText]
        public string Value;
    }

    /// <see href="https://learn.microsoft.com/en-us/windows/win32/sbscs/manifest-file-schema"/>
    public sealed class AssemblyProgID
    {
        [XmlText]
        public string Value;
    }

    /// <see href="https://learn.microsoft.com/en-us/windows/win32/sbscs/manifest-file-schema"/>
    public sealed class AssemblySupportedOS
    {
        [XmlAttribute("Id")]
        public string Id;
    }

    /// <see href="https://learn.microsoft.com/en-us/windows/win32/sbscs/manifest-file-schema"/>
    public sealed class AssemblyTypeLib
    {
        [XmlElement("tlbid")]
        public string TLBID;

        [XmlElement("version")]
        public string Version;

        [XmlElement("helpdir")]
        public string HelpDir;

        [XmlElement("resourceid")]
        public string ResourceID;

        [XmlElement("flags")]
        public string Flags;
    }

    /// <see href="https://learn.microsoft.com/en-us/windows/win32/sbscs/manifest-file-schema"/>
    public sealed class AssemblyUltraHighResolutionScrollingAware
    {
        [XmlText]
        public string Value;
    }

    /// <see href="https://learn.microsoft.com/en-us/windows/win32/sbscs/manifest-file-schema"/>
    public sealed class AssemblyWindowClass
    {
        [XmlAttribute("versioned")]
        public string Versioned;

        [XmlText]
        public string Value;
    }

    // TODO: Left off at <ElementType name="progid" />
}
