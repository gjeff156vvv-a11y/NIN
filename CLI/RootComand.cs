using System.CommandLine;
namespace NIN;

public class NINRootComand: RootCommand
{
    private readonly Argument<string?> _pathArgument = new Argument<string?>("path")
    {
        Arity = ArgumentArity.ZeroOrOne 
    };
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
        string jsonPath = Config.Instance!.AppConfig[Config.JsonKey];

        bookmarks.ProcessNewBook(bookPath,jsonPath);
        var oldDir = Bookmarks.ReadBookmarks(jsonPath);
        Status.GetSyncReport(oldDir);
    }
}
