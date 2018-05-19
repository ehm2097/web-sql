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

using System.Collections.Generic;

namespace Avat.WebSql
{
    /// <summary>
    /// Represents a collection od individual data row sets.
    /// This is a convenience class wrapping a list to avoid handling a list of lists of lists...
    /// </summary>
    class RowSetCollectionData
    {
        /// <summary>
        /// Allows reading data as a sequence of row sets.
        /// </summary>
        public IEnumerable<RowSetData> RowSets { get { return _rowsets; } }

        /// <summary>
        /// Add a new row set to the row set collection.
        /// </summary>
        /// <param name="rowset">The row set to add</param>
        public void AddRowSet(RowSetData rowset) { _rowsets.Add(rowset); }

        private List<RowSetData> _rowsets = new List<RowSetData>();
    }
}
