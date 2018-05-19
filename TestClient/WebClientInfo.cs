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

namespace Avat.WebSqlHandler.Test
{
    class WebClientInfo
    {
        public class PromptEventArgs: EventArgs
        {
            public static PromptEventArgs ForPassword = new PromptEventArgs("Password");

            public string PromptText { get; set; }
            public string UserValue { get; set; }

            private PromptEventArgs(string promptText)
            {
                PromptText = promptText;
            }
        }

        public delegate void PromptHandler(object sender, PromptEventArgs args);

        public static WebClientInfo Create(object[] args)
        {
            try
            {
                var instance = new WebClientInfo();
                instance._requestUri = new Uri(args[0] as string);
                using (var reader = new StreamReader(args[1] as string))
                {
                    instance._requestText = reader.ReadToEnd();
                }
                if(args.Length > 2)
                {
                    instance._login = args[2] as string;
                }
                return instance;
            }
            catch
            {
                return null;
            }
        }

        public Uri RequestUri{get{ return _requestUri; }}
        public string RequestText { get { return _requestText; } }
        public ICredentials Credentials { get { return GetCredentials(); } }

        public event PromptHandler PromptForPassword; 

        private WebClientInfo() { }

        private Uri _requestUri;
        private string _requestText;
        private string _login;

        private ICredentials GetCredentials()
        {
            if (string.IsNullOrEmpty(_login)) return null;
            else return new NetworkCredential(_login, GetPassword());
        }

        private string GetPassword()
        {
            try
            {
                var args = PromptEventArgs.ForPassword;
                PromptForPassword(this, args);
                return args.UserValue;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
