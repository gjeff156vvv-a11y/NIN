using System.Text.Encodings.Web;
using System.Text.Json;
using StorageLib;
using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;

namespace NIN;

public class Bookmarks
{
    private Storage storage = new();

    private readonly JsonSerializerOptions jsonOptions = new()
    {
        WriteIndented = true,
        // Автоматически делает "Cmd" -> "cmd", "Label" -> "label" и т.д.
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        // Чтобы кириллица в названии книг не превращалась в коды типа \u041f
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    public void ProcessNewBook(string bookPath, string jsonPath)
    {
        Thread.Sleep(300); // Даем системе время дописать файл

        if (!File.Exists(jsonPath)) return;

        string cmdCommand = $"zathura {bookPath}";
        string jsonText = storage.ReadAll(jsonPath).Trim();
        
        // Быстрая проверка на дубликат по команде
        if (jsonText.Contains($"\"cmd\": \"{cmdCommand}\"")) return;

         string label = GetName(bookPath);

                // Создаем объект твоей модели
        Book newBook = new()
        {
            Cmd = cmdCommand,
            Label = label,
            Glyph = "bookmark",
            Description = string.Empty,
            RunInBackground = true,
            RunInTerminal = false
        };
// Сериализуем объект в чистую JSON-строку
        string newEntry = JsonSerializer.Serialize(newBook, jsonOptions);

        int lastBracketIndex = jsonText.LastIndexOf(']');
        if (lastBracketIndex != -1)
        {
            string prefix = ",\n";
            
            // Если JSON-массив был пустым `[]`, убираем запятую перед элементом
            if (jsonText.Substring(0, lastBracketIndex).Trim().EndsWith("["))
            {
                prefix = "\n";
            }

            // Вклеиваем сериализованный объект перед закрывающей скобкой
            string updatedJson = jsonText.Substring(0, lastBracketIndex).TrimEnd() + prefix + newEntry + "\n]";
            storage.SaveAll(updatedJson, jsonPath);
        }

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
}
