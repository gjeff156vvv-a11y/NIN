using System.CommandLine;
namespace NIN;

public static class CliBuilder
{
    public static RootCommand Build()
    {
        var rootCommand = new NINRootComand();

        rootCommand.Subcommands.Add(new SetCommand());

        return rootCommand;
    }

}
