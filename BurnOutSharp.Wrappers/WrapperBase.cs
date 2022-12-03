using System;
using System.IO;
using static BurnOutSharp.Builder.Extensions;

namespace BurnOutSharp.Wrappers
{
    public abstract class WrapperBase
    {
        #region Instance Variabled

        /// <summary>
        /// Source of the original data
        /// </summary>
        protected DataSource _dataSource = DataSource.UNKNOWN;

        /// <summary>
        /// Source byte array data
        /// </summary>
        /// <remarks>This is only populated if <see cref="_dataSource"/> is <see cref="DataSource.ByteArray"/></remarks>
        protected byte[] _byteArrayData = null;

        /// <summary>
        /// Source byte array data offset
        /// </summary>
        /// <remarks>This is only populated if <see cref="_dataSource"/> is <see cref="DataSource.ByteArray"/></remarks>
        protected int _byteArrayOffset = -1;

        /// <summary>
        /// Source Stream data
        /// </summary>
        /// <remarks>This is only populated if <see cref="_dataSource"/> is <see cref="DataSource.Stream"/></remarks>
        protected Stream _streamData = null;

        #endregion

        #region Data

        /// <summary>
        /// Validate the backing data source
        /// </summary>
        /// <returns>True if the data source is valid, false otherwise</returns>
        protected bool DataSourceIsValid()
        {
            switch (_dataSource)
            {
                // Byte array data requires both a valid array and offset
                case DataSource.ByteArray:
                    return _byteArrayData != null && _byteArrayOffset >= 0;

                // Stream data requires both a valid stream
                case DataSource.Stream:
                    return _streamData != null && _streamData.CanRead && _streamData.CanSeek;
                
                // Everything else is invalid
                case DataSource.UNKNOWN:
                default:
                    return false;
            }
        }

        /// <summary>
        /// Check if a data segment is valid in the data source 
        /// </summary>
        /// <param name="position">Position in the source</param>
        /// <param name="length">Length of the data to check</param>
        /// <returns>True if the positional data is valid, false otherwise</returns>
        protected bool SegmentValid(int position, int length)
        {
            // Validate the data souece
            if (!DataSourceIsValid())
                return false;

            switch (_dataSource)
            {
                case DataSource.ByteArray:
                    return _byteArrayOffset + position + length < _byteArrayData.Length;

                case DataSource.Stream:
                    return position + length < _streamData.Length;

                // Everything else is invalid
                case DataSource.UNKNOWN:
                default:
                    return false;
            }
        }

        /// <summary>
        /// Read data from the source
        /// </summary>
        /// <param name="position">Position in the source to read from</param>
        /// <param name="length">Length of the requested data</param>
        /// <returns>Byte array containing the requested data, null on error</returns>
        protected byte[] ReadFromDataSource(int position, int length)
        {
            // Validate the data souece
            if (!DataSourceIsValid())
                return null;

            // Validate the requested segment
            if (!SegmentValid(position, length))
                return null;

            // Read and retuen the data
            byte[] sectionData = null;
            switch (_dataSource)
            {
                case DataSource.ByteArray:
                    sectionData = new byte[length];
                    Array.Copy(_byteArrayData, _byteArrayOffset + position, sectionData, 0, length);
                    break;

                case DataSource.Stream:
                    long currentLocation = _streamData.Position;
                    _streamData.Seek(position, SeekOrigin.Begin);
                    sectionData = _streamData.ReadBytes(length);
                    _streamData.Seek(currentLocation, SeekOrigin.Begin);
                    break;
            }

            return sectionData;
        }

        #endregion

        #region Printing

        /// <summary>
        /// Pretty print the Executable information
        /// </summary>
        public abstract void Print();

        #endregion
    }
}