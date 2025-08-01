using System;

namespace Loda.Share;

public static class Constanst
{
    // Hàm tạo menu LoadStrategy
    public static MenuUI CreateLoadStrategyMenu() => MenuUI.CreateSubMenu(
        Localization.T("LoadStrategy"),
        new List<MenuItem>
        {
            new MenuItem(Localization.T("Concurrent"), () => {
                Console.WriteLine($"{Localization.T("Saved")}: {Localization.T("Concurrent")}");
                Console.ReadKey(true);
            }),
            new MenuItem(Localization.T("RampUp"), () => {
                Console.WriteLine($"{Localization.T("Saved")}: {Localization.T("RampUp")}");
                Console.ReadKey(true);
            }),
            new MenuItem(Localization.T("RateLimit"), () => {
                Console.WriteLine($"{Localization.T("Saved")}: {Localization.T("RateLimit")}");
                Console.ReadKey(true);
            })
        }
    );

    // Hàm tạo menu Language
    public static MenuUI CreateLanguageMenu() => MenuUI.CreateSubMenu(
        Localization.T("Language"),
        new List<MenuItem>
        {
            new MenuItem(Localization.T("English"), () => {
                Localization.SetLanguage(AppLanguage.English);
                Console.WriteLine(Localization.T("Language") + ": English");
                Console.WriteLine("\n" + Localization.T("ReturnToMainMenuToChangeLanguage"));
                Console.ReadKey(true);
            }),
            new MenuItem(Localization.T("Vietnamese"), () => {
                Localization.SetLanguage(AppLanguage.Vietnamese);
                Console.WriteLine(Localization.T("Language") + ": Vietnamese");
                Console.WriteLine("\n" + Localization.T("ReturnToMainMenuToChangeLanguage"));
                Console.ReadKey(true);
            })
        }
    );

    // Submenu cho Settings
    public static MenuUI CreateSettingsMenu() => MenuUI.CreateSubMenu(
        Localization.T("Settings"),
        new List<MenuItem>
        {
            new MenuItem(Localization.T("Timeout"), () => {
                Console.WriteLine(Localization.T("Timeout") + ":");
                var input = Console.ReadLine();
                Console.WriteLine($"{Localization.T("Saved")}: {input} ms");
                Console.ReadKey(true);
            }),
            new MenuItem(Localization.T("ThreadCount"), () => {
                Console.WriteLine(Localization.T("ThreadCount") + ":");
                var input = Console.ReadLine();
                Console.WriteLine($"{Localization.T("Saved")}: {input}");
                Console.ReadKey(true);
            }),
            new MenuItem(Localization.T("LogLevel"), () => {
                Console.WriteLine(Localization.T("LogLevel"));
                var input = Console.ReadLine();
                Console.WriteLine($"{Localization.T("Saved")}: {input}");
                Console.ReadKey(true);
            }),
            new MenuItem(Localization.T("Retry"), () => {
                Console.WriteLine(Localization.T("Retry") + ":");
                var input = Console.ReadLine();
                Console.WriteLine($"{Localization.T("Saved")}: {input}");
                Console.ReadKey(true);
            }),
            new MenuItem(Localization.T("LoadStrategy"), () => {}, CreateLoadStrategyMenu()),
            new MenuItem(Localization.T("Language"), () => {}, CreateLanguageMenu()),
        }
    );

    public static List<MenuItem> CreateMenuItems() => new List<MenuItem>
    {
        new MenuItem(Localization.T("NewTest"), () => {
            Console.WriteLine("[New Test] ...");
            Console.WriteLine("\n" + Localization.T("PressAnyKeyToReturn"));
            Console.ReadKey(true);
        }),
        new MenuItem(Localization.T("RunTest"), () => {
            Console.WriteLine("[Run Test] ...");
            Console.WriteLine("\n" + Localization.T("PressAnyKeyToReturn"));
            Console.ReadKey(true);
        }),
        new MenuItem(Localization.T("SavedTests"), () => {
            Console.WriteLine("[Saved Tests] ...");
            Console.WriteLine("\n" + Localization.T("PressAnyKeyToReturn"));
            Console.ReadKey(true);
        }),
        // Internal: Test Local Components
        new MenuItem("Test Local Components", () => {}, CreateTestComponentMenu()),
        new MenuItem(Localization.T("Settings"), () => {}, CreateSettingsMenu()),
        new MenuItem(Localization.T("AboutHelp"), () => {
            Console.WriteLine(Localization.T("AboutText"));
            Console.WriteLine("\n" + Localization.T("PressAnyKeyToReturn"));
            Console.ReadKey(true);
        }),
        new MenuItem(Localization.T("Exit"), () => {
            Environment.Exit(0);
        })
    };

    // Submenu test các component nội bộ
    public static MenuUI CreateTestComponentMenu() => MenuUI.CreateSubMenu(
        "Test Local Components",
        new List<MenuItem>
        {
            new MenuItem("Multi Selection", () => {
                var items = new List<string> { "Apple", "Banana", "Orange", "Grape", "Mango" };
                var multi = new Components.MultiSelectionComponent<string>(items, s => s, "Select fruits");
                var selected = multi.Show();
                Console.WriteLine("Selected: " + string.Join(", ", selected));
                Console.WriteLine("Press any key to return...");
                Console.ReadKey(true);
            }),
            new MenuItem("Selection (Pagination Test)", () => {
                var items = new List<string>();
                for (int i = 1; i <= 20; i++) items.Add($"Item {i}");
                var selector = new Components.SelectionComponent<string>(items, s => s, "Select one (pagination test)");
                int idx = selector.Show();
                if (idx >= 0)
                    Console.WriteLine($"Selected: {items[idx]}");
                else
                    Console.WriteLine("No selection.");
                Console.WriteLine("Press any key to return...");
                Console.ReadKey(true);
            })
        }
    );
}
