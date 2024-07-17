using PluginBase;
using PluginBase.Models;
using Serilog;
using System.Collections.Concurrent;
using System.Reflection;

namespace Bulk_Uploader_Electron.Utils
{
    public class PluginManager
    {
        public static readonly PluginManager Instance = new PluginManager();

        private ConcurrentDictionary<string, IJobExecutionPlugin> plugins = new System.Collections.Concurrent.ConcurrentDictionary<string, IJobExecutionPlugin>();

        private ConcurrentDictionary<string, string> globalSettings = new System.Collections.Concurrent.ConcurrentDictionary<string, string>();

        public void RegisterAssembly(Assembly assembly)
        {
            foreach (Type type in assembly.GetTypes())
            {
                if (typeof(IJobExecutionPlugin).IsAssignableFrom(type))
                {
                    IJobExecutionPlugin result = (Activator.CreateInstance(type) as IJobExecutionPlugin)!;
                    if (result != null)
                    {
                        try
                        {
                            RegisterPlugin(result);
                        }
                        catch (Exception ex)
                        {
                            Log.Error("PluginManager.RegisterAssembly", ex);
                        }
                    }
                }
            }
        }

        public void RegisterPlugin(IJobExecutionPlugin plugin)
        {
            var name = plugin.GetType().Name;
            if (plugins.ContainsKey(name))
            {
                throw new Exception($"{name} Plugin already registered");
            }
            if (!plugins.TryAdd(plugin.GetType().Name, plugin))
            {
                throw new Exception($"{name} Plugin could not be added to plugin registry");
            }
        }

        public void RegisterGlobalSetting(string setting)
        {
            if (globalSettings.ContainsKey(setting))
            {
                throw new Exception($"Setting '{setting}' is already registered");
            }
            if (!globalSettings.TryAdd(setting, setting))
            {
                throw new Exception($"Setting {setting} could not be added to global settings registry");
            }
        }

        public IJobExecutionPlugin[] Plugins
        {
            get => plugins.Values.ToArray();
        }

        public string[] GlobalSettings
        {
            get => globalSettings.Values.ToArray();
        }
    }
}