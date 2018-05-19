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
using System.Collections.Generic;

namespace Avat.WebSql
{
    /// <summary>
    /// This class provides <see cref="Avat.WebSql.IValueSource"/> behaviour
    /// to subclasses by exposing their standard (C# language) properties
    /// </summary>
    abstract class ObjectValueSource : IValueSource
    {
        /// <summary>
        /// Provide the value of the property with the given name.
        /// </summary>
        /// <param name="valueName">The property name</param>
        /// <returns></returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">
        /// There is no property with the supplied name
        /// </exception>
        public object GetValue(string valueName)
        {
            var prop = GetType().GetProperty(valueName);
            if (prop == null)
                throw new KeyNotFoundException($"Unknown property '{valueName}'");
            return prop.GetValue(this);
        }
    }
}
