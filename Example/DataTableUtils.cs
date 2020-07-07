using System;
using System.Data;
using System.Linq;

namespace Utils
{
    public static class DataTableUtils
    {
        static readonly Type[] _validTypes = {typeof (int), typeof (long), typeof (float), typeof (double), };
        /// <summary>
        /// Split <see cref = "DataTable"/> to train and test table.
        /// </summary>
        /// <param name = "table">DataTable to split</param>
        /// <param name = "testSize">percentage of rows for testing</param>
        /// <returns>Tuple {trainTable, testTable} </returns>
        public static Tuple<DataTable, DataTable> TrainTestSplit(this DataTable table, double testSize)
        {
            if (table == null)
                throw new ArgumentNullException(nameof(table));
            if (testSize > 1.0 || testSize < 0.0)
                throw new ArgumentOutOfRangeException(nameof(testSize));
            var rowCnt = table.Rows.Count;
            var testRowsCnt = (int)Math.Ceiling(rowCnt * testSize);
            var trainRowsCnt = rowCnt - testRowsCnt;
            var trainTable = new DataTable("train");
            var testTable = new DataTable("test");
            for (int i = 0; i < testRowsCnt; i++)
                testTable.ImportRow(table.Rows[i]);
            for (int i = testRowsCnt; i < rowCnt; i++)
                trainTable.ImportRow(table.Rows[i]);
            return Tuple.Create(trainTable, testTable);
        }

        public static T[, ] ToDataArray<T>(this DataTable table)
        {
            if (!_validTypes.Contains(typeof (T)))
                throw new Exception($"Invalid type {typeof (T).Name}");
            var numRowss = table.Rows.Count;
            var numColss = table.Columns.Count;
            var ret = new T[numRowss, numColss];
            for (var colNdx = 0; colNdx < numColss; colNdx++)
            {
                var col = table.Columns[colNdx];
                if (col.DataType != typeof (T))
                    throw new Exception($"All columns must be {typeof (T).Name}");
            }

            for (var rowNdx = 0; rowNdx < numRowss; rowNdx++)
            {
                var row = table.Rows[rowNdx];
                var values = Enumerable.Range(0, numColss).Select(i => row.Field<T>(i)).ToList();
                for (int colNdx = 0; colNdx < values.Count; colNdx++)
                    ret[rowNdx, colNdx] = values[colNdx];
            }

            return ret;
        }

        public static T[] ToDataVector<T>(this DataTable table)
        {
            if (!_validTypes.Contains(typeof (T)))
                throw new ArgumentException($"Invalid type {typeof (T).Name}");
            var numRowss = table.Rows.Count;
            var numColss = table.Columns.Count;
            if (numColss > 1)
                throw new ArgumentException("Must be a single column datatable");
            var ret = table.Rows.Cast<DataRow>().Select(r => GetRowField<T>(r, 0)).ToArray();
            return ret;
        }

        public static Tuple<T[, ], T[]> SplitDataAndLabel<T>(this DataTable table, int labelNdx)where T : struct
        {
            return SplitDataAndLabel<T, T>(table, labelNdx);
        }

        static T GetRowField<T>(DataRow row, int i)
        {
            try
            {
                return row.Field<T>(i);
            }
            catch (InvalidCastException)
            {
                var rowVal = row[i];
                return (T)Convert.ChangeType(rowVal, typeof (T));
            }
        }

        /// <summary>
        /// Split <see cref = "DataTable"/> to the data matrix of type <typeparamref name = "TD"/> 
        /// and the label vector of type <typeparamref name = "TL"/>
        /// </summary>
        /// <typeparam name = "TD">Type of data. Should be <see cref = "double "/>, <see cref = "float "/>, <see cref = "int "/>. </typeparam>
        /// <typeparam name = "TL">Type of label. Should be <see cref = "double "/>, <see cref = "float "/>, <see cref = "int "/>. </typeparam>
        /// <param name = "table"></param>
        /// <param name = "labelNdx">Column index of the column with label values.</param>
        /// <returns></returns>
        public static Tuple<TD[, ], TL[]> SplitDataAndLabel<TD, TL>(this DataTable table, int labelNdx)where TD : struct where TL : struct
        {
            if (labelNdx < 0)
                throw new ArgumentOutOfRangeException(nameof(labelNdx));
            if (!_validTypes.Contains(typeof (TD)))
                throw new ArgumentException($"Invalid type {typeof (TD).Name}");
            if (!_validTypes.Contains(typeof (TL)))
                throw new ArgumentException($"Invalid type {typeof (TL).Name}");
            var numRowss = table.Rows.Count;
            var numColss = table.Columns.Count;
            var label = new TL[numRowss];
            var data = new TD[numRowss, numColss - 1];
            for (var rowNdx = 0; rowNdx < numRowss; rowNdx++)
            {
                var row = table.Rows[rowNdx];
                //                var rowValues = row.ItemArray;
                TD[] dataVals = new TD[numColss - 1];
                label[rowNdx] = GetRowField<TL>(row, labelNdx);
                int skip = 0;
                for (int i = 0; i < numColss; i++)
                {
                    if (i == labelNdx)
                    {
                        skip = 1;
                        continue;
                    }
                    dataVals[i - skip] = GetRowField<TD>(row, i);
                }

                //var dataVals = Enumerable.Range(0, numColss + 1)
                //    .Where(i => i != labelNdx)
                //    .Select(i => GetRowField<TD>(row, i))
                //    .ToArray();
                //label[rowNdx] = GetRowField<TL>(row, labelNdx);
                for (int colNdx = 0; colNdx < dataVals.Length; colNdx++)
                    data[rowNdx, colNdx] = dataVals[colNdx];
            }

            return Tuple.Create(data, label);
        }
    }
}