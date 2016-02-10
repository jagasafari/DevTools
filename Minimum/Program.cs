namespace Minimum
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Common.Console;
    using Common.ProcessExecution;
    using Common.ProcessExecution.Abstraction;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.OptionsModel;
    using Microsoft.Extensions.PlatformAbstractions;

    public class Program
    {
        private readonly IProcessExecutorProvider _executorProvider;
        private readonly string _msBuild;
        private readonly string _outFile;
        private readonly string _compileDirectory;
        private readonly ConsoleWriter _consoleWriter;
        private string[][] _maxMinOrdered;
        
        public Program(IApplicationEnvironment env)
        {

            var configuration = new ConfigurationBuilder()
                .SetBasePath(env.ApplicationBasePath)
                .AddJsonFile("config.json")
                .AddEnvironmentVariables()
                .Build();

            var serviceProvider = new ServiceCollection()
                .AddLogging()
                .AddOptions()
                .Configure<ProjectOptions>(configuration)
                .AddProceesProviderServices()
                .BuildServiceProvider();
            serviceProvider.GetService<ILoggerFactory>().AddConsole(LogLevel.Information);
            _executorProvider = serviceProvider.GetService<IProcessExecutorProvider>();
            var optionsAccessor = serviceProvider.GetService<IOptions<ProjectOptions>>().Value;
            _outFile = optionsAccessor.OutFile;
            _msBuild = optionsAccessor.MsBuild;
            _compileDirectory = optionsAccessor.CompileDirectory;
            _consoleWriter = new ConsoleWriter();
        }

        public void Main()
        {
            try
            {
                _consoleWriter.ConsoleWriteBackgroundColor("Initializing project graph", ConsoleColor.DarkYellow);
                _maxMinOrdered = new ProjectGraphCreator().Create();
                var nextProjIdx = 0;
                while (nextProjIdx < _maxMinOrdered.Length)
                {
                    if(!Compile())break;
                    PrepareNextProj(nextProjIdx);
                    //compile entry point porject always
                    while (!Compile())
                    {
                        UncommentNext(nextProjIdx);
                    }
                    nextProjIdx++;
                }
            }
            catch (Exception ex)
            {
                _consoleWriter.ConsolePauseAndFix($"Fail to proceed: {ex}", ConsoleColor.DarkRed);
                throw;
            }
            _consoleWriter.ConsolePauseAndFix("Success!", ConsoleColor.DarkGreen);
        }

        private void UncommentNext(int nextProjIdx)
        {
            List<string> csFilesToUncomment = GetNextCsFilesToUncommentErrorLog(nextProjIdx);
            ConvertFile(csFilesToUncomment, Uncomment);
        }

        private List<string> GetNextCsFilesToUncommentErrorLog(int nextProjIdx)
        {
            //check class/interface signature commented
            throw new NotImplementedException();
        }

        private void PrepareNextProj(int nxt)
        {
            var csFiles = new DirectoryInfo(_maxMinOrdered[nxt][(int)ProjectGraph.MaxMinDataStructureIndexes.CsprojPath]).GetFiles(
                "*.cs", SearchOption.AllDirectories).ToList().Select(fi => fi.FullName);
            ConvertFile(csFiles, s => $"//{s}");

            var disignerFiles = new DirectoryInfo(_maxMinOrdered[nxt][(int)ProjectGraph.MaxMinDataStructureIndexes.CsprojPath]).GetFiles(
                "*.*.cs", SearchOption.AllDirectories).ToList().Select(fi => fi.FullName);
            ConvertFile(disignerFiles, Uncomment);
            
            if(nxt==0) _consoleWriter.ConsolePauseAndFix($"Num projects: {_maxMinOrdered.Length}. Uncomment Entry point manually", ConsoleColor.DarkCyan);
        }
        
        private void ConvertFile(IEnumerable<string> files, Func<string, string> lineConversion)
        {
            Parallel.ForEach(files, f => ConvertLines(f, lineConversion));
        }
        
        private string Uncomment(string s) => s.Remove(0, 2);

        private void ConvertLines(string filePath, Func<string, string> conversion)
        {
            var originalLines = File.ReadAllLines(filePath);
            var commentedLines = new string[originalLines.Length];
            for (int i = 0; i < originalLines.Length; i++)
            {
                commentedLines[i] = conversion(originalLines[i]);
            }
            File.WriteAllLines(filePath, commentedLines);
        }

        private bool Compile()
        {
            var executor = _executorProvider.Create<IFinishingProcessExecutor>();
            var initialPath = Directory.GetCurrentDirectory();
            try
            {
                Directory.SetCurrentDirectory(_compileDirectory);
                executor.Execute(_msBuild, "", s => !s.Contains("0 Error(s)"));
            }
            catch (Exception ex)
            {
                _consoleWriter.ConsolePauseAndFix($"Compilation failed! For info look in {_outFile}! {Environment.NewLine} {ex}", ConsoleColor.DarkRed);
                return false;
            }
            finally
            {
                File.WriteAllText(_outFile, executor.Output);
                Directory.SetCurrentDirectory(initialPath);
            }
            return true;
        }
    }
}
