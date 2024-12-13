#if NET20 || NET35

using System;
using System.IO;

namespace BinaryObjectScanner
{
    /// <summary>
    /// Derived from the mscorlib code from .NET Framework 4.0
    /// </summary>
    internal static class OldDotNet
    {
        public static void CopyTo(this Stream source, Stream destination)
        {
            if (destination == null)
            {
                throw new ArgumentNullException("destination");
            }

            if (!source.CanRead && !source.CanWrite)
            {
                throw new ObjectDisposedException(null);
            }

            if (!destination.CanRead && !destination.CanWrite)
            {
                throw new ObjectDisposedException("destination");
            }

            if (!source.CanRead)
            {
                throw new NotSupportedException();
            }

            if (!destination.CanWrite)
            {
                throw new NotSupportedException();
            }

            byte[] array = new byte[81920];
            int count;
            while ((count = source.Read(array, 0, array.Length)) != 0)
            {
                destination.Write(array, 0, count);
            }
        }
    }
}

#endif
