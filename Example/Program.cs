using System;
using System.Data;
using System.IO;
using LightGBMSharp;
using Utils;

namespace Examples
{
    class Program
    {
        static string _dir = @"..\..\..\res\data";


        static void Main(string[] args)
        {
//            Examples.SimpleExample();
            var file= Path.Combine(_dir, "regression", "test.csv");

            var dt = new DataTable();
            using (var rdr = new CsvDataReader(file, true))
            {
//                rdr.ColumnTypes = new Type[] { typeof(int), typeof(string), typeof(int) };
                dt.Load(rdr);
            }

            foreach (DataRow r in dt.Rows)
            {
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    var col = dt.Columns[i];
                    Console.Write($"{dt.Columns[i]}:{r[i]}  ");
                }
                Console.WriteLine("");
            }

        }
    }
}
