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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Avat.WebSql.Test.Helpers;
using System.Linq;

namespace Avat.WebSql.Test
{
    [TestClass]
    public class AtomCollectionDataTest
    {
        [TestMethod]
        public void AtomCollectionData_Named()
        {
            var named1 = MakeNamedAtom();
            var named2 = MakeNamedAtom();
            var unnamed = MakeUnnamedAtom();

            var data = new AtomCollectionData();

            data.AddAtom(named1);
            AssertPlus.Throws(() => { data.AddAtom(unnamed); });
            Assert.IsNull(data[named2.Name]);
            data.AddAtom(named2);
            Assert.AreEqual(named2, data[named2.Name]);

            Assert.AreEqual(2, data.Atoms.Count());
        }

        [TestMethod]
        public void AtomCollectionData_Unnamed()
        {
            var named = MakeNamedAtom();
            var unnamed1 = MakeUnnamedAtom();
            var unnamed2 = MakeUnnamedAtom();

            var data = new AtomCollectionData();

            data.AddAtom(unnamed1);
            data.AddAtom(unnamed2);
            data.AddAtom(unnamed1);
            AssertPlus.Throws(() => { data.AddAtom(named); });
            AssertPlus.Throws(() => { var atom = data[named.Name]; });

            Assert.AreEqual(3, data.Atoms.Count());
            var atoms = data.Atoms.ToArray();
            Assert.AreEqual(unnamed1, atoms[2]);
            Assert.AreEqual(unnamed2, atoms[1]);
        }

        private const string DummyValue = "DUMMY VALUE";

        private AtomData MakeNamedAtom()
        {
            return new AtomData(Guid.NewGuid().ToString(), DummyValue);
        }

        private AtomData MakeUnnamedAtom()
        {
            return new AtomData(string.Empty, DummyValue);
        }
    }
}
