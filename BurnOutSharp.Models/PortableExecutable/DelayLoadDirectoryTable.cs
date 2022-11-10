using System.Runtime.InteropServices;

namespace BurnOutSharp.Models.PortableExecutable
{
    /// <summary>
    /// The delay-load directory table is the counterpart to the import directory
    /// table. It can be retrieved through the Delay Import Descriptor entry in
    /// the optional header data directories list (offset 200).
    /// </summary>
    /// <see href="https://learn.microsoft.com/en-us/windows/win32/debug/pe-format"/>
    [StructLayout(LayoutKind.Sequential)]
    public class DelayLoadDirectoryTable
    {
        /// <summary>
        /// Must be zero.
        /// </summary>
        /// <remarks>
        /// As yet, no attribute flags are defined. The linker sets this field to
        /// zero in the image. This field can be used to extend the record by
        /// indicating the presence of new fields, or it can be used to indicate
        /// behaviors to the delay or unload helper functions.
        /// </remarks>
        public uint Attributes;

        /// <summary>
        /// The RVA of the name of the DLL to be loaded. The name resides in the
        /// read-only data section of the image.
        /// </summary>
        /// <remarks>
        /// The name of the DLL to be delay-loaded resides in the read-only data
        /// section of the image. It is referenced through the szName field.
        /// </remarks>
        public uint Name;

        /// <summary>
        /// The RVA of the module handle (in the data section of the image) of the DLL
        /// to be delay-loaded. It is used for storage by the routine that is supplied
        /// to manage delay-loading.
        /// </summary>
        /// <remarks>
        /// The handle of the DLL to be delay-loaded is in the data section of the image.
        /// The phmod field points to the handle. The supplied delay-load helper uses
        /// this location to store the handle to the loaded DLL.
        /// </remarks>
        public uint ModuleHandle;

        /// <summary>
        /// The RVA of the delay-load import address table.
        /// </summary>
        /// <remarks>
        /// The delay import address table (IAT) is referenced by the delay import
        /// descriptor through the pIAT field. The delay-load helper updates these
        /// pointers with the real entry points so that the thunks are no longer in
        /// the calling loop. The function pointers are accessed by using the expression
        /// pINT->u1.Function.
        /// </remarks>
        public uint DelayImportAddressTable;

        /// <summary>
        /// The RVA of the delay-load name table, which contains the names of the imports
        /// that might need to be loaded. This matches the layout of the import name table.
        /// </summary>
        /// <remarks>
        /// The delay import name table (INT) contains the names of the imports that might
        /// require loading. They are ordered in the same fashion as the function pointers
        /// in the IAT. They consist of the same structures as the standard INT and are
        /// accessed by using the expression pINT->u1.AddressOfData->Name[0].
        /// </remarks>
        public uint DelayImportNameTable;

        /// <summary>
        /// The RVA of the bound delay-load address table, if it exists.
        /// </summary>
        /// <remarks>
        /// The delay bound import address table (BIAT) is an optional table of
        /// IMAGE_THUNK_DATA items that is used along with the timestamp field of the
        /// delay-load directory table by a post-process binding phase.
        /// </remarks>
        public uint BoundDelayImportTable;

        /// <summary>
        /// The RVA of the unload delay-load address table, if it exists. This is an exact
        /// copy of the delay import address table. If the caller unloads the DLL, this
        /// table should be copied back over the delay import address table so that
        /// subsequent calls to the DLL continue to use the thunking mechanism correctly.
        /// </summary>
        /// <remarks>
        /// The delay unload import address table (UIAT) is an optional table of
        /// IMAGE_THUNK_DATA items that the unload code uses to handle an explicit unload
        /// request. It consists of initialized data in the read-only section that is an
        /// exact copy of the original IAT that referred the code to the delay-load thunks.
        /// On the unload request, the library can be freed, the *phmod cleared, and the
        /// UIAT written over the IAT to restore everything to its preload state.
        /// </remarks>
        public uint UnloadDelayImportTable;

        /// <summary>
        /// The timestamp of the DLL to which this image has been bound.
        /// </summary>
        /// <remarks>
        /// The delay bound import address table (BIAT) is an optional table of
        /// IMAGE_THUNK_DATA items that is used along with the timestamp field of the
        /// delay-load directory table by a post-process binding phase.
        /// </remarks>
        public uint TimeStamp;
    }
}
