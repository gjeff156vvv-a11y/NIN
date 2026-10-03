namespace NIN;

public static class DiskScanner
{
    private static string GetName(string bookPath)
    {
        return Path.GetFileNameWithoutExtension(bookPath) ?? Path.GetFileName(bookPath);
    }

    private static void AddBooks(List<IElementMenu> dir, string[] bookPath, HashSet<string> seenCmd)
    {
        foreach (var file in bookPath)
        {
            string cmdCommand = $"{Config.Instance!.AppConfig[Config.ProgKey]} {file}";

            if(seenCmd.Contains(cmdCommand)) continue;
            seenCmd.Add(cmdCommand);
            
            string label = GetName(file);
            Book newBook = new()
            {
                Cmd = cmdCommand,
                Label = label,
                Description = string.Empty,
                RunInBackground = true,
                RunInTerminal = false
            };
            dir.Add(newBook);
        }
    }

    public static List<IElementMenu> Scan(string currentPath,HashSet<string> seenCmd)
    {
        List<IElementMenu> currentDirectory = new();
        string[] filePath = System.IO.Directory.GetFiles(currentPath);
        AddBooks(currentDirectory,filePath,seenCmd);

        string[] subDir = System.IO.Directory.GetDirectories((currentPath));

        foreach (var dir in subDir)
        {
            BookDirectory childDirectory = new(){
                Label = Path.GetFileName(dir),
                Items = Scan(dir,seenCmd)
            };

            if (childDirectory.Items.Count > 0)
            {
                currentDirectory.Add(childDirectory);
            }
        }

        return currentDirectory;
    }
}
