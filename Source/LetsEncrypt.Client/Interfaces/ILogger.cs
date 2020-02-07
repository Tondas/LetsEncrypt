using System;
using System.Threading.Tasks;

namespace LetsEncrypt.Client.Interfaces
{
    public interface ILogger
    {
        // Error

        void LogError(Exception ex);

        Task LogErrorAsync(Exception ex);

        void LogError(string subject, string message = null);

        Task LogErrorAsync(string subject, string message = null);

        // Message

        void LogMessage(string subject, string message = null);

        Task LogMessageAsync(string subject, string message = null);
    }
}