using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommandLine;
using RazorEngine;
using RazorEngine.Templating;

using GenDocSqlServer.Models;
using GenDocSqlServer.DbInfo;
using System.IO;
using System.Security.Policy;
using System.Security;
using System.Reflection;
using System.Security.Permissions;
using System.Threading;

namespace GenDocSqlServer
{

    class Program
    {
        static void Usage()
        {
            Console.WriteLine("USAGE");
            Console.WriteLine("GenDocSqlServer -S \"(localdb)\\MSSQLLocalDB\" -d AzureStorageEmulatorDb45");
        }

        static void GenerateHtml(Options options)
        {
            Console.WriteLine("Generating ... ");

            //
            // get database information
            //
            IEnumerable<TableInfo> tables;
            using (var dbinfo = new SqlServerDatabaseInformation(options.ConnectionString))
            {
                tables = dbinfo.GetTables(options.DatabaseName);
            }

            //
            // build html
            //
            var indexTemplate = File.ReadAllText(
                PathHelpers.GetAppDirFilePath("Templates\\Index.cshtml")
                );
            var indexHtml = RazorEngine.Engine.Razor.RunCompile(
                indexTemplate, 
                "index.html", 
                null, 
                new IndexViewModel { DatabaseName = options.DatabaseName, Tables = tables }, 
                null
                );

            //
            // output html,css
            //
            File.WriteAllText(
                System.IO.Path.Combine(options.OutputDirectoryPath, "index.html"),
                indexHtml,
                System.Text.Encoding.UTF8
                );

            System.IO.File.Copy(PathHelpers.GetAppDirFilePath("css\\style.css"),
                System.IO.Path.Combine(options.OutputDirectoryPath,"style.css"),
                true
                );

                Console.WriteLine("done.");
        }

        static int Main(string[] args)
        {

            // Template temporary dll deletion fails with System.UnauthorizedAccessException and files are left on disk · Issue #244 · Antaris/RazorEngine https://github.com/Antaris/RazorEngine/issues/244
            if (AppDomain.CurrentDomain.IsDefaultAppDomain())
            {
                // RazorEngine cannot clean up from the default appdomain...
                //Console.WriteLine("Switching to secound AppDomain, for RazorEngine...");
                AppDomainSetup adSetup = new AppDomainSetup();
                adSetup.ApplicationBase = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
                var current = AppDomain.CurrentDomain;
                // You only need to add strongnames when your appdomain is not a full trust environment.
                var strongNames = new StrongName[0];

                var domain = AppDomain.CreateDomain(
                    "MyMainDomain", null,
                    current.SetupInformation, new PermissionSet(PermissionState.Unrestricted),
                    strongNames);
                var exitCode = domain.ExecuteAssembly(Assembly.GetExecutingAssembly().Location, args);
                AppDomain.Unload(domain);
                //// Wait for RazorEngine to cleanup
                //Thread.Sleep(2000);
                return exitCode;
            }

            var options = new Options();
            var isValid = Parser.Default.ParseArgumentsStrict(args, options);
            if (!isValid)
            {
                Usage();
                return 1;
            }

            if (options.ConnectionString == null)
            {
                if (options.ServerInstance == null)
                {
                    Usage();
                    return 2;
                }

                options.ConnectionString = new SqlConnectionStringBuilder {
                    DataSource = options.ServerInstance,
                    IntegratedSecurity = true,
                    InitialCatalog = options.DatabaseName,
                    MultipleActiveResultSets = true,
                }.ToString();
                //$"Data Source={options.ServerInstance};Initial Catalog={options.DatabaseName};Integrated Security=True;MultipleActiveResultSets=true;";
            }

            if (options.OutputDirectoryPath == null)
            {
                var currentDir = System.IO.Directory.GetCurrentDirectory();
                options.OutputDirectoryPath = System.IO.Path.Combine(currentDir, options.DatabaseName);
                if (!System.IO.Directory.Exists(options.OutputDirectoryPath))
                {
                    System.IO.Directory.CreateDirectory(options.OutputDirectoryPath);
                }
            }

            GenerateHtml(options);

            return 0;
        }
    }
}
