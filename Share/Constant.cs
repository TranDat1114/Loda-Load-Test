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
                Console.ReadKey(true);
            }),
            new MenuItem(Localization.T("Vietnamese"), () => {
                Localization.SetLanguage(AppLanguage.Vietnamese);
                Console.WriteLine(Localization.T("Language") + ": Vietnamese");
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
            Console.ReadKey(true);
        }),
        new MenuItem(Localization.T("RunTest"), () => {
            Console.WriteLine("[Run Test] ...");
            Console.ReadKey(true);
        }),
        new MenuItem(Localization.T("SavedTests"), () => {
            Console.WriteLine("[Saved Tests] ...");
            Console.ReadKey(true);
        }),
        new MenuItem(Localization.T("Settings"), () => {}, CreateSettingsMenu()),
        new MenuItem(Localization.T("AboutHelp"), () => {
            Console.WriteLine(Localization.T("AboutText"));
            Console.ReadKey(true);
        }),
        new MenuItem(Localization.T("Exit"), () => {
            Environment.Exit(0);
        })
    };
}
