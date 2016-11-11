using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenDocSqlServer.Models
{
    public class TableInfo
    {
        public string TableName { get; set; }

        public IEnumerable<ColumnInfo> Columns { get; set; }
    }
}
