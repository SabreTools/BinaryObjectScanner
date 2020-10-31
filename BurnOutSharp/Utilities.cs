using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace BurnOutSharp
{
    internal static class Utilities
    {
        /// <summary>
        /// Search for a byte array in another array
        /// </summary>
        public static bool Contains(this byte[] stack, byte[] needle, out int position, int start = 0, int end = -1)
        {
            // Initialize the found position to -1
            position = -1;

            // If either array is null or empty, we can't do anything
            if (stack == null || stack.Length == 0 || needle == null || needle.Length == 0)
                return false;

            // If the needle array is larger than the stack array, it can't be contained within
            if (needle.Length > stack.Length)
                return false;

            // If start or end are not set properly, set them to defaults
            if (start < 0)
                start = 0;
            if (end < 0)
                end = stack.Length - needle.Length;

            for (int i = start; i < end; i++)
            {
                if (stack.EqualAt(needle, i))
                {
                    position = i;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// See if a byte array starts with another
        /// </summary>
        public static bool StartsWith(this byte[] stack, byte[] needle)
        {
            return stack.Contains(needle, out int _, start: 0, end: 1);
        }

        /// <summary>
        /// Get if a stack at a certain index is equal to a needle
        /// </summary>
        private static bool EqualAt(this byte[] stack, byte[] needle, int index)
        {
            // If we're too close to the end of the stack, return false
            if (needle.Length >= stack.Length - index)
                return false;

            for (int i = 0; i < needle.Length; i++)
            {
                if (stack[i + index] != needle[i])
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Get the file version as reported by the filesystem
        /// </summary>
        public static string GetFileVersion(string file)
        {
            if (file == null || !File.Exists(file))
                return string.Empty;

            FileVersionInfo fvinfo = FileVersionInfo.GetVersionInfo(file);
            if (fvinfo.FileVersion == null)
                return "";
            if (fvinfo.FileVersion != "")
                return fvinfo.FileVersion.Replace(", ", ".");
            else
                return fvinfo.ProductVersion.Replace(", ", ".");
        }

        /// <summary>
        /// Get the filesystem name for the given drive letter
        /// </summary>
        /// <remarks>
        /// http://pinvoke.net/default.aspx/kernel32/GetVolumeInformation.html
        /// </remarks>
        public static string GetFileSystemName(char driveLetter)
        {
            string fsName = null;

            StringBuilder volname = new StringBuilder(261);
            StringBuilder fsname = new StringBuilder(261);

            if (GetVolumeInformation($"{driveLetter}:\\", volname, volname.Capacity, out uint sernum, out uint maxlen, out FileSystemFeature flags, fsname, fsname.Capacity))
            {
                // Now you know the file system of your drive
                // NTFS or FAT16 or UDF for instance
                fsName = fsname.ToString();
            }

            return fsName;
        }

        /// <summary>
        /// Append one result to a results dictionary
        /// </summary>
        /// <param name="original">Dictionary to append to</param>
        /// <param name="key">Key to add information to</param>
        /// <param name="value">String value to add</param>
        public static void AppendToDictionary(Dictionary<string, List<string>> original, string key, string value)
        {
            AppendToDictionary(original, key, new List<string> { value });
        }

        /// <summary>
        /// Append one result to a results dictionary
        /// </summary>
        /// <param name="original">Dictionary to append to</param>
        /// <param name="key">Key to add information to</param>
        /// <param name="value">String value to add</param>
        public static void AppendToDictionary(Dictionary<string, List<string>> original, string key, List<string> values)
        {
            // If the dictionary is null, just return
            if (original == null)
                return;

            // Use a placeholder value if the key is null
            key = key ?? "NO FILENAME";

            // Add the key if needed and then append the lists
            if (!original.ContainsKey(key))
                original[key] = new List<string>();

            original[key].AddRange(values);
        }

        /// <summary>
        /// Append one results dictionary to another
        /// </summary>
        /// <param name="original">Dictionary to append to</param>
        /// <param name="addition">Dictionary to pull from</param>
        public static void AppendToDictionary(Dictionary<string, List<string>> original, Dictionary<string, List<string>> addition)
        {
            // If either dictionary is missing, just return
            if (original == null || addition == null)
                return;

            // Loop through each of the addition keys and add accordingly
            foreach (string key in addition.Keys)
            {
                if (!original.ContainsKey(key))
                    original[key] = new List<string>();

                original[key].AddRange(addition[key]);
            }
        }

        #region P/Invoke

        // https://stackoverflow.com/questions/8819188/c-sharp-classes-to-undelete-files/8820157#8820157

        // Move Method
        public const uint FileBegin = 0;
        public const uint FileCurrent = 1;
        public const uint FileEnd = 2;

        // Handle Constants
        public const uint INVALID_HANDLE_VALUE = 0;
        public const int IOCTL_STORAGE_GET_DEVICE_NUMBER = 0x2D1080;

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool CloseHandle(IntPtr handle);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr CreateFile(
             [MarshalAs(UnmanagedType.LPTStr)] string filename,
             [MarshalAs(UnmanagedType.U4)] FileAccess access,
             [MarshalAs(UnmanagedType.U4)] FileShare share,
             IntPtr securityAttributes, // optional SECURITY_ATTRIBUTES struct or IntPtr.Zero
             [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
             [MarshalAs(UnmanagedType.U4)] FileAttributes flagsAndAttributes,
             IntPtr templateFile);

        [DllImport("Kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public extern static bool GetVolumeInformation(
            string rootPathName,
            StringBuilder volumeNameBuffer,
            int volumeNameSize,
            out uint volumeSerialNumber,
            out uint maximumComponentLength,
            out FileSystemFeature fileSystemFlags,
            StringBuilder fileSystemNameBuffer,
            int nFileSystemNameSize);

        [DllImport("Kernel32.dll", SetLastError = true)]
        public extern static bool DeviceIoControl(
            IntPtr hDevice,
            uint IoControlCode,
            IntPtr InMediaRemoval,
            uint InBufferSize,
            IntPtr OutBuffer,
            int OutBufferSize,
            out int BytesReturned,
            IntPtr Overlapped);

        // Used to read in a file
        [DllImport("kernel32.dll")]
        public static extern bool ReadFile(
            IntPtr hFile,
            byte[] lpBuffer,
            uint nNumberOfBytesToRead,
            ref uint lpNumberOfBytesRead,
            IntPtr lpOverlapped);

        [DllImport("kernel32.dll")]
        public static extern bool ReadFileEx(
            IntPtr hFile,
            [Out] byte[] lpBuffer,
            uint nNumberOfBytesToRead,
            [In] ref NativeOverlapped lpOverlapped,
            IOCompletionCallback lpCompletionRoutine);

        // Used to set the offset in file to start reading
        [DllImport("kernel32.dll")]
        public static extern bool SetFilePointerEx(
            IntPtr hFile,
            long liDistanceToMove,
            ref IntPtr lpNewFilePointer,
            uint dwMoveMethod);

        [StructLayout(LayoutKind.Sequential)]
        struct STORAGE_DEVICE_NUMBER
        {
            public int DeviceType;
            public int DeviceNumber;
            public int PartitionNumber;
        }

        public enum MEDIA_TYPE : uint
        {
            Unknown,
            F5_1Pt2_512,
            F3_1Pt44_512,
            F3_2Pt88_512,
            F3_20Pt8_512,
            F3_720_512,
            F5_360_512,
            F5_320_512,
            F5_320_1024,
            F5_180_512,
            F5_160_512,
            RemovableMedia,
            FixedMedia,
            F3_120M_512,
            F3_640_512,
            F5_640_512,
            F5_720_512,
            F3_1Pt2_512,
            F3_1Pt23_1024,
            F5_1Pt23_1024,
            F3_128Mb_512,
            F3_230Mb_512,
            F8_256_128,
            F3_200Mb_512,
            F3_240M_512,
            F3_32M_512
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DISK_GEOMETRY
        {
            public long Cylinders;
            public MEDIA_TYPE MediaType;
            public int TracksPerCylinder;
            public int SectorsPerTrack;
            public int BytesPerSector;

            public long DiskSize
            {
                get
                {
                    return Cylinders * (long)TracksPerCylinder * (long)SectorsPerTrack * (long)BytesPerSector;
                }
            }
        }

        [Flags]
        public enum FileSystemFeature : uint
        {
            /// <summary>
            /// The file system preserves the case of file names when it places a name on disk.
            /// </summary>
            CasePreservedNames = 2,

            /// <summary>
            /// The file system supports case-sensitive file names.
            /// </summary>
            CaseSensitiveSearch = 1,

            /// <summary>
            /// The specified volume is a direct access (DAX) volume. This flag was introduced in Windows 10, version 1607.
            /// </summary>
            DaxVolume = 0x20000000,

            /// <summary>
            /// The file system supports file-based compression.
            /// </summary>
            FileCompression = 0x10,

            /// <summary>
            /// The file system supports named streams.
            /// </summary>
            NamedStreams = 0x40000,

            /// <summary>
            /// The file system preserves and enforces access control lists (ACL).
            /// </summary>
            PersistentACLS = 8,

            /// <summary>
            /// The specified volume is read-only.
            /// </summary>
            ReadOnlyVolume = 0x80000,

            /// <summary>
            /// The volume supports a single sequential write.
            /// </summary>
            SequentialWriteOnce = 0x100000,

            /// <summary>
            /// The file system supports the Encrypted File System (EFS).
            /// </summary>
            SupportsEncryption = 0x20000,

            /// <summary>
            /// The specified volume supports extended attributes. An extended attribute is a piece of
            /// application-specific metadata that an application can associate with a file and is not part
            /// of the file's data.
            /// </summary>
            SupportsExtendedAttributes = 0x00800000,

            /// <summary>
            /// The specified volume supports hard links. For more information, see Hard Links and Junctions.
            /// </summary>
            SupportsHardLinks = 0x00400000,

            /// <summary>
            /// The file system supports object identifiers.
            /// </summary>
            SupportsObjectIDs = 0x10000,

            /// <summary>
            /// The file system supports open by FileID. For more information, see FILE_ID_BOTH_DIR_INFO.
            /// </summary>
            SupportsOpenByFileId = 0x01000000,

            /// <summary>
            /// The file system supports re-parse points.
            /// </summary>
            SupportsReparsePoints = 0x80,

            /// <summary>
            /// The file system supports sparse files.
            /// </summary>
            SupportsSparseFiles = 0x40,

            /// <summary>
            /// The volume supports transactions.
            /// </summary>
            SupportsTransactions = 0x200000,

            /// <summary>
            /// The specified volume supports update sequence number (USN) journals. For more information,
            /// see Change Journal Records.
            /// </summary>
            SupportsUsnJournal = 0x02000000,

            /// <summary>
            /// The file system supports Unicode in file names as they appear on disk.
            /// </summary>
            UnicodeOnDisk = 4,

            /// <summary>
            /// The specified volume is a compressed volume, for example, a DoubleSpace volume.
            /// </summary>
            VolumeIsCompressed = 0x8000,

            /// <summary>
            /// The file system supports disk quotas.
            /// </summary>
            VolumeQuotas = 0x20
        }

        #endregion
    }
}
