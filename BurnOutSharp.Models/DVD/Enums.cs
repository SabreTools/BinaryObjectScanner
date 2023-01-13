using System;

namespace BurnOutSharp.Models.DVD
{
    [Flags]
    public enum ProgramChainCategory : byte
    {
        MenuTypeTitle = 0x02,
        Entry = 0x80,
    }

    [Flags]
    public enum TitleType : byte
    {
        /// <summary>
        /// Uop0 Time play or search
        /// </summary>
        Uop0TimePlayOrSearch = 0x01,

        /// <summary>
        /// Uop1 PTT play or search
        /// </summary>
        Uop1PTTPlayOrSearch = 0x02,

        /// <summary>
        /// Jump/Link/Call commands - exist
        /// </summary>
        JumpLinkCallExist = 0x04,

        /// <summary>
        /// Jump/Link/Call commands - button
        /// </summary>
        JumpLinkCallButton = 0x08,

        /// <summary>
        /// Jump/Link/Call commands - pre/post
        /// </summary>
        JumpLinkCallPrePost = 0x10,

        /// <summary>
        /// Jump/Link/Call commands - cell
        /// </summary>
        JumpLinkCallCell = 0x20,

        /// <summary>
        /// 0=one_sequential_pgc
        /// 1=not one_sequential (random, shuffle, stills, loops, or more than one pgc)
        /// </summary>
        ComplexPGC = 0x40,

        /// <summary>
        /// Reserved
        /// </summary>
        Reserved = 0x80,
    }
}