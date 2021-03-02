using System;

namespace LessIO
{
    //TODO: These are the samea s Win32. Consider whether we should expose all of these?   
    /// <summary>
    /// See https://msdn.microsoft.com/en-us/library/windows/desktop/gg258117%28v=vs.85%29.aspx
    /// </summary>
    /// <remarks>
    /// These have the same values as <see cref="System.IO.FileAttributes"/> they can generally be casted between these enums (this Enum has more values than System.IO though).
    /// Defined in C:\Program Files (x86)\Windows Kits\8.1\Include\um\winnt.h
    /// </remarks>
    [Flags]
    public enum FileAttributes
    {
		//#define FILE_ATTRIBUTE_READONLY             0x00000001
		ReadOnly		                            = 0x00000001,  
		//#define FILE_ATTRIBUTE_HIDDEN               0x00000002
		Hidden	                        	        = 0x00000002,  
		//#define FILE_ATTRIBUTE_SYSTEM               0x00000004
		System		                                = 0x00000004,  
		//#define FILE_ATTRIBUTE_DIRECTORY            0x00000010
		Directory		                            = 0x00000010,  
		//#define FILE_ATTRIBUTE_ARCHIVE              0x00000020
		Archive		                                = 0x00000020,  
		//#define FILE_ATTRIBUTE_DEVICE               0x00000040
		Device		                                = 0x00000040,  
		//#define FILE_ATTRIBUTE_NORMAL               0x00000080
		Normal		                                = 0x00000080,  
		//#define FILE_ATTRIBUTE_TEMPORARY            0x00000100
		Temporary		                            = 0x00000100,  
		//#define FILE_ATTRIBUTE_SPARSE_FILE          0x00000200
		SparseFile                      		    = 0x00000200,  
		//#define FILE_ATTRIBUTE_REPARSE_POINT        0x00000400
		ReparsePoint                        		= 0x00000400,  
		//#define FILE_ATTRIBUTE_COMPRESSED           0x00000800
		Compressed                      		    = 0x00000800,  
		//#define FILE_ATTRIBUTE_OFFLINE              0x00001000
		Offline	                        	        = 0x00001000,  
		//#define FILE_ATTRIBUTE_NOT_CONTENT_INDEXED  0x00002000
		NotContentIndexed                       	= 0x00002000,  
		//#define FILE_ATTRIBUTE_ENCRYPTED            0x00004000
		Encrypted                       		    = 0x00004000,  
		//#define FILE_ATTRIBUTE_INTEGRITY_STREAM     0x00008000
		IntegrityStream	                        	= 0x00008000,  
		//#define FILE_ATTRIBUTE_VIRTUAL              0x00010000
		Virtual		                                = 0x00010000,  
		//#define FILE_ATTRIBUTE_NO_SCRUB_DATA        0x00020000
		NoScrubData	    	                        = 0x00020000,
        /* EA is not documented
        #define FILE_ATTRIBUTE_EA                   0x00040000
        EA                                   	  = 0x00040000
        */
    }
}
