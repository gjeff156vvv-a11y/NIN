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

        var resultDir = CompoundDir(oldRootDir!,newRootDir);
        string newEntry = JsonSerializer.Serialize(resultDir, jsonOptions);
        File.WriteAllText(jsonPath,newEntry);
    }

    
    private List<IElementMenu> CompoundDir(List<IElementMenu> oldDir, List<IElementMenu> newDir)
    {
        List<IElementMenu> resultDir = oldDir;
        
        for(int i = resultDir.Count-1; i>0; i--)
        {
            if(resultDir[i].Type == ElementConstants.TypeBookmark)
            {
                if(!NewDiskRegistry.Contains(((Book)resultDir[i]).Cmd))
                    resultDir.RemoveAt(i);
            }
            else
            {
                for(int j = 0; j < newDir.Count; j++)
                {
                    if(resultDir[i].Label == newDir[j].Label)
                        ((BookDirectory)resultDir[i]).Items = CompoundDir(((BookDirectory)resultDir[i]).Items,((BookDirectory)newDir[j]).Items);
                }
            }
        }
        foreach (var file in newDir.OfType<Book>())
        {
            if(!OldJsonRegistry.Contains(file.Cmd))
                resultDir.Add(file);
        }
        foreach (var dir in newDir.OfType<BookDirectory>())
        {
            if(!oldDir.OfType<BookDirectory>().Any(odir => odir.Label == dir.Label))
                resultDir.Add(dir);
        }
        return resultDir;
    }

    private HashSet<string> RegisterOldBooks(List<IElementMenu> rootDir)
    {
        HashSet<string> newRegistry = new();
        var dir = rootDir.OfType<BookDirectory>();
        var book = rootDir.OfType<Book>();

        foreach (var file in book)
            newRegistry.Add(file.Cmd);

        foreach(var folder in dir)
            newRegistry.UnionWith(RegisterOldBooks(folder.Items));

        return newRegistry;
    }
}
