using System;
using System.Collections.Generic;
using System.Linq;

namespace LessIO.Strategies
{
    /// <summary>
    /// See <see cref="FileSystem"/> for documentation of each method of this class.
    /// </summary>
    internal abstract class FileSystemStrategy
    {
        public abstract void SetLastWriteTime(Path path, DateTime lastWriteTime);
        public abstract void SetAttributes(Path path, FileAttributes fileAttributes);
        public abstract FileAttributes GetAttributes(Path path);
        public abstract bool Exists(Path path);
        public abstract void CreateDirectory(Path path);
        public abstract void Copy(Path source, Path dest);
        public abstract void RemoveDirectory(Path path, bool recursively);
        public abstract void RemoveFile(Path path, bool force);
        public abstract System.IO.Stream CreateFile(Path path);
        public abstract IEnumerable<Path> ListContents(Path directory);

        public virtual IEnumerable<Path> ListContents(Path directory, bool recursive)
        {
            IEnumerable<Path> children = ListContents(directory);
            if (recursive)
            {
                IEnumerable<Path> grandChildren = children.SelectMany(
                    p => ListContents(p, recursive)
                );
                return Enumerable.Concat(children, grandChildren);
            }
            else
            {
                return children;
            }
        }

        public virtual bool IsDirectory(Path path)
        {
            FileAttributes attributes = GetAttributes(path);
            return (attributes & FileAttributes.Directory) == FileAttributes.Directory;
        }

        public virtual bool IsReadOnly(Path path)
        {
            FileAttributes attributes = GetAttributes(path);
            return (attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly;
        }

        public virtual void SetReadOnly(Path path, bool readOnly)
        {
            FileAttributes attributes = GetAttributes(path);
            if (readOnly)
                attributes = attributes | FileAttributes.ReadOnly;
            else
                attributes = attributes & ~FileAttributes.ReadOnly;
            SetAttributes(path, attributes);
        }
    }
}
