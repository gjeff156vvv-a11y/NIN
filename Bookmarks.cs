using System.Text.Encodings.Web;
using System.Text.Json;

namespace NIN;

public class Bookmarks
{
    HashSet<string> NewDiskRegistry = new();
    HashSet<string> OldJsonRegistry = new();
    List<IElementMenu> ResultDir = new List<IElementMenu>(); // Избегаем багов мутации

    private readonly JsonSerializerOptions jsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        PropertyNameCaseInsensitive = true,
        AllowOutOfOrderMetadataProperties = true 
    };

    public void ProcessNewBook(string rootPath, string jsonPath)
    {
        if (!File.Exists(jsonPath)) return;

        string jsonText = File.ReadAllText(jsonPath);
        List<IElementMenu> oldRootDir = JsonSerializer.Deserialize<List<IElementMenu>>(jsonText,jsonOptions)!;
        List<IElementMenu> newRootDir = DiskScanner.Scan(rootPath, NewDiskRegistry);

        OldJsonRegistry = RegisterOldBooks(oldRootDir);

        ResultDir = CompoundDir(oldRootDir!,newRootDir);
        string newEntry = JsonSerializer.Serialize(ResultDir, jsonOptions);
        File.WriteAllText(jsonPath,newEntry);
    }

    
    private List<IElementMenu> CompoundDir(List<IElementMenu> oldDir, List<IElementMenu> newDir)
    {
        List<IElementMenu> resultDir = new List<IElementMenu>(oldDir); // Избегаем багов мутации

        for (int i = resultDir.Count - 1; i >= 0; i--)
        {
            if (resultDir[i].Type == ElementConstants.TypeBookmark)
            {
                if (!NewDiskRegistry.Contains(((Book)resultDir[i]).Cmd))
                    resultDir.RemoveAt(i);
            }
            else if (resultDir[i] is BookDirectory oldFolder)
            {
                foreach (BookDirectory newFolder in newDir.OfType<BookDirectory>())
                {
                    if (oldFolder.Label == newFolder.Label)
                    {
                        List<Book> updatedBooks = new List<Book>();
                        foreach (Book b in oldFolder.Items)
                            if (NewDiskRegistry.Contains(b.Cmd)) updatedBooks.Add(b);

                        foreach (Book b in newFolder.Items)
                            if (!OldJsonRegistry.Contains(b.Cmd)) updatedBooks.Add(b);

                        oldFolder.Items = updatedBooks;
                    }
                }
            }
        }

        foreach (var file in newDir.OfType<Book>())
            if (!OldJsonRegistry.Contains(file.Cmd)) resultDir.Add(file);

        foreach (var dir in newDir.OfType<BookDirectory>())
            if (!oldDir.OfType<BookDirectory>().Any(odir => odir.Label == dir.Label)) resultDir.Add(dir);

        return resultDir;
    }

    private HashSet<string> RegisterOldBooks(List<IElementMenu> rootDir)
    {
        HashSet<string> newRegistry = new HashSet<string>();

        foreach (var file in rootDir.OfType<Book>())
            newRegistry.Add(file.Cmd);

        foreach (var folder in rootDir.OfType<BookDirectory>())
        {
            foreach (var subBook in folder.Items)
                newRegistry.Add(subBook.Cmd);
        }

        return newRegistry;
    }
    public SyncReport GetSyncReport()
    {
        int totalFolders = 0;
        int totalBooks = 0;

        foreach (var item in ResultDir)
        {
            if (item is BookDirectory folder)
            {
                totalFolders++;
                totalBooks += folder.Items.Count; // Считаем книги в папках
            }
            else if (item is Book)
            {
                totalBooks++; // Считаем книги в корне
            }
        }

        return new SyncReport { FoldersCount = totalFolders, BooksCount = totalBooks };
    }
}
