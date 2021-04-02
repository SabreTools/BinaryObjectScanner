using System;
using LessIO.Strategies;
using LessIO.Strategies.Win32;
using BadPath = System.IO.Path;
using System.Collections.Generic;

namespace LessIO
{
    /// <summary>
    /// Provides various file system operations for the current platform.
    /// </summary>
    public static class FileSystem
    {
        private static readonly Lazy<FileSystemStrategy> LazyStrategy = new Lazy<FileSystemStrategy>(() => new Win32FileSystemStrategy());
        
        private static FileSystemStrategy Strategy
        {
            get { return LazyStrategy.Value; }
        }

        /// <summary>
        /// Sets the date and time that the specified file was last written to.
        /// </summary>
        /// <param name="path">The path of the file to set the file time on.</param>
        /// <param name="lastWriteTime">
        /// The date to set for the last write date and time of specified file. 
        /// Expressed in local time.
        /// </param>
        public static void SetLastWriteTime(Path path, DateTime lastWriteTime)
        {
            Strategy.SetLastWriteTime(path, lastWriteTime);
        }

        public static void SetAttributes(Path path, LessIO.FileAttributes fileAttributes)
        {
            Strategy.SetAttributes(path, fileAttributes);
        }

        public static LessIO.FileAttributes GetAttributes(Path path)
        {
            return Strategy.GetAttributes(path);
        }

        /// <summary>
        /// Returns true if a file or directory exists at the specified path.
        /// </summary>
        public static bool Exists(Path path)
        {
            return Strategy.Exists(path);
        }

        /// <summary>
        /// Creates the specified directory.
        /// </summary>
        /// <remarks>
        /// Creates parent directories as needed.
        /// </remarks>
        public static void CreateDirectory(Path path)
        {
            Strategy.CreateDirectory(path);
        }
        
        /// <summary>
        /// Removes/deletes an existing empty directory.
        /// See https://msdn.microsoft.com/en-us/library/windows/desktop/aa365488%28v=vs.85%29.aspx
        /// </summary>
        /// <param name="path"></param>
        public static void RemoveDirectory(Path path)
        {
            RemoveDirectory(path, false);
        }

        /// <summary>
        /// Removes/deletes an existing directory.
        /// See https://msdn.microsoft.com/en-us/library/windows/desktop/aa365488%28v=vs.85%29.aspx
        /// </summary>
        /// <param name="path">The path to the directory to remove.</param>
        /// <param name="recursively">
        /// True to remove the directory and all of its contents recursively.
        /// False will remove the directory only if it is empty.
        /// </param>
        /// <remarks>Recursively implies removing contained files forcefully.</remarks>
        public static void RemoveDirectory(Path path, bool recursively)
        {
            Strategy.RemoveDirectory(path, recursively);
        }

        /// <summary>
        /// Removes/deletes an existing file.
        /// To remove a directory see <see cref="RemoveDirectory"/>.
        /// https://msdn.microsoft.com/en-us/library/windows/desktop/aa363915%28v=vs.85%29.aspx
        /// </summary>
        /// <param name="path">The file to remove.</param>
        /// <param name="forcefully">True to remove the file even if it is read-only.</param>
        public static void RemoveFile(Path path, bool forcefully)
        {
            Strategy.RemoveFile(path, forcefully);
        }

        /// <summary>
        /// Removes/deletes an existing file.
        /// To remove a directory see <see cref="RemoveDirectory"/>.
        /// https://msdn.microsoft.com/en-us/library/windows/desktop/aa363915%28v=vs.85%29.aspx
        /// </summary>
        /// <param name="path">The file to remove.</param>
        public static void RemoveFile(Path path)
        {
            Strategy.RemoveFile(path, false);
        }

        /// <summary>
        /// Copies the specified existing file to a new location. 
        /// Will throw an exception if the destination file already exists.
        /// </summary>
        public static void Copy(Path source, Path dest)
        {
            if (!Strategy.Exists(source))
                throw new Exception(string.Format("The file \"{0}\" does not exist.", source));
            Strategy.Copy(source, dest);
        }

        /// <summary>
        /// Returns true if the specified path is a directory.
        /// </summary>
        internal static bool IsDirectory(Path path)
        {
            return Strategy.IsDirectory(path);
        }

        /// <summary>
        /// Creates or overwrites the file at the specified path.
        /// </summary>
        /// <param name="path">The path and name of the file to create. Supports long file paths.</param>
        /// <returns>A <see cref="System.IO.Stream"/> that provides read/write access to the file specified in path.</returns>
        public static System.IO.Stream CreateFile(Path path)
        {
            return Strategy.CreateFile(path);
        }

        /// <summary>
        /// Returns the contents of the specified directory.
        /// </summary>
        /// <param name="directory">The path to the directory to get the contents of.</param>
        public static IEnumerable<Path> ListContents(Path directory)
        {
            return Strategy.ListContents(directory);
        }

        /// <summary>
        /// Returns the contents of the specified directory.
        /// </summary>
        /// <param name="directory">The path to the directory to get the contents of.</param>
        /// <param name="recursive">True to list the contents of any child directories.</param>
        /// <remarks>If the specified directory is not actually a directory then an empty set is returned.</remarks>
        public static IEnumerable<Path> ListContents(Path directory, bool recursive)
        {
            return Strategy.ListContents(directory, recursive);
        }
    }
}
