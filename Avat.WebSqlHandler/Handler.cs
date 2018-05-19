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

using System.Web;

namespace Avat.WebSql
{
    /// <summary>
    /// <para>
    /// This class is mainly a wrapper to handle IIS - HttpHandler communication.
    /// </para>
    /// <para>
    /// Wrapping may be necessary if the same logic should ever operate, for instance, 
    /// behind a <c>System.Net.HttpListener</c> which uses distinct data types to materialize
    /// mostly the same infomration about the HTTP request and response.
    /// (In .Net, a <c>System.Net.HttpListenerContext</c> is passed to a listener class,
    /// while a IIS handler class is passed a <c>System.Web.HttpContext</c>).
    /// </para>
    /// <para>
    /// As such, this class should not perform any web-related operation.
    /// These should be relayed to the web processor class instead.
    /// </para>
    /// </summary>
    public class Handler : IHttpHandler
    {
        /// <summary>
        /// As of this version, requests are considered stateless and data is not cached between these.
        /// </summary>
        public bool IsReusable { get { return false; } }

        /// <summary>
        /// Wrap the context into a generic structure that does not depend on <c>System.Web.HttpContext</c>
        /// and relay request processing to the generic web processor.
        /// </summary>
        /// <param name="context">Context information as sent by IIS.</param>
        public void ProcessRequest(HttpContext context)
        {
            var processor = new WebProcessor();
            processor.RequestIdentity = context.User.Identity;
            processor.RequestPath = context.Request.Path;
            processor.Process(context.Request.InputStream, context.Response.OutputStream);
        }
    }
}
