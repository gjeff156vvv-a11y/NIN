using System.CommandLine;
namespace NIN;

public class LibraryCommand: Command
{
    public LibraryCommand(): base("lib", "посмотреть количество книг и папок в библиотеке")
    {
        this.SetAction(Execute);
    }

    private void Execute(ParseResult parseResult)
    {
        ManagerConfig.Load();
        var oldDir = Bookmarks.ReadBookmarks(Config.Instance.AppConfig[Config.JsonKey]);
        Status.GetSyncReport(oldDir);
    }
}
