using SabreTools.Serialization.Interfaces;

namespace BinaryObjectScanner.Interfaces
{
    /// <summary>
    /// Mark a file type as being able to be detected
    /// </summary>
    public interface IDetectable<T> : IDetectable where T : IWrapper { }
}
