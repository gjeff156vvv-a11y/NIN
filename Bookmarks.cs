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

    public void ProcessNewBook(string[] booksPath, string jsonPath)
    {
        if (!File.Exists(jsonPath)) return;

        string jsonText = File.ReadAllText(jsonPath);
        List<Book> books = JsonSerializer.Deserialize<List<Book>>(jsonText,jsonOptions);

        AddUniqueBooks(books,booksPath);
        string newEntry = JsonSerializer.Serialize(books, jsonOptions);
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
                    return pdfDoc.Info.Title.Trim();
                }
            }
        }
        catch
        {
        }
        return Path.GetFileNameWithoutExtension(bookPath) ?? Path.GetFileName(bookPath);
    }

    private void AddUniqueBooks(List<Book> books, string[] bookPath)
    {
        foreach (var paths in bookPath)
        {
            string cmdCommand = $"zathura {paths}";

            var Dublicate = from b in books
                        where b.Cmd == cmdCommand
                        select b;

            if(!Dublicate.Any())
            {
                string label = GetName(paths);
                Book newBook = new()
                {
                    Cmd = cmdCommand,
                    Label = label,
                    Glyph = "bookmark",
                    Description = string.Empty,
                    RunInBackground = true,
                    RunInTerminal = false
                };
                books.Add(newBook);
            }
        }
    }
}
