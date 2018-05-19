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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Avat.WebSql.Test
{
    [TestClass]
    public class MetadataBuilderTest
    {
        [TestMethod]
        public void MetadataBuilder_FromReader()
        {
            var metadata = new TestMetadata();
            var test = new MetadataBuilder();

            // Repeat build to ensure proper reset
            for (int i = 0; i < 2; i++)
            {
                var reader = MockReader(metadata);
                test.Build(reader);
            }

            var expected = metadata.Columns.OrderBy(c => c.Ordinal).ToArray();
            var actual = test.Columns.OrderBy(c => c.Ordinal).ToArray();

            // Dynamic complete testing for coverage
            Assert.AreEqual(expected.Length, actual.Length);
            for(int i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i].Name, actual[i].Name);
                Assert.AreEqual(expected[i].Ordinal, actual[i].Ordinal);
                Assert.AreEqual(expected[i].DataType, actual[i].DataType);
            }

            // Static partial testing just to be sure
            Assert.AreEqual("StringColumn", actual[0].Name);
            Assert.AreEqual(typeof(int), actual[1].DataType);
            Assert.AreEqual(2, actual[2].Ordinal);
        }

        private const int ColumnNameIndex = 0;
        private const int ColumnOrdinalIndex = 1;
        private const int DataTypeIndex = 12;

        private static DataTable CreateTable(TestMetadata metadata)
        {
            var table = new DataTable();
            for(int i = 0; i <= 12; i++)
            {
                table.Columns.Add(CreateColumn(i));
            }

            foreach(var column in metadata.Columns)
            {
                var row = table.NewRow();
                row[ColumnNameIndex] = column.Name;
                row[ColumnOrdinalIndex] = column.Ordinal;
                row[DataTypeIndex] = column.DataType;
                table.Rows.Add(row);
            }

            return table;
        }

        private static DataColumn CreateColumn(int index)
        {
            switch (index)
            {
                case ColumnNameIndex: return new DataColumn("ColumnName", typeof(string));
                case ColumnOrdinalIndex: return new DataColumn("ColumnOrdinal", typeof(int));
                case DataTypeIndex: return new DataColumn("DataType", typeof(Type));
                default: return new DataColumn($"Column_{index}", typeof(int));
            }
        }

        private static IDataReader MockReader(TestMetadata metadata)
        {
            var mock = new Mock<IDataReader>(MockBehavior.Strict);
            mock.Setup(m => m.GetSchemaTable()).Returns(CreateTable(metadata));
            return mock.Object;
        }

        private class TestMetadata
        {
            public TestMetadata()
            {
                _columns.Add(MockColumnInfo("StringColumn", typeof(string)));
                _columns.Add(MockColumnInfo("IntegerColumn", typeof(int)));
                _columns.Add(MockColumnInfo("DecimalColumn", typeof(decimal)));
            }

            public IEnumerable<IDataColumnInfo> Columns { get { return _columns; } }

            private static List<IDataColumnInfo> _columns = new List<IDataColumnInfo>();

            private static IDataColumnInfo MockColumnInfo(string name, Type dataType)
            {
                var ordinal = _columns.Count;
                var mock = new Mock<IDataColumnInfo>();
                mock.Setup(m => m.Name).Returns(name);
                mock.Setup(m => m.Ordinal).Returns(ordinal);
                mock.Setup(m => m.DataType).Returns(dataType);
                return mock.Object;
            }
        }
    }
}
