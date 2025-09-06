using System;
using BinaryObjectScanner.Interfaces;
using SabreTools.Serialization.Interfaces;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// Base class for all standard detectable types with a wrapper
    /// </summary>
    public abstract class DetectableBase<T> : DetectableBase, IDetectable<T> where T : IWrapper
    {
        #region Protected Instance Variables

        /// <summary>
        /// Wrapper representing the detectable
        /// </summary>
        protected readonly T _wrapper;

        #endregion

        #region Constructors

        public DetectableBase(T? wrapper)
        {
            if (wrapper == null)
                throw new ArgumentNullException(nameof(wrapper));

            _wrapper = wrapper;
        }

        #endregion
    }
}