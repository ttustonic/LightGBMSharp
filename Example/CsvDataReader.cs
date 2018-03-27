using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Utils
{
    public class CsvDataReader: IDataReader, IDisposable
    {
        public const char DEFAULT_DELIMITER = '\t';
        public const string DEFAULT_COMMENT = "#";

        readonly Dictionary<string, int> _ordinals;
        TextReader _file;
        readonly string[] _headers;
        string[] _line;
        Type[] _columnTypes;
        readonly char _delimiter;
        readonly string _comment;

        public Type[] ColumnTypes
        {
            get { return _columnTypes; }
            set { _columnTypes = value; }
        }

        CsvDataReader(string filePath, char delimiter = DEFAULT_DELIMITER, string comment = DEFAULT_COMMENT)
        {
            _file = File.OpenText(filePath);
            Read();
            _headers = _line;
            _delimiter = delimiter;
            _comment = comment;
            _columnTypes = new Type[_headers.Length];
            for (int i=0; i<_headers.Length; i++)
            {
                _columnTypes[i] = typeof(string);
            }
        }

        public CsvDataReader(string filePath, string[] headers, char delimiter = DEFAULT_DELIMITER, string comment = DEFAULT_COMMENT)
            : this(File.OpenText(filePath), headers, delimiter, comment)
        { }


        string _firstLine = null;
        public CsvDataReader(string filePath, bool firstRowHeaders, char delimiter = DEFAULT_DELIMITER, string comment = DEFAULT_COMMENT)
        {
            _file = File.OpenText(filePath);
            _delimiter = delimiter;
            _comment = comment;

            string l = null;
            while ((l = _file.ReadLine()) != null && (l.StartsWith(_comment))) ;

            if (firstRowHeaders)
            {
                _headers = new List<string>(l.Split(new char[] { _delimiter }, StringSplitOptions.None)).ToArray();
            }
            else
            {
                _firstLine = l;
                var hcnt = l.Split(new char[] { _delimiter }, StringSplitOptions.None).Length;
                _headers = Enumerable.Range(1, hcnt).Select(i => $"col{i}").ToArray();
            }

            _columnTypes = new Type[_headers.Length];
            _ordinals = new Dictionary<string, int>(_headers.Length);
            for (int i = 0; i < _headers.Length; i++)
            {
                _ordinals.Add(_headers[i], i);
                _columnTypes[i] = typeof(string);
            }
        }

        CsvDataReader(TextReader streamReader, string[] headers, char delimiter, string comment)
        {
            _file = streamReader;
            _headers = headers;
            _delimiter = delimiter;
            _comment = comment;
            _ordinals = new Dictionary<string, int>(headers.Length);
            for (int i = 0; i < headers.Length; i++)
                _ordinals.Add(headers[i], i);
        }

        public static CsvDataReader FromCsvString(string csvString, string[] headers, char delimiter = DEFAULT_DELIMITER, string comment = DEFAULT_COMMENT)
        {
            TextReader sr = new StringReader(csvString);
            return new CsvDataReader(sr, headers, delimiter, comment);
        }

        public void Close()
        {
            _file.Close();
            _file.Dispose();
            _file = null;
        }

        public int Depth
        {
            get {return 0; }
        }

        public DataTable GetSchemaTable()
        {
            DataTable schema = new DataTable("SchemaTable");
            schema.Locale = CultureInfo.InvariantCulture;
            schema.MinimumCapacity = _headers.Length ;

            schema.Columns.Add(SchemaTableColumn.AllowDBNull, typeof(bool)).ReadOnly = true;
            schema.Columns.Add(SchemaTableColumn.BaseColumnName, typeof(string)).ReadOnly = true;
            schema.Columns.Add(SchemaTableColumn.BaseSchemaName, typeof(string)).ReadOnly = true;
            schema.Columns.Add(SchemaTableColumn.BaseTableName, typeof(string)).ReadOnly = true;
            schema.Columns.Add(SchemaTableColumn.ColumnName, typeof(string)).ReadOnly = true;
            schema.Columns.Add(SchemaTableColumn.ColumnOrdinal, typeof(int)).ReadOnly = true;
            schema.Columns.Add(SchemaTableColumn.ColumnSize, typeof(int)).ReadOnly = true;
            schema.Columns.Add(SchemaTableColumn.DataType, typeof(object)).ReadOnly = true;
            schema.Columns.Add(SchemaTableColumn.IsAliased, typeof(bool)).ReadOnly = true;
            schema.Columns.Add(SchemaTableColumn.IsExpression, typeof(bool)).ReadOnly = true;
            schema.Columns.Add(SchemaTableColumn.IsKey, typeof(bool)).ReadOnly = true;
            schema.Columns.Add(SchemaTableColumn.IsLong, typeof(bool)).ReadOnly = true;
            schema.Columns.Add(SchemaTableColumn.IsUnique, typeof(bool)).ReadOnly = true;
            schema.Columns.Add(SchemaTableColumn.NumericPrecision, typeof(short)).ReadOnly = true;
            schema.Columns.Add(SchemaTableColumn.NumericScale, typeof(short)).ReadOnly = true;
            schema.Columns.Add(SchemaTableColumn.ProviderType, typeof(int)).ReadOnly = true;

            schema.Columns.Add(SchemaTableOptionalColumn.BaseCatalogName, typeof(string)).ReadOnly = true;
            schema.Columns.Add(SchemaTableOptionalColumn.BaseServerName, typeof(string)).ReadOnly = true;
            schema.Columns.Add(SchemaTableOptionalColumn.IsAutoIncrement, typeof(bool)).ReadOnly = true;
            schema.Columns.Add(SchemaTableOptionalColumn.IsHidden, typeof(bool)).ReadOnly = true;
            schema.Columns.Add(SchemaTableOptionalColumn.IsReadOnly, typeof(bool)).ReadOnly = true;
            schema.Columns.Add(SchemaTableOptionalColumn.IsRowVersion, typeof(bool)).ReadOnly = true;

            string[] columnNames;
            columnNames = _headers;


            // null marks columns that will change for each row
            object[] schemaRow = new object[] {
                    true,					// 00- AllowDBNull
                    null,					// 01- BaseColumnName
                    string.Empty,			// 02- BaseSchemaName
                    string.Empty,			// 03- BaseTableName
                    null,					// 04- ColumnName
                    null,					// 05- ColumnOrdinal
                    int.MaxValue,			// 06- ColumnSize
                    typeof(string),			// 07- DataType
                    false,					// 08- IsAliased
                    false,					// 09- IsExpression
                    false,					// 10- IsKey
                    false,					// 11- IsLong
                    false,					// 12- IsUnique
                    DBNull.Value,			// 13- NumericPrecision
                    DBNull.Value,			// 14- NumericScale
                    (int) DbType.String,    // 15- ProviderType

                    string.Empty,			// 16- BaseCatalogName
                    string.Empty,			// 17- BaseServerName
                    false,					// 18- IsAutoIncrement
                    false,					// 19- IsHidden
                    true,					// 20- IsReadOnly
                    false					// 21- IsRowVersion
              };

            for (int i = 0; i < columnNames.Length; i++)
            {
                schemaRow[1] = columnNames[i]; // Base column name
                schemaRow[4] = columnNames[i]; // Column name
                schemaRow[5] = i; // Column ordinal

                schemaRow[7] = _columnTypes[i];

                schema.Rows.Add(schemaRow);
            }
            return schema;
        }

        public bool IsClosed
        {
            get { return _file == null; }
        }

        public bool NextResult()
        {
            return false;
        }

        public bool Read()
        {
            string l = null;
            
            while ((l = _firstLine ?? _file.ReadLine()) != null && (l.StartsWith(_comment))) ;
            if (l == null)
                return false;
            _firstLine = null;
            _line = l.Split(new char[] { _delimiter }, StringSplitOptions.None);
            if (_line.Length != _headers.Length)
                throw new Exception("Invalid line, headers length doesn't match line length");
            return true;
            /*
                        var eos = _file.EndOfStream;
                        if (eos)
                            return false;
                        string l = null;

                        while ( !_file.EndOfStream && (l = _file.ReadLine()).StartsWith(_comment)) ;
                        if (_file.EndOfStream)
                            return false;
                        _line = l.Split(_delimiter);
                        return true;
            */
        }

        public int RecordsAffected
        {
            get { throw new NotImplementedException(); }
        }

        public void Dispose()
        {
            if (_file != null)
            {
                _file.Close();
                _file.Dispose();
                _file = null;
            }
        }

        public int FieldCount
        {
            get { return (_line ?? _headers).Length; }
        }

        public bool GetBoolean(int i)
        {
            return Boolean.Parse(_line[i]);
        }

        public byte GetByte(int i)
        {
            return Byte.Parse(_line[i]);
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        public char GetChar(int i)
        {
            return Char.Parse(_line[i]);
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        public IDataReader GetData(int i)
        {
            throw new NotImplementedException();
        }

        public string GetDataTypeName(int i)
        {
            //            throw new NotImplementedException();
            return _columnTypes[i].FullName;
        }

        public DateTime GetDateTime(int i)
        {
            return DateTime.Parse(_line[i]);
        }

        public decimal GetDecimal(int i)
        {
            return Decimal.Parse(_line[i], CultureInfo.InvariantCulture);
        }

        public double GetDouble(int i)
        {
            return Double.Parse(_line[i], CultureInfo.InvariantCulture);
        }

        public Type GetFieldType(int i)
        {
            throw new NotImplementedException();
        }

        public float GetFloat(int i)
        {
            return float.Parse(_line[i], CultureInfo.InvariantCulture);
        }

        public Guid GetGuid(int i)
        {
            return Guid.Parse(_line[i]);
        }

        public short GetInt16(int i)
        {
            return Int16.Parse(_line[i]);
        }

        public int GetInt32(int i)
        {
            return Int32.Parse(_line[i]);
        }

        public long GetInt64(int i)
        {
            return Int64.Parse(_line[i]);
        }

        public string GetName(int i)
        {
            return _headers[i];
        }

        public int GetOrdinal(string name)
        {
            int result;
            if (!_ordinals.TryGetValue(name, out result))
                result = -1;
            return result;
        }

        public string GetString(int i)
        {
            return _line[i];
        }

        public object GetValue(int i)
        {
            if (((IDataRecord)this).IsDBNull(i))
                return DBNull.Value;

            if (_columnTypes[i] == typeof(string))
                return _line[i];
            return Convert.ChangeType(_line[i], _columnTypes[i], CultureInfo.InvariantCulture);
        }

        public int GetValues(object[] values)
        {
            IDataRecord record = (IDataRecord)this;
            System.ComponentModel.TypeConverter tc = new System.ComponentModel.TypeConverter();
            for (int i = 0; i < _headers.Length; i++)
            {
                if (_columnTypes[i] == typeof(string))
                    values[i] = record.GetValue(i);            
                else
                    values[i] = Convert.ChangeType(record.GetValue(i), _columnTypes[i], CultureInfo.InvariantCulture);
            }
            return _headers.Length;
        }

        public bool IsDBNull(int i)
        {
            return String.IsNullOrWhiteSpace(_line[i]);
        }

        public object this[string name]
        {
            //			get { return _line[GetOrdinal(name)]; }
            get { return GetValue( GetOrdinal(name) ); }
        }

        public object this[int i]
        {
            get { return GetValue(i); }
        }
    }
}