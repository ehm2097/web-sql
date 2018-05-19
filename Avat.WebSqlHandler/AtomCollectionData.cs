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
using System.Linq;

namespace Avat.WebSql
{
    /// <summary>
    /// Instances of this class contain a collection of <see cref="Avat.WebSql.AtomData"/>.
    /// It provides read / write function for both "named" and "unnamed" access modes.
    /// The collection may include named or unnamed atoms, but never a mix of both.
    /// </summary>
    class AtomCollectionData
    {
        /// <summary>
        /// Allows reading data as a sequence of atoms
        /// </summary>
        public IEnumerable<AtomData> Atoms { get { return _atoms; } }

        /// <summary>
        /// Allows reading atoms by names
        /// </summary>
        /// <param name="name">The atom name to look for</param>
        /// <returns>The atom having the given name, or null if none exists within the collection</returns>
        public AtomData this[string name] { get { return GetAtomByName(name); } }

        /// <summary>
        /// Add a new atom to the collection, with name check
        /// </summary>
        /// <param name="atom">The atom to add</param>
        public void AddAtom(AtomData atom)
        {
            if (atom == null) throw new ArgumentNullException("atom");

            // Check mixed mode
            if(_atoms.Count == 0)
            {
                // First atom => decide named or unnamed
                _useNames = atom.IsNamed;
            }
            else
            {
                // Check named or unnamed atom vs. current behavior
                if (_useNames != atom.IsNamed) throw new Exception("Mixed mode atoms not allowed in collection");

                // Check atom name if any
                if (_useNames && (this[atom.Name] != null)) throw new Exception(
                    $"Atom named '{atom.Name}' already exists in collection");
            }

            _atoms.Add(atom);
        }

        private List<AtomData> _atoms = new List<AtomData>();
        private bool _useNames = false;

        private AtomData GetAtomByName(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("[atom] name");
            if (!_useNames) throw new Exception("Unnamed data collection; access by name is not supported");
            return _atoms.FirstOrDefault(a => name.Equals(a.Name));
        }
    }
}
