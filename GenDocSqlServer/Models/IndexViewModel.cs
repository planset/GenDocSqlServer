using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenDocSqlServer.Models
{
    public class IndexViewModel
    {
        public string DatabaseName { get; set; }

        public IEnumerable<TableInfo> Tables { get; set; }
    }
}
