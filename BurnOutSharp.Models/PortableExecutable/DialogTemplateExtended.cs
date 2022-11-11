using System.Runtime.InteropServices;

namespace BurnOutSharp.Models.PortableExecutable
{
    /// <summary>
    /// An extended dialog box template begins with a DLGTEMPLATEEX header that describes
    /// the dialog box and specifies the number of controls in the dialog box. For each
    /// control in a dialog box, an extended dialog box template has a block of data that
    /// uses the DLGITEMTEMPLATEEX format to describe the control.
    /// 
    /// The DLGTEMPLATEEX structure is not defined in any standard header file. The
    /// structure definition is provided here to explain the format of an extended template
    /// for a dialog box.
    /// </summary>
    /// <see href="https://learn.microsoft.com/en-us/windows/win32/dlgbox/dlgtemplateex"/>
    [StructLayout(LayoutKind.Sequential)]
    public class DialogTemplateExtended
    {
        /// <summary>
        /// The version number of the extended dialog box template. This member must be
        /// set to 1.
        /// </summary>
        public ushort Version;

        /// <summary>
        /// Indicates whether a template is an extended dialog box template. If signature
        /// is 0xFFFF, this is an extended dialog box template. In this case, the dlgVer
        /// member specifies the template version number. If signature is any value other
        /// than 0xFFFF, this is a standard dialog box template that uses the DLGTEMPLATE
        /// and DLGITEMTEMPLATE structures.
        /// </summary>
        public ushort Signature;

        /// <summary>
        /// The help context identifier for the dialog box window. When the system sends a
        /// WM_HELP message, it passes this value in the wContextId member of the HELPINFO
        /// structure.
        /// </summary>
        public uint HelpID;

        /// <summary>
        /// The extended windows styles. This member is not used when creating dialog boxes,
        /// but applications that use dialog box templates can use it to create other types
        /// of windows. 
        /// </summary>
        public ExtendedWindowStyles ExtendedStyle;

        /// <summary>
        /// The style of the dialog box.
        /// 
        /// If style includes the DS_SETFONT or DS_SHELLFONT dialog box style, the DLGTEMPLATEEX
        /// header of the extended dialog box template contains four additional members (pointsize,
        /// weight, italic, and typeface) that describe the font to use for the text in the client
        /// area and controls of the dialog box. If possible, the system creates a font according
        /// to the values specified in these members. Then the system sends a WM_SETFONT message
        /// to the dialog box and to each control to provide a handle to the font.
        /// </summary>
        public WindowStyles Style;

        /// <summary>
        /// The number of controls in the dialog box.
        /// </summary>
        public ushort DialogItems;

        /// <summary>
        /// The x-coordinate, in dialog box units, of the upper-left corner of the dialog box.
        /// </summary>
        /// <remarks>
        /// The x, y, cx, and cy members specify values in dialog box units. You can convert these values
        /// to screen units (pixels) by using the MapDialogRect function.
        /// </remarks>
        public short PositionX;

        /// <summary>
        /// The y-coordinate, in dialog box units, of the upper-left corner of the dialog box.
        /// </summary>
        /// <remarks>
        /// The x, y, cx, and cy members specify values in dialog box units. You can convert these values
        /// to screen units (pixels) by using the MapDialogRect function.
        /// </remarks>
        public short PositionY;

        /// <summary>
        /// The width, in dialog box units, of the dialog box.
        /// </summary>
        /// <remarks>
        /// The x, y, cx, and cy members specify values in dialog box units. You can convert these values
        /// to screen units (pixels) by using the MapDialogRect function.
        /// </remarks>
        public short WidthX;

        /// <summary>
        /// The height, in dialog box units, of the dialog box.
        /// </summary>
        /// <remarks>
        /// The x, y, cx, and cy members specify values in dialog box units. You can convert these values
        /// to screen units (pixels) by using the MapDialogRect function.
        /// </remarks>
        public short HeightY;

        /// <summary>
        /// A variable-length array of 16-bit elements that identifies a menu resource for the dialog box.
        /// If the first element of this array is 0x0000, the dialog box has no menu and the array has no
        /// other elements. If the first element is 0xFFFF, the array has one additional element that
        /// specifies the ordinal value of a menu resource in an executable file. If the first element has
        /// any other value, the system treats the array as a null-terminated Unicode string that specifies
        /// the name of a menu resource in an executable file.
        /// </summary>
        public string MenuResource;

        /// <summary>
        /// The ordinal value of a menu resource in an executable file.
        /// </summary>
        public ushort MenuResourceOrdinal;

        /// <summary>A variable-length array of 16-bit elements that identifies the window class of the
        /// dialog box. If the first element of the array is 0x0000, the system uses the predefined dialog
        /// box class for the dialog box and the array has no other elements. If the first element is 0xFFFF,
        /// the array has one additional element that specifies the ordinal value of a predefined system
        /// window class. If the first element has any other value, the system treats the array as a
        /// null-terminated Unicode string that specifies the name of a registered window class.
        /// </summary>
        public string ClassResource;

        /// <summary>
        /// The ordinal value of a predefined system window class.
        /// </summary>
        public ushort ClassResourceOrdinal;

        /// <summary>
        /// The title of the dialog box. If the first element of this array is 0x0000, the dialog box has no
        /// title and the array has no other elements.
        /// </summary>
        public string TitleResource;

        /// <summary>
        /// The point size of the font to use for the text in the dialog box and its controls.
        /// 
        /// This member is present only if the style member specifies DS_SETFONT or DS_SHELLFONT.
        /// </summary>
        public ushort PointSize;

        /// <summary>
        /// The weight of the font. Note that, although this can be any of the values listed for the lfWeight
        /// member of the LOGFONT structure, any value that is used will be automatically changed to FW_NORMAL.
        /// 
        /// This member is present only if the style member specifies DS_SETFONT or DS_SHELLFONT.
        /// </summary>
        public ushort Weight;

        /// <summary>
        /// Indicates whether the font is italic. If this value is TRUE, the font is italic.
        /// 
        /// This member is present only if the style member specifies DS_SETFONT or DS_SHELLFONT.
        /// </summary>
        public byte Italic;

        /// <summary>
        /// The character set to be used. For more information, see the lfcharset member of LOGFONT.
        /// 
        /// This member is present only if the style member specifies DS_SETFONT or DS_SHELLFONT.
        /// </summary>
        public byte CharSet;

        /// <summary>
        /// The name of the typeface for the font.
        /// 
        /// This member is present only if the style member specifies DS_SETFONT or DS_SHELLFONT.
        /// </summary>
        public string Typeface;
    }
}
