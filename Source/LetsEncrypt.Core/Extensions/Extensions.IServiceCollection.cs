using LetsEncrypt.Core.Interfaces;
using LetsEncrypt.Core.IO;
using LetsEncrypt.Core.Loggers;
using Microsoft.Extensions.DependencyInjection;

namespace LetsEncrypt.Core.Extensions
{
    public static partial class Extensions
    {
        public static IServiceCollection AddAllApplicationServices(this IServiceCollection services)
        {
            services.AddSingleton<ILogger, LocalFileLogger>(); // ConsoleLogger
            services.AddSingleton(typeof(LocalFileHandler));

            return services;
        }
    }
}