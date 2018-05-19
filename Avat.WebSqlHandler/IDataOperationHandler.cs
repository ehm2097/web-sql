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

using System.Data;
using System.Data.Common;

namespace Avat.WebSql
{
    /// <summary>
    /// Classes implement this interface to handle various types of DB operations
    /// </summary>
    interface IDataOperationHandler
    {
        /// <summary>
        /// Database to execute operations onto.
        /// </summary>
        IDbConnection Database { get; set; }

        /// <summary>
        /// Name identifying this operation among other similar operations.
        /// </summary>
        string OperationName { get; set; }

        /// <summary>
        /// Specialized utility to extract metadata information
        /// </summary>
        IMetadataBuilder MetadataBuilder { get; set; }

        /// <summary>
        /// Execute the underlying operation and returns a result.
        /// </summary>
        /// <param name="arguments">The arguments to execute the operation</param>
        /// <returns>The data resulting from the operation</returns>
        RowSetCollectionData Execute(AtomCollectionData arguments);
    }
}
