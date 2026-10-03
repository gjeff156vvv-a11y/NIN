using System.CommandLine;
namespace NIN;

public static class CliBuilder
{
    public static RootCommand Build()
    {
        var rootCommand = new NINRootComand();

        rootCommand.Subcommands.Add(new SetCommand());
        rootCommand.Subcommands.Add(new CheckCommand());
        rootCommand.Subcommands.Add(new LibraryCommand());

        return rootCommand;
    }

}
