using Pars = System.Collections.Generic.Dictionary<string, string>;
namespace NIN;

public class Config
{
    public const string DirKey = "dir";
    public const string JsonKey = "json";
    public const string ProgKey = "prog";

    public static Config? Instance { get; private set; }

    public Pars AppConfig{get;private set;}

    public Config()
    {
        AppConfig = new Pars() 
        {
            { DirKey , "/home/gjeff/Documents/books/" },
            { JsonKey, "/home/gjeff/.local/state/noctalia/plugins/data/dunarand/bookmarks/data.json" },
            { ProgKey, "zathura" }
        };
    }

    public Config(Pars newConf)
    {
        AppConfig = newConf;
    }

    public static void Initialize(Config configInstance)
    {
        Instance = configInstance;
    }
}
