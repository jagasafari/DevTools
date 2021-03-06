namespace Minimum
{
    using System;
    using System.IO;
    using Common.Console;
    using Common.ProcessExecution.Abstraction;
    using Microsoft.Extensions.OptionsModel;

    public class CompileWorker
    {
        private readonly IProcessExecutorProvider _executorProvider;
        private readonly string _msBuild;
        private readonly string _outFile;
        private readonly string _compileDirectory;
        private readonly ConsoleWriter _consoleWriter;
        public CompileWorker(IOptions<CompileWorkerOptions> optionsAccessor, ConsoleWriter consoleWriter, IProcessExecutorProvider provider)
        {
            _msBuild = optionsAccessor.Value.MsBuild;
            _outFile = optionsAccessor.Value.OutFile;
            _compileDirectory = optionsAccessor.Value.CompileDirectory;
            _consoleWriter = consoleWriter;
            _executorProvider = provider;
        }
        public bool Compile()
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
                _consoleWriter.ConsolePauseAndFix($"Compilation failed! For info look in {_outFile}: {ex}", ConsoleColor.DarkRed);
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
