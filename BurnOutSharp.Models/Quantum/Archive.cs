namespace BurnOutSharp.Models.Quantum
{
    /// <summary>
    /// Quantum archive file structure
    /// </summary>
    /// <see href="https://handwiki.org/wiki/Software:Quantum_compression"/>
    public class Archive
    {
        /// <summary>
        /// Quantum header
        /// </summary>
        public Header Header { get; set; }

        /// <summary>
        /// This is immediately followed by the list of files
        /// </summary>
        public FileDescriptor[] FileList { get; set; }

        // Immediately following the list of files is the compressed data. 
    }
}