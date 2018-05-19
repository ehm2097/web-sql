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
    /// This class deserializes JSON content into a paramreter value collection.
    /// </summary>
    class JsonParameterDeserializer : IParameterDeserializer
    {
        /// <summary>
        /// Deserialize JSON content
        /// </summary>
        /// <param name="reader">A <c>System.IO.TextReader</c> to read JSON content from</param>
        /// <param name="additionalInfo">Resolver for special non-value parameters</param>
        /// <returns>The operation parameters as a collection of atomic values</returns>
        public AtomCollectionData Deserialize(TextReader reader, IValueSource additionalInfo)
        {
            try
            {
                _context = new ParseContext(additionalInfo);

                var text = reader.ReadToEnd();
                var rootToken = JToken.Parse(text);

                switch (rootToken.Type)
                {
                    case JTokenType.Object:
                        FromObject(rootToken as JObject);
                        break;

                    // Extension point: add capability to read from different structure (array etc.)

                    default: throw new Exception($"Unable to read parameters from a {rootToken.Type}");
                }

                return _context.Result;
            }
            catch(Exception ex)
            {
                throw new Exception("JSON deserialization error", ex);
            }
        }

        private enum SpecialValueKinds { Unknown, Property }

        private const string AdditionalInfoKindKey = "kind";
        private const string AdditionalInfoNameKey = "name";

        private ParseContext _context = null;

        private static SpecialValueKinds ParseKind(string text)
        {
            try
            {
                return (SpecialValueKinds)Enum.Parse(typeof(SpecialValueKinds), text);
            }
            catch
            {
                return SpecialValueKinds.Unknown;
            }
        } 

        private void FromObject(JObject root)
        {
            foreach (var entry in root)
            {
                try
                {
                    _context.Result.AddAtom(new AtomData(entry.Key, FromJson(entry.Value)));
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error parsing parameter '{entry.Key}'", ex);
                }
            }
        }

        private object FromJson(JToken token)
        {
            switch (token.Type)
            {
                case JTokenType.String: return token.ToObject<string>();
                case JTokenType.Float: return token.ToObject<decimal>();
                case JTokenType.Integer: return token.ToObject<int>();
                case JTokenType.Object: return GetAdditionalInformation(token as JObject);
                default: throw new Exception($"JSON token type {token.Type} not supported as parameter");
            }
        }

        private object GetAdditionalInformation(JObject specification)
        {
            var kindName = specification.Value<string>(AdditionalInfoKindKey);
            var kind = ParseKind(kindName);

            switch(kind)
            {
                case SpecialValueKinds.Property:
                    var name = specification.Value<string>(AdditionalInfoNameKey);
                    return _context.AdditionalInfo.GetValue(name);

                // Extension point: add capability for other invocation kinds (method etc.)

                default: throw new Exception($"Unsupported additional information kind '{kindName}'");
            }
        }

        private class ParseContext
        {
            public ParseContext(IValueSource additionalInfo)
            {
                _additionalInfo = additionalInfo;
                _result = new AtomCollectionData();
            }

            public IValueSource AdditionalInfo { get { return _additionalInfo; } }
            public AtomCollectionData Result { get { return _result; } }

            private IValueSource _additionalInfo;
            private AtomCollectionData _result;
        }
    }
}
