/*
 * Implementation of the Microsoft Installer (msi.dll)
 *
 * Copyright 2002 Mike McCormack for CodeWeavers
 * Copyright 2011 Bernhard Loos
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
using System.Linq;
using LibGSF.Input;
using static LibMSI.LibmsiQuery;
using static LibMSI.LibmsiRecord;
using static LibMSI.LibmsiSQLInput;
using static LibMSI.MsiPriv;

namespace LibMSI
{
    internal class LibmsiRowEntry
    {
        /// <summary>
        /// Used during sorting
        /// </summary>
        public LibmsiWhereView WhereView { get; set; }

        public int[] Values { get; set; } = new int[1];
    }

    internal class JOINTABLE
    {
        public JOINTABLE Next { get; set; }

        public LibmsiView View { get; set; }

        public int ColCount { get; set; }

        public int RowCount { get; set; }

        public int TableIndex { get; set; }
    }

    internal class LibmsiOrderInfo
    {
        public int ColCount { get; set; }

        public LibmsiResult Error { get; set; }

        public ext_column[] Columns { get; set; } = new ext_column[1];
    }

    internal class LibmsiWhereView : LibmsiView
    {
        #region Constants

        private const int INITIAL_REORDER_SIZE = 16;

        private const int INVALID_ROW_INDEX = -1;

        /// <summary>
        /// Comparison to a constant value
        /// </summary>
        private const int CONST_EXPR = 1;

        /// <summary>
        /// Comparison to a table involved with a CONST_EXPR comaprison
        /// </summary>
        private const int JOIN_TO_CONST_EXPR = 0x10000;

        #endregion

        #region Properties

        public LibmsiDatabase Database { get; set; }

        public JOINTABLE Tables { get; set; }

        public int RowCount { get; set; }

        public int ColCount { get; set; }

        public int TableCount { get; set; }

        public LibmsiRowEntry[] Reorder { get; set; }

        /// <summary>
        /// Number of entries available in reorder
        /// </summary>
        public int ReorderSize { get; set; }

        public expr Condition { get; set; }

        public int RecIndex { get; set; }

        public LibmsiOrderInfo OrderInfo { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Private constructor
        /// </summary>
        private LibmsiWhereView() { }

        public static LibmsiResult Create(LibmsiDatabase db, out LibmsiView view, string tables, expr cond)
        {
            view = null;

            LibmsiWhereView wv = new LibmsiWhereView
            {
                Database = db,
                Condition = cond,
            };

            LibmsiResult r;
            int tablesPtr = 0; // tables[0]
            while (tablesPtr < tables.Length && tables[tablesPtr] != '\0')
            {
                int ptr = tables.IndexOf(' ', tablesPtr);
                if (ptr != -1)
                {
                    char[] temp = tables.ToCharArray();
                    temp[ptr] = '\0';
                    tables = new string(temp);
                }

                JOINTABLE table = new JOINTABLE();
                r = LibmsiTableView.Create(db, tables, out LibmsiView table_view);
                if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                {
                    Console.Error.WriteLine($"Can't create table: {tables}");
                    wv.Delete();
                    return LibmsiResult.LIBMSI_RESULT_BAD_QUERY_SYNTAX;
                }

                table.View = table_view;
                r = table.View.GetDimensions(out _, out int col_count);
                if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                {
                    Console.Error.WriteLine("Can't get table dimensions");
                    wv.Delete();
                    return r;
                }

                table.ColCount = col_count;
                wv.ColCount += table.ColCount;
                table.TableIndex = wv.TableCount++;

                table.Next = wv.Tables;
                wv.Tables = table;

                if (ptr == -1)
                    break;

                tablesPtr = ptr + 1;
            }

            bool valid = false;
            if (cond != null)
            {
                r = wv.VerifyCondition(cond, ref valid);
                if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                {
                    wv.Delete();
                    return r;
                }

                if (!valid)
                {
                    wv.Delete();
                    return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;;
                }
            }

            view = wv;
            return LibmsiResult.LIBMSI_RESULT_SUCCESS;
        }
        
        #endregion

        #region Functions

        /// <inheritdoc/>
        public override LibmsiResult FetchInt(int row, int col, out int val)
        {
            val = 0;
            if (Tables == null)
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;

            LibmsiResult r = FindRow(row, out int[] rows);
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                return r;

            JOINTABLE table = FindTable(col, out col);
            if (table == null)
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;

            return table.View.FetchInt(rows[table.TableIndex], col, out val);
        }

        /// <inheritdoc/>
        public override LibmsiResult FetchStream(int row, int col, out GsfInput stm)
        {
            stm = null;
            if (Tables == null)
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;

            LibmsiResult r = FindRow(row, out int[] rows);
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                return r;

            JOINTABLE table = FindTable(col, out col);
            if (table == null)
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;

            return table.View.FetchStream(rows[table.TableIndex], col, out stm);
        }

        /// <inheritdoc/>
        public override LibmsiResult GetRow(int row, out LibmsiRecord rec)
        {
            rec = null;
            if (Tables == null)
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;

            return MsiViewGetRow(Database, this, row, out rec);
        }

        /// <inheritdoc/>
        public override LibmsiResult SetRow(int row, LibmsiRecord rec, int mask)
        {
            if (Tables == null)
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;

            LibmsiResult r = FindRow(row, out int[] rows);
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                return r;

            if (mask >= 1 << ColCount)
                return LibmsiResult.LIBMSI_RESULT_INVALID_PARAMETER;

            JOINTABLE table = Tables;
            int mask_copy = mask;
            do
            {
                for (int i = 0; i < table.ColCount; i++)
                {
                    if ((mask_copy & (1 << i)) == 0)
                        continue;

                    r = table.View.GetColumnInfo(i + 1, out _, out int type, out _, out _);
                    if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                        return r;

                    if ((type & MSITYPE_KEY) != 0)
                        return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;
                }

                mask_copy >>= table.ColCount;
            }
            while (mask_copy != 0 && (table = table.Next) != null);

            table = Tables;

            int offset = 0;
            do
            {
                int col_count = table.ColCount;
                int reduced_mask = (mask >> offset) & ((1 << col_count) - 1);

                if (reduced_mask == 0)
                {
                    offset += col_count;
                    continue;
                }

                LibmsiRecord reduced = LibmsiRecord.Create(col_count);
                if (reduced == null)
                    return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;

                for (int i = 1; i <= col_count; i++)
                {
                    r = RecordCopyField(rec, i + offset, reduced, i);
                    if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                        break;
                }

                offset += col_count;

                if (r == LibmsiResult.LIBMSI_RESULT_SUCCESS)
                    r = table.View.SetRow(rows[table.TableIndex], reduced, reduced_mask);

            }
            while ((table = table.Next) != null);

            return r;
        }

        /// <inheritdoc/>
        public override LibmsiResult DeleteRow(int row)
        {
            if (Tables == null)
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;

            LibmsiResult r = FindRow(row, out int[] rows);
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                return r;

            if (TableCount > 1)
                return LibmsiResult.LIBMSI_RESULT_CALL_NOT_IMPLEMENTED;

            return Tables.View.DeleteRow(rows[0]);
        }

        /// <inheritdoc/>
        public override LibmsiResult Execute(LibmsiRecord record)
        {
            JOINTABLE table = Tables;
            if (table == null)
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;

            LibmsiResult r = InitReorder();
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                return r;

            do
            {
                table.View.Execute(null);
                r = table.View.GetDimensions(out int row_count, out _);
                if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                {
                    Console.Error.WriteLine("Failed to get table dimensions");
                    return r;
                }

                table.RowCount = row_count;

                // Each table must have at least one row
                if (table.RowCount == 0)
                    return LibmsiResult.LIBMSI_RESULT_SUCCESS;
            }
            while ((table = table.Next) != null);

            JOINTABLE[] ordered_tables = OrderTables();

            int[] rows = new int[TableCount];
            for (int i = 0; i < TableCount; i++)
            {
                rows[i] = INVALID_ROW_INDEX;
            }

            r =  CheckCondition(record, ordered_tables, rows);
            if (OrderInfo != null)
                OrderInfo.Error = LibmsiResult.LIBMSI_RESULT_SUCCESS;

            LibmsiRowEntry[] temp = Reorder;
            Array.Sort(temp, CompareEntry);
            Reorder = temp;

            if (OrderInfo != null)
                r = OrderInfo.Error;

            return r;
        }

        /// <inheritdoc/>
        public override LibmsiResult Close()
        {
            JOINTABLE table = Tables;
            if (table == null)
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;

            do
            {
                table.View.Close();
            }
            while ((table = table.Next) != null);

            return LibmsiResult.LIBMSI_RESULT_SUCCESS;
        }

        /// <inheritdoc/>
        public override LibmsiResult GetDimensions(out int rows, out int cols)
        {
            rows = 0; cols = 0;
            if (Tables == null)
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;

            if (Reorder == null)
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;

            rows = RowCount;
            cols = ColCount;

            return LibmsiResult.LIBMSI_RESULT_SUCCESS;
        }

        /// <inheritdoc/>
        public override LibmsiResult GetColumnInfo(int n, out string name, out int type, out bool temporary, out string table_name)
        {
            name = null; type = 0; temporary = false; table_name = null;
            if (Tables == null)
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;

            JOINTABLE table = FindTable(n, out n);
            if (table == null)
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;

            return table.View.GetColumnInfo(n, out name, out type, out temporary, out table_name);
        }

        /// <inheritdoc/>
        public override LibmsiResult Delete()
        {
            JOINTABLE table = Tables;
            while (table != null)
            {
                table.View.Delete();
                table.View = null;
                JOINTABLE next = table.Next;
                table = next;
            }

            Tables = null;
            TableCount = 0;

            FreeReorder();

            OrderInfo = null;

            return LibmsiResult.LIBMSI_RESULT_SUCCESS;
        }

        /// <inheritdoc/>
        public override LibmsiResult FindMatchingRows(int col, int val, out int row, ref LibmsiColumnHashEntry handle)
        {
            row = 0;
            if (Tables == null)
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;

            if (col == 0 || col > ColCount)
                return LibmsiResult.LIBMSI_RESULT_INVALID_PARAMETER;

            for (int i = handle.Row; i < RowCount; i++)
            {
                if (FetchInt(i, col, out int row_value) != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                    continue;

                if (row_value == val)
                {
                    row = i;
                    handle = handle.Next;
                    return LibmsiResult.LIBMSI_RESULT_SUCCESS;
                }
            }

            return LibmsiResult.NO_MORE_ITEMS;
        }

        /// <inheritdoc/>
        public override LibmsiResult Sort(column_info columns)
        {
            JOINTABLE table = Tables;
            if (table == null)
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;

            int count = 0;
            column_info column = columns;
            while (column != null)
            {
                count++;
                column = column.Next;
            }

            if (count == 0)
                return LibmsiResult.LIBMSI_RESULT_SUCCESS;

            LibmsiOrderInfo orderinfo = new LibmsiOrderInfo
            {
                ColCount = count,
            };

            column = columns;

            for (int i = 0; i < count; i++)
            {
                orderinfo.Columns[i].Unparsed = new Tuple<string, string>(column.Column, column.Table);
                LibmsiResult r = ParseColumn(orderinfo.Columns[i], out _);
                if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                    return r;
            }

            OrderInfo = orderinfo;

            return LibmsiResult.LIBMSI_RESULT_SUCCESS;
        }

        #endregion

        #region Utilities

        private void FreeReorder()
        {
            if (Reorder == null)
                return;

            Reorder = null;
            ReorderSize = 0;
            RowCount = 0;
        }

        private LibmsiResult InitReorder()
        {
            LibmsiRowEntry[] nre = new LibmsiRowEntry[INITIAL_REORDER_SIZE];
            FreeReorder();

            Reorder = nre;
            ReorderSize = INITIAL_REORDER_SIZE;

            return LibmsiResult.LIBMSI_RESULT_SUCCESS;
        }

        private LibmsiResult FindRow(int row, out int[] values)
        {
            values = null;
            if (row >= RowCount)
                return LibmsiResult.NO_MORE_ITEMS;

            values = Reorder[row].Values;
            return LibmsiResult.LIBMSI_RESULT_SUCCESS;
        }

        private LibmsiResult AddRow(int[] vals)
        {
            if (ReorderSize <= RowCount)
            {
                int newsize = ReorderSize * 2;
                LibmsiRowEntry[] new_reorder = new LibmsiRowEntry[newsize];
                Array.Copy(Reorder, new_reorder, ReorderSize);
                Reorder = new_reorder;
                ReorderSize = newsize;
            }

            LibmsiRowEntry nre = new LibmsiRowEntry
            {
                Values = new int[TableCount],
            };

            Reorder[RowCount++] = nre;
            Array.Copy(vals, nre.Values, TableCount);
            nre.WhereView = this;

            return LibmsiResult.LIBMSI_RESULT_SUCCESS;
        }

        private JOINTABLE FindTable(int col, out int table_col)
        {
            table_col = 0;
            JOINTABLE table = Tables;
            if (col == 0 || col > ColCount)
                return null;

            while (col > table.ColCount)
            {
                col -= table.ColCount;
                table = table.Next;
                if (table == null)
                    return null;
            }

            table_col = col;
            return table;
        }

        private LibmsiResult ParseColumn(ext_column column, out int column_type)
        {
            column_type = 0;
            JOINTABLE table = Tables;
            LibmsiResult r;

            do
            {
                string table_name;
                if (column.Unparsed.Item2 != null)
                {
                    r = table.View.GetColumnInfo(1, out _, out _, out _, out table_name);
                    if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                        return r;

                    if (table_name != column.Unparsed.Item2)
                        continue;
                }

                for (int i = 1; i <= table.ColCount; i++)
                {
                    r = table.View.GetColumnInfo(i, out string col_name, out column_type, out _, out _);
                    if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                        return r;

                    if (col_name != column.Unparsed.Item1)
                        continue;

                    column.Parsed = new Tuple<int, JOINTABLE>(i, table);
                    return LibmsiResult.LIBMSI_RESULT_SUCCESS;
                }
            }
            while ((table = table.Next) != null);

            Console.Error.WriteLine($"Couldn't find column {column.Unparsed.Item2}.{column.Unparsed.Item1}");
            return LibmsiResult.LIBMSI_RESULT_BAD_QUERY_SYNTAX;
        }

        /// <summary>
        /// Reorders the tablelist in a way to evaluate the condition as fast as possible
        /// </summary>
        private JOINTABLE[] OrderTables()
        {
            JOINTABLE table;
            JOINTABLE[] tables = new JOINTABLE[TableCount + 1];
            if (Condition != null)
            {
                table = null;
                ReorderCheck(Condition, tables, false, ref table);
                table = null;
                ReorderCheck(Condition, tables, true, ref table);
            }

            table = Tables;
            while (table != null)
            {
                AddToArray(tables, 0, table);
                table = table.Next;
            }
            return tables;
        }

        private LibmsiResult ExprEvalBinary(int [] rows, complex_expr expr, out int val, LibmsiRecord record)
        {
            val = 0;
            LibmsiResult rl = Evaluate(rows, expr.Left, out int lval, record);
            if (rl != LibmsiResult.LIBMSI_RESULT_SUCCESS && rl != LibmsiResult.LIBMSI_RESULT_CONTINUE)
                return rl;
            LibmsiResult rr = Evaluate(rows, expr.Right, out int rval, record);
            if (rr != LibmsiResult.LIBMSI_RESULT_SUCCESS && rr != LibmsiResult.LIBMSI_RESULT_CONTINUE)
                return rr;

            if (rl == LibmsiResult.LIBMSI_RESULT_CONTINUE || rr == LibmsiResult.LIBMSI_RESULT_CONTINUE)
            {
                if (rl == rr)
                {
                    val = 1;
                    return LibmsiResult.LIBMSI_RESULT_CONTINUE;
                }

                if (expr.Op == OP_AND)
                {
                    if ((rl == LibmsiResult.LIBMSI_RESULT_CONTINUE && rval == 0) || (rr == LibmsiResult.LIBMSI_RESULT_CONTINUE && lval == 0))
                    {
                        val = 0;
                        return LibmsiResult.LIBMSI_RESULT_SUCCESS;
                    }
                }
                else if (expr.Op == OP_OR)
                {
                    if ((rl == LibmsiResult.LIBMSI_RESULT_CONTINUE && rval != 0) || (rr == LibmsiResult.LIBMSI_RESULT_CONTINUE && lval != 0))
                    {
                        val = 1;
                        return LibmsiResult.LIBMSI_RESULT_SUCCESS;
                    }
                }

                val = 1;
                return LibmsiResult.LIBMSI_RESULT_CONTINUE;
            }

            switch (expr.Op)
            {
                case OP_EQ:
                    val = (lval == rval) ? 1 : 0;
                    break;
                case OP_AND:
                    val = (lval != 0 && rval != 0) ? 1 : 0;;
                    break;
                case OP_OR:
                    val = (lval != 0 || rval != 0) ? 1 : 0;;
                    break;
                case OP_GT:
                    val = (lval > rval) ? 1 : 0;;
                    break;
                case OP_LT:
                    val = (lval < rval) ? 1 : 0;;
                    break;
                case OP_LE:
                    val = (lval <= rval) ? 1 : 0;;
                    break;
                case OP_GE:
                    val = (lval >= rval) ? 1 : 0;;
                    break;
                case OP_NE:
                    val = (lval != rval) ? 1 : 0;;
                    break;
                default:
                    Console.Error.WriteLine($"Unknown operator {expr.Op}");
                    return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;
            }

            return LibmsiResult.LIBMSI_RESULT_SUCCESS;
        }

        private static LibmsiResult ExprFetchValue(ext_column expr, int[] rows, out int val)
        {
            JOINTABLE table = expr.Parsed.Item2;
            if (rows[table.TableIndex] == INVALID_ROW_INDEX)
            {
                val = 1;
                return LibmsiResult.LIBMSI_RESULT_CONTINUE;
            }

            return table.View.FetchInt(rows[table.TableIndex], expr.Parsed.Item1, out val);
        }

        private LibmsiResult ExprEvalUnary(int[] rows, complex_expr expr, out int val, LibmsiRecord record)
        {
            val = 0;
            LibmsiResult r = ExprFetchValue(expr.Left.Column, rows, out int lval);
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                return r;

            switch (expr.Op)
            {
                case OP_ISNULL:
                    val = (lval == 0) ? 1 : 0;;
                    break;
                case OP_NOTNULL:
                    val = (lval != 0) ? 1 : 0;;
                    break;
                default:
                    Console.Error.WriteLine($"Unknown operator {expr.Op}");
                    return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;
            }

            return LibmsiResult.LIBMSI_RESULT_SUCCESS;
        }

        private LibmsiResult ExprEvalString(int[] rows, expr expr, LibmsiRecord record, out string str)
        {
            LibmsiResult r = LibmsiResult.LIBMSI_RESULT_SUCCESS;
            switch (expr.Type)
            {
                case EXPR_COL_NUMBER_STRING:
                    r = ExprFetchValue(expr.Column, rows, out int val);
                    if (r == LibmsiResult.LIBMSI_RESULT_SUCCESS)
                        str = Database.Strings.LookupId(val);
                    else
                        str = null;

                    break;

                case EXPR_SVAL:
                    str = expr.SVal;
                    break;

                case EXPR_WILDCARD:
                    str = RecordGetStringRaw(record, ++RecIndex);
                    break;

                default:
                    Console.Error.WriteLine("Invalid expression type");
                    r = LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;
                    str = null;
                    break;
            }

            return r;
        }

        private LibmsiResult ExprEvlStrCmp(int[] rows, complex_expr expr, out int val, LibmsiRecord record)
        {
            val = 1;
            LibmsiResult r = ExprEvalString(rows, expr.Left, record, out string l_str);
            if (r == LibmsiResult.LIBMSI_RESULT_CONTINUE)
                return r;

            r = ExprEvalString(rows, expr.Right, record, out string r_str);
            if (r == LibmsiResult.LIBMSI_RESULT_CONTINUE)
                return r;

            int sr;
            if (l_str == r_str || (string.IsNullOrEmpty(l_str) && string.IsNullOrEmpty(r_str)))
                sr = 0;
            else if (l_str != null && r_str == null)
                sr = 1;
            else if (r_str != null && l_str == null)
                sr = -1;
            else
                sr = l_str.CompareTo(r_str);

            val = (expr.Op == OP_EQ && (sr == 0)) || (expr.Op == OP_NE && (sr != 0)) ? 1 : 0;;

            return LibmsiResult.LIBMSI_RESULT_SUCCESS;
        }

        private LibmsiResult Evaluate(int[] rows, expr cond, out int val, LibmsiRecord record)
        {
            val = 0;
            if (cond == null)
            {
                val = 1;
                return LibmsiResult.LIBMSI_RESULT_SUCCESS;
            }

            LibmsiResult r;
            int tval;
            switch (cond.Type)
            {
                case EXPR_COL_NUMBER:
                    r = ExprFetchValue(cond.Column, rows, out tval);
                    if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                        return r;

                    val = tval - 0x8000;
                    return LibmsiResult.LIBMSI_RESULT_SUCCESS;

                case EXPR_COL_NUMBER32:
                    r = ExprFetchValue(cond.Column, rows, out tval);
                    if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                        return r;

                    unchecked { val = tval - (int)0x80000000; }
                    return r;

                case EXPR_UVAL:
                    val = (int)cond.UVal;
                    return LibmsiResult.LIBMSI_RESULT_SUCCESS;

                case EXPR_COMPLEX:
                    return ExprEvalBinary(rows, cond.Expr, out val, record);

                case EXPR_UNARY:
                    return ExprEvalUnary(rows, cond.Expr, out val, record);

                case EXPR_STRCMP:
                    return ExprEvlStrCmp(rows, cond.Expr, out val, record);

                case EXPR_WILDCARD:
                    val = record.GetInt(++RecIndex);
                    return LibmsiResult.LIBMSI_RESULT_SUCCESS;

                default:
                    Console.Error.WriteLine("Invalid expression type");
                    break;
            }

            return LibmsiResult.LIBMSI_RESULT_SUCCESS;
        }

        private LibmsiResult CheckCondition(LibmsiRecord record, JOINTABLE[] tables, int[] table_rows)
        {
            LibmsiResult r = LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;

            int tablesPtr = 0; // tables[0]
            for (table_rows[tables[tablesPtr].TableIndex] = 0;
                table_rows[tables[tablesPtr].TableIndex] < tables[tablesPtr].RowCount;
                table_rows[tables[tablesPtr].TableIndex]++)
            {
                RecIndex = 0;
                r = Evaluate(table_rows, Condition, out int val, record );
                if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS && r != LibmsiResult.LIBMSI_RESULT_CONTINUE)
                    break;

                if (val != 0)
                {
                    if (tablesPtr < tables.Length - 1 && tables[tablesPtr + 1] != null)
                    {
                        r = CheckCondition(record, tables.Skip(tablesPtr + 1).ToArray(), table_rows);
                        if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                            break;
                    }
                    else
                    {
                        if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                            break;

                        AddRow(table_rows);
                    }
                }
            }

            table_rows[tables[tablesPtr].TableIndex] = INVALID_ROW_INDEX;
            return r;
        }

        private static int CompareEntry(object left, object right)
        {
            LibmsiRowEntry le = (LibmsiRowEntry)left;
            LibmsiRowEntry re = (LibmsiRowEntry)right;
            LibmsiWhereView wv = le.WhereView;
            LibmsiOrderInfo order = wv.OrderInfo;

            // TODO: Re-enable this assert
            // assert(le.wv == re.wv);

            if (order != null)
            {
                for (int i = 0; i < order.ColCount; i++)
                {
                    ext_column column = order.Columns[i];
                    LibmsiResult r = column.Parsed.Item2.View.FetchInt(le.Values[column.Parsed.Item2.TableIndex], column.Parsed.Item1, out int l_val);
                    if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                    {
                        order.Error = r;
                        return 0;
                    }

                    r = column.Parsed.Item2.View.FetchInt(re.Values[column.Parsed.Item2.TableIndex], column.Parsed.Item1, out int r_val);
                    if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                    {
                        order.Error = r;
                        return 0;
                    }

                    if (l_val != r_val)
                        return l_val < r_val ? -1 : 1;
                }
            }

            for (int j = 0; j < wv.TableCount; j++)
            {
                if (le.Values[j] != re.Values[j])
                    return le.Values[j] < re.Values[j] ? -1 : 1;
            }

            return 0;
        }

        private static void AddToArray(JOINTABLE[] array, int arrayPtr, JOINTABLE elem)
        {
            while (arrayPtr < array.Length && array[arrayPtr] != null && array[arrayPtr] != elem)
            {
                arrayPtr++;
            }

            if (array[arrayPtr] == null)
                array[arrayPtr] = elem;
        }

        private static bool InArray(JOINTABLE[] array, int arrayPtr, JOINTABLE elem)
        {
            while (arrayPtr < array.Length && array[arrayPtr] != null && array[arrayPtr] != elem)
            {
                arrayPtr++;
            }

            return array[arrayPtr] != null;
        }

        private static int ReorderCheck(expr expr, JOINTABLE[] ordered_tables, bool process_joins, ref JOINTABLE lastused)
        {
            int res = 0;
            switch (expr.Type)
            {
                case EXPR_WILDCARD:
                case EXPR_SVAL:
                case EXPR_UVAL:
                    return 0;

                case EXPR_COL_NUMBER:
                case EXPR_COL_NUMBER32:
                case EXPR_COL_NUMBER_STRING:
                    if (InArray(ordered_tables, 0, expr.Column.Parsed.Item2))
                        return JOIN_TO_CONST_EXPR;

                    lastused = expr.Column.Parsed.Item2;
                    return CONST_EXPR;

                case EXPR_STRCMP:
                case EXPR_COMPLEX:
                    res = ReorderCheck(expr.Expr.Right, ordered_tables, process_joins, ref lastused);
                    res += ReorderCheck(expr.Expr.Left, ordered_tables, process_joins, ref lastused);
                    if (res == 0)
                        return 0;
                    if (res == CONST_EXPR)
                        AddToArray(ordered_tables, 0, lastused);
                    if (process_joins && res == JOIN_TO_CONST_EXPR + CONST_EXPR)
                        AddToArray(ordered_tables, 0, lastused);

                    return res;

                case EXPR_UNARY:
                    res += ReorderCheck(expr.Expr.Left, ordered_tables, process_joins, ref lastused);
                    if (res == 0)
                        return 0;
                    if (res == CONST_EXPR)
                        AddToArray(ordered_tables, 0, lastused);
                    if (process_joins && res == JOIN_TO_CONST_EXPR + CONST_EXPR)
                        AddToArray(ordered_tables, 0, lastused);
                    return res;

                default:
                    Console.Error.WriteLine($"Unknown expr type: {expr.Type}");
                    return 0x1000000;
            }
        }

        private LibmsiResult VerifyCondition(expr cond, ref bool valid)
        {
            LibmsiResult r;

            switch (cond.Type)
            {
                case EXPR_COLUMN:
                {
                    valid = false;

                    r = ParseColumn(cond.Column, out int type);
                    if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                        break;

                    if ((type & MSITYPE_STRING) != 0)
                        cond.Type = EXPR_COL_NUMBER_STRING;
                    else if ((type & 0xff) == 4)
                        cond.Type = EXPR_COL_NUMBER32;
                    else
                        cond.Type = EXPR_COL_NUMBER;

                    valid = true;
                    break;
                }
                case EXPR_COMPLEX:
                    r = VerifyCondition(cond.Expr.Left, ref valid);
                    if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                        return r;
                    if (!valid)
                        return LibmsiResult.LIBMSI_RESULT_SUCCESS;
                    r = VerifyCondition(cond.Expr.Right, ref valid);
                    if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                        return r;

                    // Check the type of the comparison
                    if ((cond.Expr.Left.Type == EXPR_SVAL) || (cond.Expr.Left.Type == EXPR_COL_NUMBER_STRING)
                        || (cond.Expr.Right.Type == EXPR_SVAL) || (cond.Expr.Right.Type == EXPR_COL_NUMBER_STRING))
                    {
                        switch (cond.Expr.Op)
                        {
                            case OP_EQ:
                            case OP_NE:
                                break;
                            default:
                                valid = false;
                                return LibmsiResult.LIBMSI_RESULT_INVALID_PARAMETER;
                        }

                        // FIXME: check we're comparing a string to a column
                        cond.Type = EXPR_STRCMP;
                    }

                    break;
                case EXPR_UNARY:
                    if (cond.Expr.Left.Type != EXPR_COLUMN)
                    {
                        valid = false;
                        return LibmsiResult.LIBMSI_RESULT_INVALID_PARAMETER;
                    }

                    r = VerifyCondition(cond.Expr.Left, ref valid);
                    if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                        return r;

                    break;
                case EXPR_IVAL:
                    valid = true;
                    cond.Type = EXPR_UVAL;
                    cond.UVal = (uint)cond.IVal;
                    break;
                case EXPR_WILDCARD:
                    valid = true;
                    break;
                case EXPR_SVAL:
                    valid = true;
                    break;
                default:
                    Console.Error.WriteLine("Invalid expression type");
                    valid = false;
                    break;
            }

            return LibmsiResult.LIBMSI_RESULT_SUCCESS;
        }

        #endregion
    }
}