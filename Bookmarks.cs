using System.Text.Encodings.Web;
using System.Text.Json;
using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;

namespace NIN;

public class Bookmarks
{
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

        List<string> seenCmd = new();

        BookDirectory newRootDir = Scan(rootPath, seenCmd);

        var resultDir = CompoundDir(oldRootDir!,newRootDir);
        string newEntry = JsonSerializer.Serialize(resultDir, jsonOptions);
        File.WriteAllText(jsonPath,newEntry);
    }

    private string GetName(string bookPath)
    {
        try
        {
            // Открываем PDF только для чтения метаданных
            using (PdfDocument pdfDoc = PdfReader.Open(bookPath, PdfDocumentOpenMode.InformationOnly))
            {
                // Если внутри файла прописан Title и он не пустой — берем его
                if (pdfDoc.Info != null && !string.IsNullOrWhiteSpace(pdfDoc.Info.Title))
                {
                    //return pdfDoc.Info.Title.Trim();
                }
            }
        }
        catch
        {
        }
        return Path.GetFileNameWithoutExtension(bookPath) ?? Path.GetFileName(bookPath);
    }

    private void AddBooks(BookDirectory dir, string[] bookPath, List<string> seenCmd)
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
            dir.Items.Add(newBook);
        }
    }

    private BookDirectory Scan(string currentPath,List<string> seenCmd)
    {
        var currentDirectory = new BookDirectory
        {
            Label = Path.GetDirectoryName(currentPath)!
        };

        string[] filePath = System.IO.Directory.GetFiles(currentPath);
        AddBooks(currentDirectory,filePath,seenCmd);

        string[] subDir = System.IO.Directory.GetDirectories((currentPath));

        foreach (var dir in subDir)
        {
            BookDirectory childDirectory = Scan(dir,seenCmd);

            if (childDirectory.Items.Count > 0)
            {
                currentDirectory.Items.Add(childDirectory);
            }
        }

        return currentDirectory;
    }
    
    private BookDirectory CompoundDir(BookDirectory oldDir, BookDirectory newDir)
    {
        BookDirectory resultDir = new();


        return resultDir;
    }
}
