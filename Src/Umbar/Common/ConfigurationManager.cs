using System.Text.Json;
using Umbar.Models;

namespace Umbar
{
    public static class ConfigurationManager
    {
        private static readonly string _path = Path.Combine(AppContext.BaseDirectory, "config.json");
        private static Config? _config;
        public static async Task<Config> GetAsync(bool force = false)
        {
            if (_config == null || force)
            {
                if (!File.Exists(_path))
                {
                    var json = JsonSerializer.Serialize(new Config(), AppJsonSerializerContext.Default.Config);

                    await File.WriteAllTextAsync(_path, json);
                }
                var content = await File.ReadAllTextAsync(_path);
                _config = JsonSerializer.Deserialize(content, AppJsonSerializerContext.Default.Config)!;
                if (_config == null)
                    throw new ApplicationException("Unable to get config");
            }
            return _config;
        }
        public static async Task<Config?> GetAsync(
            string path
            )
        {
            if (string.IsNullOrWhiteSpace(path))
                return null;

            var content = await File.ReadAllTextAsync(_path);
            var config = JsonSerializer.Deserialize(content, AppJsonSerializerContext.Default.Config)!;

            return config;
        }
        public static async Task UpdateAsync(Config config)
        {
            var json = JsonSerializer.Serialize(config, AppJsonSerializerContext.Default.Config);

            await File.WriteAllTextAsync(_path, json);

            _config = config;
        }
    }

}