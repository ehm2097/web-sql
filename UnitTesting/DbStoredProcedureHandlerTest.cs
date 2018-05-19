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
    public class DbStoredProcedureHandlerTest
    {
        [TestMethod]
        public void DbStoredProcedureHandler_Execute()
        {
            var commander = new EchoProcedureEmulator();

            var test = new DbStoredProcedureHandler();
            test.Database = commander.Database;
            test.OperationName = commander.OperationName;
            test.MetadataBuilder = commander.MetadataBuilder;

            var arg1 = new AtomData("String", "The quick brown fox etc. etc.");
            var arg2 = new AtomData("Integer", 123456);
            var arg3 = new AtomData("Decimal", 3.14159m);

            var arguments = new AtomCollectionData();
            arguments.AddAtom(arg1);
            arguments.AddAtom(arg2);
            arguments.AddAtom(arg3);

            var result = test.Execute(arguments);

            var rowsets = result.RowSets.ToArray();
            Assert.AreEqual(3, rowsets.Length);

            Assert.AreEqual(arg1.Name, rowsets[0].Rows.First().Atoms.First().Name);
            Assert.AreEqual(arg2.Name, rowsets[1].Rows.First().Atoms.First().Name);
            Assert.AreEqual(arg3.Name, rowsets[2].Rows.First().Atoms.First().Name);

            Assert.AreEqual(arg1.Value, rowsets[0].Rows.First().Atoms.First().Value);
            Assert.AreEqual(arg2.Value, rowsets[1].Rows.First().Atoms.First().Value);
            Assert.AreEqual(arg3.Value, rowsets[2].Rows.First().Atoms.First().Value);
        }

        private class EchoProcedureEmulator
        {
            public EchoProcedureEmulator()
            {
                _db = MockDbConnection();
                _mb = MockMetadataBuilder();
            }

            public string OperationName { get { return "ECHO"; } }

            public IDbConnection Database { get { return _db; } }
            public IMetadataBuilder MetadataBuilder { get { return _mb; } }

            private const int RecordCount = 2;

            private IDbConnection _db;
            private IMetadataBuilder _mb;

            private Dictionary<string, object> _data = new Dictionary<string, object>();
            private IEnumerator<KeyValuePair<string, object>> _browser;
            private int _recordNumber = 0;

            private void AddParameter(IDataParameter p)
            {
                _data[p.ParameterName] = p.Value;
            }

            private bool NextResult()
            {
                var result = _browser.MoveNext();
                if (result) _recordNumber = 0;
                return result;
            }

            private bool Read()
            {
                if(_recordNumber < RecordCount) _recordNumber++;
                return (_recordNumber < RecordCount);
            }

            private object GetValue(int ordinal)
            {
                Assert.AreEqual(0, ordinal);
                Assert.IsTrue(_recordNumber < RecordCount);
                return _browser.Current.Value;
            }

            private IEnumerable<IDataColumnInfo> GetColumns()
            {
                var result = new ColumnInfo();
                result.Name = _browser.Current.Key;
                result.Ordinal = 0;
                result.DataType = _browser.Current.Value.GetType();
                return new IDataColumnInfo[] { result };
            }

            private IDbConnection MockDbConnection()
            {
                var mock = new Mock<IDbConnection>();
                mock.Setup(m => m.CreateCommand()).Returns(MockCommand());
                return mock.Object;
            }

            private IDbCommand MockCommand()
            {
                var mock = new Mock<IDbCommand>();
                mock.SetupSet(m => m.CommandType = It.IsAny<CommandType>())
                    .Callback((CommandType t) => { Assert.AreEqual(CommandType.StoredProcedure, t); });
                mock.SetupSet(m => m.CommandText = It.IsAny<string>())
                    .Callback((string t) => { Assert.AreEqual(OperationName, t);  });
                mock.Setup(m => m.Parameters).Returns(MockParameters());
                mock.Setup(m => m.CreateParameter()).Returns(MockParameter());
                mock.Setup(m => m.ExecuteReader()).Returns(() => MockReader());
                return mock.Object;
            }

            private IDataParameterCollection MockParameters()
            {
                var mock = new Mock<IDataParameterCollection>();
                mock.Setup(m => m.Add(It.IsAny<object>()))
                    .Callback((object p) => { AddParameter((IDataParameter)p); });
                return mock.Object;
            }

            private IDataReader MockReader()
            {
                _browser = _data.GetEnumerator();
                _browser.MoveNext();

                var mock = new Mock<IDataReader>();
                mock.Setup(m => m.NextResult()).Returns(() => NextResult());
                mock.Setup(m => m.Read()).Returns(() => Read());
                mock.Setup(m => m.GetValue(It.IsAny<int>())).Returns((int i) => GetValue(i));
                return mock.Object;
            }

            private IMetadataBuilder MockMetadataBuilder()
            {
                var mock = new Mock<IMetadataBuilder>();
                mock.Setup(m => m.Columns).Returns(() => GetColumns());
                return mock.Object;
            }

            private IDbDataParameter MockParameter()
            {
                var mock = new Mock<IDbDataParameter>();
                mock.SetupProperty(m => m.ParameterName);
                mock.SetupProperty(m => m.Value);
                return mock.Object;
            }
        }

        private class ColumnInfo : IDataColumnInfo
        {
            public string Name { get; set; }
            public int Ordinal { get; set; }
            public Type DataType { get; set; }
        }
    }
}
