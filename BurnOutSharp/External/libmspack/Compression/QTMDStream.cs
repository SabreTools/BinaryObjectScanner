/* This file is part of libmspack.
 * (C) 2003-2004 Stuart Caie.
 *
 * The Quantum method was created by David Stafford, adapted by Microsoft
 * Corporation.
 *
 * libmspack is free software; you can redistribute it and/or modify it under
 * the terms of the GNU Lesser General Public License (LGPL) version 2.1
 *
 * For further details, see the file COPYING.LIB distributed with libmspack
 */

namespace LibMSPackSharp.Compression
{
    public class QTMDStream : CompressionStream
    {
        /// <summary>
        /// Decoding window
        /// </summary>
        public byte[] Window { get; set; }

        /// <summary>
        /// Window size
        /// </summary>
        public uint WindowSize { get; set; }

        /// <summary>
        /// Decompression offset within window
        /// </summary>
        public uint WindowPosition { get; set; }

        /// <summary>
        /// Bytes remaining for current frame
        /// </summary>
        public uint FrameTODO { get; set; }

        /// <summary>
        /// High: arith coding state
        /// </summary>
        public ushort High { get; set; }

        /// <summary>
        /// Low: arith coding state
        /// </summary>
        public ushort Low { get; set; }

        /// <summary>
        /// Current: arith coding state
        /// </summary>
        public ushort Current { get; set; }

        /// <summary>
        /// Have we started decoding a new frame?
        /// </summary>
        public byte HeaderRead { get; set; }

        // Four literal models, each representing 64 symbols

        /// <summary>
        /// For literals from   0 to  63 (selector = 0)
        /// </summary>
        public QTMDModel Model0 { get; set; }

        /// <summary>
        /// For literals from  64 to 127 (selector = 1)
        /// </summary>
        public QTMDModel Model1 { get; set; }

        /// <summary>
        /// For literals from 128 to 191 (selector = 2)
        /// </summary>
        public QTMDModel Model2 { get; set; }

        /// <summary>
        /// For literals from 129 to 255 (selector = 3)
        /// </summary>
        public QTMDModel Model3 { get; set; }

        // Three match models.

        /// <summary>
        /// For match with fixed length of 3 bytes
        /// </summary>
        public QTMDModel Model4 { get; set; }

        /// <summary>
        /// For match with fixed length of 4 bytes
        /// </summary>
        public QTMDModel Model5 { get; set; }

        /// <summary>
        /// For variable length match, encoded with model6len model
        /// </summary>
        public QTMDModel Model6 { get; set; }

        public QTMDModel Model6Len { get; set; }

        /// <summary>
        /// Selector model. 0-6 to say literal (0,1,2,3) or match (4,5,6)
        /// </summary>
        public QTMDModel Model7 { get; set; }

        // Symbol arrays for all models

        public QTMDModelSym[] Model0Symbols { get; set; } = new QTMDModelSym[64 + 1];

        public QTMDModelSym[] Model1Symbols { get; set; } = new QTMDModelSym[64 + 1];

        public QTMDModelSym[] Model2Symbols { get; set; } = new QTMDModelSym[64 + 1];

        public QTMDModelSym[] Model3Symbols { get; set; } = new QTMDModelSym[64 + 1];

        public QTMDModelSym[] Model4Symbols { get; set; } = new QTMDModelSym[24 + 1];

        public QTMDModelSym[] Model5Symbols { get; set; } = new QTMDModelSym[36 + 1];

        public QTMDModelSym[] Model6Symbols { get; set; } = new QTMDModelSym[42 + 1];

        public QTMDModelSym[] Model6LenSymbols { get; set; } = new QTMDModelSym[27 + 1];

        public QTMDModelSym[] Model7Symbols { get; set; } = new QTMDModelSym[7 + 1];

        public override int HUFF_ERROR() => (int)Error.MSPACK_ERR_OK;
    }
}
