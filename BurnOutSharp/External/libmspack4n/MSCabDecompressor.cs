using System;

namespace LibMSPackN
{
	/// <summary>
	///  Used internally to manage the decompressor. Technically you need one of these per thread (see code comments in libmspack, but I'm assuming single threaded or safe multithreaded access here).
	/// </summary>
	internal sealed class MSCabDecompressor : IDisposable
	{
		private IntPtr _pDecompressor;
		private static volatile MSCabDecompressor _default;
		private static readonly object SyncRoot = new object();

		private MSCabDecompressor()
		{
			_pDecompressor = CreateInstance();
		}

		~MSCabDecompressor()
		{
			Dispose();
		}

		public void Dispose()
		{
			if (_pDecompressor != IntPtr.Zero)
			{
				DestroyInstance(_pDecompressor);
				_pDecompressor = IntPtr.Zero;
			}
		}

		/// <summary>
		/// Creates a native instance of the decompressor and returns the pointer. Must be destroyed via <see cref="DestroyInstance"/>.
		/// </summary>
		internal static IntPtr CreateInstance()
		{
			var ptr = NativeMethods.mspack_create_cab_decompressor(IntPtr.Zero);
			if (ptr == IntPtr.Zero)
				throw new Exception("Failed to create cab_decompressor.");
			return ptr;
		}

		/// <summary>
		/// Destroys the specified instance that was created with <see cref="CreateInstance"/>.
		/// </summary>
		/// <param name="pDecompressor"></param>
		internal static void DestroyInstance(IntPtr pDecompressor)
		{
			NativeMethods.mspack_destroy_cab_decompressor(pDecompressor);
		}

		internal IntPtr Pointer
		{
			get
			{
				ThrowOnInvalidState();
				return _pDecompressor;
			}
		}

		/// <summary>
		/// Returns the defalut instance of a decompressor. DO NOT DISPOSE IT!
		/// </summary>
		public static MSCabDecompressor Default
		{
			get
			{
				if (_default == null)
				{
					lock (SyncRoot)
					{
						if (_default == null)
							_default = new MSCabDecompressor();
					}
				}
				return _default;
			}
		}

		public bool IsInvalidState
		{
			get { return _pDecompressor == IntPtr.Zero; }
		}

		private void ThrowOnInvalidState()
		{
			if (IsInvalidState)
				throw new ObjectDisposedException(typeof(MSCabDecompressor).Name);
		}
	}
}
