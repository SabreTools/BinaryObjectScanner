namespace BurnOutSharp.Models.Compression.Quantum
{
    public static class Constants
    {
        /// <summary>
        /// Mask for Quantum Compression Level
        /// </summary>
        public const ushort MASK_QUANTUM_LEVEL = 0x00F0;

        /// <summary>
        /// Lowest Quantum Level (1)
        /// </summary>
        public const ushort QUANTUM_LEVEL_LO = 0x0010;

        /// <summary>
        /// Highest Quantum Level (7)
        /// </summary>
        public const ushort QUANTUM_LEVEL_HI = 0x0070;

        /// <summary>
        /// Amount to shift over to get int
        /// </summary>
        public const ushort SHIFT_QUANTUM_LEVEL = 4;

        /// <summary>
        /// Mask for Quantum Compression Memory
        /// </summary>
        public const ushort MASK_QUANTUM_MEM = 0x1F00;

        /// <summary>
        /// Lowest Quantum Memory (10)
        /// </summary>
        public const ushort QUANTUM_MEM_LO = 0x0A00;

        /// <summary>
        /// Highest Quantum Memory (21)
        /// </summary>
        public const ushort QUANTUM_MEM_HI = 0x1500;

        /// <summary>
        /// Amount to shift over to get int
        /// </summary>
        public const ushort SHIFT_QUANTUM_MEM = 8;
    }
}