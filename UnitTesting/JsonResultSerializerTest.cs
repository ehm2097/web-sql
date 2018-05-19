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
using System.IO;
using System.Text;
using System.Globalization;

namespace Avat.WebSql.Test
{
    [TestClass]
    public class JsonResultSerializerTest
    {
        [TestInitialize]
        public void Setup()
        {
            _serializer = new JsonResultSerializer();
        }

        [TestMethod]
        public void JsonResult_SerializeData_Good()
        {
            var data = new RowSetCollectionData();

            var rowset1 = new RowSetData();
            data.AddRowSet(rowset1);

            var row11 = new AtomCollectionData();
            row11.AddAtom(new AtomData(Name11, StringValue));
            rowset1.AddRow(row11);

            var row12 = new AtomCollectionData();
            row12.AddAtom(new AtomData(Name11, DecimalValue));
            rowset1.AddRow(row12);

            var row13 = new AtomCollectionData();
            row13.AddAtom(new AtomData(Name11, IntegerValue));
            rowset1.AddRow(row13);

            var rowset2 = new RowSetData();
            data.AddRowSet(rowset2);

            var row21 = new AtomCollectionData();
            row21.AddAtom(new AtomData(Name21, StringValue));
            row21.AddAtom(new AtomData(Name22, DecimalValue));
            row21.AddAtom(new AtomData(Name23, IntegerValue));
            rowset2.AddRow(row21);

            using (var writer = new StringWriter())
            {
                _serializer.SerializeData(writer, data);
                var text = writer.ToString();
                Console.WriteLine(text);

                Assert.AreEqual("{:[[{:}{:}{:}][{:::}]]}", Skeletonize(text, "[]{}:"));
                Assert.AreEqual(1, GetOccurrenceCount(DataName, text));
                Assert.AreEqual(0, GetOccurrenceCount(ErrorName, text));

                Assert.AreEqual(3, GetOccurrenceCount(Name11, text));
                Assert.AreEqual(1, GetOccurrenceCount(Name21, text));
                Assert.AreEqual(1, GetOccurrenceCount(Name22, text));
                Assert.AreEqual(1, GetOccurrenceCount(Name23, text));
                Assert.AreEqual(2, GetOccurrenceCount(StringValue, text));
                Assert.AreEqual(2, GetOccurrenceCount(DecimalValue.ToString(CultureInfo.InvariantCulture), text));
                Assert.AreEqual(2, GetOccurrenceCount(IntegerValue.ToString(CultureInfo.InvariantCulture), text));
            }
        }

        [TestMethod]
        public void JsonResult_SerializeData_Bad()
        {
            var data = new RowSetCollectionData();

            var rowset = new RowSetData();
            data.AddRowSet(rowset);

            var row1 = new AtomCollectionData();
            row1.AddAtom(new AtomData());
            rowset.AddRow(row1);

            using (var writer = new StringWriter())
            {
                _serializer.SerializeData(writer, data);
                var text = writer.ToString();
                Console.WriteLine(text);

                Assert.AreEqual(1, GetOccurrenceCount(ErrorName, text));
            }
        }

        [TestMethod]
        public void JsonResult_SerializeError()
        {
            Exception ex = null;
            for(int i = 0; i < 5; i++)
            {
                ex = new Exception(ErrorText, ex);
            }

            using (var writer = new StringWriter())
            {
                _serializer.SerializeError(writer, ex);
                var text = writer.ToString();
                Console.WriteLine(text);

                Assert.AreEqual("{:[{::}{::}{::}{::}{::}]}", Skeletonize(text, "[]{}:"));
                Assert.AreEqual(0, GetOccurrenceCount(DataName, text));
                Assert.AreEqual(1, GetOccurrenceCount(ErrorName, text));
                Assert.AreEqual(5, GetOccurrenceCount(ErrorText, text));
            }
        }

        private const string Name11 = "Field11";
        private const string Name21 = "Field21";
        private const string Name22 = "Field22";
        private const string Name23 = "Field23";

        private const string DataName = "data";
        private const string ErrorName = "error";

        private const string ErrorText = "SomeError";

        private const string StringValue = "Justa li'l harmless text";
        private const decimal DecimalValue = 1793.333m;
        private const int IntegerValue = 747;

        private JsonResultSerializer _serializer;

        private static string Skeletonize(string text, string skeletonChars)
        {
            var sb = new StringBuilder();
            foreach(var ch in text)
            {
                if (skeletonChars.IndexOf(ch) >= 0) sb.Append(ch);
            }
            return sb.ToString();
        }

        private static int GetOccurrenceCount(string ofText, string inText)
        {
            int count = 0;
            int size = ofText.Length;
            var workText = inText;
            while(!string.IsNullOrEmpty(workText))
            {
                var pos = workText.IndexOf(ofText);
                if (pos >= 0)
                {
                    count++;
                    workText = workText.Substring(pos + size);
                }
                else break;
            }
            return count;
        }
    }
}
