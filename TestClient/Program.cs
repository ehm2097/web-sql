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

namespace Avat.WebSqlHandler.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var info = WebClientInfo.Create(args);
            info.PromptForPassword += LocalPromptForUserValue;
            if(info == null)
            {
                Console.WriteLine("Usage: TestClient.exe <site URL> <JSON input file>");
            }
            else
            {
                var client = new LocalWebClient();
                Console.WriteLine("Web request input is");
                Console.WriteLine(info.RequestText);
                Console.WriteLine();
                Console.WriteLine($"Connecting to {info.RequestUri} ...");
                Console.WriteLine();
                Console.WriteLine(client.ExecuteRequest(info));
            }

            Console.WriteLine("Press any key to stop");
            Console.ReadKey();
        }

        static void LocalPromptForUserValue(object sender, WebClientInfo.PromptEventArgs args)
        {
            Console.Write($"{args.PromptText}: ");
            args.UserValue = Console.ReadLine();
            Console.WriteLine();
        }
    }
}
