using System;
using LessIO;
using System.Runtime.InteropServices;

namespace LibMSPackN
{
    /// <summary>
    /// Represents a file contained inside of a cab. Returned from <see cref="MSCabinet.GetFiles()"/>.
    /// </summary>
    public sealed class MSCompressedFile
	{
		private readonly MSCabinet _parentCabinet;
		private readonly NativeMethods.mscabd_file _nativeFile;
		private readonly IntPtr _pNativeFile;

		internal MSCompressedFile(MSCabinet parentCabinet, IntPtr pNativeFile)
		{
			//NOTE: we don't need to explicitly clean the nativeFile up. It is cleaned up by the parent cabinet.
			_parentCabinet = parentCabinet;
			_pNativeFile = pNativeFile;
			_nativeFile = (NativeMethods.mscabd_file)Marshal.PtrToStructure(_pNativeFile, typeof (NativeMethods.mscabd_file));
		}

		private void ThrowOnInvalidState()
		{
			if (_parentCabinet.IsInvalidState)
				throw new InvalidOperationException("Parent cabinet is no longer in a valid state. Ensure it was not disposed.");
			if (_pNativeFile == IntPtr.Zero)
				throw new InvalidOperationException("Native file is not initialized.");
		}

		public string Filename
		{
			get 
			{ 
				ThrowOnInvalidState();
				return _nativeFile.filename;
			}
		}

		public uint LengthInBytes 
		{
			get
			{
				ThrowOnInvalidState();
				return _nativeFile.length;
			}
		}

		public MSCompressedFile Next
		{
			get
			{
				MSCompressedFile next;
				if (_nativeFile.next != IntPtr.Zero)
					next = new MSCompressedFile(_parentCabinet, _nativeFile.next);
				else
					next = null;
				return next;
			}
		}

		public void ExtractTo(string destinationFilename)
		{
			ThrowOnInvalidState();
			IntPtr pDestinationFilename = IntPtr.Zero;
            //TODO: Delete the file if it exists. If there are any issues ovewriting the dest file (e.g. it's readonly) MSPACK gives essentialy no error information.
			string longDestinationFilename = new Path(destinationFilename).WithWin32LongPathPrefix();
            try
			{
				pDestinationFilename = Marshal.StringToCoTaskMemAnsi(longDestinationFilename);
				var result = NativeMethods.mspack_invoke_mscab_decompressor_extract(_parentCabinet.Decompressor, _pNativeFile, pDestinationFilename);
				if (result != NativeMethods.MSPACK_ERR.MSPACK_ERR_OK)
					throw new Exception(string.Format("Error '{0}' extracting file to {1}.", result, longDestinationFilename));

                FileSystem.SetLastWriteTime(new LessIO.Path(longDestinationFilename), GetModifiedTime());

                var theAttributes = FileSystem.GetAttributes(new LessIO.Path(longDestinationFilename));
                
				if ((_nativeFile.attribs & NativeMethods.mscabd_file_attribs.MSCAB_ATTRIB_ARCH) == NativeMethods.mscabd_file_attribs.MSCAB_ATTRIB_ARCH)
					theAttributes |= FileAttributes.Archive;
				if ((_nativeFile.attribs & NativeMethods.mscabd_file_attribs.MSCAB_ATTRIB_HIDDEN) == NativeMethods.mscabd_file_attribs.MSCAB_ATTRIB_HIDDEN)
					theAttributes |= FileAttributes.Hidden;
				if ((_nativeFile.attribs & NativeMethods.mscabd_file_attribs.MSCAB_ATTRIB_RDONLY) == NativeMethods.mscabd_file_attribs.MSCAB_ATTRIB_RDONLY)
					theAttributes |= FileAttributes.ReadOnly;
				if ((_nativeFile.attribs & NativeMethods.mscabd_file_attribs.MSCAB_ATTRIB_SYSTEM) == NativeMethods.mscabd_file_attribs.MSCAB_ATTRIB_SYSTEM)
					theAttributes |= FileAttributes.System;

                FileSystem.SetAttributes(new LessIO.Path(longDestinationFilename), theAttributes);
			}
			finally
			{
				if (pDestinationFilename != IntPtr.Zero)
					Marshal.FreeCoTaskMem(pDestinationFilename);
			}
		}

		private DateTime GetModifiedTime()
		{
			return new DateTime(_nativeFile.date_y, _nativeFile.date_m, _nativeFile.date_d, _nativeFile.time_h, _nativeFile.time_m, _nativeFile.time_s);
		}
	}
}