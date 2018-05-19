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
using System.IO;
using System.Linq;

namespace Avat.WebSql.Test
{
    [TestClass]
    public class JsonParameterDeserializerTest
    {
        [TestMethod]
        public void JsonParameterDeserialize_Ok()
        {
            using (var reader = new StreamReader(@"data\parameters_ok.json"))
            {
                var additionalInfo = MockAdditionalInfo();
                var test = new JsonParameterDeserializer();
                var result = test.Deserialize(reader, additionalInfo);

                // Check reader is used completely but NOT closed
                Assert.IsTrue(string.IsNullOrWhiteSpace(reader.ReadToEnd()));

                // Check we have some result (any at all...)
                Assert.IsNotNull(result);
                Assert.AreEqual(4, result.Atoms.Count());
                var atomValues = result.Atoms.Select(a => a.Value).ToArray();
                var atomNames = result.Atoms.Select(a => a.Name).ToArray();

                // Check individual results
                Assert.AreEqual(Atom0Value, atomValues[0]);
                Assert.AreEqual(Atom1Value, atomValues[1]);
                Assert.AreEqual(Atom2Value, atomValues[2]);
                Assert.AreEqual(SpecialPropertyValue, atomValues[3]);

                // Check some keys too
                Assert.AreEqual(Atom0Name, atomNames[0]);
                Assert.AreEqual(Atom3Name, atomNames[3]);
            }
        }

        private const string SpecialPropertyName = "Special";
        private const string SpecialPropertyValue = "Oh so special!";

        private const string Atom0Name = "stringParam";
        private const string Atom3Name = "specialParam";

        private const string Atom0Value = "Some awesome text";
        private const decimal Atom1Value = 123.45m;
        private const int Atom2Value = 12345;

        private static IValueSource MockAdditionalInfo()
        {
            var mock = new Mock<IValueSource>(MockBehavior.Strict);
            mock.Setup(m => m.GetValue(It.Is<string>(p => p.Equals(SpecialPropertyName)))).Returns(SpecialPropertyValue);
            return mock.Object;
        }
    }
}
