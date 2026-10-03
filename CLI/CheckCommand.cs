using System.CommandLine;
namespace NIN;

public class CheckCommand: Command
{
    private readonly Argument<string?> _pathArgument = new Argument<string?>("path")
    {
        Arity = ArgumentArity.ZeroOrOne 
    };
    public CheckCommand() : base("check","Сканировать папку на диске и показать, сколько книг там найдено")
    {
        Arguments.Add(_pathArgument);
        this.SetAction(Execute);
    }

    private void Execute(ParseResult parseResult)
    {
        string? path= parseResult.GetValue(_pathArgument)!;
        ManagerConfig.Load();

        string bookPath;
        if(path != null) bookPath = path!;
        else bookPath = Config.Instance!.AppConfig[Config.DirKey];

        var newDir = DiskScanner.Scan(bookPath, new HashSet<string>());
        if (newDir.Count == 0)
        {
            Console.WriteLine("\n❌ \u001b[31mВ указанной папке файлы книг или подпапки не найдены!\u001b[0m");
            return;
        }
        Status.GetSyncReport(newDir);
    }
}
