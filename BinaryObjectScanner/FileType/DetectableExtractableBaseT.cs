using System;
using BinaryObjectScanner.Interfaces;
using SabreTools.Serialization.Interfaces;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// Base class for all standard detectable/extractable types with a wrapper
    /// </summary>
    public abstract class DetectableExtractableBase<T> : DetectableExtractableBase, IDetectable<T>, IExtractable<T>
        where T : IWrapper
    {
        #region Protected Instance Variables

        /// <summary>
        /// Wrapper representing the detectable/extractable
        /// </summary>
        protected T _wrapper { get; private set; }

        #endregion

        #region Constructors

        public DetectableExtractableBase(T? wrapper)
        {
            if (wrapper == null)
                throw new ArgumentNullException(nameof(wrapper));

            _wrapper = wrapper;
        }

        #endregion
    }
}