/* A Bison parser, made by GNU Bison 3.8.2.  */

/* Bison implementation for Yacc-like parsers in C

   Copyright (C) 1984, 1989-1990, 2000-2015, 2018-2021 Free Software Foundation,
   Inc.

   This program is free software: you can redistribute it and/or modify
   it under the terms of the GNU General Public License as published by
   the Free Software Foundation, either version 3 of the License, or
   (at your option) any later version.

   This program is distributed in the hope that it will be useful,
   but WITHOUT ANY WARRANTY; without even the implied warranty of
   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
   GNU General Public License for more details.

   You should have received a copy of the GNU General Public License
   along with this program.  If not, see <https://www.gnu.org/licenses/>.  */

/* As a special exception, you may create a larger work that contains
   part or all of the Bison parser skeleton and distribute that work
   under terms of your choice, so long as that work isn't itself a
   parser generator using the skeleton or a modified version thereof
   as a parser skeleton.  Alternatively, if you modify or redistribute
   the parser skeleton itself, you may (at your option) remove this
   special exception, which will cause the skeleton and the resulting
   Bison output files to be licensed under the GNU General Public
   License without this special exception.

   This special exception was added by the Free Software Foundation in
   version 2.2 of Bison.  */

/* C LALR(1) parser skeleton written by Richard Stallman, by
   simplifying the original so-called "semantic" parser.  */

/* DO NOT RELY ON FEATURES THAT ARE NOT DOCUMENTED in the manual,
   especially those whose name start with YY_ or yy_.  They are
   private implementation details that can be changed or removed.  */

/* All symbols defined below should begin with yy or YY, to avoid
   infringing on user name space.  This should be done even for local
   variables, as they might otherwise be expanded by user macros.
   There are some unavoidable exceptions within include files to
   define necessary library symbols; they are noted "INFRINGES ON
   USER NAME SPACE" below.  */

/*
 * Implementation of the Microsoft Installer (msi.dll)
 *
 * Copyright 2002-2004 Mike McCormack for CodeWeavers
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301, USA
 */

using System;
using System.Collections.Generic;
using LibMSI.Views;
using static LibMSI.Internal.MsiPriv;

namespace LibMSI.Internal
{
    #region Generated Enums

    /// <summary>
    /// Token kinds.
    /// </summary>
    internal enum sql_tokentype
    {
        SQL_EMPTY = -2,
        SQL_EOF = 0,                   /* "end of file"  */
        SQL_error = 256,               /* error  */
        SQL_UNDEF = 257,               /* "invalid token"  */
        TK_ALTER = 258,                /* TK_ALTER  */
        TK_AND = 259,                  /* TK_AND  */
        TK_BY = 260,                   /* TK_BY  */
        TK_CHAR = 261,                 /* TK_CHAR  */
        TK_COMMA = 262,                /* TK_COMMA  */
        TK_CREATE = 263,               /* TK_CREATE  */
        TK_DELETE = 264,               /* TK_DELETE  */
        TK_DROP = 265,                 /* TK_DROP  */
        TK_DISTINCT = 266,             /* TK_DISTINCT  */
        TK_DOT = 267,                  /* TK_DOT  */
        TK_EQ = 268,                   /* TK_EQ  */
        TK_FREE = 269,                 /* TK_FREE  */
        TK_FROM = 270,                 /* TK_FROM  */
        TK_GE = 271,                   /* TK_GE  */
        TK_GT = 272,                   /* TK_GT  */
        TK_HOLD = 273,                 /* TK_HOLD  */
        TK_ADD = 274,                  /* TK_ADD  */
        TK_ID = 275,                   /* TK_ID  */
        TK_ILLEGAL = 276,              /* TK_ILLEGAL  */
        TK_INSERT = 277,               /* TK_INSERT  */
        TK_INT = 278,                  /* TK_INT  */
        TK_INTEGER = 279,              /* TK_INTEGER  */
        TK_INTO = 280,                 /* TK_INTO  */
        TK_IS = 281,                   /* TK_IS  */
        TK_KEY = 282,                  /* TK_KEY  */
        TK_LE = 283,                   /* TK_LE  */
        TK_LONG = 284,                 /* TK_LONG  */
        TK_LONGCHAR = 285,             /* TK_LONGCHAR  */
        TK_LP = 286,                   /* TK_LP  */
        TK_LT = 287,                   /* TK_LT  */
        TK_LOCALIZABLE = 288,          /* TK_LOCALIZABLE  */
        TK_MINUS = 289,                /* TK_MINUS  */
        TK_NE = 290,                   /* TK_NE  */
        TK_NOT = 291,                  /* TK_NOT  */
        TK_NULL = 292,                 /* TK_NULL  */
        TK_OBJECT = 293,               /* TK_OBJECT  */
        TK_OR = 294,                   /* TK_OR  */
        TK_ORDER = 295,                /* TK_ORDER  */
        TK_PRIMARY = 296,              /* TK_PRIMARY  */
        TK_RP = 297,                   /* TK_RP  */
        TK_SELECT = 298,               /* TK_SELECT  */
        TK_SET = 299,                  /* TK_SET  */
        TK_SHORT = 300,                /* TK_SHORT  */
        TK_SPACE = 301,                /* TK_SPACE  */
        TK_STAR = 302,                 /* TK_STAR  */
        TK_STRING = 303,               /* TK_STRING  */
        TK_TABLE = 304,                /* TK_TABLE  */
        TK_TEMPORARY = 305,            /* TK_TEMPORARY  */
        TK_UPDATE = 306,               /* TK_UPDATE  */
        TK_VALUES = 307,               /* TK_VALUES  */
        TK_WHERE = 308,                /* TK_WHERE  */
        TK_WILDCARD = 309,             /* TK_WILDCARD  */
        END_OF_FILE = 310,             /* END_OF_FILE  */
        ILLEGAL = 311,                 /* ILLEGAL  */
        SPACE = 312,                   /* SPACE  */
        UNCLOSED_STRING = 313,         /* UNCLOSED_STRING  */
        COMMENT = 314,                 /* COMMENT  */
        FUNCTION = 315,                /* FUNCTION  */
        COLUMN = 316,                  /* COLUMN  */
        TK_LIKE = 318,                 /* TK_LIKE  */
        TK_NEGATION = 319              /* TK_NEGATION  */
    };

    /// <summary>
    /// Symbol kind.
    /// </summary>
    internal enum yysymbol_kind_t
    {
        YYSYMBOL_YYEMPTY = -2,
        YYSYMBOL_YYEOF = 0,                      /* "end of file"  */
        YYSYMBOL_YYerror = 1,                    /* error  */
        YYSYMBOL_YYUNDEF = 2,                    /* "invalid token"  */
        YYSYMBOL_TK_ALTER = 3,                   /* TK_ALTER  */
        YYSYMBOL_TK_AND = 4,                     /* TK_AND  */
        YYSYMBOL_TK_BY = 5,                      /* TK_BY  */
        YYSYMBOL_TK_CHAR = 6,                    /* TK_CHAR  */
        YYSYMBOL_TK_COMMA = 7,                   /* TK_COMMA  */
        YYSYMBOL_TK_CREATE = 8,                  /* TK_CREATE  */
        YYSYMBOL_TK_DELETE = 9,                  /* TK_DELETE  */
        YYSYMBOL_TK_DROP = 10,                   /* TK_DROP  */
        YYSYMBOL_TK_DISTINCT = 11,               /* TK_DISTINCT  */
        YYSYMBOL_TK_DOT = 12,                    /* TK_DOT  */
        YYSYMBOL_TK_EQ = 13,                     /* TK_EQ  */
        YYSYMBOL_TK_FREE = 14,                   /* TK_FREE  */
        YYSYMBOL_TK_FROM = 15,                   /* TK_FROM  */
        YYSYMBOL_TK_GE = 16,                     /* TK_GE  */
        YYSYMBOL_TK_GT = 17,                     /* TK_GT  */
        YYSYMBOL_TK_HOLD = 18,                   /* TK_HOLD  */
        YYSYMBOL_TK_ADD = 19,                    /* TK_ADD  */
        YYSYMBOL_TK_ID = 20,                     /* TK_ID  */
        YYSYMBOL_TK_ILLEGAL = 21,                /* TK_ILLEGAL  */
        YYSYMBOL_TK_INSERT = 22,                 /* TK_INSERT  */
        YYSYMBOL_TK_INT = 23,                    /* TK_INT  */
        YYSYMBOL_TK_INTEGER = 24,                /* TK_INTEGER  */
        YYSYMBOL_TK_INTO = 25,                   /* TK_INTO  */
        YYSYMBOL_TK_IS = 26,                     /* TK_IS  */
        YYSYMBOL_TK_KEY = 27,                    /* TK_KEY  */
        YYSYMBOL_TK_LE = 28,                     /* TK_LE  */
        YYSYMBOL_TK_LONG = 29,                   /* TK_LONG  */
        YYSYMBOL_TK_LONGCHAR = 30,               /* TK_LONGCHAR  */
        YYSYMBOL_TK_LP = 31,                     /* TK_LP  */
        YYSYMBOL_TK_LT = 32,                     /* TK_LT  */
        YYSYMBOL_TK_LOCALIZABLE = 33,            /* TK_LOCALIZABLE  */
        YYSYMBOL_TK_MINUS = 34,                  /* TK_MINUS  */
        YYSYMBOL_TK_NE = 35,                     /* TK_NE  */
        YYSYMBOL_TK_NOT = 36,                    /* TK_NOT  */
        YYSYMBOL_TK_NULL = 37,                   /* TK_NULL  */
        YYSYMBOL_TK_OBJECT = 38,                 /* TK_OBJECT  */
        YYSYMBOL_TK_OR = 39,                     /* TK_OR  */
        YYSYMBOL_TK_ORDER = 40,                  /* TK_ORDER  */
        YYSYMBOL_TK_PRIMARY = 41,                /* TK_PRIMARY  */
        YYSYMBOL_TK_RP = 42,                     /* TK_RP  */
        YYSYMBOL_TK_SELECT = 43,                 /* TK_SELECT  */
        YYSYMBOL_TK_SET = 44,                    /* TK_SET  */
        YYSYMBOL_TK_SHORT = 45,                  /* TK_SHORT  */
        YYSYMBOL_TK_SPACE = 46,                  /* TK_SPACE  */
        YYSYMBOL_TK_STAR = 47,                   /* TK_STAR  */
        YYSYMBOL_TK_STRING = 48,                 /* TK_STRING  */
        YYSYMBOL_TK_TABLE = 49,                  /* TK_TABLE  */
        YYSYMBOL_TK_TEMPORARY = 50,              /* TK_TEMPORARY  */
        YYSYMBOL_TK_UPDATE = 51,                 /* TK_UPDATE  */
        YYSYMBOL_TK_VALUES = 52,                 /* TK_VALUES  */
        YYSYMBOL_TK_WHERE = 53,                  /* TK_WHERE  */
        YYSYMBOL_TK_WILDCARD = 54,               /* TK_WILDCARD  */
        YYSYMBOL_END_OF_FILE = 55,               /* END_OF_FILE  */
        YYSYMBOL_ILLEGAL = 56,                   /* ILLEGAL  */
        YYSYMBOL_SPACE = 57,                     /* SPACE  */
        YYSYMBOL_UNCLOSED_STRING = 58,           /* UNCLOSED_STRING  */
        YYSYMBOL_COMMENT = 59,                   /* COMMENT  */
        YYSYMBOL_FUNCTION = 60,                  /* FUNCTION  */
        YYSYMBOL_COLUMN = 61,                    /* COLUMN  */
        YYSYMBOL_62_AGG_FUNCTION_ = 62,          /* AGG_FUNCTION.  */
        YYSYMBOL_TK_LIKE = 63,                   /* TK_LIKE  */
        YYSYMBOL_TK_NEGATION = 64,               /* TK_NEGATION  */
        YYSYMBOL_YYACCEPT = 65,                  /* $accept  */
        YYSYMBOL_query = 66,                     /* query  */
        YYSYMBOL_onequery = 67,                  /* onequery  */
        YYSYMBOL_oneinsert = 68,                 /* oneinsert  */
        YYSYMBOL_onecreate = 69,                 /* onecreate  */
        YYSYMBOL_oneupdate = 70,                 /* oneupdate  */
        YYSYMBOL_onedelete = 71,                 /* onedelete  */
        YYSYMBOL_onealter = 72,                  /* onealter  */
        YYSYMBOL_alterop = 73,                   /* alterop  */
        YYSYMBOL_onedrop = 74,                   /* onedrop  */
        YYSYMBOL_table_def = 75,                 /* table_def  */
        YYSYMBOL_column_def = 76,                /* column_def  */
        YYSYMBOL_column_and_type = 77,           /* column_and_type  */
        YYSYMBOL_column_type = 78,               /* column_type  */
        YYSYMBOL_data_type_l = 79,               /* data_type_l  */
        YYSYMBOL_data_type = 80,                 /* data_type  */
        YYSYMBOL_data_count = 81,                /* data_count  */
        YYSYMBOL_oneselect = 82,                 /* oneselect  */
        YYSYMBOL_selectfrom = 83,                /* selectfrom  */
        YYSYMBOL_selcollist = 84,                /* selcollist  */
        YYSYMBOL_collist = 85,                   /* collist  */
        YYSYMBOL_from = 86,                      /* from  */
        YYSYMBOL_unorderdfrom = 87,              /* unorderdfrom  */
        YYSYMBOL_tablelist = 88,                 /* tablelist  */
        YYSYMBOL_expr = 89,                      /* expr  */
        YYSYMBOL_val = 90,                       /* val  */
        YYSYMBOL_constlist = 91,                 /* constlist  */
        YYSYMBOL_update_assign_list = 92,        /* update_assign_list  */
        YYSYMBOL_column_assignment = 93,         /* column_assignment  */
        YYSYMBOL_const_val = 94,                 /* const_val  */
        YYSYMBOL_column_val = 95,                /* column_val  */
        YYSYMBOL_column = 96,                    /* column  */
        YYSYMBOL_selcolumn = 97,                 /* selcolumn  */
        YYSYMBOL_table = 98,                     /* table  */
        YYSYMBOL_id = 99,                        /* id  */
        YYSYMBOL_string = 100,                   /* string  */
        YYSYMBOL_number = 101                    /* number  */
    };

    #endregion

    #region Classes

    internal class sql_str
    {
        public string Data { get; set; }

        public int Len { get; set; }
    }

    internal class complex_expr
    {
        public int Op { get; set; }

        public expr Left { get; set; }

        public expr Right { get; set; }
    }

    internal class ext_column
    {
        // TODO: Make into struct of { column, table }
        public Tuple<string, string> Unparsed
        {
            get => o as Tuple<string, string>;
            set => o = value;
        }

        // TODO: Make into struct of { column, table }
        public Tuple<int, JOINTABLE> Parsed
        {
            get => o as Tuple<int, JOINTABLE>;
            set => o = value;
        }

        private object o = new object();
    }

    internal class expr
    {
        public int Type { get; set; }

        #region Union (u)

        public complex_expr Expr
        {
            get => u as complex_expr;
            set => u = value;
        }

        public int IVal
        {
            get
            {
                if (u is int)
                    return (int)u;

                return default;
            }
            set => u = value;
        }

        public uint UVal
        {
            get
            {
                if (u is uint)
                    return (uint)u;

                return default;
            }
            set => u = value;
        }

        public string SVal
        {
            get => u as string;
            set => u = value;
        }

        public ext_column Column
        {
            get => u as ext_column;
            set => u = value;
        }

        private object u = new object();

        #endregion
    }

    #endregion

    #region Generated Classes

    /// <summary>
    /// Value type.
    /// </summary>
    /// <remarks>Union</remarks>
    internal class SQL_STYPE
    {
        public sql_str Str { get; set; }

        public string String { get; set; }
        
        public column_info ColumnList { get; set; }

        public LibmsiView Query { get; set; }

        public expr Expr { get; set; }

        public ushort ColumnType { get; set; }

        public int Integer { get; set; }
    }

    #endregion

    internal class LibmsiSQLInput
    {
        #region Constants

        public const int OP_EQ = 1;
        public const int OP_AND = 2;
        public const int OP_OR = 3;
        public const int OP_GT = 4;
        public const int OP_LT = 5;
        public const int OP_LE = 6;
        public const int OP_GE = 7;
        public const int OP_NE = 8;
        public const int OP_ISNULL = 9;
        public const int OP_NOTNULL = 10;

        public const int EXPR_COMPLEX = 1;
        public const int EXPR_COLUMN = 2;
        public const int EXPR_COL_NUMBER = 3;
        public const int EXPR_IVAL = 4;
        public const int EXPR_SVAL = 5;
        public const int EXPR_UVAL = 6;
        public const int EXPR_STRCMP = 7;
        public const int EXPR_WILDCARD = 9;
        public const int EXPR_COL_NUMBER_STRING = 10;
        public const int EXPR_COL_NUMBER32 = 11;
        public const int EXPR_UNARY = 12;

        #endregion

        #region Generated Constants

        /// <summary>
        /// Identify Bison output, and Bison version.
        /// </summary>
        private const int YYBISON = 30802;

        /// <summary>
        /// Bison version string.
        /// </summary>
        private const string YYBISON_VERSION = "3.8.2";

        /// <summary>
        /// Skeleton name.
        /// </summary>
        private const string YYSKELETON_NAME = "yacc.c";

        /// <summary>
        /// Pure parsers.
        /// </summary>
        private const int YYPURE = 1;

        /// <summary>
        /// Push parsers.
        /// </summary
        private const int YYPUSH = 0;

        /// <summary>
        /// Pull parsers.
        /// </summary>
        private const int YYPULL = 1;

        /// <summary>
        /// State number of the termination state.
        /// </summary>
        private const int YYFINAL = 36;

        /// <summary>
        /// Last index in YYTABLE.
        /// </summary>
        private const int YYLAST = 156;

        /// <summary>
        /// Number of terminals.
        /// </summary>
        private const int YYNTOKENS = 65;

        /// <summary>
        /// Number of nonterminals.
        /// </summary>
        private const int YYNNTS = 37;

        /// <summary>
        /// Number of rules.
        /// </summary>
        private const int YYNRULES = 87;

        /// <summary>
        /// Number of states.
        /// </summary>
        private const int YYNSTATES = 154;

        /// <summary>
        /// Last valid token kind.
        /// </summary>
        private const int YYMAXUTOK = 319;

        /// <summary>
        /// Symbol number corresponding to TOKEN-NUM as returned by Lexer.
        /// </summary>
        private static readonly byte[] yytranslate =
        {
            0,     2,     2,     2,     2,     2,     2,     2,     2,     2,
            2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
            2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
            2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
            2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
            2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
            2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
            2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
            2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
            2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
            2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
            2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
            2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
            2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
            2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
            2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
            2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
            2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
            2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
            2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
            2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
            2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
            2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
            2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
            2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
            2,     2,     2,     2,     2,     2,     1,     2,     3,     4,
            5,     6,     7,     8,     9,    10,    11,    12,    13,    14,
            15,    16,    17,    18,    19,    20,    21,    22,    23,    24,
            25,    26,    27,    28,    29,    30,    31,    32,    33,    34,
            35,    36,    37,    38,    39,    40,    41,    42,    43,    44,
            45,    46,    47,    48,    49,    50,    51,    52,    53,    54,
            55,    56,    57,    58,    59,    60,    61,    62,    63,    64
        };

        private const int YYPACK_NINF = -80;

        private const int YYTABLE_NINF = -85;

        /// <summary>
        /// Index in YYTABLE of the portion describing STATE-NUM.
        /// </summary>
        private static readonly short[] yypact =
        {
            36,   -44,   -39,    -1,   -26,     9,    50,    37,    56,   -80,
            -80,   -80,   -80,   -80,   -80,   -80,   -80,    37,    37,    37,
            -80,    25,    37,    37,   -18,   -80,   -80,   -80,   -80,    -1,
            78,    47,    76,   -80,    57,   -80,   -80,   105,    72,    51,
            55,   100,   -80,    81,   -80,   -80,   -18,    37,    37,   -80,
            -80,    37,   -80,    37,    62,    37,   -12,   -12,   -80,   -80,
            63,   102,   108,   101,    76,    97,    45,    83,     2,   -80,
            62,     3,    94,   -80,   -80,   126,   -80,   -80,   127,    93,
            62,    37,    52,    37,   -80,   106,   -80,   -80,   -80,   -80,
            -80,   -80,    31,   103,   118,    37,   111,    11,    62,    62,
            60,    60,    60,   -11,    60,    60,    60,   -12,    88,     3,
            -80,   -80,   117,   -80,   -80,   -80,   -80,   -80,   117,   -80,
            -80,   107,   -80,   -80,   -12,   -80,   -80,   138,   -80,   -80,
            -80,   -80,   -80,   109,   -80,   -80,   -80,   -80,   -80,   112,
            -80,   110,   -80,   -80,   -80,   -80,    52,   -80,   113,   140,
            95,    52,   -80,   -80
        };

        /// <summary>
        /// Default reduction number in state STATE-NUM.
        /// Performed when YYTABLE does not specify something else to do.  Zero
        /// means the default is an error.
        /// </summary>
        private static readonly byte[] yydefact =
        {
            0,     0,     0,     0,     0,     0,     0,     0,     0,     2,
            5,     4,     6,     7,     8,     9,     3,     0,     0,     0,
            16,    51,     0,     0,     0,    85,    45,    86,    40,     0,
            43,     0,    82,    83,     0,    84,     1,     0,     0,    52,
            54,     0,    22,     0,    41,    42,     0,     0,     0,    21,
            20,     0,    17,     0,     0,     0,     0,     0,    44,    81,
            15,    71,     0,     0,    80,    18,     0,     0,     0,    25,
            0,    53,     0,    78,    55,    54,    48,    50,    46,     0,
            0,     0,     0,     0,    19,    32,    36,    37,    34,    38,
            35,    26,    27,    30,    12,     0,     0,     0,     0,     0,
            0,     0,     0,     0,     0,     0,     0,     0,     0,    14,
            72,    87,     0,    76,    77,    73,    74,    79,     0,    28,
            29,     0,    13,    24,     0,    56,    57,    58,    59,    68,
            67,    63,    60,     0,    65,    62,    61,    64,    47,     0,
            75,     0,    39,    31,    23,    66,     0,    33,     0,    69,
            10,     0,    11,    70
        };

        private static readonly sbyte[] yypgoto =
        {
            -80,   -80,   -80,   -80,   -80,   -80,   -80,   -80,   -80,   -80,
            -80,   -80,   -47,   -80,   -80,   -80,   -80,   -80,   124,   104,
            -53,   120,   -80,    96,    19,    26,     5,    73,   -80,   -79,
            -7,   -29,   -80,    14,    -6,   -80,   -16
        };

        private static readonly byte[] yydefgoto =
        {
            0,     8,     9,    10,    11,    12,    13,    14,    52,    15,
            67,    68,    65,    91,    92,    93,   141,    16,    28,    29,
            77,    20,    21,    39,    71,   128,   148,    60,    61,   129,
            130,    73,    30,    63,    64,    33,   116
        };

        /// <summary>
        /// What to do in state STATE-NUM.  If
        /// positive, shift that token.  If negative, reduce the rule whose
        /// number is the opposite.  If YYTABLE_NINF, syntax error.
        /// </summary>
        private static readonly short[] yytable =
        {
            32,    35,    25,   115,    79,    17,    69,    98,    25,    95,
            18,    35,    35,    35,    19,    98,    35,    35,    32,    62,
            31,    34,    66,    22,    66,   133,   134,    78,    78,    26,
            27,    37,    38,    40,    23,    76,    42,    43,    31,     1,
            32,    59,    99,    96,     2,     3,     4,    72,   123,    35,
            99,    85,    62,   125,   138,   -49,    36,    25,     5,    47,
            31,    24,    55,    72,   119,    41,    66,   149,    86,    75,
            25,   144,   149,    72,    87,    88,   111,   117,    78,     6,
            25,   120,    25,    89,   111,    46,   112,     7,   -84,    97,
            90,    72,    72,    70,   112,    78,   140,    26,    27,   109,
            113,    48,   142,    53,    54,    56,   114,   100,   113,    81,
            101,   102,    57,    83,   114,    84,    80,   126,   127,    49,
            103,    82,   104,    50,    51,    94,   105,   131,   132,   106,
            135,   136,   137,    55,   107,   108,   122,   118,   124,   121,
            139,   111,    98,   146,   143,   152,   145,   151,    44,    45,
            58,    74,   147,     0,   110,   150,   153
        };

        private static readonly short[] yycheck =
        {
            6,     7,    20,    82,    57,    49,    53,     4,    20,     7,
            49,    17,    18,    19,    15,     4,    22,    23,    24,    48,
            6,     7,    51,    49,    53,    36,    37,    56,    57,    47,
            48,    17,    18,    19,    25,    47,    22,    23,    24,     3,
            46,    47,    39,    41,     8,     9,    10,    54,    95,    55,
            39,     6,    81,    42,   107,     0,     0,    20,    22,    12,
            46,    11,     7,    70,    33,    40,    95,   146,    23,    55,
            20,   124,   151,    80,    29,    30,    24,    83,   107,    43,
            20,    50,    20,    38,    24,     7,    34,    51,    12,    70,
            45,    98,    99,    31,    34,   124,   112,    47,    48,    80,
            48,    44,   118,    31,    53,     5,    54,    13,    48,     7,
            16,    17,    31,    12,    54,    18,    53,    98,    99,    14,
            26,    13,    28,    18,    19,    42,    32,   101,   102,    35,
            104,   105,   106,     7,     7,    42,    18,    31,    27,    36,
            52,    24,     4,    31,    37,    50,    37,     7,    24,    29,
            46,    55,    42,    -1,    81,    42,   151
        };

        /// <summary>
        /// The symbol kind of the accessing symbol of state STATE-NUM.
        /// </summary>
        private static readonly byte[] yystos =
        {
            0,     3,     8,     9,    10,    22,    43,    51,    66,    67,
            68,    69,    70,    71,    72,    74,    82,    49,    49,    15,
            86,    87,    49,    25,    11,    20,    47,    48,    83,    84,
            97,    98,    99,   100,    98,    99,     0,    98,    98,    88,
            98,    40,    98,    98,    83,    86,     7,    12,    44,    14,
            18,    19,    73,    31,    53,     7,     5,    31,    84,    99,
            92,    93,    96,    98,    99,    77,    96,    75,    76,    77,
            31,    89,    95,    96,    88,    98,    47,    85,    96,    85,
            53,     7,    13,    12,    18,     6,    23,    29,    30,    38,
            45,    78,    79,    80,    42,     7,    41,    89,     4,    39,
            13,    16,    17,    26,    28,    32,    35,     7,    42,    89,
            92,    24,    34,    48,    54,    94,   101,    99,    31,    33,
            50,    36,    18,    77,    27,    42,    89,    89,    90,    94,
            95,    90,    90,    36,    37,    90,    90,    90,    85,    52,
            101,    81,   101,    37,    85,    37,    31,    42,    91,    94,
            42,     7,    50,    91
        };

        /// <summary>
        /// Symbol kind of the left-hand side of rule RULE-NUM.
        /// </summary>
        private static readonly byte[] yyr1 =
        {
            0,    65,    66,    67,    67,    67,    67,    67,    67,    67,
            68,    68,    69,    69,    70,    70,    71,    72,    72,    72,
            73,    73,    74,    75,    76,    76,    77,    78,    78,    78,
            79,    79,    80,    80,    80,    80,    80,    80,    80,    81,
            82,    82,    83,    84,    84,    84,    85,    85,    85,    86,
            86,    86,    87,    87,    88,    88,    89,    89,    89,    89,
            89,    89,    89,    89,    89,    89,    89,    90,    90,    91,
            91,    92,    92,    93,    94,    94,    94,    94,    95,    96,
            96,    97,    97,    97,    98,    99,   100,   101
        };

        /// <summary>
        /// Number of symbols on the right-hand side of rule RULE-NUM.
        /// </summary>
        private static readonly byte[] yyr2 =
        {
            0,     2,     1,     1,     1,     1,     1,     1,     1,     1,
            10,    11,     6,     7,     6,     4,     2,     4,     5,     6,
            1,     1,     3,     4,     3,     1,     2,     1,     2,     2,
            1,     3,     1,     4,     1,     1,     1,     1,     1,     1,
            2,     3,     2,     1,     3,     1,     1,     3,     1,     2,
            4,     1,     2,     4,     1,     3,     3,     3,     3,     3,
            3,     3,     3,     3,     3,     3,     4,     1,     1,     1,
            3,     1,     3,     3,     1,     2,     1,     1,     1,     3,
            1,     3,     1,     1,     1,     1,     1,     1
        };

        private const int YYENOMEM = -2;

        /// <summary>
        /// Initial size of the parser's stacks.
        /// </summary>
        private const int YYINITDEPTH = 200;

        /// <summary>
        /// Maximum size the stacks can grow to (effective only
        /// if the built-in stack extension method is used).
        /// </summary>
        /// <remarks>
        /// Do not make this value too large; the results are undefined if
        /// YYSTACK_ALLOC_MAXIMUM < YYSTACK_BYTES (YYMAXDEPTH)
        /// evaluated with infinite-precision integer arithmetic.
        /// </remarks>
        private const int YYMAXDEPTH = 10000;

        #endregion

        #region Properties

        public LibmsiDatabase Database { get; set; }

        public string Command { get; set; }

        public int N { get; set; }

        public int Len { get; set; }

        public LibmsiResult R { get; set; }

        /// <summary>
        /// View structure for the resulting query.  This value
        /// tracks the view currently being created so we can free
        /// this view on syntax error.
        /// </summary>
        public LibmsiView View { get; set; }

        public LinkedList<object> Mem { get; set; } = new LinkedList<object>();

        #endregion

        #region Functions

        public static LibmsiResult ParseSQL(LibmsiDatabase db, string command, out LibmsiView phview, LinkedList<object> mem)
        {
            phview = null;
            LibmsiSQLInput sql = new LibmsiSQLInput
            {
                Database = db,
                Command = command,
                N = 0,
                Len = 0,
                R = LibmsiResult.LIBMSI_RESULT_BAD_QUERY_SYNTAX,
                View = phview,
                Mem = mem,
            };

            LibmsiResult r = sql.Parse();
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
            {
                if (sql.View != null)
                {
                    sql.View.Delete();
                    sql.View = null;
                }

                return sql.R;
            }

            return LibmsiResult.LIBMSI_RESULT_SUCCESS;
        }

        #endregion

        #region Generated Functions

        /// <summary>
        /// Symbol number corresponding to TOKEN-NUM
        /// as returned by yylex, with out-of-bounds checking.
        /// </summary>
        private yysymbol_kind_t YYTRANSLATE(sql_tokentype yyx)
            => (0 <= yyx && (int)yyx <= YYMAXUTOK) ? (yysymbol_kind_t)yytranslate[(int)yyx] : yysymbol_kind_t.YYSYMBOL_YYUNDEF;

        /// <summary>
        /// yyparse.
        /// </summary>
        public LibmsiResult Parse()
        {
            // Lookahead token kind.
            sql_tokentype yychar;

            // The semantic value of the lookahead symbol.
            SQL_STYPE yylval = new SQL_STYPE();

            // Number of syntax errors so far.
            int sql_nerrs = 0;

            int yystate = 0;

            // Number of tokens to shift before error messages enabled.
            int yyerrstatus = 0;

            // Refer to the stacks through separate pointers, to allow yyoverflow
            // to reallocate them elsewhere.

            // Their size.
            long yystacksize = YYINITDEPTH;

            // The state stack: array, bottom, top.
            byte[] yyssa = new byte[YYINITDEPTH];
            int yyss = 0; // yyssa[0]
            int yyssp = yyss; // yyssa[0]

            // The semantic value stack: array, bottom, top.
            SQL_STYPE[] yyvsa = new SQL_STYPE[YYINITDEPTH];
            int yyvs = 0; // yyvsa[0]
            int yyvsp = 0; // yyvsa[0]

            int yyn;

            // The return value of yyparse.
            LibmsiResult yyresult;

            // Lookahead symbol kind.
            yysymbol_kind_t yytoken = yysymbol_kind_t.YYSYMBOL_YYEMPTY;

            // The variables used to return semantic value and location from the action routines.
            SQL_STYPE yyval = new SQL_STYPE();

            // The number of symbols on the RHS of the reduced rule.
            // Keep to zero when no symbol should be popped.
            int yylen = 0;

            // YYDPRINTF ((stderr, "Starting parse\n"));

            yychar = sql_tokentype.SQL_EMPTY; // Cause a token to be read.

            goto yysetstate;

            #region yynewstate -- push a new state, which is found in yystate.

        yynewstate:
            // In all cases, when you get here, the value and location stacks
            //  have just been pushed.  So pushing a state here evens the stacks.
            yyssp++;

            #endregion

            #region yysetstate -- set current state (the top of the stack) to yystate.

        yysetstate:
            //YYDPRINTF ((stderr, "Entering state %d\n", yystate));
            //YY_ASSERT (0 <= yystate && yystate < YYNSTATES);
            yyssa[yyssp] = (byte)yystate;

            if (yyss + yystacksize - 1 <= yyssp)
            {
                // Get the current used size of the three stacks, in elements.
                int yysize = yyssp - yyss + 1;

                // Extend the stack our own way.
                if (YYMAXDEPTH <= yystacksize)
                    goto yyexhaustedlab;

                yystacksize *= 2;
                if (YYMAXDEPTH < yystacksize)
                    yystacksize = YYMAXDEPTH;

                yyssp = yyss + yysize - 1;
                yyvsp = yyvs + yysize - 1;

                // YYDPRINTF ((stderr, "Stack size increased to %ld\n", YY_CAST (long, yystacksize)));
                if (yyss + yystacksize - 1 <= yyssp)
                    goto yyabortlab;
            }

            if (yystate == YYFINAL)
                goto yyacceptlab;

            goto yybackup;

            #endregion

            #region yybackup.

        yybackup:
            // Do appropriate processing given the current state.  Read a
            // lookahead token if we need one and don't already have one.

            // First try to decide what to do without reference to lookahead token.
            yyn = yypact[yystate];
            if (yyn == YYPACK_NINF)
                goto yydefault;

            // Not known => get a lookahead token if don't already have one.

            // YYCHAR is either empty, or end-of-input, or a valid lookahead.
            if (yychar == sql_tokentype.SQL_EMPTY)
            {
                //YYDPRINTF ((stderr, "Reading a token\n"));
                yychar = Lexer(ref yylval);
            }

            if (yychar <= sql_tokentype.SQL_EOF)
            {
                yychar = sql_tokentype.SQL_EOF;
                yytoken = yysymbol_kind_t.YYSYMBOL_YYEOF;
                //YYDPRINTF ((stderr, "Now at end of input.\n"));
            }
            else if (yychar == sql_tokentype.SQL_error)
            {
                // The scanner already issued an error message, process directly
                //  to error recovery.  But do not keep the error token as
                //  lookahead, it is too special and may lead us to an endless
                //  loop in error recovery.
                yychar = sql_tokentype.SQL_UNDEF;
                yytoken = yysymbol_kind_t.YYSYMBOL_YYerror;
                goto yyerrlab1;
            }
            else
            {
                yytoken = YYTRANSLATE(yychar);
                //YY_SYMBOL_PRINT("Next token is", yytoken, &yylval, &yylloc);
            }

            // If the proper action on seeing token YYTOKEN is to reduce or to
            //  detect an error, take that action.
            yyn += (int)yytoken;
            if (yyn < 0 || YYLAST < yyn || yycheck[yyn] != (short)yytoken)
                goto yydefault;

            yyn = yytable[yyn];
            if (yyn <= 0)
            {
                yyn = -yyn;
                goto yyreduce;
            }

            // Count tokens shifted since error; after three, turn off error status.
            if (yyerrstatus != 0)
                yyerrstatus--;

            // Shift the lookahead token.
            yystate = yyn;
            yyvsa[++yyvsp] = yylval;

            // Discard the shifted token.
            yychar = sql_tokentype.SQL_EMPTY;
            goto yynewstate;

            #endregion

            #region yydefault -- do the default action for the current state.

        yydefault:
            yyn = yydefact[yystate];
            if (yyn == 0)
                goto yyerrlab;

            goto yyreduce;

            #endregion

            #region yyreduce -- do a reduction.

        yyreduce:
            // yyn is the number of a rule to reduce with.
            yylen = yyr2[yyn];

            /* If YYLEN is nonzero, implement the default value of the action:
                '$$ = $1'.

                Otherwise, the following line sets YYVAL to garbage.
                This behavior is undocumented and Bison
                users should not rely upon it.  Assigning to YYVAL
                unconditionally makes the parser a bit smaller, and it avoids a
                GCC warning that YYVAL may be used uninitialized.  */

            yyval = yyvsa[yyvsp + 1 - yylen];
            switch (yyn)
            {
                case 2: /* query: onequery  */
                {
                    View = (yyvsa[yyvsp].Query);
                    break;
                }

                case 10: /* oneinsert: TK_INSERT TK_INTO table TK_LP collist TK_RP TK_VALUES TK_LP constlist TK_RP  */
                {
                    LibmsiInsertView.Create(Database, out LibmsiView insert, (yyvsa[yyvsp + -7].String), (yyvsa[yyvsp + -5].ColumnList), (yyvsa[yyvsp + -1].ColumnList), false);
                    if (insert == null)
                        goto yyabortlab;

                    View = insert;
                    yyval.Query = insert;
                    break;
                }

                case 11: /* oneinsert: TK_INSERT TK_INTO table TK_LP collist TK_RP TK_VALUES TK_LP constlist TK_RP TK_TEMPORARY  */
                {
                    LibmsiInsertView.Create(Database, out LibmsiView insert, (yyvsa[yyvsp + -8].String), (yyvsa[yyvsp + -6].ColumnList), (yyvsa[yyvsp + -2].ColumnList), true);
                    if (insert == null)
                        goto yyabortlab;

                    View = insert;
                    yyval.Query = insert;
                    break;
                }

                case 12: /* onecreate: TK_CREATE TK_TABLE table TK_LP table_def TK_RP  */
                {
                    if ((yyvsa[yyvsp + -1].ColumnList) == null)
                        goto yyabortlab;

                    LibmsiResult r = LibmsiCreateView.Create(Database, out LibmsiView create, (yyvsa[yyvsp + -3].String), (yyvsa[yyvsp + -1].ColumnList), false);
                    if (create == null)
                    {
                        R = r;
                        goto yyabortlab;
                    }

                    View = create;
                    yyval.Query = create;
                    break;
                }

                case 13: /* onecreate: TK_CREATE TK_TABLE table TK_LP table_def TK_RP TK_HOLD  */
                {
                    if ((yyvsa[yyvsp + -2].ColumnList) == null)
                        goto yyabortlab;

                    LibmsiCreateView.Create(Database, out LibmsiView create, (yyvsa[yyvsp + -4].String), (yyvsa[yyvsp + -2].ColumnList), true);
                    if (create == null)
                        goto yyabortlab;

                    View = create;
                    yyval.Query = create;
                    break;
                }

                case 14: /* oneupdate: TK_UPDATE table TK_SET update_assign_list TK_WHERE expr  */
                {
                    LibmsiUpdateView.Create(Database, out LibmsiView update, (yyvsa[yyvsp + -4].String), (yyvsa[yyvsp + -2].ColumnList), (yyvsa[yyvsp + 0].Expr));
                    if (update == null)
                        goto yyabortlab;

                    View = update;
                    yyval.Query = update;
                    break;
                }

                case 15: /* oneupdate: TK_UPDATE table TK_SET update_assign_list  */
                {
                    LibmsiUpdateView.Create(Database, out LibmsiView update, (yyvsa[yyvsp + -2].String), (yyvsa[yyvsp + 0].ColumnList), null);
                    if (update == null)
                        goto yyabortlab;

                    View = update;
                    yyval.Query = update;
                    break;
                }

                case 16: /* onedelete: TK_DELETE from  */
                {
                    LibmsiDeleteView.Create(Database, out LibmsiView delete, (yyvsa[yyvsp + 0].Query));
                    if (delete == null)
                        goto yyabortlab;

                    View = delete;
                    yyval.Query = delete;
                    break;
                }

                case 17: /* onealter: TK_ALTER TK_TABLE table alterop  */
                {
                    LibmsiAlterView.Create(Database, out LibmsiView alter, (yyvsa[yyvsp + -1].String), null, (yyvsa[yyvsp + 0].Integer));
                    if (alter == null)
                        goto yyabortlab;

                    View = alter;
                    yyval.Query = alter;
                    break;
                }

                case 18: /* onealter: TK_ALTER TK_TABLE table TK_ADD column_and_type  */
                {
                    LibmsiAlterView.Create(Database, out LibmsiView alter, (yyvsa[yyvsp + -2].String), (yyvsa[yyvsp + 0].ColumnList), 0);
                    if (alter == null)
                        goto yyabortlab;

                   View = alter;
                     yyval.Query = alter;
                    break;
                }

                case 19: /* onealter: TK_ALTER TK_TABLE table TK_ADD column_and_type TK_HOLD  */
                {
                    LibmsiAlterView.Create(Database, out LibmsiView alter, (yyvsa[yyvsp + -3].String), (yyvsa[yyvsp + -1].ColumnList), 1);
                    if (alter == null)
                        goto yyabortlab;

                   View = alter;
                     yyval.Query = alter;
                    break;
                }

                case 20: /* alterop: TK_HOLD  */
                {
                    (yyval.Integer) = 1;
                    break;
                }

                case 21: /* alterop: TK_FREE  */
                {
                    (yyval.Integer) = -1;
                    break;
                }

                case 22: /* onedrop: TK_DROP TK_TABLE table  */
                {
                    LibmsiResult r = LibmsiDropView.Create(Database, out LibmsiView drop, (yyvsa[yyvsp + 0].String));
                    if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS || (yyval.Query == null))
                        goto yyabortlab;

                    View = drop;
                    yyval.Query = drop;
                    break;
                }

                case 23: /* table_def: column_def TK_PRIMARY TK_KEY collist  */
                {
                    column_info column_list = yyvsa[yyvsp + -3].ColumnList;
                    if (MarkPrimaryKeys(ref column_list, (yyvsa[yyvsp + 0].ColumnList)))
                    {
                        yyvsa[yyvsp + -3].ColumnList = column_list;
                        (yyval.ColumnList) = (yyvsa[yyvsp + -3].ColumnList);
                    }
                    else
                    {
                        yyvsa[yyvsp + -3].ColumnList = column_list;
                        (yyval.ColumnList) = null;
                    }

                    break;
                }

                case 24: /* column_def: column_def TK_COMMA column_and_type  */
                {
                    column_info ci;
                    for (ci = (yyvsa[yyvsp + -2].ColumnList); ci.Next != null; ci = ci.Next);

                    ci.Next = (yyvsa[yyvsp + 0].ColumnList);
                    (yyval.ColumnList) = (yyvsa[yyvsp + -2].ColumnList);
                    break;
                }

                case 25: /* column_def: column_and_type  */
                {
                    (yyval.ColumnList) = (yyvsa[yyvsp + 0].ColumnList);
                    break;
                }

                case 26: /* column_and_type: column column_type  */
                {
                    (yyval.ColumnList) = (yyvsa[yyvsp + -1].ColumnList);
                    (yyval.ColumnList).Type = ((yyvsa[yyvsp + 0].ColumnType) | MSITYPE_VALID);
                    (yyval.ColumnList).Temporary = ((yyvsa[yyvsp + 0].ColumnType) & MSITYPE_TEMPORARY) != 0 ? true : false;
                    break;
                }

                case 27: /* column_type: data_type_l  */
                {
                    (yyval.ColumnType) = (yyvsa[yyvsp + 0].ColumnType);
                    break;
                }

                case 28: /* column_type: data_type_l TK_LOCALIZABLE  */
                {
                    (yyval.ColumnType) = (ushort)((yyvsa[yyvsp + -1].ColumnType) | MSITYPE_LOCALIZABLE);
                    break;
                }

                case 29: /* column_type: data_type_l TK_TEMPORARY  */
                {
                    (yyval.ColumnType) = (ushort)((yyvsa[yyvsp + -1].ColumnType) | MSITYPE_TEMPORARY);
                    break;
                }

                case 30: /* data_type_l: data_type  */
                {
                    (yyval.ColumnType) |= MSITYPE_NULLABLE;
                    break;
                }

                case 31: /* data_type_l: data_type TK_NOT TK_NULL  */
                {
                    (yyval.ColumnType) = (yyvsa[yyvsp + -2].ColumnType);
                    break;
                }

                case 32: /* data_type: TK_CHAR  */
                {
                    (yyval.ColumnType) = MSITYPE_STRING | 1;
                    break;
                }

                case 33: /* data_type: TK_CHAR TK_LP data_count TK_RP  */
                {
                    (yyval.ColumnType) = (ushort)(MSITYPE_STRING | 0x400 | (yyvsa[yyvsp + -1].ColumnType));
                    break;
                }

                case 34: /* data_type: TK_LONGCHAR  */
                {
                    (yyval.ColumnType) = MSITYPE_STRING | 0x400;
                    break;
                }

                case 35: /* data_type: TK_SHORT  */
                {
                    (yyval.ColumnType) = 2 | 0x400;
                    break;
                }

                case 36: /* data_type: TK_INT  */
                {
                    (yyval.ColumnType) = 2 | 0x400;
                    break;
                }

                case 37: /* data_type: TK_LONG  */
                {
                    (yyval.ColumnType) = 4;
                    break;
                }

                case 38: /* data_type: TK_OBJECT  */
                {
                    (yyval.ColumnType) = MSITYPE_STRING | MSITYPE_VALID;
                    break;
                }

                case 39: /* data_count: number  */
                {
                    if (((yyvsa[yyvsp + 0].Integer) > 255) || ((yyvsa[yyvsp + 0].Integer) < 0))
                        goto yyabortlab;

                    (yyval.ColumnType) = (ushort)(yyvsa[yyvsp + 0].Integer);
                    break;
                }

                case 40: /* oneselect: TK_SELECT selectfrom  */
                {
                    (yyval.Query) = (yyvsa[yyvsp + 0].Query);
                    break;
                }

                case 41: /* oneselect: TK_SELECT TK_DISTINCT selectfrom  */
                {
                    LibmsiResult r = LibmsiDistinctView.Create(Database, out LibmsiView distinct, (yyvsa[yyvsp + 0].Query));
                    if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                        goto yyabortlab;

                    View = distinct;
                    yyval.Query = distinct;
                    break;
                }

                case 42: /* selectfrom: selcollist from  */
                {
                    if ((yyvsa[yyvsp + -1].ColumnList) != null)
                    {
                        LibmsiResult r = LibmsiSelectView.Create(Database, out LibmsiView select, (yyvsa[yyvsp + 0].Query), (yyvsa[yyvsp + -1].ColumnList));
                        if (r != LibmsiResult .LIBMSI_RESULT_SUCCESS)
                            goto yyabortlab;

                        View = select;
                        yyval.Query = select;
                    }
                    else
                    {
                        (yyval.Query) = (yyvsa[yyvsp + 0].Query);
                    }

                    break;
                }

                case 44: /* selcollist: selcolumn TK_COMMA selcollist  */
                {
                    (yyvsa[yyvsp + -2].ColumnList).Next = (yyvsa[yyvsp + 0].ColumnList);
                    break;
                }

                case 45: /* selcollist: TK_STAR  */
                {
                    (yyval.ColumnList) = null;
                    break;
                }

                case 47: /* collist: column TK_COMMA collist  */
                {
                    (yyvsa[yyvsp + -2].ColumnList).Next = (yyvsa[yyvsp + 0].ColumnList);
                    break;
                }

                case 48: /* collist: TK_STAR  */
                {
                    (yyval.ColumnList) = null;
                    break;
                }

                case 49: /* from: TK_FROM table  */
                {
                    LibmsiResult r = LibmsiTableView.Create(Database, (yyvsa[yyvsp + 0].String), out LibmsiView table);
                    if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS || (yyval.Query == null))
                        goto yyabortlab;

                    View = table;
                    yyval.Query = table;
                    break;
                }

                case 50: /* from: unorderdfrom TK_ORDER TK_BY collist  */
                {
                    if ((yyvsa[yyvsp + 0].ColumnList) != null)
                    {
                        LibmsiResult r = (yyvsa[yyvsp + -3].Query).Sort(yyvsa[yyvsp + 0].ColumnList);
                        if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                            goto yyabortlab;
                    }

                    (yyval.Query) = (yyvsa[yyvsp + -3].Query);
                    break;
                }

                case 52: /* unorderdfrom: TK_FROM tablelist  */
                {
                    LibmsiResult r = LibmsiWhereView.Create(Database, out LibmsiView where, (yyvsa[yyvsp + 0].String), null);
                    if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                        goto yyabortlab;

                    View = where;
                    yyval.Query = where;
                    break;
                }

                case 53: /* unorderdfrom: TK_FROM tablelist TK_WHERE expr  */
                {
                    LibmsiResult r = LibmsiWhereView.Create(Database, out LibmsiView where, (yyvsa[yyvsp + -2].String), (yyvsa[yyvsp + 0].Expr));
                    if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                        goto yyabortlab;

                    View = where;
                    yyval.Query = where;
                    break;
                }

                case 54: /* tablelist: table  */
                {
                    (yyval.String) = (yyvsa[yyvsp + 0].String);
                    break;
                }

                case 55: /* tablelist: table TK_COMMA tablelist  */
                {
                    (yyval.String) = AddTable((yyvsa[yyvsp + 0].String), (yyvsa[yyvsp + -2].String));
                    if (yyval.String == null)
                        goto yyabortlab;

                    break;
                }

                case 56: /* expr: TK_LP expr TK_RP  */
                {
                    (yyval.Expr) = (yyvsa[yyvsp + -1].Expr);
                    if (yyval.Expr == null)
                        goto yyabortlab;
                    
                    break;
                }

                case 57: /* expr: expr TK_AND expr  */
                {
                    (yyval.Expr) = BuildExprComplex((yyvsa[yyvsp + -2].Expr), OP_AND, (yyvsa[yyvsp + 0].Expr));
                    if (yyval.Expr == null)
                        goto yyabortlab;

                    break;
                }

                case 58: /* expr: expr TK_OR expr  */
                {
                    (yyval.Expr) = BuildExprComplex((yyvsa[yyvsp + -2].Expr), OP_OR, (yyvsa[yyvsp + 0].Expr));
                    if (yyval.Expr == null)
                        goto yyabortlab;

                    break;
                }

                case 59: /* expr: column_val TK_EQ val  */
                {
                    (yyval.Expr) = BuildExprComplex((yyvsa[yyvsp + -2].Expr), OP_EQ, (yyvsa[yyvsp + 0].Expr));
                    if (yyval.Expr == null)
                        goto yyabortlab;

                    break;
                }

                case 60: /* expr: column_val TK_GT val  */
                {
                    (yyval.Expr) = BuildExprComplex((yyvsa[yyvsp + -2].Expr), OP_GT, (yyvsa[yyvsp + 0].Expr));
                    if (yyval.Expr == null)
                        goto yyabortlab;

                    break;
                }

                case 61: /* expr: column_val TK_LT val  */
                {
                    (yyval.Expr) = BuildExprComplex((yyvsa[yyvsp + -2].Expr), OP_LT, (yyvsa[yyvsp + 0].Expr));
                    if (yyval.Expr == null)
                        goto yyabortlab;

                    break;
                }

                case 62: /* expr: column_val TK_LE val  */
                {
                    (yyval.Expr) = BuildExprComplex((yyvsa[yyvsp + -2].Expr), OP_LE, (yyvsa[yyvsp + 0].Expr));
                    if (yyval.Expr == null)
                        goto yyabortlab;

                    break;
                }

                case 63: /* expr: column_val TK_GE val  */
                {
                    (yyval.Expr) = BuildExprComplex((yyvsa[yyvsp + -2].Expr), OP_GE, (yyvsa[yyvsp + 0].Expr));
                    if (yyval.Expr == null)
                        goto yyabortlab;

                    break;
                }

                case 64: /* expr: column_val TK_NE val  */
                {
                    (yyval.Expr) = BuildExprComplex((yyvsa[yyvsp + -2].Expr), OP_NE, (yyvsa[yyvsp + 0].Expr));
                    if (yyval.Expr == null)
                        goto yyabortlab;

                    break;
                }

                case 65: /* expr: column_val TK_IS TK_NULL  */
                {
                    (yyval.Expr) = BuildExprUnary((yyvsa[yyvsp + -2].Expr), OP_ISNULL);
                    if (yyval.Expr == null)
                        goto yyabortlab;

                    break;
                }

                case 66: /* expr: column_val TK_IS TK_NOT TK_NULL  */
                {
                    (yyval.Expr) = BuildExprUnary((yyvsa[yyvsp + -3].Expr), OP_NOTNULL);
                    if (yyval.Expr == null)
                        goto yyabortlab;

                    break;
                }

                case 69: /* constlist: const_val  */
                {
                    (yyval.ColumnList) = AllocColumn(null, null);
                    if (yyval.ColumnList == null)
                        goto yyabortlab;

                    (yyval.ColumnList).Val = (yyvsa[yyvsp + 0].Expr);
                    break;
                }

                case 70: /* constlist: const_val TK_COMMA constlist  */
                {
                    (yyval.ColumnList) = AllocColumn(null, null);
                    if (yyval.ColumnList == null)
                        goto yyabortlab;

                    (yyval.ColumnList).Val = (yyvsa[yyvsp + -2].Expr);
                    (yyval.ColumnList).Next = (yyvsa[yyvsp + 0].ColumnList);
                    break;
                }

                case 72: /* update_assign_list: column_assignment TK_COMMA update_assign_list  */
                {
                    (yyval.ColumnList) = (yyvsa[yyvsp + -2].ColumnList);
                    (yyval.ColumnList).Next = (yyvsa[yyvsp + 0].ColumnList);
                    break;
                }

                case 73: /* column_assignment: column TK_EQ const_val  */
                {
                    (yyval.ColumnList) = (yyvsa[yyvsp + -2].ColumnList);
                    (yyval.ColumnList).Val = (yyvsa[yyvsp + 0].Expr);
                    break;
                }

                case 74: /* const_val: number  */
                {
                    (yyval.Expr) = BuildExprIVal(yyvsa[yyvsp + 0].Integer);
                    if (yyval.Expr == null)
                        goto yyabortlab;

                    break;
                }

                case 75: /* const_val: TK_MINUS number  */
                {
                    (yyval.Expr) = BuildExprIVal(-(yyvsa[yyvsp + 0].Integer));
                    if (yyval.Expr == null)
                        goto yyabortlab;

                    break;
                }

                case 76: /* const_val: TK_STRING  */
                {
                    (yyval.Expr) = BuildExprSVal((yyvsa[yyvsp + 0].Str));
                    if (yyval.Expr == null)
                        goto yyabortlab;

                    break;
                }

                case 77: /* const_val: TK_WILDCARD  */
                {
                    (yyval.Expr) = BuildExprWildcard();
                    if (yyval.Expr == null)
                        goto yyabortlab;

                    break;
                }

                case 78: /* column_val: column  */
                {
                    (yyval.Expr) = BuildExprColumn((yyvsa[yyvsp + 0].ColumnList));
                    if (yyval.Expr == null)
                        goto yyabortlab;

                    break;
                }

                case 79: /* column: table TK_DOT id  */
                {
                    (yyval.ColumnList) = AllocColumn((yyvsa[yyvsp + -2].String), (yyvsa[yyvsp + 0].String));
                    if (yyval.ColumnList == null)
                        goto yyabortlab;

                    break;
                }

                case 80: /* column: id  */
                {
                    (yyval.ColumnList) = AllocColumn(null, (yyvsa[yyvsp + 0].String));
                    if (yyval.ColumnList == null)
                        goto yyabortlab;

                    break;
                }

                case 81: /* selcolumn: table TK_DOT id  */
                {
                    (yyval.ColumnList) = AllocColumn((yyvsa[yyvsp + -2].String), (yyvsa[yyvsp + 0].String));
                    if (yyval.ColumnList == null)
                        goto yyabortlab;

                    break;
                }

                case 82: /* selcolumn: id  */
                {
                    (yyval.ColumnList) = AllocColumn(null, (yyvsa[yyvsp + 0].String));
                    if (yyval.ColumnList == null)
                        goto yyabortlab;

                    break;
                }

                case 83: /* selcolumn: string  */
                {
                    (yyval.ColumnList) = AllocColumn(null, (yyvsa[yyvsp + 0].String));
                    if (yyval.ColumnList == null)
                        goto yyabortlab;

                    break;
                }

                case 84: /* table: id  */
                {
                    (yyval.String) = (yyvsa[yyvsp + 0].String);
                    break;
                }

                case 85: /* id: TK_ID  */
                {
                    if (UnescapeString((yyvsa[yyvsp + 0].Str), out string str) != LibmsiResult.LIBMSI_RESULT_SUCCESS || str == null)
                        goto yyabortlab;

                    yyval.String = str;
                    break;
                }

                case 86: /* string: TK_STRING  */
                {
                    if (UnescapeString((yyvsa[yyvsp + 0].Str),out string str) != LibmsiResult.LIBMSI_RESULT_SUCCESS || str == null)
                        goto yyabortlab;

                    yyval.String = str;
                    break;
                }

                case 87: /* number: TK_INTEGER  */
                {
                    (yyval.Integer) = StringToInt();
                    break;
                }

                default:
                    break;
            }

            /* User semantic actions sometimes alter yychar, and that requires
                that yytoken be updated with the new translation.  We take the
                approach of translating immediately before every use of yytoken.
                One alternative is translating here after every semantic action,
                but that translation would be missed if the semantic action invokes
                goto yyabortlab, goto yyacceptlab, or goto yyerrorlab immediately after altering yychar or
                if it invokes YYBACKUP.  In the case of goto yyabortlab or goto yyacceptlab, an
                incorrect destructor might then be invoked immediately.  In the
                case of goto yyerrorlab or YYBACKUP, subsequent parser actions might lead
                to an incorrect destructor call or verbose syntax error message
                before the lookahead is translated.  */

            yyvsp -= yylen;
            yyssp -= yylen;
            yylen = 0;

            yyvsa[++yyvsp] = yyval;

            // Now 'shift' the result of the reduction.  Determine what state
            // that goes to, based on the state we popped back to and the rule
            //  number reduced by.
            {
                int yylhs = yyr1[yyn] - YYNTOKENS;
                int yyi = yypgoto[yylhs] + yyssa[yyssp];
                yystate = (0 <= yyi && yyi <= YYLAST && yycheck[yyi] == yyssa[yyssp] ? yytable[yyi] : yydefgoto[yylhs]);
            }

            goto yynewstate;

            #endregion

            #region yyerrlab -- here on detecting error.

        yyerrlab:
            // Make sure we have latest lookahead translation.  See comments at
            //  user semantic actions for why this is necessary.
            yytoken = yychar == sql_tokentype.SQL_EMPTY ? yysymbol_kind_t.YYSYMBOL_YYEMPTY : YYTRANSLATE (yychar);

            // If not already recovering from an error, report this error.
            if (yyerrstatus == 0)
            {
                ++sql_nerrs;
                Console.Error.WriteLine("Syntax error");
            }

            if (yyerrstatus == 3)
            {
                // If just tried and failed to reuse lookahead token after an error, discard it.
                if (yychar <= sql_tokentype.SQL_EOF)
                {
                    // Return failure if at end of input.
                    if (yychar ==sql_tokentype. SQL_EOF)
                        goto yyabortlab;
                }
                else
                {
                    yychar = sql_tokentype.SQL_EMPTY;
                }
            }

            // Else will try to reuse lookahead token after shifting the error token.
            goto yyerrlab1;

            #endregion

            #region yyerrorlab -- error raised explicitly by goto yyerrorlab.

        yyerrorlab:

            ++sql_nerrs;

            // Do not reclaim the symbols of the rule whose action triggered this goto yyerrorlab.
            yyvsp -= yylen;
            yyssp -= yylen;
            yylen = 0;
            yystate = yyssa[yyssp];
            goto yyerrlab1;

            #endregion

            #region yyerrlab1 -- common code for both syntax error and goto yyerrorlab.

        yyerrlab1:
            yyerrstatus = 3; // Each real token shifted decrements this.

            // Pop stack until we find a state that shifts the error token.
            for (;;)
            {
                yyn = yypact[yystate];
                if (yyn != YYPACK_NINF)
                {
                    yyn += (int)yysymbol_kind_t.YYSYMBOL_YYerror;
                    if (0 <= yyn && yyn <= YYLAST && yycheck[yyn] == (short)yysymbol_kind_t.YYSYMBOL_YYerror)
                    {
                        yyn = yytable[yyn];
                        if (0 < yyn)
                            break;
                    }
                }

                // Pop the current state because it cannot handle the error token.
                if (yyssp == yyss)
                    goto yyabortlab;


                yyvsp -= 1;
                yyssp -= 1;
                yystate = yyssa[yyssp];
            }

            yyvsa[++yyvsp] = yylval;

            // Shift the error token.
            yystate = yyn;
            goto yynewstate;

            #endregion

            #region yyacceptlab -- goto yyacceptlab comes here.

        yyacceptlab:
            yyresult = LibmsiResult.LIBMSI_RESULT_SUCCESS;
            goto yyreturnlab;

            #endregion

            #region yyabortlab -- goto yyabortlab comes here.

        yyabortlab:
            yyresult = LibmsiResult.LIBMSI_RESULT_ACCESS_DENIED;
            goto yyreturnlab;

            #endregion

            #region yyexhaustedlab -- goto yyexhaustedlab (memory exhaustion) comes here.

        yyexhaustedlab:
            Console.Error.WriteLine("Memory exhausted");
            yyresult = LibmsiResult.LIBMSI_RESULT_INVALID_HANDLE;
            goto yyreturnlab;

            #endregion

            #region yyreturnlab -- parsing is finished, clean up and return.

        yyreturnlab:
            if (yychar != sql_tokentype.SQL_EMPTY)
            {
                // Make sure we have latest lookahead translation.  See comments at
                // user semantic actions for why this is necessary.
                yytoken = YYTRANSLATE (yychar);
            }

            // Do not reclaim the symbols of the rule whose action triggered
            // this goto yyabortlab or goto yyacceptlab.
            yyvsp -= yylen;
            yyssp -= yylen;

            while (yyssp != yyss)
            {
                yyvsp -= 1;
                yyssp -= 1;
            }

            #endregion

            return yyresult;
        }

        #endregion

        #region Utilities

        private string AddTable(string list, string table)
        {
            string str = $"{list} \0{table}";
            Mem.AddLast(str);
            return str;
        }

        private column_info AllocColumn(string table, string column)
        {
            column_info col = new column_info
            {
                Table = table,
                Column = column,
                Val = null,
                Type = 0,
                Next = null,
            };

            Mem.AddLast(col);
            return col;
        }

        private sql_tokentype Lexer(ref SQL_STYPE SQL_lval)
        {
            sql_tokentype token;
            do
            {
                N += Len;
                if (N >= Command.Length || Command[N] == '\0')
                    return 0;  // End of input

                Len = Tokenize.GetToken(Command, N, out token, out int skip);
                if (Len == 0)
                    break;

                SQL_lval.Str = new sql_str
                {
                    Data = Command.Substring(N, Len),
                    Len = Len,
                };

                N += skip;
            }
            while (token == sql_tokentype.TK_SPACE);

            return token;
        }

        private LibmsiResult UnescapeString(sql_str strdata, out string str)
        {
            str = null;
            int p = 0; // strdata.Data[0]
            int len = strdata.Len;

            // Match quotes
            if (((strdata.Data[0]=='`') && (strdata.Data[len - 1]!='`')) || ((strdata.Data[0]=='\'') && (strdata.Data[len - 1]!='\'')))
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;

            // If there's quotes, remove them
            if (((strdata.Data[0]=='`') && (strdata.Data[len - 1]=='`')) || ((strdata.Data[0]=='\'') && (strdata.Data[len - 1]=='\'')))
            {
                p++;
                len -= 2;
            }

            str = strdata.Data.Substring(p, len);
            Mem.AddLast(str);
            return LibmsiResult.LIBMSI_RESULT_SUCCESS;
        }

        private int StringToInt()
        {
            int r = 0;
            for (int i = 0; i < Len; i++)
            {
                if ('0' > Command[N + i] || '9' < Command[N + i])
                {
                    Console.Error.WriteLine("Should only be numbers here!");
                    break;
                }

                r = (Command[N + i] - '0') + r * 10;
            }

            return r;
        }

        private expr BuildExprWildcard()
        {
            expr e = new expr { Type = EXPR_WILDCARD };
            Mem.AddLast(e);
            return e;
        }

        private expr BuildExprComplex(expr l, int op, expr r)
        {
            expr e = new expr
            {
                Type = EXPR_COMPLEX,
                Expr = new complex_expr
                {
                    Left = l,
                    Op = op,
                    Right = r,
                }
            };
            Mem.AddLast(e);
            return e;
        }

        private expr BuildExprUnary(expr l, int op)
        {
            expr e = new expr
            {
                Type = EXPR_UNARY,
                Expr = new complex_expr
                {
                    Left = l,
                    Op = op,
                    Right = null,
                }
            };
            Mem.AddLast(e);
            return e;
        }

        private expr BuildExprColumn(column_info column)
        {
            expr e = new expr
            {
                Type = EXPR_COLUMN,
                Column = new ext_column
                {
                    Unparsed = new Tuple<string, string>(column.Column, column.Table),
                }
            };
            Mem.AddLast(e);
            return e;
        }

        private expr BuildExprIVal(int val)
        {
            expr e = new expr
            {
                Type = EXPR_IVAL,
                IVal = val,
            };
            Mem.AddLast(e);
            return e;
        }

        private expr BuildExprSVal(sql_str str)
        {
            if (UnescapeString(str, out string unescaped) != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                return null;

            expr e = new expr
            {
                Type = EXPR_SVAL,
                SVal = unescaped,
            };
            Mem.AddLast(e);
            return e;
        }

        private void SwapColumns(ref column_info cols, column_info A, int idx)
        {
            int i = 0;

            column_info ptr = cols, preA = null, preB = null, B = null;
            while (ptr != null)
            {
                if (i++ == idx)
                    B = ptr;
                else if (B != null)
                    preB = ptr;

                if (ptr.Next == A)
                    preA = ptr;

                ptr = ptr.Next;
            }

            if (preB != null)
                preB.Next = A;
            if (preA != null)
                preA.Next = B;

            ptr = A.Next;
            A.Next = B.Next;
            B.Next = ptr;

            if (idx == 0)
                cols = A;
        }

        private bool MarkPrimaryKeys(ref column_info cols, column_info keys)
        {
            column_info k;
            bool found = true;
            int count;

            for (k = keys, count = 0; k != null && found; k = k.Next, count++)
            {
                column_info c;
                int idx;

                found = false;
                for (c = cols, idx = 0; c != null && !found; c = c.Next, idx++)
                {
                    if (k.Column != c.Column)
                        continue;

                    c.Type |= MSITYPE_KEY;
                    found = true;
                    if (idx != count)
                        SwapColumns(ref cols, c, count);
                }
            }

            return found;
        }

        #endregion
    }
}