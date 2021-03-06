namespace Minimum
{
    using System;
    using Common.Console;
    using Common.ProcessExecution;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.OptionsModel;
    using Microsoft.Extensions.PlatformAbstractions;

    public class Program
    {
        private readonly IServiceProvider _provider;
        private readonly ConsoleWriter _consoleWriter;
        private int _nextProjectId;
        private readonly bool _continueWithProjectProcessing;
        
        public Program(IApplicationEnvironment env)
        {

            var configuration = new ConfigurationBuilder()
                .SetBasePath(env.ApplicationBasePath)
                .AddJsonFile("config.json")
                .AddEnvironmentVariables()
                .Build();

            _provider = new ServiceCollection()
                .AddLogging()
                .AddOptions()
                .Configure<ProjectOptions>(configuration)
                .Configure<CompileWorkerOptions>(configuration)
                .Configure<OutputAnalazerOptions>(configuration)
                .AddProceesProviderServices()
                .AddTransient<ConsoleWriter>()
                .AddTransient<OutputAnalazer>()
                .AddTransient<CompileWorker>()
                .AddTransient<ProjectProcessor>()
                .BuildServiceProvider();
                
                
            _provider.GetService<ILoggerFactory>().AddConsole(LogLevel.Information);
            _consoleWriter = _provider.GetService<ConsoleWriter>();
            
            var optionsAccessor = _provider.GetService<IOptions<ProjectOptions>>().Value;
            _nextProjectId = optionsAccessor.NextProjectInx;
            _continueWithProjectProcessing = optionsAccessor.ContinueWithProjectProcessing;
        }

        public void Main()
        {
            try
            {
                _consoleWriter.ConsoleWriteBackgroundColor("Initializing project graph", ConsoleColor.DarkYellow);
                
                var compileWorker = _provider.GetService<CompileWorker>();
                var projectProcessor = _provider.GetService<ProjectProcessor>();
                
                while (_nextProjectId < projectProcessor.NumberOfProjectsToProcess)
                {
                    if(!_continueWithProjectProcessing)
                    {
                        if(!compileWorker.Compile())break;
                        projectProcessor.PrepareNextProj(_nextProjectId);
                    }
                    
                    while (!compileWorker.Compile()) projectProcessor.UncommentMoreProjectFiles(_nextProjectId);
                    _nextProjectId++;
                }
            }
            catch (Exception ex)
            {
                _consoleWriter.ConsolePauseAndFix($"Fail to proceed: {ex}", ConsoleColor.DarkRed);
                throw;
            }
            _consoleWriter.ConsolePauseAndFix("Success!", ConsoleColor.DarkGreen);
        }
    }
}