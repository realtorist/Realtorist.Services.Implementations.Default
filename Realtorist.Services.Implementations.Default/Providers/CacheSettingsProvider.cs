using Realtorist.DataAccess.Abstractions;
using Realtorist.Models.Settings;
using Realtorist.Services.Abstractions.Providers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Realtorist.Services.Implementations.Default.Providers
{
    /// <summary>
    /// Settings provider which loads them from the cache or from database if not cached
    /// </summary>
    public class CacheSettingsProvider : ICachedSettingsProvider
    {
        private readonly ISettingsDataAccess _settingsDataAccess;
        private readonly IDictionary<string, object> _cache = new Dictionary<string, object>();

        public CacheSettingsProvider(ISettingsDataAccess settingsDataAccess)
        {
            _settingsDataAccess = settingsDataAccess ?? throw new ArgumentNullException(nameof(settingsDataAccess));
        }

        public async Task<T> GetSettingAsync<T>(string settingType) where T : new()
        {
            if (!_cache.ContainsKey(settingType))
            {
                _cache[settingType] = await _settingsDataAccess.GetSettingAsync<T>(settingType);
            }

            return (T)_cache[settingType];
        }

        public void ResetSettingCache(string settingType)
        {
            if (_cache.ContainsKey(settingType))
            {
                _cache.Remove(settingType);
            }
        }
    }
}
