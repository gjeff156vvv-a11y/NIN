using System.CommandLine;
namespace NIN;

class Program
{
    static void Main(string[] args)
    {
        var rootCommand = new RootCommand("CLI утилита для занесение книг в библиотеку");

        var setCommand = new Command("set", "изменить настройки приложения");

        var settingNameArgument = new Argument<string>("setting-name", "Имя настройки (dir, delay, fgcolor)");
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

            Console.WriteLine($"[Конфиг] Изменяем {settingName} на значение: {value}");
        });

        return await rootCommand.InvokeAsync(args);

    }
}
