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
using System.IO;

namespace Avat.WebSql
{
    /// <summary>
    /// Classes implementing this interface are able to serialize complete
    /// result of data operation into a text form
    /// </summary>
    interface IResultSerializer
    {
        /// <summary>
        /// Serialize result data to text
        /// </summary>
        /// <param name="writer">A <c>System.IO.TextWriter</c> to serialize to</param>
        /// <param name="data">A structured collection of result data</param>
        void SerializeData(TextWriter writer, RowSetCollectionData data);

        /// <summary>
        /// Serialize error information to text
        /// </summary>
        /// <param name="writer">A <c>System.IO.TextWriter</c> to serialize to</param>
        /// <param name="ex"></param>
        void SerializeError(TextWriter writer, Exception ex);
    }
}
