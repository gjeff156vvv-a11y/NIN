namespace NIN;

public struct SyncReport
{
    public int FoldersCount { get; set; }
    public int BooksCount { get; set; }
}

public static class ReportPrinter
{
    public static void Print(SyncReport report)
    {
        Console.WriteLine("\n✨ Синхронизация успешно завершена!");
        Console.WriteLine($"📂 Всего папок на первом уровне: \u001b[34m{report.FoldersCount}\u001b[0m");
        Console.WriteLine($"📚 Всего книг в базе:           \u001b[32m{report.BooksCount}\u001b[0m");
    }
}

