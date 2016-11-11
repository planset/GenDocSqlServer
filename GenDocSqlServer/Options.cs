using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommandLine;

namespace GenDocSqlServer
{
    public class Options
    {
        [Option('S', "ServerInstance", Required = false,
          HelpText = "ServerInstance (ex. '(localdb)\\MSSQLLocalDB'.")]
        public string ServerInstance { get; set; }

        [Option('C', "ConnectionString", Required = false,
            HelpText = "Connectiong string")]
        public string ConnectionString { get; set; }

        [Option('o', "OutputDirectory", Required = false,
          HelpText = "Directory path.")]
        public string OutputDirectoryPath { get; set; }

        [Option('d', "Database", Required = true,
          HelpText = "Database name.")]
        public string DatabaseName { get; set; }
    }
}
