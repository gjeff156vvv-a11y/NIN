using Pars = System.Collections.Generic.Dictionary<string, string>;
namespace NIN;

public class Config
{
    public static Config Instance { get; private set; }

    public Pars AppConfig{get;private set;}

    public Config()
    {
        AppConfig = new Pars() 
        {
            { "PathBookDir" , "/home/gjeff/Documents/books/" },
            { "PathBookmarksJson", "/home/gjeff/.local/state/noctalia/plugins/data/dunarand/bookmarks/data.json" },
            { "prog", "zathura" }
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
