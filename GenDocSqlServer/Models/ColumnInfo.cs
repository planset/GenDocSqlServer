using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenDocSqlServer.Models
{
    public class ColumnInfo
    {
        public string ColumnName { get; set; }
        public string Description { get; set; }
        public string InPrimaryKey { get; set; }
        public string IsForeignKey { get; set; }
        public string DataType { get; set; }
        public string Length { get; set; }
        public string NumericPrecision { get; set; }
        public string NumericScale { get; set; }
        public string Nullable { get; set; }
        public string Computed { get; set; }
        public string Identity { get; set; }
        public string DefaultValue { get; set; }
    }
}
