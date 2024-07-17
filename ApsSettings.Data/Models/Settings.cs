using PluginBase;

namespace ApsSettings.Data.Models
{
    public class Setting : ISimpleSetting
    {
        public int SettingId { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }
}