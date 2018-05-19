/*
  
MIT License

Copyright (c) 2018 avat

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

*/

using System;
using System.Collections.Generic;
using System.Data;

namespace Avat.WebSql
{
    /// <summary>
    /// Provides metadata information based on a given source
    /// </summary>
    class MetadataBuilder : IMetadataBuilder
    {
        /// <summary>
        /// Metadata information for all columns
        /// </summary>
        public IEnumerable<IDataColumnInfo> Columns { get { return _columns; } }

        /// <summary>
        /// Build information from a data reader.
        /// </summary>
        /// <param name="reader"></param>
        public void Build(IDataReader reader)
        {
            try
            {
                _columns.Clear();
                InternalBuild(reader.GetSchemaTable());
            }
            catch(Exception ex)
            {
                throw new Exception("Error processing dataset columns", ex);
            }
        }

        private static MetaColumn<string> NameMetaColumn = new MetaColumn<string>("ColumnName", 0);
        private static MetaColumn<int> OrdinalMetaColumn = new MetaColumn<int>("ColumnOrdinal", 1);
        private static MetaColumn<Type> TypeMetaColumn = new MetaColumn<Type>("DataType", 12);

        private static void CheckMetaColumns(DataColumnCollection metaColumns)
        {
            NameMetaColumn.CheckExists(metaColumns);
            OrdinalMetaColumn.CheckExists(metaColumns);
            TypeMetaColumn.CheckExists(metaColumns);
        }

        private List<IDataColumnInfo> _columns = new List<IDataColumnInfo>();

        private void InternalBuild(DataTable columnTable)
        {
            CheckMetaColumns(columnTable.Columns);
            foreach (DataRow row in columnTable.Rows)
            {
                _columns.Add(CreateColumnInfo(row));
            }
        }

        private ColumnInfo CreateColumnInfo(DataRow row)
        {
            var column = new ColumnInfo();
            column.Name = NameMetaColumn.GetValue(row);
            column.Ordinal = OrdinalMetaColumn.GetValue(row);
            column.DataType = TypeMetaColumn.GetValue(row);
            return column;
        }

        private class MetaColumn<T>
        {
            public MetaColumn(string name, int ordinal)
            {
                _name = name;
                _ordinal = ordinal;
            }

            public T GetValue(DataRow row)
            {
                return row.Field<T>(_ordinal);
            }

            public void CheckExists(DataColumnCollection columns)
            {
                foreach (DataColumn column in columns)
                {
                    if (column.Ordinal != _ordinal) continue;
                    if (!column.ColumnName.Equals(_name)) continue;
                    if (column.DataType != typeof(T)) continue;
                    return;
                }

                throw new Exception($"No matching column found for '{_name}'");
            }

            private string _name;
            private int _ordinal;
        }

        private class ColumnInfo : IDataColumnInfo
        {
            public string Name { get; set; }
            public int Ordinal { get; set; }
            public Type DataType { get; set; }
        }
    }
}
