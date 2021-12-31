using System.Collections.Generic;
using System.IO;

namespace WindowsExporter.Core.Helper
{
    internal static class FileHelper
    {
        public static List<string> ReadAllLines(string file)
        {
            List<string> lines = new List<string>();
            using (FileStream fileStream = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (StreamReader streamReader = new StreamReader(fileStream))
                {
                    while (streamReader.Peek() > -1)
                    {
                        lines.Add(streamReader.ReadLine());
                    }
                }
            }
            return lines;
        }
    }
}
