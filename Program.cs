using System;
using System.Collections.Generic;
using Loda.Share;
namespace Loda
{
    class Program
    {
        private static MenuUI menu;

        // Hàm tái tạo menu, truyền engine
        public static void RebuildMenu(Engine engine)
        {
            menu = new MenuUI(Localization.T("AppTitle"), Constanst.CreateMenuItems(), 0, 0, 64);
            engine.UIManager.SetDrawAction(() => menu.DrawMenu());
        }

        static void Main(string[] args)
        {
            SettingHelper.Load();
            ConsoleHelper.SetupConsole(64);
            ConsoleHelper.EnableCtrlCExit();

            var engine = new Engine();
            RebuildMenu(engine);

            // Đăng ký callback đổi ngôn ngữ để tái tạo menu
            Localization.OnLanguageChanged += () => RebuildMenu(engine);

            engine.OnUpdate += _ =>
            {
                if (menu.IsActive && Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true);
                    menu.HandleInput(key);
                }
                else if (!menu.IsActive)
                {
                    menu.Reset();
                }
            };

            engine.OnRender += () => { /* UIManager sẽ tự vẽ menu qua SetDrawAction */ };

            engine.Run();
        }
    }
}