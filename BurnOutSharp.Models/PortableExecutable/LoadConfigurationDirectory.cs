namespace BurnOutSharp.Models.PortableExecutable
{
    /// <summary>
    /// The data directory entry for a pre-reserved SEH load configuration
    /// structure must specify a particular size of the load configuration
    /// structure because the operating system loader always expects it to
    /// be a certain value. In that regard, the size is really only a
    /// version check. For compatibility with Windows XP and earlier versions
    /// of Windows, the size must be 64 for x86 images.
    /// </summary>
    /// <see href="https://learn.microsoft.com/en-us/windows/win32/debug/pe-format"/>
    public class LoadConfigurationDirectory
    {
        /// <summary>
        /// Flags that indicate attributes of the file, currently unused.
        /// </summary>
        public uint Characteristics;

        /// <summary>
        /// Date and time stamp value. The value is represented in the number of
        /// seconds that have elapsed since midnight (00:00:00), January 1, 1970,
        /// Universal Coordinated Time, according to the system clock. The time
        /// stamp can be printed by using the C runtime (CRT) time function.
        /// </summary>
        public uint TimeDateStamp;

        /// <summary>
        /// Major version number.
        /// </summary>
        public ushort MajorVersion;

        /// <summary>
        /// Minor version number.
        /// </summary>
        public ushort MinorVersion;

        /// <summary>
        /// The global loader flags to clear for this process as the loader starts
        /// the process. 
        /// </summary>
        public uint GlobalFlagsClear;

        /// <summary>
        /// The global loader flags to set for this process as the loader starts
        /// the process.
        /// </summary>
        public uint GlobalFlagsSet;

        /// <summary>
        /// The default timeout value to use for this process's critical sections
        /// that are abandoned.
        /// </summary>
        public uint CriticalSectionDefaultTimeout;

        #region DeCommitFreeBlockThreshold

        /// <summary>
        /// Memory that must be freed before it is returned to the system, in bytes.
        /// </summary>
        public uint DeCommitFreeBlockThreshold_PE32;

        /// <summary>
        /// Memory that must be freed before it is returned to the system, in bytes.
        /// </summary>
        public ulong DeCommitFreeBlockThreshold_PE32Plus;

        #endregion

        #region DeCommitTotalFreeThreshold

        /// <summary>
        /// Total amount of free memory, in bytes.
        /// </summary>
        public uint DeCommitTotalFreeThreshold_PE32;

        /// <summary>
        /// Total amount of free memory, in bytes.
        /// </summary>
        public ulong DeCommitTotalFreeThreshold_PE32Plus;

        #endregion

        #region LockPrefixTable

        /// <summary>
        /// [x86 only] The VA of a list of addresses where the LOCK prefix is used so
        /// that they can be replaced with NOP on single processor machines.
        /// </summary>
        public uint LockPrefixTable_PE32;

        /// <summary>
        /// [x86 only] The VA of a list of addresses where the LOCK prefix is used so
        /// that they can be replaced with NOP on single processor machines.
        /// </summary>
        public ulong LockPrefixTable_PE32Plus;

        #endregion

        #region MaximumAllocationSize

        /// <summary>
        /// Maximum allocation size, in bytes.
        /// </summary>
        public uint MaximumAllocationSize_PE32;

        /// <summary>
        /// Maximum allocation size, in bytes.
        /// </summary>
        public ulong MaximumAllocationSize_PE32Plus;

        #endregion

        #region VirtualMemoryThreshold

        /// <summary>
        /// Maximum virtual memory size, in bytes.
        /// </summary>
        public uint VirtualMemoryThreshold_PE32;

        /// <summary>
        /// Maximum virtual memory size, in bytes.
        /// </summary>
        public ulong VirtualMemoryThreshold_PE32Plus;

        #endregion

        #region ProcessAffinityMask

        /// <summary>
        /// Setting this field to a non-zero value is equivalent to calling
        /// SetProcessAffinityMask with this value during process startup (.exe only)
        /// </summary>
        public uint ProcessAffinityMask_PE32;

        /// <summary>
        /// Setting this field to a non-zero value is equivalent to calling
        /// SetProcessAffinityMask with this value during process startup (.exe only)
        /// </summary>
        public ulong ProcessAffinityMask_PE32Plus;

        #endregion

        /// <summary>
        /// Process heap flags that correspond to the first argument of the
        /// HeapCreate function. These flags apply to the process heap that
        /// is created during process startup.
        /// </summary>
        public uint ProcessHeapFlags;

        /// <summary>
        /// The service pack version identifier.
        /// </summary>
        public ushort CSDVersion;

        /// <summary>
        /// Must be zero.
        /// </summary>
        public ushort Reserved;

        #region EditList

        /// <summary>
        /// Reserved for use by the system.
        /// </summary>
        public uint EditList_PE32;

        /// <summary>
        /// Reserved for use by the system.
        /// </summary>
        public ulong EditList_PE32Plus;

        #endregion

        #region SecurityCookie

        /// <summary>
        /// A pointer to a cookie that is used by Visual C++ or GS implementation.
        /// </summary>
        public uint SecurityCookie_PE32;

        /// <summary>
        /// A pointer to a cookie that is used by Visual C++ or GS implementation.
        /// </summary>
        public ulong SecurityCookie_PE32Plus;

        #endregion

        #region SEHandlerTable

        /// <summary>
        /// [x86 only] The VA of the sorted table of RVAs of each valid, unique
        /// SE handler in the image.
        /// </summary>
        public uint SEHandlerTable_PE32;

        /// <summary>
        /// [x86 only] The VA of the sorted table of RVAs of each valid, unique
        /// SE handler in the image.
        /// </summary>
        public ulong SEHandlerTable_PE32Plus;

        #endregion

        #region SEHandlerCount

        /// <summary>
        /// [x86 only] The count of unique handlers in the table.
        /// </summary>
        public uint SEHandlerCount_PE32;

        /// <summary>
        /// [x86 only] The count of unique handlers in the table.
        /// </summary>
        public ulong SEHandlerCount_PE32Plus;

        #endregion

        #region GuardCFCheckFunctionPointer

        /// <summary>
        /// The VA where Control Flow Guard check-function pointer is stored.
        /// </summary>
        public uint GuardCFCheckFunctionPointer_PE32;

        /// <summary>
        /// The VA where Control Flow Guard check-function pointer is stored.
        /// </summary>
        public ulong GuardCFCheckFunctionPointer_PE32Plus;

        #endregion

        #region GuardCFDispatchFunctionPointer

        /// <summary>
        /// The VA where Control Flow Guard dispatch-function pointer is stored.
        /// </summary>
        public uint GuardCFDispatchFunctionPointer_PE32;

        /// <summary>
        /// The VA where Control Flow Guard dispatch-function pointer is stored.
        /// </summary>
        public ulong GuardCFDispatchFunctionPointer_PE32Plus;

        #endregion

        #region GuardCFFunctionTable

        /// <summary>
        /// The VA of the sorted table of RVAs of each Control Flow Guard
        /// function in the image.
        /// </summary>
        public uint GuardCFFunctionTable_PE32;

        /// <summary>
        /// The VA of the sorted table of RVAs of each Control Flow Guard
        /// function in the image.
        /// </summary>
        public ulong GuardCFFunctionTable_PE32Plus;

        #endregion

        #region GuardCFFunctionCount

        /// <summary>
        /// The count of unique RVAs in the above table.
        /// </summary>
        public uint GuardCFFunctionCount_PE32;

        /// <summary>
        /// The count of unique RVAs in the above table.
        /// </summary>
        public ulong GuardCFFunctionCount_PE32Plus;

        #endregion

        /// <summary>
        /// Control Flow Guard related flags.
        /// </summary>
        public GuardFlags GuardFlags;

        /// <summary>
        /// Code integrity information.
        /// </summary>
        /// <remarks>12 bytes</remarks>
        public byte[] CodeIntegrity;

        #region GuardAddressTakenIatEntryTable

        /// <summary>
        /// The VA where Control Flow Guard address taken IAT table is stored.
        /// </summary>
        public uint GuardAddressTakenIatEntryTable_PE32;

        /// <summary>
        /// The VA where Control Flow Guard address taken IAT table is stored.
        /// </summary>
        public ulong GuardAddressTakenIatEntryTable_PE32Plus;

        #endregion

        #region GuardAddressTakenIatEntryCount

        /// <summary>
        /// The count of unique RVAs in the above table.
        /// </summary>
        public uint GuardAddressTakenIatEntryCount_PE32;

        /// <summary>
        /// The count of unique RVAs in the above table.
        /// </summary>
        public ulong GuardAddressTakenIatEntryCount_PE32Plus;

        #endregion

        #region GuardLongJumpTargetTable

        /// <summary>
        /// The VA where Control Flow Guard long jump target table is stored.
        /// </summary>
        public uint GuardLongJumpTargetTable_PE32;

        /// <summary>
        /// The VA where Control Flow Guard long jump target table is stored.
        /// </summary>
        public ulong GuardLongJumpTargetTable_PE32Plus;

        #endregion

        #region GuardLongJumpTargetCount

        /// <summary>
        /// The count of unique RVAs in the above table.
        /// </summary>
        public uint GuardLongJumpTargetCount_PE32;

        /// <summary>
        /// The count of unique RVAs in the above table.
        /// </summary>
        public ulong GuardLongJumpTargetCount_PE32Plus;

        #endregion
    }
}
