using System;
using System.Collections.Generic;
using Loda.Share;
namespace Loda
{
    class Program
    {
        static void Main(string[] args)
        {
            SettingHelper.Load();
            ConsoleHelper.SetupConsole(64);
            ConsoleHelper.EnableCtrlCExit();

            var engine = new Engine();
            var menu = new MenuUI("Loda Load Test Menu", Constanst.menuItems, 0, 0, 64);
            engine.UIManager.SetDrawAction(() => menu.DrawMenu());

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