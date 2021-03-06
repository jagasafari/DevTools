namespace Minimum{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Common.Console;
    using Microsoft.Extensions.OptionsModel;

    public class OutputAnalazer
    {
        private readonly string _outputfile;
        private readonly ConsoleWriter _consoleWriter;
        public OutputAnalazer(IOptions<OutputAnalazerOptions> optionsAccessor, ConsoleWriter consoleWriter){
           _outputfile = optionsAccessor.Value.OutFile;
           _consoleWriter = consoleWriter;
        }
        
        public HashSet<string> GetNextCsFilesToUncommentErrorLog(int nextProjIdx)
        {
            var knownErrors = new HashSet<string>(){"error: C","error: C"};
            var typesToUncomment = new HashSet<string>();
            var lines = File.ReadAllLines(_outputfile);
            foreach(var line in lines)
            {
                if(knownErrors.Any(line.Contains))
                {
                    _consoleWriter.ConsoleWriteBackgroundColor($"known error: {line}", ConsoleColor.DarkGray);
                    var match = Regex.Match(line, @"error : (.+?)""(.+?)""").Groups[2].Value;
                    typesToUncomment.Add(match);
                    continue;
                }
                if(line.Contains("error: ")) _consoleWriter.ConsoleWriteBackgroundColor($"Unknown error: {line}", ConsoleColor.DarkMagenta);
            }
            return typesToUncomment;
        }
   }
}
