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
using System.Configuration;
using System.Data.Common;

namespace Avat.WebSql.Test
{
    [TestClass]
    public class SoftTypedDbFactoryTest
    {
        [TestInitialize]
        public void Setup()
        {
            _expectedDb = MockConnection();
        }

        [TestMethod]
        public void SoftTypedDbFactory_Ok()
        {
            var setup = MockSetup();

            var factory = new SoftTypedDbFactory(setup);
            var actualDb = factory.GetConnection(GoodConnectionName);
            Assert.AreSame(_expectedDb, actualDb);
            Assert.AreEqual(GoodConnectionString, actualDb.ConnectionString);
        }

        private const string GoodConnectionName = "DB-GOOD";
        private const string GoodConnectionString = "connection";
        private const string GoodProviderName = "Namespace.ProvierClass";

        private DbConnection _expectedDb;

        private IApplicationSetup MockSetup()
        {
            var mock = new Mock<IApplicationSetup>(MockBehavior.Strict);
            mock.Setup(m => m.GetDbProviderFactory(It.Is<string>(p => p.Equals(GoodProviderName)))).Returns(MockProviderFactory());
            mock.Setup(m => m.GetConnectionSettings(It.Is<string>(p => p.Equals(GoodConnectionName)))).Returns(MakeConnectionSettings());
            return mock.Object;
        } 

        private DbProviderFactory MockProviderFactory()
        {
            var mock = new Mock<DbProviderFactory>(MockBehavior.Strict);
            mock.Setup(m => m.CreateConnection()).Returns(_expectedDb);
            return mock.Object;
        }

        private static DbConnection MockConnection()
        {
            // MockBehavior.Strict should be used here in order to check that the connection is returned "as is" with only
            // the connection string set. However the "strict" mode results in a warning if the Dispose() method of the
            // connection mockup is NOT setup as well. But Dispose cannot be setup with Moq as it is not virtual.
            // Mocking IDbConnection would do the trick but DbProviderFactory should return a DbConnection so this is a dead end.
            // TODO: Find a way to allow MockBehavior.Strict or setup (to fail) ALL DbConnection methods (pretty gruesome...)  
            // var mock = new Mock<DbConnection>(MockBehavior.Strict);
            var mock = new Mock<DbConnection>();
            mock.SetupProperty(m => m.ConnectionString);
            // mock.Setup(m => m.Dispose());
            return mock.Object;
        }

        private static ConnectionStringSettings  MakeConnectionSettings()
        {
            return new ConnectionStringSettings(GoodConnectionName, GoodConnectionString, GoodProviderName);
        }
    }
}
