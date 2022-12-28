namespace BurnOutSharp.Models.Compression.Quantum
{
    public enum SelectorModel
    {
        /// <summary>
        /// Literal model, 64 entries, start at symbol 0
        /// </summary>
        SELECTOR_0 = 0,

        /// <summary>
        /// Literal model, 64 entries, start at symbol 64
        /// </summary>
        SELECTOR_1 = 1,

        /// <summary>
        /// Literal model, 64 entries, start at symbol 128
        /// </summary>
        SELECTOR_2 = 2,

        /// <summary>
        /// Literal model, 64 entries, start at symbol 192
        /// </summary>
        SELECTOR_3 = 3,

        /// <summary>
        /// LZ model, 3 character matches, max 24 entries, start at symbol 0
        /// </summary>
        SELECTOR_4 = 4,

        /// <summary>
        /// LZ model, 4 character matches, max 36 entries, start at symbol 0
        /// </summary>
        SELECTOR_5 = 5,

        /// <summary>
        /// LZ model, 5+ character matches, max 42 entries, start at symbol 0
        /// </summary>
        SELECTOR_6_POSITION = 6,

        /// <summary>
        /// LZ model, 5+ character matches, 27 entries, start at symbol 0
        /// </summary>
        SELECTOR_6_LENGTH = 7,
    }
}