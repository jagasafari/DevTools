namespace Minimum
{
    using System;
    using System.IO;
    using System.Text.RegularExpressions;

    public static class FileWorker
    {
        public static void ConvertLines(string filePath, Func<string, string> conversion, out string fileNamespace)
        {
            var originalLines = File.ReadAllLines(filePath);
            var commentedLines = new string[originalLines.Length];
            fileNamespace = string.Empty;
            for (int i = 0; i < originalLines.Length; i++)
            {
                if(originalLines[i].Contains("namespace"))
                    fileNamespace = Regex.Match(originalLines[i], @"(.?)namespace (.+?)").Groups[2].Value;
                
                commentedLines[i] = conversion(originalLines[i]);
            }
            File.WriteAllLines(filePath, commentedLines);
        }
    }
}
