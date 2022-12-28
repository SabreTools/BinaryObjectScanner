using System.Runtime.InteropServices;

namespace BurnOutSharp.Models.PortableExecutable
{
    /// <summary>
    /// A block of text used by an extended dialog box template to describe the extended dialog box.
    /// For a description of the format of an extended dialog box template, see DLGTEMPLATEEX.
    /// </summary>
    /// <see href="https://learn.microsoft.com/en-us/windows/win32/dlgbox/dlgitemtemplateex"/>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class DialogItemTemplateExtended
    {
        /// <summary>
        /// The help context identifier for the control. When the system sends a WM_HELP message,
        /// it passes the helpID value in the dwContextId member of the HELPINFO structure.
        /// </summary>
        public uint HelpID;

        /// <summary>
        /// The extended styles for a window. This member is not used to create controls in dialog
        /// boxes, but applications that use dialog box templates can use it to create other types
        /// of windows.
        /// </summary>
        public ExtendedWindowStyles ExtendedStyle;

        /// <summary>
        /// The style of the control. This member can be a combination of window style values
        /// (such as WS_BORDER) and one or more of the control style values (such as
        /// BS_PUSHBUTTON and ES_LEFT).
        /// </summary>
        public WindowStyles Style;

        /// <summary>
        /// The x-coordinate, in dialog box units, of the upper-left corner of the control.
        /// This coordinate is always relative to the upper-left corner of the dialog box's
        /// client area.
        /// </summary>
        /// <remarks>
        /// The x, y, cx, and cy members specify values in dialog box units. You can convert these values to screen
        /// units (pixels) by using the MapDialogRect function.
        /// </remarks>
        public short PositionX;

        /// <summary>
        /// The y-coordinate, in dialog box units, of the upper-left corner of the control.
        /// This coordinate is always relative to the upper-left corner of the dialog box's
        /// client area.
        /// </summary>
        /// <remarks>
        /// The x, y, cx, and cy members specify values in dialog box units. You can convert these values to screen
        /// units (pixels) by using the MapDialogRect function.
        /// </remarks>
        public short PositionY;

        /// <summary>
        /// The width, in dialog box units, of the control.
        /// </summary>
        /// <remarks>
        /// The x, y, cx, and cy members specify values in dialog box units. You can convert these values to screen
        /// units (pixels) by using the MapDialogRect function.
        /// </remarks>
        public short WidthX;

        /// <summary>
        /// The height, in dialog box units, of the control.
        /// </summary>
        /// <remarks>
        /// The x, y, cx, and cy members specify values in dialog box units. You can convert these values to screen
        /// units (pixels) by using the MapDialogRect function.
        /// </remarks>
        public short HeightY;

        /// <summary>
        /// The control identifier.
        /// </summary>
        public uint ID;

        /// <summary>
        /// A variable-length array of 16-bit elements that specifies the window class of the control. If
        /// the first element of this array is any value other than 0xFFFF, the system treats the array as
        /// a null-terminated Unicode string that specifies the name of a registered window class.
        /// 
        /// If the first element is 0xFFFF, the array has one additional element that specifies the ordinal
        /// value of a predefined system class.
        /// </summary>
        /// <remarks>
        /// If you specify character strings in the class and title arrays, you must use Unicode strings. Use the
        /// MultiByteToWideChar function to generate Unicode strings from ANSI strings.
        /// </remarks>
        public string ClassResource;

        /// <summary>
        /// The ordinal value of a predefined system class.
        /// </summary>
        public DialogItemTemplateOrdinal ClassResourceOrdinal;

        /// <summary>
        /// A variable-length array of 16-bit elements that contains the initial text or resource identifier of the
        /// control. If the first element of this array is 0xFFFF, the array has one additional element that
        /// specifies the ordinal value of a resource, such as an icon, in an executable file. You can use a
        /// resource identifier for controls, such as static icon controls, that load and display an icon or other
        /// resource rather than text. If the first element is any value other than 0xFFFF, the system treats the
        /// array as a null-terminated Unicode string that specifies the initial text.
        /// </summary>
        /// <remarks>
        /// If you specify character strings in the class and title arrays, you must use Unicode strings. Use the
        /// MultiByteToWideChar function to generate Unicode strings from ANSI strings.
        /// </remarks>
        public string TitleResource;

        /// <summary>
        /// An ordinal value of a resource, such as an icon, in an executable file
        /// </summary>
        public ushort TitleResourceOrdinal;

        /// <summary>
        /// The creation data array begins at the next WORD boundary after the title array. This creation data
        /// can be of any size and format. If the first word of the creation data array is nonzero, it indicates
        /// the size, in bytes, of the creation data (including the size word).
        /// </summary>
        public ushort CreationDataSize;

        /// <summary>
        /// The creation data array begins at the next WORD boundary after the title array. This creation data
        /// can be of any size and format. The control's window procedure must be able to interpret the data.
        /// When the system creates the control, it passes a pointer to this data in the lParam parameter of the
        /// WM_CREATE message that it sends to the control.
        /// </summary>
        public byte[] CreationData;
    }
}
