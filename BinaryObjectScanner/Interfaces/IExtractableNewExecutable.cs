﻿using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Interfaces
{
    /// <summary>
    /// Mark a NewExecutable type as being able to be extracted
    /// </summary>
    public interface IExtractableNewExecutable
    {
        /// <summary>
        /// Extract a NewExecutable to a temporary path, if possible
        /// </summary>
        /// <param name="file">Path to the input file</param>
        /// <param name="nex">NewExecutable representing the read-in file</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns>Path to extracted files, null on error</returns>
        string? Extract(string file, NewExecutable nex, bool includeDebug);
    }
}
