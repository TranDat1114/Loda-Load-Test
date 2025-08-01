using System;

namespace Loda.Share;

    public static class ConsoleHelper
    {
        public static void SetupConsole(int width = 64)
        {
            if (OperatingSystem.IsWindows())
            {
                try { Console.WindowWidth = width; } catch { }
            }
        }

        public static void EnableCtrlCExit()
        {
            Console.CancelKeyPress += (sender, e) =>
            {
                Console.Clear();
                Console.WriteLine("Đã thoát chương trình (Ctrl+C)");
                Environment.Exit(0);
            };
        }
    }

