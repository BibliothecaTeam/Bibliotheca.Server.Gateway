using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Bibliotheca.Server.Gateway.Core.Utilities
{
    public static class PathUtility
    {
        public static string GetFullPath(string prefixPath, string suffixPath)
        {
            var path = Path.Combine(prefixPath, suffixPath);
            path = path.Replace("\\\\", "/");
            path = path.Replace("\\", "/");
            var pathParts = path.Split('/');

            int dotsNumber = 0;
            var pathParsed = new List<string>();
            for (int i = pathParts.Length - 1; i >= 0; --i)
            {
                if (pathParts[i] == "..")
                {
                    dotsNumber++;
                }
                else
                {
                    if (dotsNumber == 0)
                    {
                        pathParsed.Add(pathParts[i]);
                    }
                    else
                    {
                        dotsNumber--;
                    }
                }
            }

            pathParsed.Reverse();
            path = String.Join("/", pathParsed);
            return path;
        }

        public static string GetLastDirectory(string path)
        {
            path = path.Replace("\\", "/");
            path = path.TrimEnd('/');
            var pathsParts = path.Split('/');
            return pathsParts.Last();
        }
    }
}