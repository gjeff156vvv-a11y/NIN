using System.CommandLine;
namespace NIN;

public class NINRootComand: RootCommand
{
    private readonly Argument<string> _pathArgument = new Argument<string?>("path");
    public NINRootComand() : base("CLI утилита для занесение книг в библиотеку")
    {
        Arguments.Add(_pathArgument);
        this.SetAction(Execute);
    }

    private void Execute(ParseResult parseResult)
    {
        string? path= parseResult.GetValue(_pathArgument)!;
        ManagerConfig.Load();
        Bookmarks bookmarks = new();

        string bookPath;
        if(path != null) bookPath = path!;
        else bookPath = Config.Instance!.AppConfig[Config.DirKey];

        bookmarks.ProcessNewBook(bookPath, Config.Instance!.AppConfig[Config.JsonKey]);
        SyncReport report = bookmarks.GetSyncReport();
        ReportPrinter.Print(report);
    }
}
