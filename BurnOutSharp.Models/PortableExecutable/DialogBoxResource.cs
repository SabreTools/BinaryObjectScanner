namespace BurnOutSharp.Models.PortableExecutable
{
    /// <summary>
    /// A dialog box is also one resource entry in the resource file. It consists of one
    /// DLGTEMPLATE dialog box header structure plus one DLGITEMTEMPLATE structure for each
    /// control in the dialog box. The DLGTEMPLATEEX and the DLGITEMTEMPLATEEX structures
    /// describe the format of extended dialog box resources.
    /// </summary>
    /// <see href="https://learn.microsoft.com/en-us/windows/win32/menurc/resource-file-formats"/>
    public sealed class DialogBoxResource
    {
        #region Dialog template

        /// <summary>
        /// Dialog box header structure
        /// </summary>
        public DialogTemplate DialogTemplate;

        /// <summary>
        /// Dialog box extended header structure
        /// </summary>
        public DialogTemplateExtended ExtendedDialogTemplate;

        #endregion

        #region Dialog item templates

        /// <summary>
        /// Following the DLGTEMPLATE header in a standard dialog box template are one or more
        /// DLGITEMTEMPLATE structures that define the dimensions and style of the controls in the dialog
        /// box. The cdit member specifies the number of DLGITEMTEMPLATE structures in the template.
        /// These DLGITEMTEMPLATE structures must be aligned on DWORD boundaries.
        /// </summary>
        public DialogItemTemplate[] DialogItemTemplates;

        /// <summary>
        /// Following the DLGTEMPLATEEX header in an extended dialog box template is one or more
        /// DLGITEMTEMPLATEEX structures that describe the controls of the dialog box. The cDlgItems
        /// member of the DLGITEMTEMPLATEEX structure specifies the number of DLGITEMTEMPLATEEX
        /// structures that follow in the template.
        /// </summary>
        public DialogItemTemplateExtended[] ExtendedDialogItemTemplates;

        #endregion
    }
}
