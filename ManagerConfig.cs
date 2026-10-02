using StorageLib;
using Tomlyn.Serialization;
using Tomlyn;
 
namespace NIN;

[TomlSerializable(typeof(Config))]
internal partial class ConfigTomlContext : TomlSerializerContext
{
}

public static class ManagerConfig
{
    private static string fileName = "settings.toml";

    private static readonly TomlTypeInfo<Config> ConfigTypeInfo = ConfigTomlContext.Default.Config;

    public static Config Load()
    {
        string content = Storage.ReadAll(fileName);
        if (content == null)
        {
            var defaultConfig = new Config();
            Save(defaultConfig);
            return defaultConfig;
        }
        return TomlSerializer.Deserialize(content, ConfigTypeInfo) ?? new Config();
    }

    public static void Save(Config config)
    {
        string tomlString = TomlSerializer.Serialize(config, ConfigTypeInfo);
        Storage.SaveAll(tomlString,fileName);
    }
}
