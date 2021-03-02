using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.InteropServices;
using FileAttributes = System.IO.FileAttributes;

namespace LessIO.Strategies.Win32
{
    /// <summary>
    /// Native/Win32 methods for accessing file system. Primarily to get around <see cref="System.IO.PathTooLongException"/>.
    /// Good references:
    /// * https://blogs.msdn.microsoft.com/bclteam/2007/03/26/long-paths-in-net-part-2-of-3-long-path-workarounds-kim-hamilton/
    /// </summary>
    internal class NativeMethods
    {
        /// <summary>
        /// Specified in Windows Headers for default maximum path. To go beyond this length you must prepend <see cref="LongPathPrefix"/> to the path.
        /// </summary>
        internal const int MAX_PATH = 260;
        /// <summary>
        /// This is the special prefix to prepend to paths to support up to 32,767 character paths.
        /// See https://msdn.microsoft.com/en-us/library/windows/desktop/aa365247(v=vs.85).aspx#maxpath
        /// </summary>
        internal static readonly string LongPathPrefix = @"\\?\";

        /// <summary>
        /// This is the special prefix to prepend to UNC paths to support up to 32,767 character paths.
        /// See https://msdn.microsoft.com/en-us/library/windows/desktop/aa365247(v=vs.85).aspx#maxpath
        /// </summary>
        internal static readonly string LongPathPrefixUNC = @"\\?\UNC\";

        internal static IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);

        [StructLayout(LayoutKind.Sequential)]
        internal class SECURITY_ATTRIBUTES
        {
            //internal unsafe byte* pSecurityDescriptor = (byte*)null;
            internal IntPtr pSecurityDescriptor;
            internal int nLength;
            internal int bInheritHandle;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct FILETIME
        {
            internal uint dwLowDateTime;
            internal uint dwHighDateTime;
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct WIN32_FIND_DATA
        {
            internal EFileAttributes dwFileAttributes;
            internal FILETIME ftCreationTime;
            internal FILETIME ftLastAccessTime;
            internal FILETIME ftLastWriteTime;
            internal int nFileSizeHigh;
            internal int nFileSizeLow;
            internal int dwReserved0;
            internal int dwReserved1;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)]
            internal string cFileName;
            // not using this
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
            internal string cAlternate;
        }

        [Flags]
        internal enum EFileAccess : uint
        {
            FILE_READ_ATTRIBUTES = 0x00000080,
            FILE_WRITE_ATTRIBUTES = 0x00000100,
            GenericRead = 0x80000000,
            GenericWrite = 0x40000000,
            GenericExecute = 0x20000000,
            GenericAll = 0x10000000
        }

        [Flags]
        internal enum EFileShare : uint
        {
            None = 0x00000000,
            Read = 0x00000001,
            Write = 0x00000002,
            Delete = 0x00000004
        }

        internal enum ECreationDisposition : uint
        {
            New = 1,
            CreateAlways = 2,
            OpenExisting = 3,
            OpenAlways = 4,
            TruncateExisting = 5
        }

        [Flags]
        internal enum EFileAttributes : uint
        {
            None = 0x0,
            Readonly = 0x00000001,
            Hidden = 0x00000002,
            System = 0x00000004,
            Directory = 0x00000010,
            Archive = 0x00000020,
            Device = 0x00000040,
            Normal = 0x00000080,
            Temporary = 0x00000100,
            SparseFile = 0x00000200,
            ReparsePoint = 0x00000400,
            Compressed = 0x00000800,
            Offline = 0x00001000,
            NotContentIndexed = 0x00002000,
            Encrypted = 0x00004000,
            Write_Through = 0x80000000,
            Overlapped = 0x40000000,
            NoBuffering = 0x20000000,
            RandomAccess = 0x10000000,
            SequentialScan = 0x08000000,
            DeleteOnClose = 0x04000000,
            BackupSemantics = 0x02000000,
            PosixSemantics = 0x01000000,
            OpenReparsePoint = 0x00200000,
            OpenNoRecall = 0x00100000,
            FirstPipeInstance = 0x00080000
        }

        /// <summary>
        /// https://msdn.microsoft.com/en-us/library/windows/desktop/aa363855%28v=vs.85%29.aspx
        /// </summary>
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true, BestFitMapping = false)]
        internal static extern bool CreateDirectory(string path, SECURITY_ATTRIBUTES lpSecurityAttributes);


        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        internal static extern IntPtr FindFirstFile(string lpFileName, out
                                WIN32_FIND_DATA lpFindFileData);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        internal static extern bool FindNextFile(IntPtr hFindFile, out
                                        WIN32_FIND_DATA lpFindFileData);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool FindClose(IntPtr hFindFile);

        


        /// <summary>
        /// https://msdn.microsoft.com/en-us/library/windows/desktop/aa365535%28v=vs.85%29.aspx
        /// </summary>
        [DllImport("kernel32.dll", EntryPoint = "SetFileAttributes", CharSet = CharSet.Auto, SetLastError = true, BestFitMapping = false)]
        internal static extern bool SetFileAttributes(string lpFileName, uint dwFileAttributes);

        /// <summary>
        /// https://msdn.microsoft.com/en-us/library/windows/desktop/aa364944%28v=vs.85%29.aspx
        /// </summary>
        /// <param name="lpFileName"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern uint GetFileAttributes(string lpFileName);

        // Invalid is from C:\Program Files (x86)\Windows Kits\8.1\Include\um\fileapi.h
        //#define INVALID_FILE_ATTRIBUTES ((DWORD)-1)
        internal static readonly uint INVALID_FILE_ATTRIBUTES = 0xffffffff;


        [DllImport("kernel32.dll", EntryPoint = "RemoveDirectory", CharSet = CharSet.Auto, SetLastError = true, BestFitMapping = false)]
        internal static extern bool RemoveDirectory(string lpPathName);

        [DllImport("kernel32.dll", EntryPoint = "DeleteFile", CharSet = CharSet.Auto, SetLastError = true, BestFitMapping = false)]
        internal static extern bool DeleteFile(string path);

        /// <summary>
        /// See https://msdn.microsoft.com/en-us/library/windows/desktop/aa363858%28v=vs.85%29.aspx
        /// </summary>
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern SafeFileHandle CreateFile(string lpFileName,
            EFileAccess dwDesiredAccess,
            EFileShare dwShareMode,
            IntPtr lpSecurityAttributes,
            ECreationDisposition dwCreationDisposition,
            EFileAttributes dwFlagsAndAttributes,
            IntPtr hTemplateFile);

        // This binding only allows setting creation and last write times.
        // The last access time parameter must be zero; that time is not
        // modified.
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool SetFileTime(
            IntPtr hFile,
            ref long lpCreationTime,
            IntPtr lpLastAccessTime,
            ref long lpLastWriteTime);

        /// <summary>
        /// https://msdn.microsoft.com/en-us/library/windows/desktop/aa363851%28v=vs.85%29.aspx
        /// </summary>
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        internal static extern bool CopyFile(string lpExistingFileName, string lpNewFileName, bool bFailIfExists);


        internal const uint FORMAT_MESSAGE_FROM_SYSTEM = 0x00001000;

        /// <summary>
        /// https://msdn.microsoft.com/en-us/library/windows/desktop/ms679351%28v=vs.85%29.aspx
        /// </summary>
        [DllImport("kernel32.dll")]
        internal static extern uint FormatMessage(uint dwFlags, IntPtr lpSource, uint dwMessageId, uint dwLanguageId, [Out] System.Text.StringBuilder lpBuffer, uint nSize, IntPtr Arguments);
    }
}
