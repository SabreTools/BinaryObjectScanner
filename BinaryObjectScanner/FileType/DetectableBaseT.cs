using BinaryObjectScanner.Interfaces;
using SabreTools.Serialization.Interfaces;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// Base class for all standard detectable types with a wrapper
    /// </summary>
    public abstract class DetectableBase<T> : DetectableBase, IDetectable<T> where T : IWrapper
    {
        
    }
}