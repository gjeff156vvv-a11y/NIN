using System.CommandLine;

namespace NIN;

class Program
{
    static async Task<int> Main(string[] args)
    {
        var rootCommand = new RootCommand("CLI утилита для занесение книг в библиотеку");

        var setCommand = new Command("set", "изменить настройки приложения");

        var settingNameArgument = new Argument<string>("setting-name");
        var valueArgument = new Argument<string>("value");

        // Добавляем аргументы внутрь команды set
        setCommand.Arguments.Add(settingNameArgument);
        setCommand.Arguments.Add(valueArgument);

        // 5. Добавляем подкоманду set в корневую команду
        rootCommand.Subcommands.Add(setCommand);

        setCommand.SetAction(parseResult => 
        {
            string name = parseResult.GetValue(settingNameArgument)!;
            string value = parseResult.GetValue(valueArgument)!;
            Set(name, value);
        });

        rootCommand.SetAction((parseResult) => 
        {
            Root(parseResult);
        });

return rootCommand.Parse(args).Invoke();
 }


    private static void Set(string settingName,string value)
    {
        ManagerConfig.Load();
        Config.Instance!.AppConfig[settingName] = value;
        ManagerConfig.Save();
        Console.WriteLine($"[Конфиг] Успешно перезаписан. {Config.Instance!.AppConfig[settingName]} теперь равен: {value}");
    }

    private static void Root(ParseResult parseResult)
    {
        ManagerConfig.Load();
        Bookmarks bookmarks = new();

        string[] existingFiles = Directory.GetFiles(Config.Instance!.AppConfig[Config.DirKey]);
        bookmarks.ProcessNewBook(existingFiles, Config.Instance.AppConfig[Config.JsonKey]);
    }
}
