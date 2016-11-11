using GenDocSqlServer.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenDocSqlServer.DbInfo
{

    public interface IDatabaseInformationProvider
    {
        IEnumerable<TableInfo> GetTables(string databaseName);

//        IEnumerable<ColumnInfo> GetColumns(string tableName);
    }

}
