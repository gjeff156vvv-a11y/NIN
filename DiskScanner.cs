using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;
namespace NIN;

public static class DiskScanner
{
    private static string GetName(string bookPath)
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

    private static void AddBooks(BookDirectory dir, string[] bookPath, HashSet<string> seenCmd)
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

    public static BookDirectory Scan(string currentPath,HashSet<string> seenCmd)
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
}
