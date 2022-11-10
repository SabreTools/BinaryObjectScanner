using System.Xml.Serialization;

namespace BurnOutSharp.Models.PortableExecutable
{
    /// <see href="https://learn.microsoft.com/en-us/windows/win32/sbscs/manifest-file-schema"/>
    [XmlRoot(ElementName = "assembly", Namespace = "urn:schemas-microsoft-com:asm.v1")]
    public class AssemblyManifest
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
    public class AssemblyBindingRedirect
    {
        [XmlAttribute("oldVersion")]
        public string OldVersion;

        [XmlAttribute("newVersion")]
        public string NewVersion;
    }

    /// <see href="https://learn.microsoft.com/en-us/windows/win32/sbscs/manifest-file-schema"/>
    public class AssemblyCOMClass
    {
        [XmlAttribute("clsid")]
        public string CLSID;

        [XmlAttribute("threadingModel")]
        public string ThreadingModel;

        [XmlAttribute("progid")]
        public string Progid;

        [XmlAttribute("tlbid")]
        public string TLBID;

        [XmlAttribute("description")]
        public string Description;

        [XmlElement("progid")]
        public AssemblyProgID[] ProgIDs;
    }

    /// <see href="https://learn.microsoft.com/en-us/windows/win32/sbscs/manifest-file-schema"/>
    public class AssemblyCOMInterfaceExternalProxyStub
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
    public class AssemblyCOMInterfaceProxyStub
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
    public class AssemblyCommonLanguageRuntimeClass
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
    public class AssemblyCommonLanguageSurrogateClass
    {
        [XmlAttribute("clsid")]
        public string CLSID;

        [XmlAttribute("name")]
        public string Name;

        [XmlAttribute("runtimeVersion")]
        public string RuntimeVersion;
    }

    /// <see href="https://learn.microsoft.com/en-us/windows/win32/sbscs/manifest-file-schema"/>
    public class AssemblyDependency
    {
        [XmlElement("dependentAssembly")]
        public AssemblyDependentAssembly DependentAssembly;

        [XmlAttribute("optional")]
        public string Optional;
    }

    /// <see href="https://learn.microsoft.com/en-us/windows/win32/sbscs/manifest-file-schema"/>
    public class AssemblyDependentAssembly
    {
        [XmlElement("assemblyIdentity")]
        public AssemblyIdentity AssemblyIdentity;

        [XmlElement("bindingRedirect")]
        public AssemblyBindingRedirect[] BindingRedirect;
    }

    /// <see href="https://learn.microsoft.com/en-us/windows/win32/sbscs/manifest-file-schema"/>
    public class AssemblyDescription
    {
        [XmlText]
        public string Value;
    }

    /// <see href="https://learn.microsoft.com/en-us/windows/win32/sbscs/manifest-file-schema"/>
    public class AssemblyFile
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
    public class AssemblyIdentity
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
    public class AssemblyNoInherit
    {
    }

    /// <see href="https://learn.microsoft.com/en-us/windows/win32/sbscs/manifest-file-schema"/>
    public class AssemblyNoInheritable
    {
    }

    /// <see href="https://learn.microsoft.com/en-us/windows/win32/sbscs/manifest-file-schema"/>
    public class AssemblyProgID
    {
        [XmlText]
        public string Value;
    }

    /// <see href="https://learn.microsoft.com/en-us/windows/win32/sbscs/manifest-file-schema"/>
    public class AssemblySupportedOS
    {
        [XmlAttribute("Id")]
        public string Id;
    }

    /// <see href="https://learn.microsoft.com/en-us/windows/win32/sbscs/manifest-file-schema"/>
    public class AssemblyTypeLib
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
    public class AssemblyWindowClass
    {
        [XmlText]
        public string Value;

        [XmlAttribute("versioned")]
        public string Versioned;
    }

    // TODO: Left off at <ElementType name="progid" />
}
