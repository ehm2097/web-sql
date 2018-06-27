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
using System.Linq;
using System.Security.Principal;

namespace Avat.WebSql
{

    /// <summary>
    /// This class is the main orchestrator of web request processing.
    /// </summary>
    class WebProcessor
    {
        /// <summary>
        /// The "path" part of the web request Uri
        /// </summary>
        /// <remarks>
        /// The calling program is expected to initialize this property before calling
        /// <see cref="Avat.WebSql.WebProcessor.Process(Stream, Stream)"/>
        /// </remarks>
        public string RequestPath { get; set; }

        /// <summary>
        /// The request author identity as known by the server (IIS).
        /// </summary>
        /// <remarks>
        /// As of this version, the server is sole responsible for authenticating
        /// the request author, using whichever method is configured (including "Anonymous").
        /// The calling program is expected to initialize this property before calling
        /// <see cref="Avat.WebSql.WebProcessor.Process(Stream, Stream)"/>
        /// </remarks>
        public IIdentity RequestIdentity { get; set; }

        /// <summary>
        /// Process a single web request.
        /// </summary>
        /// <param name="inputStream">Stream to read request content from</param>
        /// <param name="outputStream">Stream to write response content to</param>
        public void Process(Stream inputStream, Stream outputStream)
        {
            try
            {
                var arguments = DeserializeParameters(inputStream);
                var resultData = InternalProcess(arguments);
                SerializeResult(outputStream, resultData);
            }
            catch (Exception ex)
            {
                SerializeError(outputStream, ex);
            }
        }

        private string GetOperationName()
        {
            var parts = RequestPath.Split('/');
            return parts.Last();
        }

        private AtomCollectionData DeserializeParameters(Stream stream)
        {
            using (stream)
            {
                var deserializer = new JsonParameterDeserializer();
                var reader = new StreamReader(stream);
                var additionalInfo = new AdditionalInfoSource(this);
                return deserializer.Deserialize(reader, additionalInfo);
            }
        }

        private void SerializeResult(Stream stream, RowSetCollectionData data)
        {
            using (stream)
            {
                var serializer = new JsonResultSerializer();
                var writer = new StreamWriter(stream);
                serializer.SerializeData(writer, data);
                writer.Flush();
            }
        }

        private void SerializeError(Stream stream, Exception ex)
        {
            using (stream)
            {
                var serializer = new JsonResultSerializer();
                var writer = new StreamWriter(stream);
                serializer.SerializeError(writer, ex);
                writer.Flush();
            }
        }

        private RowSetCollectionData InternalProcess(AtomCollectionData arguments)
        {
            var setup = new WebApplicationSetup();
            var dbFactory = new SoftTypedDbFactory(setup);
            var operationHandler = new DbStoredProcedureHandler();
            operationHandler.MetadataBuilder = new MetadataBuilder();
            operationHandler.OperationName = GetOperationName();

            using (var db = dbFactory.GetConnection("db"))
            {
                operationHandler.Database = db;
                return operationHandler.Execute(arguments);
            }
        }

        private class AdditionalInfoSource: ObjectValueSource
        {
            public AdditionalInfoSource(WebProcessor parent)
            {
                RequestIdentityName = parent.RequestIdentity.Name;
            }

            public string RequestIdentityName { get; set; }
        }
    }
}
