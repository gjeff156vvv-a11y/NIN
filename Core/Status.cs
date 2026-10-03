namespace NIN;

public struct SyncReport
{
    public int FoldersCount { get; set; }
    public int BooksCount { get; set; }
}

public static class Status
{
    private static void Print(SyncReport report)
    {
        Console.WriteLine("\n✨ Синхронизация успешно завершена!");
        Console.WriteLine($"📂 Всего папок на первом уровне: \u001b[34m{report.FoldersCount}\u001b[0m");
        Console.WriteLine($"📚 Всего книг в базе:           \u001b[32m{report.BooksCount}\u001b[0m");
    }
    public static void GetSyncReport(List<IElementMenu> ResultDir)
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

        Print(new SyncReport { FoldersCount = totalFolders, BooksCount = totalBooks });
    }
}

