/* This file is part of libmspack.
 * (C) 2003-2018 Stuart Caie.
 *
 * libmspack is free software; you can redistribute it and/or modify it under
 * the terms of the GNU Lesser General Public License (LGPL) version 2.1
 *
 * For further details, see the file COPYING.LIB distributed with libmspack
 */

namespace LibMSPackSharp.CAB
{
    /// <summary>
    /// There is one of these for every cabinet a folder spans
    /// </summary>
    public class FolderData
    {
        public FolderData Next { get; set; }

        /// <summary>
        /// Cabinet file of this folder span
        /// </summary>
        public Cabinet Cab { get; set; }

        /// <summary>
        /// Cabinet offset of first datablock
        /// </summary>
        public long Offset { get; set; }
    };
}
