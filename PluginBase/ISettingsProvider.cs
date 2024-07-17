namespace PluginBase
{
    public interface ISimpleSetting
    {
        int SettingId { get; set; }
        string Key { get; set; }
        string Value { get; set; }
    }

    public enum GlobalKeyResult
    {
        Added,
        Exists
    }

    public interface ISettingsProvider
    {
        ISimpleSetting GetSetting(string key, bool force = true);

        //ISimpleSetting[] GetSettings(string[] keys, bool force);

        ISimpleSetting SetSetting(string key, string value, bool force = true);

        //ISimpleSetting[] SetSettings(ISimpleSetting[] settings, bool force);

        public GlobalKeyResult RegisterGlobalSetting(string key, string? defaultValue);
    }
}