using ApsSettings.Data.Models;
using PluginBase;

namespace ApsSettings.Data.DataUtils
{
    public static class SettingsUtiltities
    {
        public static ISimpleSetting GetSetting(DataContext context, string settingName)
        {
            Setting? setting = context.Settings.Where(p => p.Key == settingName).FirstOrDefault();
            if (setting == null)
            {
                setting = new Setting
                {
                    Key = settingName,
                    Value = ""
                };
                context.Settings.Add(setting);
                context.SaveChanges();
            }
            return setting;
        }
    }
}