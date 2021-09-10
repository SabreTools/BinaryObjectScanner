namespace BurnOutSharp.ExecutableType.Microsoft.Entries
{
    /// <summary>
    /// Each entry in the export address table is a field that uses one of two formats in the following table.
    /// If the address specified is not within the export section (as defined by the address and length that are indicated in the optional header), the field is an export RVA, which is an actual address in code or data.
    /// Otherwise, the field is a forwarder RVA, which names a symbol in another DLL.
    /// </summary>
    /// <remarks>https://docs.microsoft.com/en-us/windows/win32/debug/pe-format#the-pdata-section</remarks>
    public class FunctionTableEntry
    {
        #region 32-bit MIPS

        /// <summary>
        /// The VA of the corresponding function.
        /// </summary>
        public uint MIPSBeginAddress;

        /// <summary>
        /// The VA of the end of the function.
        /// </summary>
        public uint MIPSEndAddress;

        /// <summary>
        /// The pointer to the exception handler to be executed.
        /// </summary>
        public uint MIPSExceptionHandler;

        /// <summary>
        /// The pointer to additional information to be passed to the handler.
        /// </summary>
        public uint MIPSHandlerData;

        /// <summary>
        /// The VA of the end of the function's prolog.
        /// </summary>
        public uint MIPSPrologEndAddress;

        #endregion

        #region ARM, PowerPC, SH3 and SH4 Windows CE

        /// <summary>
        /// The VA of the corresponding function.
        /// </summary>
        public uint ARMBeginAddress;

        /// <summary>
        /// The VA of the end of the function.
        /// 
        /// 8 bits      Prolog Length       The number of instructions in the function's prolog.
        /// 22 bits     Function Length     The number of instructions in the function.
        /// 1 bit       32-bit Flag         If set, the function consists of 32-bit instructions. If clear, the function consists of 16-bit instructions.
        /// 1 bit       Exception Flag      If set, an exception handler exists for the function. Otherwise, no exception handler exists.
        /// </summary>
        public uint ARMLengthsAndFlags;

        #endregion

        #region x64 and Itanium

        /// <summary>
        /// The RVA of the corresponding function.
        /// </summary>
        public uint X64BeginAddress;

        /// <summary>
        /// The RVA of the end of the function.
        /// </summary>
        public uint X64EndAddress;

        /// <summary>
        /// The RVA of the unwind information.
        /// </summary>
        public uint X64UnwindInformation;

        #endregion
    }
}