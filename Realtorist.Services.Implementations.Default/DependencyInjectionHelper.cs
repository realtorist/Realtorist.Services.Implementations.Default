using Microsoft.Extensions.DependencyInjection;
using Realtorist.Services.Abstractions.Cache;
using Realtorist.Services.Abstractions.Communication;
using Realtorist.Services.Abstractions.Events;
using Realtorist.Services.Abstractions.Geo;
using Realtorist.Services.Abstractions.Providers;
using Realtorist.Services.Implementations.Default.Cache;
using Realtorist.Services.Implementations.Default.Communication;
using Realtorist.Services.Implementations.Default.Events;
using Realtorist.Services.Implementations.Default.Geo;
using Realtorist.Services.Implementations.Default.Providers;

namespace Realtorist.Services.Implementations.Default
{
    /// <summary>
    /// Provides dependency injection helper methods
    /// </summary>
    public static class DependencyInjectionHelper
    {
        /// <summary>
        /// Configures default services
        /// </summary>
        /// <param name="services">Services collection</param>
        public static void ConfigureDefaultServices(this IServiceCollection services)
        {
            services.AddScoped<IEmailClient, EmailClient>();

            services.AddTransient<ICacheFactory, InMemoryLFUCacheFactory>();

            services.AddSingleton<CacheSettingsProvider>();
            services.AddSingleton<ISettingsProvider>(x => x.GetRequiredService<CacheSettingsProvider>());
            services.AddSingleton<ICachedSettingsProvider>(x => x.GetRequiredService<CacheSettingsProvider>());

            services.AddSingleton<IEncryptionProvider, DefaultEncryptionProvider>();

            services.AddSingleton<IEventLogger, EventLogger>();

            services.AddTransient<ICoordinatesFixer, CoordinatesFixer>();
        }
    }
}
