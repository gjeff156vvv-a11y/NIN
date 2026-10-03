using System.Text.Encodings.Web;
using System.Text.Json;

namespace NIN;

public class Bookmarks
{
    HashSet<string> NewDiskRegistry = new();
    HashSet<string> OldJsonRegistry = new();
    private readonly JsonSerializerOptions jsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    public void ProcessNewBook(string rootPath, string jsonPath)
    {
        if (!File.Exists(jsonPath)) return;

        string jsonText = File.ReadAllText(jsonPath);
        BookDirectory oldRootDir = JsonSerializer.Deserialize<BookDirectory>(jsonText,jsonOptions)!;
        BookDirectory newRootDir = DiskScanner.Scan(rootPath, NewDiskRegistry);

        OldJsonRegistry = RegisterOldBooks(oldRootDir);

        var resultDir = CompoundDir(oldRootDir!,newRootDir);
        string newEntry = JsonSerializer.Serialize(resultDir, jsonOptions);
        File.WriteAllText(jsonPath,newEntry);
    }

    
    private BookDirectory CompoundDir(BookDirectory oldDir, BookDirectory newDir)
    {
        BookDirectory resultDir = oldDir;
        
        for(int i = resultDir.Items.Count-1; i>0; i--)
        {
            if(resultDir.Items[i].Type == ElementConstants.TypeBookmark)
            {
                if(!NewDiskRegistry.Contains(((Book)resultDir.Items[i]).Cmd))
                    resultDir.Items.RemoveAt(i);
            }
            else
            {
                for(int j = 0; j < newDir.Items.Count; j++)
                {
                    if(resultDir.Items[i].Label == newDir.Items[j].Label)
                        resultDir.Items[i] = CompoundDir((BookDirectory)resultDir.Items[i],(BookDirectory)newDir.Items[j]);
                }
            }
        }
        foreach (var file in newDir.Items.OfType<Book>())
        {
            if(!OldJsonRegistry.Contains(file.Cmd))
                resultDir.Items.Add(file);
        }
        return resultDir;
    }

    private HashSet<string> RegisterOldBooks(BookDirectory rootDir)
    {
        HashSet<string> newRegistry = new();
        var dir = rootDir.Items.OfType<BookDirectory>();
        var book = rootDir.Items.OfType<Book>();

        foreach (var file in book)
            newRegistry.Add(file.Cmd);

        foreach(var folder in dir)
            newRegistry.UnionWith(RegisterOldBooks(folder));

        return newRegistry;
    }
}
