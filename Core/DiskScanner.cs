namespace NIN;

public static class DiskScanner
{
    private static string GetName(string bookPath)
    {
        return Path.GetFileNameWithoutExtension(bookPath) ?? Path.GetFileName(bookPath);
    }

    private static List<Book> AddBooks(string[] bookPath, HashSet<string> seenCmd)
    {
        List<Book> books = new List<Book>();
        foreach (var file in bookPath)
        {
            string cmdCommand = $"{Config.Instance!.AppConfig[Config.ProgKey]} {file}";

            if (seenCmd.Contains(cmdCommand)) continue;
            seenCmd.Add(cmdCommand);

            Book newBook = new Book()
            {
                Cmd = cmdCommand,
                Label = GetName(file),
                Description = string.Empty,
                RunInBackground = true,
                RunInTerminal = false
            };
            books.Add(newBook);
        }
        return books;
    }

    public static List<IElementMenu> Scan(string currentPath, HashSet<string> seenCmd)
    {
        List<IElementMenu> currentDirectory = new List<IElementMenu>();

        string[] filePath = System.IO.Directory.GetFiles(currentPath);
        foreach (Book rootBook in AddBooks(filePath, seenCmd))
        {
            currentDirectory.Add(rootBook);
        }

        string[] subDir = System.IO.Directory.GetDirectories(currentPath);
        foreach (var dir in subDir)
        {
            string[] subFiles = System.IO.Directory.GetFiles(dir);
            List<Book> folderBooks = AddBooks(subFiles, seenCmd);

            if (folderBooks.Count > 0)
            {
                BookDirectory childDirectory = new BookDirectory()
                {
                    Label = Path.GetFileName(dir),
                    Items = folderBooks // Теперь типы идеально совпадают!
                };
                currentDirectory.Add(childDirectory);
            }
        }

        return currentDirectory;
    }
}

