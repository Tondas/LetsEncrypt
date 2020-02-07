using LetsEncrypt.Client.Interfaces;
using LetsEncrypt.Client.IO;
using LetsEncrypt.Client.Loggers;
using Microsoft.Extensions.DependencyInjection;

namespace LetsEncrypt.Client.Extensions
{
    public static partial class Extensions
    {
        public static IServiceCollection AddAllApplicationServices(this IServiceCollection services)
        {
            services.AddSingleton<ILogger, LocalFileLogger>(); // ConsoleLogger
            services.AddSingleton(typeof(LocalStorage));

            return services;
        }
    }
}