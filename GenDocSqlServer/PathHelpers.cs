using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenDocSqlServer
{
    public class PathHelpers
    {
        public static string GetAppDirFilePath(string path)
        {
            var appPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            var appDir = System.IO.Path.GetDirectoryName(appPath);
            return System.IO.Path.Combine(appDir, path);
        }
    }
}
