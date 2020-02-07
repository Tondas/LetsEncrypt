using LetsEncrypt.Client.Interfaces;
using System;
using System.Threading.Tasks;

namespace LetsEncrypt.Client.Loggers
{
    public class ConsoleLogger : ILogger
    {
        // Error

        public void LogError(Exception ex)
        {
            LogToConsole(ex.Message, ConsoleColor.Red);
            LogToConsole(ex.StackTrace, ConsoleColor.Red);
        }

        public Task LogErrorAsync(Exception ex)
        {
            LogToConsole(ex.Message, ConsoleColor.Red);
            LogToConsole(ex.StackTrace, ConsoleColor.Red);

            return Task.CompletedTask;
        }

        public void LogError(string subject, string message = null)
        {
            LogToConsole(subject, ConsoleColor.Red);
            if (!string.IsNullOrEmpty(message))
            {
                LogToConsole(message, ConsoleColor.Red);
            }
        }

        public Task LogErrorAsync(string subject, string message = null)
        {
            LogToConsole(subject, ConsoleColor.Red);
            if (!string.IsNullOrEmpty(message))
            {
                LogToConsole(message, ConsoleColor.Red);
            }

            return Task.CompletedTask;
        }

        // Info

        public void LogMessage(string subject, string message = null)
        {
            LogToConsole(subject, ConsoleColor.Yellow);
            if (!string.IsNullOrEmpty(message))
            {
                LogToConsole(message, ConsoleColor.Yellow);
            }
        }

        public Task LogMessageAsync(string subject, string message = null)
        {
            LogToConsole(subject, ConsoleColor.Yellow);
            if (!string.IsNullOrEmpty(message))
            {
                LogToConsole(message, ConsoleColor.Yellow);
            }

            return Task.CompletedTask;
        }

        // Private Methods

        private void LogToConsole(string message, ConsoleColor color = default(ConsoleColor))
        {
            var originalColor = Console.ForegroundColor;
            if (color != default(ConsoleColor))
            {
                Console.ForegroundColor = color;
            }
            Console.WriteLine(message);
            Console.ForegroundColor = originalColor;
        }
    }
}