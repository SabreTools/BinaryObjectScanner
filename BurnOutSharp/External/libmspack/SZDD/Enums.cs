/* This file is part of libmspack.
 * (C) 2003-2018 Stuart Caie.
 *
 * libmspack is free software; you can redistribute it and/or modify it under
 * the terms of the GNU Lesser General Public License (LGPL) version 2.1
 *
 * For further details, see the file COPYING.LIB distributed with libmspack
 */

namespace LibMSPackSharp.SZDD
{
    public enum Format
    {
        /// <summary>
        /// a regular SZDD file
        /// </summary>
        MSSZDD_FMT_NORMAL = 0,

        /// <summary>
        /// a special QBasic SZDD file
        /// </summary>
        MSSZDD_FMT_QBASIC = 1,
    }

    public enum Parameters
    {
        /// <summary>
        /// The missing character
        /// </summary>
        MSSZDDC_PARAM_MISSINGCHAR = 0,
    }
}
