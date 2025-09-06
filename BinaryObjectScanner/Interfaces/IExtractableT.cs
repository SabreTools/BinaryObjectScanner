using SabreTools.Serialization.Interfaces;

namespace BinaryObjectScanner.Interfaces
{
    /// <summary>
    /// Mark a file type as being able to be extracted
    /// </summary>
    public interface IExtractable<T> : IExtractable where T : IWrapper { }
}
