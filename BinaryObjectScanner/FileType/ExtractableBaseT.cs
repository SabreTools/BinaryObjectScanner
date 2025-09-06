using System;
using BinaryObjectScanner.Interfaces;
using SabreTools.Serialization.Interfaces;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// Base class for all standard extractable types with a wrapper
    /// </summary>
    public abstract class ExtractableBase<T> : ExtractableBase, IExtractable<T> where T : IWrapper
    {
        #region Protected Instance Variables

        /// <summary>
        /// Wrapper representing the extractable
        /// </summary>
        protected T _wrapper { get; private set; }

        #endregion

        #region Constructors

        public ExtractableBase(T? wrapper)
        {
            if (wrapper == null)
                throw new ArgumentNullException(nameof(wrapper));

            _wrapper = wrapper;
        }

        #endregion
    }
}