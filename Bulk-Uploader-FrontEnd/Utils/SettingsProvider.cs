using ApsSettings.Data;
using ApsSettings.Data.Models;
using PluginBase;

namespace Bulk_Uploader_Electron.Utils
{
    
    
    public class SettingsProvider : ISettingsProvider
    {
        public SettingsProvider(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        private readonly IServiceProvider serviceProvider;

        public GlobalKeyResult RegisterGlobalSetting(string key, string? defaultValue = null)
        {
            if (AppSettings.Instance.GlobalSettings.ContainsKey(key))
            {
                return GlobalKeyResult.Exists;
            }

            if (AppSettings.Instance.GlobalSettings.TryAdd(key, key))
            {
                if (string.IsNullOrEmpty(defaultValue))
                {
                    GetSetting(key, true);
                }
                else
                {
                    SetSetting(key, defaultValue, true);
                }
                    return GlobalKeyResult.Added;
            }

            throw new Exception($"Could not add global setting: {key}");

        }

        public ISimpleSetting GetSetting(string key, bool force = true)
        {
            DataContext context = serviceProvider.GetService<DataContext>()!;
            Setting? setting = context.Settings.Where(p => p.Key == key).FirstOrDefault();
            if (setting == null)
            {
                if (!force) throw new InvalidDataException($"{key} is not found in settings registry");
                setting = new Setting
                {
                    Key = key,
                    Value = ""
                };
                context.Settings.Add(setting);
                context.SaveChanges();
            }
            return setting;
        }

        public ISimpleSetting SetSetting(string key, string value, bool force = true)
        {
            if (string.IsNullOrEmpty(key)) throw new InvalidDataException($"No key if found for settign");
            DataContext context = serviceProvider.GetService<DataContext>()!;
            Setting? oldSetting = context.Settings.Where(p => p.Key == key).FirstOrDefault();
            if (oldSetting == null)
            {
                if (!force) throw new InvalidDataException($"{key} is not found in settings registry");
                var newSetting = new Setting
                {
                    Key = key,
                    Value = value
                };
                context.Settings.Add(newSetting);
                context.SaveChanges();
                return newSetting;
            }
            else
            {
                if (value == oldSetting.Value)
                {
                    return oldSetting;
                }
                else
                {
                    oldSetting.Value = value;
                    context.SaveChanges();
                    return oldSetting;
                }
            }
        }
    }
}