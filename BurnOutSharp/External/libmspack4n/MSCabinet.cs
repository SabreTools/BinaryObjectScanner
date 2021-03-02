using System;
using LessIO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace LibMSPackN
{
	/// <summary>
	/// Represents a .cab file.
	/// </summary>
	/// <remarks>
	/// Example usage:
	/// <code>
	/// const string cabinetFilename = @"c:\somecab.cab";
	/// using (var cabinet = new MSCabinet(cabinetFilename))
	/// {
	/// 	var outputDir = Path.Combine(Assembly.GetExecutingAssembly().Location, Path.GetFileNameWithoutExtension(cabinetFilename));
	/// 	foreach (MSCabFile containedFile in cabinet.GetFiles())
	/// 	{
	/// 		Debug.Print(containedFile.Filename);
	/// 		Debug.Print(containedFile.LengthInBytes.ToString(CultureInfo.InvariantCulture));
	/// 		containedFile.ExtractTo(Path.Combine(outputDir, containedFile.Filename));
	/// 	}
	/// }
	/// </code>
	/// </remarks>
	public sealed class MSCabinet : IDisposable
	{	
		private NativeMethods.mscabd_cabinet _nativeCabinet = new NativeMethods.mscabd_cabinet();
		private IntPtr _pNativeCabinet;
		private readonly string _cabinetFilename;
		private IntPtr _pCabinetFilenamePinned;
		private IntPtr _pDecompressor;

		public MSCabinet(string cabinetFilename)
		{
            cabinetFilename = new Path(cabinetFilename).WithWin32LongPathPrefix();
            _cabinetFilename = cabinetFilename;
			_pCabinetFilenamePinned = Marshal.StringToCoTaskMemAnsi(_cabinetFilename);// needs to be pinned as we use the address in unmanaged code.
			_pDecompressor = MSCabDecompressor.CreateInstance();

			// open cabinet:
			_pNativeCabinet = NativeMethods.mspack_invoke_mscab_decompressor_open(_pDecompressor, _pCabinetFilenamePinned);
			if (_pNativeCabinet == IntPtr.Zero)
			{
				var lasterror = NativeMethods.mspack_invoke_mscab_decompressor_last_error(_pDecompressor);
				throw new Exception("Failed to open cabinet. Last error:" + lasterror);
			}
			//Marshal.PtrToStructure(_pNativeCabinet, _nativeCabinet);
			_nativeCabinet = (NativeMethods.mscabd_cabinet) Marshal.PtrToStructure(_pNativeCabinet, typeof (NativeMethods.mscabd_cabinet));
		}

		public string LocalFilePath
		{
			get { return _cabinetFilename; }
		}

		~MSCabinet()
		{
			Close(false);
		}
		
		public void Close(bool isDisposing)
		{
			Debug.Print("Disposing MSCabinet for {0}. isDisposing:{1}", _cabinetFilename, isDisposing);
			if (_pNativeCabinet != IntPtr.Zero)
			{
				NativeMethods.mspack_invoke_mscab_decompressor_close(_pDecompressor, _pNativeCabinet);
				_pNativeCabinet = IntPtr.Zero;
			}

			if (_pDecompressor != IntPtr.Zero)
			{
				MSCabDecompressor.DestroyInstance(_pDecompressor);
				_pDecompressor = IntPtr.Zero;
			}
			if (_pCabinetFilenamePinned!= IntPtr.Zero)
			{
				Marshal.FreeCoTaskMem(_pCabinetFilenamePinned);
				_pCabinetFilenamePinned = IntPtr.Zero;
			}
			GC.SuppressFinalize(this);
		}

		void IDisposable.Dispose()
		{
			Close(true);
		}

		[DebuggerStepThrough, DebuggerHidden]
		private void ThrowOnInvalidState()
		{
			if (_pNativeCabinet == IntPtr.Zero)
				throw new InvalidOperationException("Cabinet not initialized.");
			if (_pDecompressor == IntPtr.Zero)
				throw new InvalidOperationException("Decompressor not initialized.");
		}

		internal bool IsInvalidState
		{
			get { return _pNativeCabinet == IntPtr.Zero || _pDecompressor == IntPtr.Zero; }
		}

		public MSCabinetFlags Flags
		{
			get
			{
				ThrowOnInvalidState();
				return (MSCabinetFlags)_nativeCabinet.flags;
			}
		}

		public string PrevName
		{
			get
			{
				ThrowOnInvalidState();
				return _nativeCabinet.prevname;
			}
		}

		public string NextName
		{
			get
			{
				ThrowOnInvalidState();
				return _nativeCabinet.nextname;
			}
		}

		internal IntPtr Decompressor
		{
			get { return _pDecompressor; }
		}

		public IEnumerable<MSCompressedFile> GetFiles()
		{
			ThrowOnInvalidState();
			
			IntPtr pNextFile = _nativeCabinet.files;
			MSCompressedFile containedFile;
			if (pNextFile != IntPtr.Zero)
				containedFile = new MSCompressedFile(this, pNextFile);
			else
				containedFile = null;

			while (containedFile != null)
			{
				yield return containedFile;
				containedFile = containedFile.Next;
			}
		}

		/// <summary>
		/// Appends specified cabinet to this one, forming or extending a cabinet set.
		/// </summary>
		/// <param name="nextCabinet">The cab to append to this one.</param>
		public void Append(MSCabinet nextCabinet)
		{
			var result = NativeMethods.mspack_invoke_mscab_decompressor_append(_pDecompressor, _pNativeCabinet, nextCabinet._pNativeCabinet);
			if (result != NativeMethods.MSPACK_ERR.MSPACK_ERR_OK)
				throw new Exception(string.Format("Error '{0}' appending cab '{1}' to {2}.", result, nextCabinet._cabinetFilename, _cabinetFilename));
			
			// after a successul append remarshal over the nativeCabinet struct5ure as it now represents the combined state.
			_nativeCabinet = (NativeMethods.mscabd_cabinet)Marshal.PtrToStructure(_pNativeCabinet, typeof(NativeMethods.mscabd_cabinet));
			nextCabinet._nativeCabinet = (NativeMethods.mscabd_cabinet)Marshal.PtrToStructure(nextCabinet._pNativeCabinet, typeof(NativeMethods.mscabd_cabinet));
		}
	}

	/// <summary>
	/// Used with <see cref="MSCabinet.Flags"/>
	/// </summary>
	[Flags]
	public enum MSCabinetFlags
	{
		/** Cabinet header flag: cabinet has a predecessor */
		MSCAB_HDR_PREVCAB = 0x01,
		/** Cabinet header flag: cabinet has a successor */
		MSCAB_HDR_NEXTCAB = 0x02,
		/** Cabinet header flag: cabinet has reserved header space */
		MSCAB_HDR_RESV = 0x04
	}
}