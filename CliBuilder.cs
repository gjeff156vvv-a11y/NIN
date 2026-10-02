using System.CommandLine;
namespace NIN;

public static class CliBuilder
{
    public static RootCommand Build()
    {
        var rootCommand = new RootCommand( "CLI утилита для занесение книг в библиотеку");
        var setCommand = new Command("set", "изменить настройки приложения");
        var dirCommand = new Command("dir", "Поиск новых книг в указанной папке");

        var settingNameArgument = new Argument<string>("setting-name");
        var valueArgument = new Argument<string>("value");
        var pathArgument = new Argument<string>("path");

        setCommand.Arguments.Add(settingNameArgument);
        setCommand.Arguments.Add(valueArgument);
        dirCommand.Arguments.Add(pathArgument);

        // 5. Добавляем подкоманду set в корневую команду
        rootCommand.Subcommands.Add(setCommand);
        rootCommand.Subcommands.Add(dirCommand);

        setCommand.SetAction(parseResult => 
        {
            string name = parseResult.GetValue(settingNameArgument)!;
            string value = parseResult.GetValue(valueArgument)!;
            Set(name, value);
        });

        dirCommand.SetAction(parseResult => 
        {
            string path= parseResult.GetValue(pathArgument)!;
            Root(parseResult,path);
        });
        rootCommand.SetAction((parseResult) => 
        {
            Root(parseResult);
        });

        return rootCommand;
    }

    private static void Set(string settingName,string value)
    {
        ManagerConfig.Load();
        Config.Instance!.AppConfig[settingName] = value;
        ManagerConfig.Save();
        Console.WriteLine($"[Конфиг] Успешно перезаписан. {Config.Instance!.AppConfig[settingName]} теперь равен: {value}");
    }

    private static void Root(ParseResult parseResult, string? path = null)
    {
        ManagerConfig.Load();
        Bookmarks bookmarks = new();

        string bookPath;
        if(path == null) bookPath = path!;
        else bookPath = Config.Instance!.AppConfig[Config.DirKey];

        string[] existingFiles = Directory.GetFiles(bookPath);
        bookmarks.ProcessNewBook(existingFiles, Config.Instance!.AppConfig[Config.JsonKey]);
    }
}
