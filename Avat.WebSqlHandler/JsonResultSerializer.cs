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

using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace Avat.WebSql
{
    /// <summary>
    /// This class serializes result data into JSON content.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The result is a JSON object having either a "data" property, or an "error" property, or both.
    /// Whenever the "error" part is present, the "data" part may be missing, or incomplete, or otherwise unreliable.
    /// </para>
    /// <para>The "data" part, if present, is structured as follows:</para>
    /// <list type="bullet">
    /// <item><term>Content</term><description>Array of row sets</description></item>
    /// <item><term>Row set</term><description>Array of rows</description></item>
    /// <item><term>Row</term><description>Object bearing properties</description></item>
    /// <item><term>Row property</term><description>Row field as a named value</description></item>
    /// </list>
    /// <para>The "error" part, if present, is structured as follows:</para>
    /// <list type="bullet">
    /// <item><term>Content</term><description>Array of error information</description></item>
    /// <item><term>Error information</term><description>Object bearing properties</description></item>
    /// <item><term>Error property "type"</term><description>.Net exception data type name</description></item>
    /// <item><term>Error property "message"</term><description>Error message</description></item>
    /// </list>
    /// <para>
    /// The error that has occurred and all its recursive causes (as referenced by <c>InnerException</c>)
    /// are rendered as a list, starting with the top-most error and ending to the lowest level known cause
    /// </para>
    /// <para>
    /// The error may be set up by the calling program, or triggered inside this very object
    /// by a failed attempt to serialize data provided by the calling program.
    /// </para>
    /// </remarks>
    class JsonResultSerializer : IResultSerializer
    {
        /// <summary>
        /// Serialize result data to JSON
        /// </summary>
        /// <param name="writer">A <c>System.IO.TextWriter</c> to write JSON to</param>
        /// <param name="data">A structured collection of result data</param>
        public void SerializeData(TextWriter writer, RowSetCollectionData data)
        {
            var resultToken = new JObject();

            try
            {
                InternalSerializeData(resultToken, data);
            }
            catch (Exception ex)
            {
                InternalSerializeError(resultToken, ex);
            }

            writer.Write(resultToken.ToString());
        }

        /// <summary>
        /// Serialize error information to JSON
        /// </summary>
        /// <param name="writer">A <c>System.IO.TextWriter</c> to write JSON to</param>
        /// <param name="ex"></param>
        public void SerializeError(TextWriter writer, Exception ex)
        {
            var resultToken = new JObject();
            InternalSerializeError(resultToken, ex);
            writer.Write(resultToken.ToString());
        }

        private const string DataName = "data";
        private const string ErrorName = "error";
        private const string ErrorTypeName = "type";
        private const string ErrorMessageName = "message";

        private void InternalSerializeError(JObject resultToken, Exception ex)
        {
            var errorArrayToken = new JArray();
            resultToken[ErrorName] = errorArrayToken;

            while (ex != null)
            {
                var errorToken = new JObject();
                errorToken[ErrorTypeName] = ex.GetType().Name;
                errorToken[ErrorMessageName] = ex.Message;
                errorArrayToken.Add(errorToken);
                ex = ex.InnerException;
            }
        }

        private void InternalSerializeData(JObject resultToken, RowSetCollectionData data)
        {
            var rowsetArrayToken = new JArray();
            resultToken[DataName] = rowsetArrayToken;
            foreach (var rowset in data.RowSets)
            {
                var rowsetToken = new JArray();
                rowsetArrayToken.Add(rowsetToken);

                foreach (var row in rowset.Rows)
                {
                    var rowToken = new JObject();
                    rowsetToken.Add(rowToken);

                    foreach (var atom in row.Atoms)
                    {
                        rowToken[atom.Name] = JToken.FromObject(atom.Value);
                    }
                }
            }
        }
    }
}
