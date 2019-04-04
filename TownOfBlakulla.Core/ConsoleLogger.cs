using System;

namespace TownOfBlakulla.Core
{
    public class ConsoleLogger : ILogger
    {
        private readonly object mutex = new object();

        public void Error(string message)
        {
            lock (mutex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write($"[{DateTime.Now:yyyy-MM-dd hh:mm:ss}] ");
                Console.WriteLine(message);
                Console.ResetColor();
            }
        }
    }
}