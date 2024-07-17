using PluginBase.Models;
using Serilog;
using System.Collections.Concurrent;
using System.Reflection;

namespace Bulk_Uploader_Electron.ClientApp.src.Utilities
{
    public class BatchManager
    {
        public static readonly BatchManager Instance = new BatchManager();

        private readonly ConcurrentDictionary<string, IBatchRegistry> batches = new System.Collections.Concurrent.ConcurrentDictionary<string, IBatchRegistry>();

        public void RegisterAssembly(Assembly assembly)
        {
            foreach (Type type in assembly.GetTypes())
            {
                if (typeof(IBatchRegistry).IsAssignableFrom(type))
                {
                    IBatchRegistry result = (Activator.CreateInstance(type) as IBatchRegistry)!;
                    if (result != null)
                    {
                        try
                        {
                            RegisterBatch(result);
                        }
                        catch (Exception ex)
                        {
                            Log.Error("BatchManager.RegisterAssembly", ex);
                        }
                    }
                }
            }
        }

        public IBatchRegistry? GetBatch(string key)
        {
            return batches.ContainsKey(key) ? batches[key] : null;
        }

        public void RegisterBatch(IBatchRegistry batch)
        {
            var name = batch.BatchDetails.Key;
            if (string.IsNullOrEmpty(name)) throw new NullReferenceException("Batch key cannot be null");
            if (batches.ContainsKey(name))
            {
                throw new Exception($"{name} Batch already registered");
            }
            if (!batches.TryAdd(name, batch))
            {
                throw new Exception($"{name} Batch could not be added to batch registry");
            }
        }

        public IBatchRegistry[] Batches
        {
            get => batches.Values.ToArray();
        }
    }
}