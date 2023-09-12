using System.Text;

namespace BinaryObjectScanner.Wrappers
{
    public interface IWrapper
    {
        /// <summary>
        /// Get a human-readable description of the wrapper
        /// </summary>
        string Description();

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        StringBuilder PrettyPrint();
        
#if NET6_0_OR_GREATER
        /// <summary>
        /// Export the item information as JSON
        /// </summary>
        string ExportJSON();
#endif
    }
}