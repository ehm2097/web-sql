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

using System.IO;

namespace Avat.WebSql
{
    /// <summary>
    /// classes implementing this interface are able to deserialize text content
    /// into operation parameter values.
    /// </summary>
    interface IParameterDeserializer
    {
        /// <summary>
        /// Deserialize text content
        /// </summary>
        /// <param name="reader">A <c>System.IO.TextReader</c> to deserialize from</param>
        /// <param name="additionalInfo">Resolver for special non-value parameters</param>
        /// <returns>The operation parameters as a collection of atomic values</returns>
        /// <remarks>
        /// The implementation is expected to read all relevant content but leave the reader open.
        /// </remarks>
        AtomCollectionData Deserialize(TextReader reader, IValueSource additionalInfo);
    }
}
