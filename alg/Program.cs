namespace alg
{
    using System;
    using System.IO;
    using alg.Contracts;
    using Common.Console;

    public class Program
    {
        private string _errorOut = "error.txt";
        private ConsoleWriter _consoleWriter;
        public Program()
        {
              _consoleWriter = new ConsoleWriter();
        }
        
        public void Main()
        {
            INextStep nxt = GetNextStep();
                    
            while (!nxt.Completed())
            {
                _consoleWriter.ConsoleWriteBackgroundColor("in while loop", ConsoleColor.DarkYellow);
                try
                {
                    if (nxt.ExpectedComilationSucces() && !nxt.Compile())
                    {
                        _consoleWriter.ConsolePauseAndFix("Expected compilation success, but failed!", ConsoleColor.Red);
                    }
                    nxt = nxt.Uncomment();
                }
                catch (Exception exception)
                {
                    _consoleWriter.ConsolePauseAndFix("Do something about exception!", ConsoleColor.Red);
                    File.WriteAllText(_errorOut, $"{exception}");
                }
            }
        }
        
        private INextStep GetNextStep()
        {
            _consoleWriter.ConsoleWriteBackgroundColor("Give next step type", ConsoleColor.Blue);
            var nextStepType = Console.ReadLine();
            Type nextstepType = Type.GetType(nextStepType);

            _consoleWriter.ConsoleWriteBackgroundColor($"entering main while loop with nextStep of type: {nextstepType.Name}",
                    ConsoleColor.Green);
            return (INextStep)Activator.CreateInstance(nextstepType);
        }
    }
}
