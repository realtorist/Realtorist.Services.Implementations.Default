using System;
using Microsoft.Extensions.DependencyInjection;
using Realtorist.Extensions.Base;
using Realtorist.Extensions.Base.Helpers;
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
    /// Provides extension with default service implementations
    /// </summary>
    public class DefaultServicesExtension : IConfigureServicesExtension
    {
        public int Priority => (int)ExtensionPriority.RegisterDefaultImplementations;

        public void ConfigureServices(IServiceCollection services, IServiceProvider serviceProvider)
        {
            services.AddScopedServiceIfNotRegisteredYet<IEmailClient, EmailClient>();

            services.AddTransientServiceIfNotRegisteredYet<ICacheFactory, InMemoryLFUCacheFactory>();

            services.AddSingletonServiceIfNotRegisteredYet<CacheSettingsProvider>();
            services.AddSingletonServiceIfNotRegisteredYet<ISettingsProvider>(x => x.GetRequiredService<CacheSettingsProvider>());
            services.AddSingletonServiceIfNotRegisteredYet<ICachedSettingsProvider>(x => x.GetRequiredService<CacheSettingsProvider>());

            services.AddSingletonServiceIfNotRegisteredYet<IEncryptionProvider, DefaultEncryptionProvider>();

            services.AddSingletonServiceIfNotRegisteredYet<IEventLogger, EventLogger>();

            services.AddTransientServiceIfNotRegisteredYet<ICoordinatesFixer, CoordinatesFixer>();
        }
    }
}
