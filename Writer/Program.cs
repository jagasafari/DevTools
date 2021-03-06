namespace DevTool
{
    using System;
    using Common.Console;
    using Common.ProcessExecution;
    using Common.ProcessExecution.Abstraction;
    using Microsoft.Extensions.DependencyInjection;

    public class Program
    {
        private ConsoleWriter _consoleWriter;
        private IProcessExecutorProvider _executorProvider;
        public Program()
        {
            _consoleWriter = new ConsoleWriter();
            IServiceProvider serviceProvider = new ServiceCollection()
                .AddProceesProviderServices()
                .BuildServiceProvider();
            _executorProvider=serviceProvider.GetService<IProcessExecutorProvider>();
        }
        public void Main()
        {
            while (true)
            {
                _consoleWriter.ConsolePauseAndFix("enter key when code implemented", ConsoleColor.Yellow);
                var executor = _executorProvider.Create<IFinishingProcessExecutor>();
                
                executor.Execute(_dnx, $@"-p ""{testProjectPath}"" test", x => x.Equals("Failed"));
            }
        }
    }
}
