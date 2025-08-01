using System;
using System.Collections.Generic;
using Loda.Share;
namespace Loda;

class Program
{
    private static MenuUI menu = new(Localization.T("AppTitle"), Constanst.CreateMenuItems(), 0, 0, 64);

    static void Main(string[] args)
    {
        SettingHelper.Load();
        ConsoleHelper.SetupConsole(64);
        ConsoleHelper.EnableCtrlCExit();

        var engine = new Engine();
        engine.UIManager.SetDrawAction(() => menu.DrawMenu());

        // Đăng ký callback đổi ngôn ngữ để tái tạo menu
        Localization.OnLanguageChanged += () =>
        {
            menu = new MenuUI(Localization.T("AppTitle"), Constanst.CreateMenuItems(), 0, 0, 64);
        };

        engine.OnUpdate += _ =>
        {
            if (menu.IsActive && Console.KeyAvailable)
            {
                var key = Console.ReadKey(true);
                menu.HandleInput(key);
            }
            else if (!menu.IsActive)
            {
                // Nếu mục hiện tại là Exit thì gọi OnSelect để thoát
                var items = Constanst.CreateMenuItems();
                var exitItem = items.FindLast(i => i.Title == Loda.Share.Localization.T("Exit"));
                if (menu != null && exitItem != null && menu.IsActive == false)
                {
                    // Kiểm tra nếu menu đang ở main menu và mục Exit được chọn
                    exitItem.OnSelect?.Invoke();
                }
                menu!.Reset();
            }
        };

        engine.OnRender += () => { /* UIManager sẽ tự vẽ menu qua SetDrawAction */ };

        engine.Run();
    }
}
