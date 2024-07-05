using System;
using System.IO;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Пожалуйста, укажите путь к целевой папке.");
            return;
        }

        string directoryPath = args[0];

        try
        {
            // Проверяем существование указанной папки
            if (!Directory.Exists(directoryPath))
            {
                Console.WriteLine($"Папка '{directoryPath}' не существует.");
                return;
            }

            // Получаем текущее время
            DateTime now = DateTime.Now;

            // Удаляем файлы и подпапки в указанной директории, если они не использовались более 30 минут
            CleanDirectory(directoryPath, now);

            Console.WriteLine("Операция завершена успешно.");
        }
        catch (UnauthorizedAccessException ex)
        {
            Console.WriteLine($"Ошибка доступа: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Произошла ошибка: {ex.Message}");
        }
    }

    static void CleanDirectory(string directoryPath, DateTime currentTime)
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);

        // Перебираем все файлы в текущей директории
        foreach (FileInfo file in directoryInfo.GetFiles())
        {
            try
            {
                // Если файл не использовался более 30 минут, удаляем его
                if ((currentTime - file.LastAccessTime) > TimeSpan.FromMinutes(30))
                {
                    file.Delete();
                    Console.WriteLine($"Удален файл: {file.FullName}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Не удалось удалить файл '{file.FullName}': {ex.Message}");
            }
        }

        // Рекурсивно вызываем этот метод для подпапок
        foreach (DirectoryInfo subDirectory in directoryInfo.GetDirectories())
        {
            CleanDirectory(subDirectory.FullName, currentTime);
        }

        // После удаления файлов, проверяем пустоту текущей папки
        if (IsDirectoryEmpty(directoryPath))
        {
            try
            {
                // Если папка пуста, удаляем её
                directoryInfo.Delete();
                Console.WriteLine($"Удалена папка: {directoryInfo.FullName}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Не удалось удалить пустую папку '{directoryInfo.FullName}': {ex.Message}");
            }
        }
    }

    static bool IsDirectoryEmpty(string path)
    {
        return !Directory.EnumerateFileSystemEntries(path).Any();
    }
}
