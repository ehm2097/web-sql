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
using System.Net;
using System.Text;

namespace Avat.WebSqlHandler.Test
{
    class LocalWebClient
    {
        public string ExecuteRequest(WebClientInfo info)
        {

            var request = WebRequest.Create(info.RequestUri);
            request.Method = "PUT";

            var credentials = info.Credentials;
            if(credentials != null)
            {
                request.UseDefaultCredentials = false;
                request.Credentials = credentials;
            }

            try
            {
                using (var stream = request.GetRequestStream())
                {
                    var writer = new StreamWriter(stream);
                    writer.Write(info.RequestText);
                    writer.Flush();
                }
            }
            catch(Exception ex)
            {
                return $"Error processing request: {ex.Message} ({ex.GetType().FullName})";
            }

            try
            {
               return DumpResponse(request.GetResponse(), "response-data");
            }
            catch(WebException ex)
            {
                return DumpResponse(ex.Response, "response-error");
            }
            catch (Exception ex)
            {
                return $"Error processing response: {ex.Message} ({ex.GetType().FullName})";
            }
        }

        private string DumpResponse(WebResponse response, string tag)
        {
            var result = new StringBuilder();
            result.AppendLine($"<{tag}>");
            if (response == null)
            {
                result.Append("No response");
            }
            else
            {
                var stream = response.GetResponseStream();
                if(stream == null)
                {
                    result.Append("No response data");
                }
                else
                {
                    using (stream)
                    {
                        var reader = new StreamReader(stream);
                        result.AppendLine(reader.ReadToEnd());
                    }
                }
            }
            result.AppendLine($"</{tag}>");
            return result.ToString();
        }
    }
}
