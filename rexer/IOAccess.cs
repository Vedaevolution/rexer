using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace rexer
{
    public static class IOAccess
    {
  
        public static string[] GetFilesInPath(string path)
        {
            var files = Directory.GetFiles(path);
            return files;
        }
    }
}
