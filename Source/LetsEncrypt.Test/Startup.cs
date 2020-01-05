using LetsEncrypt.Core.Extensions;
using LetsEncrypt.Core.Interfaces;
using LetsEncrypt.Core.IO;
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
        protected static LocalFileHandler LocalFileHandler => ServiceProvider.GetRequiredService<LocalFileHandler>();

        #endregion Fields + Properties

        // Private Methods

        private static IServiceProvider InitDependencyInjection()
        {
            var services = new ServiceCollection();
            services.AddAllApplicationServices();
            return services.BuildServiceProvider();
        }
    }
}