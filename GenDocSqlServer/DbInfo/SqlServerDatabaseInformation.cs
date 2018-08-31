using GenDocSqlServer.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenDocSqlServer.DbInfo
{
    public class SqlServerDatabaseInformation : IDatabaseInformationProvider, IDisposable
    {
        private DbContext db = null;

        public SqlServerDatabaseInformation(string connectionString)
        {
            this.db = new DbContext(connectionString);
        }

        public void Dispose()
        {
            if (this.db != null)
            {
                this.db.Dispose();
            }
        }

        private IEnumerable<ColumnInfo> GetColumns(string tableName)
        {
            if (this.db != null)
            {
                var sql = this.GetColumnsQuery();
                var columns = db.Database
                    .SqlQuery<ColumnInfo>(sql, new SqlParameter("@TableName", tableName))
                    .ToList();
                return columns;
            }

            return new ColumnInfo[] { };
        }

        public IEnumerable<TableInfo> GetTables(string DatabaseName)
        {
            if (this.db != null)
            {

                var tables = db.Database.SqlQuery<TableInfo>(@"
SELECT 
	tbl.name AS TableName,
    ISNULL(exprop.value, '') AS Description
FROM sys.tables AS tbl
    LEFT OUTER JOIN sys.extended_properties exprop 
		ON exprop.major_id = tbl.object_id
		And exprop.minor_id = 0
        AND exprop.name = 'MS_Description'
ORDER BY tbl.name
").ToList();
                foreach (var table in tables)
                {
                    table.Columns = this.GetColumns(table.TableName);
                }

                return tables;
            }

            return new TableInfo[] { };
        }

        private string GetColumnsQuery()
        {
            var appPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            var appDir = System.IO.Path.GetDirectoryName(appPath);
            var sqlPath = System.IO.Path.Combine(appDir, "sql\\GetColumnInfo.sql");
            return File.ReadAllText(sqlPath);
        }
    }

}

