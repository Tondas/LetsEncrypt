using LetsEncrypt.Client.Interfaces;
using LetsEncrypt.Client.IO;
using LetsEncrypt.Client.Loggers;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace LetsEncrypt.Test
{
    public class Startup
    {
        #region Fields + Properties

        private static Lazy<IServiceProvider> _serviceProvider = new Lazy<IServiceProvider>(InitDependencyInjection);
        protected static IServiceProvider ServiceProvider => _serviceProvider.Value;

        protected static ILogger Logger => ServiceProvider.GetRequiredService<ILogger>();
        protected static LocalStorage LocalFileHandler => ServiceProvider.GetRequiredService<LocalStorage>();

        #endregion Fields + Properties

        // Private Methods

        private static IServiceProvider InitDependencyInjection()
        {
            var services = new ServiceCollection();
            services.AddSingleton<ILogger, LocalFileLogger>(); // ConsoleLogger
            services.AddSingleton(typeof(LocalStorage));

            return services.BuildServiceProvider();
        }
    }
}