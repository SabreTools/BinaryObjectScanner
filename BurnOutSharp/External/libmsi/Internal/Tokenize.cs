/*
** 2001 September 15
**
** The author disclaims copyright to this source code.  In place of
** a legal notice, here is a blessing:
**
**    May you do good and not evil.
**    May you find forgiveness for yourself and forgive others.
**    May you share freely, never taking more than you give.
**
*************************************************************************
** A tokenizer for SQL
**
** This file contains C code that splits an SQL input string up into
** individual tokens and sends those tokens one-by-one over to the
** parser for analysis.
*/

using System.Linq;

namespace LibMSI.Internal
{
    /// <summary>
    /// The token value for this keyword
    /// </summary>
    internal class Keyword
    {
        /// <summary>
        /// All the keywords of the SQL language are stored as in a hash
        /// table composed of instances of the following structure.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The token value for this keyword
        /// </summary>
        public sql_tokentype TokenType { get; set; }
    }

    internal class Tokenize
    {
        #region Constants

        public const int MAX_TOKEN_LEN = 11;

        /// <summary>
        /// These are the keywords
        /// They MUST be in alphabetical order
        /// </summary>
        public static readonly Keyword[] KeywordTable = new Keyword[]
        {
            new Keyword { Name = "ADD",         TokenType = sql_tokentype.TK_ADD },
            new Keyword { Name = "ALTER",       TokenType = sql_tokentype.TK_ALTER },
            new Keyword { Name = "AND",         TokenType = sql_tokentype.TK_AND },
            new Keyword { Name = "BY",          TokenType = sql_tokentype.TK_BY },
            new Keyword { Name = "CHAR",        TokenType = sql_tokentype.TK_CHAR },
            new Keyword { Name = "CHARACTER",   TokenType = sql_tokentype.TK_CHAR },
            new Keyword { Name = "CREATE",      TokenType = sql_tokentype.TK_CREATE },
            new Keyword { Name = "DELETE",      TokenType = sql_tokentype.TK_DELETE },
            new Keyword { Name = "DISTINCT",    TokenType = sql_tokentype.TK_DISTINCT },
            new Keyword { Name = "DROP",        TokenType = sql_tokentype.TK_DROP },
            new Keyword { Name = "FREE",        TokenType = sql_tokentype.TK_FREE },
            new Keyword { Name = "FROM",        TokenType = sql_tokentype.TK_FROM },
            new Keyword { Name = "HOLD",        TokenType = sql_tokentype.TK_HOLD },
            new Keyword { Name = "INSERT",      TokenType = sql_tokentype.TK_INSERT },
            new Keyword { Name = "INT",         TokenType = sql_tokentype.TK_INT },
            new Keyword { Name = "INTEGER",     TokenType = sql_tokentype.TK_INT },
            new Keyword { Name = "INTO",        TokenType = sql_tokentype.TK_INTO },
            new Keyword { Name = "IS",          TokenType = sql_tokentype.TK_IS },
            new Keyword { Name = "KEY",         TokenType = sql_tokentype.TK_KEY },
            new Keyword { Name = "LIKE",        TokenType = sql_tokentype.TK_LIKE },
            new Keyword { Name = "LOCALIZABLE", TokenType = sql_tokentype.TK_LOCALIZABLE },
            new Keyword { Name = "LONG",        TokenType = sql_tokentype.TK_LONG },
            new Keyword { Name = "LONGCHAR",    TokenType = sql_tokentype.TK_LONGCHAR },
            new Keyword { Name = "NOT",         TokenType = sql_tokentype.TK_NOT },
            new Keyword { Name = "NULL",        TokenType = sql_tokentype.TK_NULL },
            new Keyword { Name = "OBJECT",      TokenType = sql_tokentype.TK_OBJECT },
            new Keyword { Name = "OR",          TokenType = sql_tokentype.TK_OR },
            new Keyword { Name = "ORDER",       TokenType = sql_tokentype.TK_ORDER },
            new Keyword { Name = "PRIMARY",     TokenType = sql_tokentype.TK_PRIMARY },
            new Keyword { Name = "SELECT",      TokenType = sql_tokentype.TK_SELECT },
            new Keyword { Name = "SET",         TokenType = sql_tokentype.TK_SET },
            new Keyword { Name = "SHORT",       TokenType = sql_tokentype.TK_SHORT },
            new Keyword { Name = "TABLE",       TokenType = sql_tokentype.TK_TABLE },
            new Keyword { Name = "TEMPORARY",   TokenType = sql_tokentype.TK_TEMPORARY },
            new Keyword { Name = "UPDATE",      TokenType = sql_tokentype.TK_UPDATE },
            new Keyword { Name = "VALUES",      TokenType = sql_tokentype.TK_VALUES },
            new Keyword { Name = "WHERE",       TokenType = sql_tokentype.TK_WHERE },
        };

        public static readonly int KEYWORD_COUNT = KeywordTable.Length;

        /// <summary>
        /// If X is a character that can be used in an identifier then
        /// IsIdChar[X] will be 1.  Otherwise IsIdChar[X] will be 0.
        /// 
        /// In this implementation, an identifier can be a string of
        /// alphabetic characters, digits, and "_" plus any character
        /// with the high-order bit set.  The latter rule means that
        /// any sequence of UTF-8 characters or characters taken from
        /// an extended ISO8859 character set can form an identifier.
        /// </summary>
        public static readonly byte[] IsIdChar = new byte[]
        {
            /* x0 x1 x2 x3 x4 x5 x6 x7 x8 x9 xA xB xC xD xE xF */
                0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,  /* 0x */
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,  /* 1x */
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0,  /* 2x */
                1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0,  /* 3x */
                0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,  /* 4x */
                1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 1,  /* 5x */
                0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,  /* 6x */
                1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0,  /* 7x */
                1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,  /* 8x */
                1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,  /* 9x */
                1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,  /* Ax */
                1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,  /* Bx */
                1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,  /* Cx */
                1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,  /* Dx */
                1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,  /* Ex */
                1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,  /* Fx */
        };

        #endregion

        #region Functions

        /// <summary>
        /// Comparison function for binary search.
        /// </summary>
        public static int CompareKeyword(string m1, Keyword m2)
        {
            int p = 0, q = 0;
            for (; p < m1.Length && m1[p] != '\0'; p++, q++)
            {
                if ((ushort)m1[p] > 127)
                    return 1;

                char c = m1[p];
                if (c >= 'a' && c <= 'z')
                    c ^= (char)('A' ^ 'a');

                if (c != m2.Name[q])
                    return (int)c - (int)m2.Name[q];
            }

            return (int)m1[p] - (int)m2.Name[q];
        }

        /// <summary>
        /// This function looks up an identifier to determine if it is a
        /// keyword.  If it is a keyword, the token code of that keyword is 
        /// returned.  If the input is not a keyword, TK_ID is returned.
        /// </summary>
        public static sql_tokentype FindKeyword(string z, int n)
        {
            if (n > MAX_TOKEN_LEN)
                return sql_tokentype.TK_ID;

            string str = z.Substring(n) + '\0';
            Keyword r = KeywordTable.FirstOrDefault(k => CompareKeyword(str, k) == 0);
            if (r != default)
                return r.TokenType;

            return sql_tokentype.TK_ID;
        }

        /// <summary>
        /// Return the length of the token that begins at <paramref name="zz"/>[0].  Return
        /// -1 if the token is (or might be) incomplete.  Store the token
        /// type in *tokenType before returning.
        /// </summary>
        public static int GetToken(string zz, int ptr, out sql_tokentype tokenType, out int skip)
        {
            int z = ptr; // zz[0]
            int i = 0;

            skip = 0;
            switch (zz[z])
            {
                // TK_SPACE
                case ' ': case '\t': case '\n': case '\f':
                    for (i = 1; char.IsWhiteSpace(zz[z + i]) && zz[z + i] != '\r'; i++);
                    tokenType = sql_tokentype.TK_SPACE;
                    return i;

                // TK_MINUS
                case '-':
                    if (z + 1 >= zz.Length || zz[z + 1] == 0)
                    {
                        tokenType = sql_tokentype.TK_ILLEGAL;
                        return -1;
                    }

                    tokenType = sql_tokentype.TK_MINUS;
                    return 1;

                // TK_LP
                case '(':
                    tokenType = sql_tokentype.TK_LP;
                    return 1;

                // TK_RP
                case ')':
                    tokenType = sql_tokentype.TK_RP;
                    return 1;

                // TK_STAR
                case '*':
                    tokenType = sql_tokentype.TK_STAR;
                    return 1;

                // TK_EQ
                case '=':
                    tokenType = sql_tokentype.TK_EQ;
                    return 1;

                // TK_LT, TK_LE, TK_NE
                case '<':
                    if (zz[z + 1] == '=')
                    {
                        tokenType = sql_tokentype.TK_LE;
                        return 2;
                    }
                    else if (zz[z + 1] == '>')
                    {
                        tokenType = sql_tokentype.TK_NE;
                        return 2;
                    }
                    else
                    {
                        tokenType = sql_tokentype.TK_LT;
                        return 1;
                    }

                // TK_GT, TK_GE
                case '>':
                    if (zz[z + 1] == '=')
                    {
                        tokenType = sql_tokentype.TK_GE;
                        return 2;
                    }
                    else
                    {
                        tokenType = sql_tokentype.TK_GT;
                        return 1;
                    }

                // TK_NE
                case '!':
                    if (zz[z + 1] != '=')
                    {
                        tokenType = sql_tokentype.TK_ILLEGAL;
                        return 2;
                    }
                    else
                    {
                        tokenType = sql_tokentype.TK_NE;
                        return 2;
                    }

                // TK_WILDCARD
                case '?':
                    tokenType = sql_tokentype.TK_WILDCARD;
                    return 1;

                // TK_COMMA
                case ',':
                    tokenType = sql_tokentype.TK_COMMA;
                    return 1;

                // TK_ID, TK_STRING
                case '`': case '\'':
                {
                    int delim = zz[z];
                    for (i = 1; i < zz.Length && zz[z + i] != 0; i++)
                    {
                        if (zz[z + i] == delim)
                            break;
                    }

                    if (zz[z + i] != 0)
                        i++;

                    if (delim == '`')
                        tokenType = sql_tokentype.TK_ID;
                    else
                        tokenType = sql_tokentype.TK_STRING;

                    return i;
                }

                // TK_DOT, TK_INTEGER
                case '.':
                    if (!char.IsDigit(zz[z + 1]))
                    {
                        tokenType = sql_tokentype.TK_DOT;
                        return 1;
                    }

                    tokenType = sql_tokentype.TK_INTEGER;
                    for (i = 1; char.IsDigit(zz[z + i]); i++);
                    return i;

                // TK_INTEGER
                case '0': case '1': case '2': case '3': case '4':
                case '5': case '6': case '7': case '8': case '9':
                    tokenType = sql_tokentype.TK_INTEGER;
                    for (i = 1; char.IsDigit(zz[z + i]); i++);
                    return i;

                // TK_ID
                case '[':
                    for(i = 1; zz[z + i] != 0 && zz[z + i - 1] != ']'; i++);
                    tokenType = sql_tokentype.TK_ID;
                    return i;

                default:
                    if (IsIdChar[zz[z]] == 0)
                        break;

                    for (i = 1; IsIdChar[zz[z + i]] != 0; i++);
                    tokenType = FindKeyword(zz, z + i);
                    if (tokenType == sql_tokentype.TK_ID && zz[z + i] == '`')
                        skip = 1;

                    return i;
            }

            tokenType = sql_tokentype.TK_ILLEGAL;
            return 1;
        }

        #endregion
    }
}