using System.CommandLine;
namespace NIN;

class Program
{
    static void Main(string[] args)
    {
        var rootCommand = new RootCommand("CLI утилита для занесение книг в библиотеку");

        var setCommand = new Command("set", "изменить настройки приложения");

        var settingNameArgument = new Argument<string>("setting-name", "Имя настройки (BookPath, BookmarksPath)");
        var valueArgument = new Argument<string>("value", "Новое значение для настройки");

        // Добавляем аргументы внутрь команды set
        setCommand.AddArgument(settingNameArgument);
        setCommand.AddArgument(valueArgument);

        // 5. Добавляем подкоманду set в корневую команду
        rootCommand.Add(setCommand);

        setCommand.Action = new CliAction((ParseResult parseResult) =>
        {
            // Извлекаем значения аргументов по их объектам
            string settingName = parseResult.GetValue(settingNameArgument)!.ToLower();
            string value = parseResult.GetValue(valueArgument)!;

            Config configToModify = ManagerConfig.Load();

            switch(settingName)
            {
                case "dir": configToModify.PathBookDir = value; break;
                case "json": configToModify.PathBookmarksJson = value; break;
            }
            ManagerConfig.Save(configToModify);

            Console.WriteLine($"[Конфиг] Успешно перезаписан. {settingName} теперь равен: {value}");
        });

        rootCommand.Action = new CliAction((ParseResult parseResult) => 
        {
            Config config = ManagerConfig.Load();

            
            string[] existingFiles = Directory.GetFiles(settings.PathBookDir, "*.pdf");
            foreach (string filePath in existingFiles)
            {
                bookmarks.ProcessNewBook(filePath, settings.PathBookmarksJson);
            }
        })
        return await rootCommand.InvokeAsync(args);

    }
}
