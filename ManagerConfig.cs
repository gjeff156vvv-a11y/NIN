using StorageLib;
using ConfParser;
 
namespace NIN;

public static class ManagerConfig
{
    private static FileStorage setting = new FileStorage("settings.config");

    public static void Load()
    {
        Config config;
        string content = setting.ReadAll();
        if (content == null) config  = new Config();
        else config = new Config(Parser.Deserialize(content));

        Config.Initialize(config); 
    }

    public static void Save()
    {
        string text = Parser.Serialize(Config.Instance.AppConfig);
        setting.SaveAll(text);
    }
}
