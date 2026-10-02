using System.CommandLine;

namespace NIN;

class Program
{
    static int Main(string[] args)
    {
        // Вызываем сборку нашей CLI структуры из отдельного файла
        RootCommand rootCommand = CliBuilder.Build();

        // 2. Вызываем новый метод Invoke БЕЗ await
        return rootCommand.Parse(args).Invoke();    
    }
}
