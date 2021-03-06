﻿/*
  
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
using System.Configuration;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avat.WebSql
{
    /// <summary>
    /// This interface provides access to relevant application context information.
    /// </summary>
    /// <remarks>
    /// Using this interface instead of the implementing class is also useful for unit testing.
    /// </remarks>
    interface IApplicationSetup
    {
        /// <summary>
        /// Returns the connection string settings for a given connection name
        /// </summary>
        /// <param name="connectionName">The connection name</param>
        /// <returns>
        /// An instance of <c>System.Configuration.ConnectiocStringSettings</c>
        /// or null if the connection name is not known.
        /// </returns>
        ConnectionStringSettings GetConnectionSettings(string connectionName);

        /// <summary>
        /// Returns the provider class for a given database access provider name
        /// </summary>
        /// <param name="providerName">
        /// The provider name (for instance "system.data.sqlclient")
        /// </param>
        /// <returns>
        /// An instance of <c>System.DAta.Common.DbProviderFactory</c>
        /// or null if the provider name is not known.
        /// </returns>
        DbProviderFactory GetDbProviderFactory(string providerName);
    }
}
