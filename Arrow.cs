using System;
using System.Diagnostics;
using System.IO;

internal static class Arrow
{
    private static int selectedIndex;

    public static void Start()
    {
        DriveInfo[] drives = DriveInfo.GetDrives();
        selectedIndex = 0;

        while (true)
        {
            Console.Clear();
            Console.WriteLine("Выберите диск:");

            for (int i = 0; i < drives.Length; i++)
            {
                string prefix = i == selectedIndex ? "==> " : "    ";
                Console.ForegroundColor = i == selectedIndex ? ConsoleColor.Yellow : ConsoleColor.White;
                double usedSpaceGb = Math.Round((drives[i].TotalSize - drives[i].TotalFreeSpace) / (1024.0 * 1024 * 1024), 2); // Рассчитываем использованное пространство диска в гигабайтах
                Console.WriteLine($"{prefix}{i + 1}. {drives[i].Name} ({usedSpaceGb} GB used)");
            }
            Console.ForegroundColor = ConsoleColor.White;
            ConsoleKeyInfo key = Console.ReadKey();

            if (key.Key == ConsoleKey.Escape)
            {
                return;
            }
            else if (key.Key == ConsoleKey.UpArrow && selectedIndex > 0)
            {
                selectedIndex--;
            }
            else if (key.Key == ConsoleKey.DownArrow && selectedIndex < drives.Length - 1)
            {
                selectedIndex++;
            }
            else if (key.Key == ConsoleKey.Enter)
            {
                ExploreDrive(drives[selectedIndex]);
            }
        }
    }

    private static void ExploreDrive(DriveInfo drive)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine($"Путь: {drive.RootDirectory.FullName}");
            Console.WriteLine("\nФайлы и папки:");
            DisplayFileSystemEntries(drive.RootDirectory);

            ConsoleKeyInfo key = Console.ReadKey();
            if (key.Key == ConsoleKey.Escape)
            {
                break;
            }
            else if (key.Key == ConsoleKey.Enter)
            {
                FileSystemInfo selectedEntry = drive.RootDirectory.GetFileSystemInfos()[selectedIndex];

                if (selectedEntry is DirectoryInfo directory)
                {
                    ExploreDirectory(directory);
                }
                else if (selectedEntry is FileInfo file)
                {
                    OpenFile(file);
                }
            }
            else if (key.Key == ConsoleKey.UpArrow && selectedIndex > 0)
            {
                selectedIndex--;
            }
            else if (key.Key == ConsoleKey.DownArrow)
            {
                FileSystemInfo[] entries = drive.RootDirectory.GetFileSystemInfos();
                if (selectedIndex < entries.Length - 1)
                {
                    selectedIndex++;
                }
            }
        }
    }

    private static void ExploreDirectory(DirectoryInfo directory)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine($"Путь: {directory.FullName}");
            Console.WriteLine("\nФайлы и папки:");
            DisplayFileSystemEntries(directory);

            ConsoleKeyInfo key = Console.ReadKey();

            if (key.Key == ConsoleKey.Escape)
            {
                break;  // Вместо возврата, теперь мы просто выходим из цикла
                        // так же необходимо добавить "break;" в ExploreDrive, если мы хотим возвращаться к предыдущему меню, а не закрывать приложение.
            }

            else if (key.Key == ConsoleKey.Enter)
            {
                FileSystemInfo selectedEntry = directory.GetFileSystemInfos()[selectedIndex];

                if (selectedEntry is DirectoryInfo subDirectory)
                {
                    ExploreDirectory(subDirectory);
                }
                else if (selectedEntry is FileInfo file)
                {
                    OpenFile(file);
                }
            }
            else if (key.Key == ConsoleKey.UpArrow && selectedIndex > 0)
            {
                selectedIndex--;
            }
            else if (key.Key == ConsoleKey.DownArrow)
            {
                FileSystemInfo[] entries = directory.GetFileSystemInfos();

                if (selectedIndex < entries.Length - 1)
                {
                    selectedIndex++;
                }
            }
        }
    }

    private static void DisplayFileSystemEntries(DirectoryInfo directory)
    {
        FileSystemInfo[] entries = directory.GetFileSystemInfos();

        for (int i = 0; i < entries.Length; i++)
        {
            string prefix = i == selectedIndex ? "==> " : "    ";
            Console.ForegroundColor = i == selectedIndex ? ConsoleColor.Yellow : ConsoleColor.White;
            DateTime creationTime = entries[i].CreationTime; // Получаем дату и время создания файла или директории
            Console.WriteLine($"{prefix}{i + 1}. {entries[i].Name} (Created at {creationTime})");
        }

        Console.ForegroundColor = ConsoleColor.White;
    }

    private static void OpenFile(FileInfo file)
    {
        Console.Clear();
        Console.WriteLine($"Открытие файла: {file.FullName}");

        try
        {
            Process.Start(new ProcessStartInfo(file.FullName) { UseShellExecute = true });
        }
        catch
        {
            Console.WriteLine("Не могу открыть файл.");
            Console.ReadKey();
        }
    }
}