using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace LibMSPackN
{
	internal static class NativeMethods
	{
		// ReSharper disable InconsistentNaming
		// ReSharper disable EnumUnderlyingTypeIsInt
		/// <summary>
		/// A very simple test/usage example of the raw native mspack API.
		/// </summary>
		public static void DoTest()
		{
			IntPtr pDecompressor = mspack_create_cab_decompressor(IntPtr.Zero);
			const string cabFilename = @"C:\projects\LibMSPackDotNet\LibMSPackDotNetTest\bin\Debug\huge-lzx15.cab";
			const string outFilename = @"C:\projects\LibMSPackDotNet\LibMSPackDotNetTest\bin\Debug\huge-lzx15.out";

			IntPtr pCabFilename = Marshal.StringToCoTaskMemAnsi(cabFilename);
			IntPtr pOutFilename = Marshal.StringToCoTaskMemAnsi(outFilename);

			IntPtr pCabinet = mspack_invoke_mscab_decompressor_open(pDecompressor, pCabFilename);
			mscabd_cabinet cabStruct = (mscabd_cabinet) Marshal.PtrToStructure(pCabinet, typeof (mscabd_cabinet));
			IntPtr pFirstFile = cabStruct.files;
			Debug.Print("pFirstFile: 0x" + pFirstFile.ToInt32().ToString("x"));
			MSPACK_ERR result = mspack_invoke_mscab_decompressor_extract(pDecompressor, pFirstFile, pOutFilename);
			Debug.Print("extract result: {0}", result);
			mspack_invoke_mscab_decompressor_close(pDecompressor, pCabinet);
			mspack_destroy_cab_decompressor(pDecompressor);

			Marshal.FreeCoTaskMem(pOutFilename);
			Marshal.FreeCoTaskMem(pCabFilename);
		}

		/// <summary>
		/// A structure which represents a single file in a cabinet or cabinet set.
		/// * All fields are READ ONLY.
		/// </summary>
		[StructLayout(LayoutKind.Explicit, CharSet = CharSet.Ansi, Size = 36)]
		internal struct mscabd_file
		{
			/**
		   * The next file in the cabinet or cabinet set, or NULL if this is the
		   * final file.
		   */
			[FieldOffset(0)] public IntPtr /*mscabd_file*/ next;

			/**
		   * The filename of the file.
		   *
		   * A null terminated string of up to 255 bytes in length, it may be in
		   * either ISO-8859-1 or UTF8 format, depending on the file attributes.
		   *
		   * @see attribs
		   */
			[MarshalAs(UnmanagedType.LPStr)] [FieldOffset(4)] public string filename;

			/** The uncompressed length of the file, in bytes. */
			[FieldOffset(8)] public uint length;

			/**
			 * File attributes.
			 *
			 * The following attributes are defined:
			 * - #MSCAB_ATTRIB_RDONLY indicates the file is write protected.
			 * - #MSCAB_ATTRIB_HIDDEN indicates the file is hidden.
			 * - #MSCAB_ATTRIB_SYSTEM indicates the file is a operating system file.
			 * - #MSCAB_ATTRIB_ARCH indicates the file is "archived".
			 * - #MSCAB_ATTRIB_EXEC indicates the file is an executable program.
			 * - #MSCAB_ATTRIB_UTF_NAME indicates the filename is in UTF8 format rather
			 *   than ISO-8859-1.
			 */
			[FieldOffset(12)] public mscabd_file_attribs attribs;

			/** File's last modified time, hour field. */
			[FieldOffset(16)] public byte time_h;

			/** File's last modified time, minute field. */
			[FieldOffset(17)] public byte time_m;

			/** File's last modified time, second field. */
			[FieldOffset(18)] public byte time_s;

			/** File's last modified date, day field. */
			[FieldOffset(19)] public byte date_d;

			/** File's last modified date, month field. */
			[FieldOffset(20)] public byte date_m;

			/** File's last modified date, year field. */
			[FieldOffset(24)] public int date_y;

			/** A pointer to the folder that contains this file. */
			[FieldOffset(28)] public IntPtr /*mscabd_folder*/ folder;

			/** The uncompressed offset of this file in its folder. */
			[FieldOffset(32)] public uint offset;
		}

		[Flags]
		internal enum mscabd_file_attribs : int
		{
			/** mscabd_file::attribs attribute: file is read-only. */
			MSCAB_ATTRIB_RDONLY = 0x01,
			/** mscabd_file::attribs attribute: file is hidden. */
			MSCAB_ATTRIB_HIDDEN = 0x02,
			/** mscabd_file::attribs attribute: file is an operating system file. */
			MSCAB_ATTRIB_SYSTEM = 0x04,
			/** mscabd_file::attribs attribute: file is "archived". */
			MSCAB_ATTRIB_ARCH = 0x20,
			/** mscabd_file::attribs attribute: file is an executable program. */
			MSCAB_ATTRIB_EXEC = 0x40,
			/** mscabd_file::attribs attribute: filename is UTF8, not ISO-8859-1. */
			MSCAB_ATTRIB_UTF_NAME = 0x80
		}

		/// <summary>
		/// A structure which represents a single cabinet file.
		/// 
		///  All fields are READ ONLY.
		/// 
		///  If this cabinet is part of a merged cabinet set, the #files and #folders
		///  fields are common to all cabinets in the set, and will be identical.
		/// 
		///  @see mscab_decompressor::open(), mscab_decompressor::close(),
		///       mscab_decompressor::search()
		/// </summary>
		[StructLayout(LayoutKind.Explicit, Size = 60, CharSet = CharSet.Ansi)]
		internal class mscabd_cabinet
		{
			/**
			 * The next cabinet in a chained list, if this cabinet was opened with
			 * mscab_decompressor::search(). May be NULL to mark the end of the
			 * list.
			 */
			[FieldOffset(0)] public IntPtr /*mscabd_cabinet*/ next;

			/**
			 * The filename of the cabinet. More correctly, the filename of the
			 * physical file that the cabinet resides in. This is given by the
			 * library user and may be in any format.
			 */
			[MarshalAs(UnmanagedType.LPStr)] [FieldOffset(4)] public string filename;

			/** The file offset of cabinet within the physical file it resides in. */
			[FieldOffset(8)] public off_t base_offset;

			/** The length of the cabinet file in bytes. */
			[FieldOffset(12)] public UInt32 length;

			/** The previous cabinet in a cabinet set, or NULL. */
			[FieldOffset(16)] public IntPtr /*mscabd_cabinet*/ prevcab;

			/** The next cabinet in a cabinet set, or NULL. */
			[FieldOffset(20)] public IntPtr /*mscabd_cabinet*/ nextcab;

			/** The filename of the previous cabinet in a cabinet set, or NULL. */
			[MarshalAs(UnmanagedType.LPStr)] [FieldOffset(24)] public string prevname;

			/** The filename of the next cabinet in a cabinet set, or NULL. */
			[MarshalAs(UnmanagedType.LPStr)] [FieldOffset(28)] public string nextname;

			/** The name of the disk containing the previous cabinet in a cabinet
			 * set, or NULL.
			 */
			[MarshalAs(UnmanagedType.LPStr)] [FieldOffset(32)] public string previnfo;

			/** The name of the disk containing the next cabinet in a cabinet set,
			 * or NULL.
			 */
			[MarshalAs(UnmanagedType.LPStr)] [FieldOffset(36)] public string nextinfo;

			/** A list of all files in the cabinet or cabinet set. */
			[FieldOffset(40)] public IntPtr /*mscabd_file*/ files;

			/** A list of all folders in the cabinet or cabinet set. */
			[FieldOffset(44)] public IntPtr /*mscabd_folder*/ folders;

			/** 
			 * The set ID of the cabinet. All cabinets in the same set should have
			 * the same set ID.
			 */
			[FieldOffset(48)] public ushort set_id;

			/**
			 * The index number of the cabinet within the set. Numbering should
			 * start from 0 for the first cabinet in the set, and increment by 1 for
			 * each following cabinet.
			 */
			[FieldOffset(50)] public ushort set_index;

			/**
			 * The number of bytes reserved in the header area of the cabinet.
			 *
			 * If this is non-zero and flags has MSCAB_HDR_RESV set, this data can
			 * be read by the calling application. It is of the given length,
			 * located at offset (base_offset + MSCAB_HDR_RESV_OFFSET) in the
			 * cabinet file.
			 *
			 * @see flags
			 */
			[FieldOffset(52)] public ushort header_resv;

			/**
			 * Header flags.
			 *
			 * - MSCAB_HDR_PREVCAB indicates the cabinet is part of a cabinet set, and
			 *                     has a predecessor cabinet.
			 * - MSCAB_HDR_NEXTCAB indicates the cabinet is part of a cabinet set, and
			 *                     has a successor cabinet.
			 * - MSCAB_HDR_RESV indicates the cabinet has reserved header space.
			 *
			 * @see prevname, previnfo, nextname, nextinfo, header_resv
			 */

			[FieldOffset(56)] //NOTE the unexpected pack here. But this is the way it is in C!
			public int flags;
		}

		/// <summary>
		/// Creates a new CAB decompressor.
		/// </summary>
		/// <param name="sys">a custom mspack_system structure, or NULL to use the default. PASS NULL!</param>
		/// <returns>A pointer to mscab_decompressor or null.</returns>
		[DllImport("mspack.dll", SetLastError = false, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		internal static extern IntPtr mspack_create_cab_decompressor(IntPtr sys);

		/// <summary>
		/// Destroys an existing CAB decompressor.
		/// </summary>
		/// <param name="mscab_decompressor_self">the #mscab_decompressor to destroy</param>
		[DllImport("mspack.dll", SetLastError = false, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		internal static extern void mspack_destroy_cab_decompressor(IntPtr mscab_decompressor_self);

		/// <summary>
		/// Opens a cabinet file and reads its contents.
		///
		/// If the file opened is a valid cabinet file, all headers will be read
		/// and a mscabd_cabinet structure will be returned, with a full list of
		/// folders and files.
		///
		/// In the case of an error occuring, NULL is returned and the error code
		/// is available from last_error().
		///
		/// The filename pointer should be considered "in use" until close() is
		/// called on the cabinet.
		/// </summary>
		/// <param name="mscab_decompressor_self">a self-referential pointer to the mscab_decompressor instance (returned from <see cref="mspack_create_cab_decompressor"/>) being called</param>
		/// <param name="pinnedFilename">the filename of the cabinet file. 
		/// *NOTE*: Since this string is allocated in the "managed world" (probably with <see cref="Marshal.StringToCoTaskMemAnsi"/>) it must be pinned lifetime of this cabinet as libmspack uses this string during future operations on the cabinet. Otherwise the GC will move it or free it. Be sure to clean it up later!
		/// </param>
		/// <returns>a pointer to a mscabd_cabinet structure, or NULL on failure</returns>
		[DllImport("mspack.dll", SetLastError = false, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		internal static extern IntPtr /*mscabd_cabinet*/ mspack_invoke_mscab_decompressor_open(IntPtr mscab_decompressor_self, IntPtr pinnedFilename);

		/// <summary>
		/// Closes a previously opened cabinet or cabinet set.
		///
		/// This closes a cabinet, all cabinets associated with it via the
		/// mscabd_cabinet::next, mscabd_cabinet::prevcab and
		/// mscabd_cabinet::nextcab pointers, and all folders and files. All
		/// memory used by these entities is freed.
		///
		/// The cabinet pointer is now invalid and cannot be used again. All
		/// mscabd_folder and mscabd_file pointers from that cabinet or cabinet
		/// set are also now invalid, and cannot be used again.
		///
		/// If the cabinet pointer given was created using search(), it MUST be
		/// the cabinet pointer returned by search() and not one of the later
		/// cabinet pointers further along the mscabd_cabinet::next chain.
		/// 
		/// If extra cabinets have been added using append() or prepend(), these
		/// will all be freed, even if the cabinet pointer given is not the first
		/// cabinet in the set. Do NOT close() more than one cabinet in the set.
		///
		/// The mscabd_cabinet::filename is not freed by the library, as it is
		/// not allocated by the library. The caller should free this itself if
		/// necessary, before it is lost forever.
		/// </summary>
		/// <param name="mscab_decompressor_self">a self-referential pointer to the mscab_decompressor instance (returned from <see cref="mspack_create_cab_decompressor"/>) being called</param>
		/// <param name="cab">the cabinet to close</param>
		[DllImport("mspack.dll", SetLastError = false, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		internal static extern void mspack_invoke_mscab_decompressor_close(IntPtr mscab_decompressor_self, IntPtr /*mscabd_cabinet*/ cab);

		/// <summary>
		/// Extracts a file from a cabinet or cabinet set.
		///
		/// This extracts a compressed file in a cabinet and writes it to the given
		/// filename.
		///
		/// The MS-DOS filename of the file, mscabd_file::filename, is NOT USED
		/// by extract(). The caller must examine this MS-DOS filename, copy and
		/// change it as necessary, create directories as necessary, and provide
		/// the correct filename as a parameter, which will be passed unchanged
		/// to the decompressor's mspack_system::open()
		///
		/// If the file belongs to a split folder in a multi-part cabinet set,
		/// and not enough parts of the cabinet set have been loaded and appended
		/// or prepended, an error will be returned immediately.
		/// </summary>
		/// <param name="mscab_decompressor_self">a self-referential pointer to the mscab_decompressor instance (returned from <see cref="mspack_create_cab_decompressor"/>) being called</param>
		/// <param name="file">the file to be decompressed</param>
		/// <param name="filename">
		/// the filename of the file being written to.
		/// Allocate this file with <see cref="Marshal.StringToCoTaskMemAnsi"/> (more expaination in <see cref="mspack_invoke_mscab_decompressor_open"/>).
		/// </param>
		/// <returns>an error code, or MSPACK_ERR_OK if successful</returns>
		[DllImport("mspack.dll", SetLastError = false, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		internal static extern MSPACK_ERR mspack_invoke_mscab_decompressor_extract(IntPtr mscab_decompressor_self, IntPtr file, IntPtr filename);

		[DllImport("mspack.dll", SetLastError = false, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		internal static extern MSPACK_ERR mspack_invoke_mscab_decompressor_last_error(IntPtr mscab_decompressor_self);

		[DllImport("mspack.dll", SetLastError = false, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		internal static extern MSPACK_ERR mspack_invoke_mscab_decompressor_append(IntPtr mscab_decompressor_self, IntPtr /*mscabd_cabinet*/ cab, IntPtr /*mscabd_cabinet*/ nextcab);

		/// <summary>
		/// All compressors and decompressors use the same set of error codes. Most
		/// methods return an error code directly. For methods which do not
		/// return error codes directly, the error code can be obtained with the
		/// <see cref="mspack_invoke_mscab_decompressor_last_error"/> method.
		/// </summary>
		internal enum MSPACK_ERR : int
		{
			/// Error code: no error 
			MSPACK_ERR_OK = 0,

			/// Error code: bad arguments to method 
			MSPACK_ERR_ARGS = 1,

			/// Error code: error opening file 
			MSPACK_ERR_OPEN = 2,

			/// Error code: error reading file 
			MSPACK_ERR_READ = 3,

			/// Error code: error writing file 
			MSPACK_ERR_WRITE = 4,

			/// Error code: seek error 
			MSPACK_ERR_SEEK = 5,

			/// Error code: out of memory 
			MSPACK_ERR_NOMEMORY = 6,

			/// Error code: bad "magic id" in file 
			MSPACK_ERR_SIGNATURE = 7,

			/// Error code: bad or corrupt file format 
			MSPACK_ERR_DATAFORMAT = 8,

			/// Error code: bad checksum or CRC 
			MSPACK_ERR_CHECKSUM = 9,

			/// Error code: error during compression 
			MSPACK_ERR_CRUNCH = 10,

			/// Error code: error during decompression 
			MSPACK_ERR_DECRUNCH = 11
		}

		/// <summary>
		/// Used by <see cref="mscabd_cabinet"/>.
		/// </summary>
		[StructLayout(LayoutKind.Sequential, Size = 4)]
		internal struct off_t
		{
			private Int32 value;

			public off_t(Int32 x)
			{
				value = x;
			}

			public static implicit operator off_t(Int32 x)
			{
				return new off_t(x);
			}

			public static implicit operator Int32(off_t x)
			{
				return x.value;
			}
		}
		// ReSharper restore EnumUnderlyingTypeIsInt
		// ReSharper restore InconsistentNaming
	}
}
