using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using BadPath = System.IO.Path;

namespace LessIO.Strategies.Win32
{
    internal sealed class Win32FileSystemStrategy : FileSystemStrategy
    {
        public Win32FileSystemStrategy()
        {
        }

        private static Exception CreateWin32LastErrorException(string userMessage, params object[] args)
        {
            uint lastError = (uint)Marshal.GetLastWin32Error();
            
            const int bufferByteCapacity = 1024 * 32;
            StringBuilder buffer = new StringBuilder(bufferByteCapacity);// NOTE: capacity here will be interpreted by StringBuilder as chars not bytes, but that's ok since it is definitely bigger in byte terms.
            uint bufferCharLength = NativeMethods.FormatMessage(NativeMethods.FORMAT_MESSAGE_FROM_SYSTEM, IntPtr.Zero, lastError, 0, buffer, bufferByteCapacity, IntPtr.Zero);

            string systemErrorMessage;
            if (bufferCharLength == 0)
            {   //FormatMessage failed:
                systemErrorMessage = string.Format("Error code=0x{1:x8}", lastError);
            }
            else
            {
                Debug.Assert(bufferCharLength < buffer.Capacity, "unexpected result capacity");
                char[] systemChars = new char[bufferCharLength];
                buffer.CopyTo(0, systemChars, 0, (int)bufferCharLength);
                systemErrorMessage = new string(systemChars);
            }

            var formattedUserMessage = string.Format(userMessage, args);
            return new Exception(formattedUserMessage + " System error information:'" + systemErrorMessage + "'");
        }

        public override void SetLastWriteTime(Path path, DateTime lastWriteTime)
        {
            SafeFileHandle h = NativeMethods.CreateFile(
                path.WithWin32LongPathPrefix(),
                NativeMethods.EFileAccess.FILE_READ_ATTRIBUTES | NativeMethods.EFileAccess.FILE_WRITE_ATTRIBUTES,
                NativeMethods.EFileShare.Read | NativeMethods.EFileShare.Write| NativeMethods.EFileShare.Delete,
                IntPtr.Zero,// See https://github.com/activescott/LessIO/issues/4 before changing these flags.
                NativeMethods.ECreationDisposition.OpenExisting,
                NativeMethods.EFileAttributes.None,
                IntPtr.Zero);
            using (h)
            {
                if (h.IsInvalid)
                    throw CreateWin32LastErrorException("Error opening file {0}.", path);
                var modifiedTime = lastWriteTime.ToFileTime();
                if (!NativeMethods.SetFileTime(
                        h.DangerousGetHandle(),
                        ref modifiedTime, IntPtr.Zero, ref modifiedTime))
                    throw CreateWin32LastErrorException("Error setting times for {0}.", path);
            }
        }

        public override void SetAttributes(Path path, FileAttributes fileAttributes)
        {
            var result = NativeMethods.SetFileAttributes(path.WithWin32LongPathPrefix(), (uint)fileAttributes);
            if (!result)
            {
                throw CreateWin32LastErrorException("Error setting file attributes on '{0}'.", path);
            }
        }

        public override FileAttributes GetAttributes(Path path)
        {
            var result = NativeMethods.GetFileAttributes(path.WithWin32LongPathPrefix());
            if (result == NativeMethods.INVALID_FILE_ATTRIBUTES)
            {
                throw CreateWin32LastErrorException("Error getting file attributes for '{0}'.", path);
            }
            return (FileAttributes)result;
        }

        public override void CreateDirectory(Path path)
        {
            // Since System.IO.Directory.Create() creates all neecessary directories, we emulate here:
            string pathString = path.PathString;
            var dirsToCreate = new List<String>();
            int lengthRoot = path.PathRoot.Length;

            var firstNonRootPathIndex = pathString.IndexOfAny(Path.DirectorySeperatorChars, lengthRoot);
            if (firstNonRootPathIndex == -1)
            {
                // this is a directory directly off of the root (because no, non-root directory seperators exist)
                firstNonRootPathIndex = pathString.Length - 1; // set it to the whole path so that the loop below will create the root dir
            }
            var i = firstNonRootPathIndex;
            while (i < pathString.Length)
            {
                if (Path.IsDirectorySeparator(pathString[i]) || i == pathString.Length - 1)
                {
                    Path currentPath = new Path(pathString.Substring(0, i + 1));
                    if (!Exists(currentPath))
                    {
                        bool succeeded = NativeMethods.CreateDirectory(currentPath.WithWin32LongPathPrefix(), null);
                        if (!succeeded)
                            throw CreateWin32LastErrorException("Error creating directory '{0}'.", currentPath);
                    }
                    Debug.Assert(Exists(currentPath), "path should always exists at this point!");
                }
                i++;
            }
        }

        public override bool Exists(Path path)
        {
            var result = NativeMethods.GetFileAttributes(path.WithWin32LongPathPrefix());
            return (result != NativeMethods.INVALID_FILE_ATTRIBUTES);
        }

        public override void RemoveDirectory(Path path, bool recursively)
        {
            if (recursively)
            {   // first gather contents and remove all content
                RemoveDirectoryRecursively(path);
            }
            else
            {
                var succeeded = NativeMethods.RemoveDirectory(path.WithWin32LongPathPrefix());
                if (!succeeded)
                    throw CreateWin32LastErrorException("Error removing directory '{0}'.", path);
            }
        }

        private void RemoveDirectoryRecursively(Path dirName)
        {
            var list = ListContents(dirName, true).ToArray();
            /* 
                We need to delete leaf nodes in the file hierarchy first to make sure all directories are empty.
                We identify leaf nodes by their depth (greatest number of path parts)
            */
            Func<Path,int> pathPartCount = (Path p) => p.PathString.Split(Path.DirectorySeperatorChars).Length;
            Array.Sort<Path>(list, (a,b) => pathPartCount(b) - pathPartCount(a));
            Array.ForEach(list
                , p => {
                    if (FileSystem.IsDirectory(p))
                        FileSystem.RemoveDirectory(p);
                    else
                        FileSystem.RemoveFile(p, true);
                    Debug.Assert(FileSystem.Exists(p) == false, string.Format("Files/directory still exits:'{0}'", p));
                }
            );
            Debug.Assert(list.All(p => FileSystem.Exists(p) == false), "Some files/directories still exits?");
            FileSystem.RemoveDirectory(dirName, false);
        }

        public override void RemoveFile(Path path, bool force)
        {
            if (IsReadOnly(path) && force)
                SetReadOnly(path, false);

            var succeeded = NativeMethods.DeleteFile(path.WithWin32LongPathPrefix());
            if (!succeeded)
                throw CreateWin32LastErrorException("Error deleting file '{0}'.", path);
        }

        public override void Copy(Path source, Path dest)
        {
            var succeeded = NativeMethods.CopyFile(source.WithWin32LongPathPrefix(), dest.WithWin32LongPathPrefix(), true);
            if (!succeeded)
                throw CreateWin32LastErrorException("Error copying file '{0}' to '{1}'.", source, dest);
        }

        /// <summary>
        /// Creates or overwrites the file at the specified path.
        /// </summary>
        /// <param name="path">The path and name of the file to create. Supports long file paths.</param>
        /// <returns>A <see cref="System.IO.Stream"/> that provides read/write access to the file specified in path.</returns>
        public override System.IO.Stream CreateFile(Path path)
        {
            if (path.IsEmpty)
                throw new ArgumentNullException("path");

            NativeMethods.EFileAccess fileAccess = NativeMethods.EFileAccess.GenericWrite | NativeMethods.EFileAccess.GenericRead;
            NativeMethods.EFileShare fileShareMode = NativeMethods.EFileShare.None;//exclusive
            NativeMethods.ECreationDisposition creationDisposition = NativeMethods.ECreationDisposition.CreateAlways;
            SafeFileHandle hFile = NativeMethods.CreateFile(path.WithWin32LongPathPrefix(), fileAccess, fileShareMode, IntPtr.Zero, creationDisposition, NativeMethods.EFileAttributes.Normal, IntPtr.Zero);
            if (hFile.IsInvalid)
                throw CreateWin32LastErrorException("Error creating file at path '{0}'.", path);
            return new System.IO.FileStream(hFile, System.IO.FileAccess.ReadWrite);
        }

        public override IEnumerable<Path> ListContents(Path directory)
        {
            //NOTE: An important part of our contract is that if directory is not a directory, we return an empty set:
            if (!Exists(directory) || !IsDirectory(directory))
                yield break;

            // normalize dirName so we can assume it doesn't have a slash on the end:
            string dirName = directory.PathString;
            dirName = dirName.TrimEnd(Path.DirectorySeperatorChars);
            dirName = new Path(dirName).WithWin32LongPathPrefix();

            NativeMethods.WIN32_FIND_DATA findData;
            IntPtr findHandle = NativeMethods.FindFirstFile(dirName + @"\*", out findData);
            if (findHandle != NativeMethods.INVALID_HANDLE_VALUE)
            {
                try
                {
                    bool found;
                    do
                    {
                        string currentFileName = findData.cFileName;
                        if (currentFileName != "." && currentFileName != "..")
                        {
                            //NOTE: Instantiating the Path here will strip off the Win32 prefix that is added here as needed:
                            yield return Path.Combine(dirName, currentFileName);
                        }
                        // find next
                        found = NativeMethods.FindNextFile(findHandle, out findData);
                    } while (found);
                }
                finally
                {
                    NativeMethods.FindClose(findHandle);
                }
            }
        }
    }
}
