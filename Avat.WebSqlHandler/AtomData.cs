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


namespace Avat.WebSql
{
    /// <summary>
    /// Instances of this class are used to carry an "atomic" piece of information,
    /// like a text or numeric value.
    /// </summary>
    class AtomData
    {
        /// <summary>
        /// Default constructor - creates an uninitialized instance
        /// </summary>
        public AtomData() { }

        /// <summary>
        /// Convenience constructor to create an initialized instance
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public AtomData(string name, object value)
        {
            Name = name;
            Value = value;
        }

        /// <summary>
        /// A name to identify this value
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The actual value
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Convenience property to check named vs. unnamed atom
        /// </summary>
        public bool IsNamed { get { return !string.IsNullOrEmpty(Name); } }
    }
}
