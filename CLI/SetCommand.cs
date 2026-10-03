using System.CommandLine;
namespace NIN;

public class SetCommand : Command
{
    // Объявляем аргументы как приватные поля класса, чтобы они были доступны в SetAction
    private readonly Argument<string> _nameArgument = new("setting-name");
    private readonly Argument<string> _valueArgument = new("value");

    public SetCommand() : base("set", "Изменить настройки приложения")
    {
        // 1. Добавляем аргументы
        Arguments.Add(_nameArgument);
        Arguments.Add(_valueArgument);

        // 2. Привязываем обработчик
        this.SetAction(Execute);
    }

    private void Execute(ParseResult parseResult)
    {
        string name = parseResult.GetValue(_nameArgument)!;
        string value = parseResult.GetValue(_valueArgument)!;
        
        // Вызываем логику (можно вызывать методы вашего менеджера конфигурации)
        ManagerConfig.Load();
        Config.Instance!.AppConfig[name] = value;
        ManagerConfig.Save();
        
        Console.WriteLine($"Успешно перезаписано {name} на {value}");
    }
}

