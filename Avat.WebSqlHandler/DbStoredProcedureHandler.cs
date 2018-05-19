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
using System.Data;
using System.Linq;

namespace Avat.WebSql
{
    /// <summary>
    /// This class handles operations materialized in executed stored procedures.
    /// </summary>
    class DbStoredProcedureHandler : IDataOperationHandler
    {
        /// <summary>
        /// Database to execute operations onto.
        /// </summary>
        public IDbConnection Database { get; set; }

        /// <summary>
        /// The stored procedure name
        /// </summary>
        public string OperationName { get; set; }

        /// <summary>
        /// Specialized utility to extract metadata information
        /// </summary>
        public IMetadataBuilder MetadataBuilder { get; set; }

        /// <summary>
        /// Execute the underlying stored procedure and returns a result.
        /// </summary>
        /// <param name="arguments">The arguments to execute the stored procedure</param>
        /// <returns>The data resulting from the stored procedure</returns>
        public RowSetCollectionData Execute(AtomCollectionData arguments)
        {
            if (Database == null) throw new InvalidOperationException("Missing database connection");
            if (OperationName == null) throw new InvalidOperationException("Missing operation name");
            if (MetadataBuilder == null) throw new InvalidOperationException("Missing metadata builder");

            Database.Open();
            using (var cmd = Database.CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = OperationName;
                SetupArguments(cmd, arguments);

                var result = new RowSetCollectionData();
                using (var reader = cmd.ExecuteReader())
                {
                    return ProcessData(reader);
                }
            }
        }

        private void SetupArguments(IDbCommand cmd, AtomCollectionData arguments)
        {
            foreach (var argument in arguments.Atoms)
            {
                try
                {
                    var sqlParameter = cmd.CreateParameter();
                    sqlParameter.ParameterName = argument.Name;
                    sqlParameter.Value = argument.Value;
                    cmd.Parameters.Add(sqlParameter);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error setting parameter '{argument.Name}'", ex);
                }
            }
        }

        private RowSetCollectionData ProcessData(IDataReader reader)
        {
            var result = new RowSetCollectionData();
            do
            {
                MetadataBuilder.Build(reader);
                var rowset = new RowSetData();
                while (reader.Read())
                {
                    var row = new AtomCollectionData();
                    var columns = MetadataBuilder.Columns.OrderBy(c => c.Ordinal);
                    foreach (var column in columns)
                    {
                        try
                        {
                            var atom = new AtomData();
                            atom.Name = column.Name;
                            atom.Value = reader.GetValue(column.Ordinal);
                            row.AddAtom(atom);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception($"Error reading column {column.Ordinal} ({column.Name})", ex);
                        }
                    }
                    rowset.AddRow(row);
                }
                result.AddRowSet(rowset);
            }
            while (reader.NextResult());
            return result;
        }
    }
}
