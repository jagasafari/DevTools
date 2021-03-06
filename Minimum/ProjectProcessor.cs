namespace Minimum
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Common.Console;
    using static Minimum.ProjectGraph;

    public class ProjectProcessor
    {
        private readonly ConsoleWriter _consoleWriter;
        private readonly OutputAnalazer _outputAnalayzer;
        private readonly HashSet<string> _namespacesSet_1;

        public ProjectProcessor(ConsoleWriter consoleWriter, OutputAnalazer outputAnalazer)
        {
            _consoleWriter = consoleWriter;
            _outputAnalayzer = outputAnalazer;
        }

        public int NumberOfProjectsToProcess => ProjectGraph.GraphLength;

        public void PrepareNextProj(int projectIdx)
        {
            Parallel.ForEach(GetFiles(projectIdx, "*.cs"), f =>
             {
                 string fileNamespace;
                 FileWorker.ConvertLines(f, s => $"// {s}", out fileNamespace);
             });
            if (projectIdx == 0) _consoleWriter.ConsolePauseAndFix($"Num projects: {NumberOfProjectsToProcess}. Uncomment Entry point manually", ConsoleColor.DarkCyan);
        }

        public void UncommentMoreProjectFiles(int nextProjIdx)
        {
            var typesToUncomment = _outputAnalayzer.GetNextCsFilesToUncommentErrorLog(nextProjIdx);

            UncommentProjectFiles(typesToUncomment, Uncomment, nextProjIdx);
        }

        private IEnumerable<string> GetFiles(int projectIdx, string filePattern) =>
            new DirectoryInfo(ProjectGraph.Graph[projectIdx][(int)MaxMinDataStructureIndexes.CsprojPath]).GetFiles(
                filePattern, SearchOption.AllDirectories).ToList().Select(fi => fi.FullName);

        private string Uncomment(string s) => s.Remove(0, 2);

        private void UncommentProjectFiles(IEnumerable<string> typesToUncomment, Func<string, string> lineConversion, int nextProjIdx)
        {
            var files = GetFiles(nextProjIdx, "*.cs");
            int count = 0;
            foreach (var f in files)
            {
                var lines = File.ReadAllLines(f);
                foreach (var l in lines)
                {
                    if (!IsCommented(l)) break;
                    if (typesToUncomment.Any(t => IsExpectedType(t, l)))
                    {
                        string fileNamespace;
                        FileWorker.ConvertLines(f, Uncomment, out fileNamespace);
                        count++;
                        break;
                    }
                }
                if (count == typesToUncomment.Count()) break;
            }
        }

        private bool IsExpectedType(string t, string l) =>
            l.Contains($"class {t}") || l.Contains($"interface {t}");

        private bool IsCommented(string l) => l.Length > 1 && l.StartsWith("//");
    }
}
