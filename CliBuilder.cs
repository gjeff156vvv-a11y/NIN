using System.CommandLine;
namespace NIN;

public static class CliBuilder
{
    public static RootCommand Build()
    {
        var rootCommand = new RootCommand( "CLI утилита для занесение книг в библиотеку");
        var pathArgument = new Argument<string?>("path");
        rootCommand.Arguments.Add(pathArgument);

        rootCommand.Subcommands.Add(new SetCommand());

        rootCommand.SetAction((parseResult) => 
        {
            string? path= parseResult.GetValue(pathArgument)!;
            Root(parseResult,path);
        });

        return rootCommand;
    }

    private static void Root(ParseResult parseResult, string? path = null)
    {
        ManagerConfig.Load();
        Bookmarks bookmarks = new();

        string bookPath;
        if(path != null) bookPath = path!;
        else bookPath = Config.Instance!.AppConfig[Config.DirKey];

        bookmarks.ProcessNewBook(bookPath, Config.Instance!.AppConfig[Config.JsonKey]);
    }
}
