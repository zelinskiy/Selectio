using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace Selectio.Services
{
    public class SandboxService
    {
        private readonly string connectionString;

        public SandboxService(string connectionString)
        {
            this.connectionString = connectionString;
        }
        


        public bool TryExecuteQuery(string query, ref string output)
        {
            using (var con = new SqlConnection(connectionString))
            {
                con.Open();
                try
                {
                    var command = new SqlCommand(query, con);
                    using (command)
                    {
                        var rows = ReadAllRows(command.ExecuteReader());
                        output = string.Join("\n", RowsToStrings(rows));
                        return true;
                    }
                }
                catch (SqlException ex)
                {
                    output = ex.Message;
                    return false;
                }
            }
        }

        public string FlushDatabase()
        {
            var flushQuery = @"
DECLARE @name VARCHAR(128)
DECLARE @SQL VARCHAR(254)

SELECT @name = (SELECT TOP 1 [name] FROM sysobjects WHERE [type] = 'U' AND category = 0 ORDER BY [name])

WHILE @name IS NOT NULL
BEGIN
    SELECT @SQL = 'DROP TABLE [dbo].[' + RTRIM(@name) +']'
    EXEC (@SQL)
    SELECT @name = (SELECT TOP 1 [name] FROM sysobjects WHERE [type] = 'U' AND category = 0 AND [name] > @name ORDER BY [name])
END

";
            var outp = "";
            TryExecuteQuery(flushQuery, ref outp);
            return outp;
        }


        private static IEnumerable<IDataRecord> ReadAllRows(SqlDataReader reader)
        {
            using (reader)
            {
                while (reader.Read())
                {
                    yield return reader;
                }
            }
        }

        private static IEnumerable<string> RowsToStrings(IEnumerable<IDataRecord> rows)
        {
            foreach (var r in rows)
            {
                var t = new List<string>();
                for (int i = 0; i < r.FieldCount; i++)
                {
                    t.Add(r[i].ToString());
                }
                yield return $"({string.Join(",", t)})";
            }
        }


    }
}
