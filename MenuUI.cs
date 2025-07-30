using System;
using System.Collections.Generic;
using System.Text;

namespace Loda
{
    public class MenuItem
    {
        public string Title { get; set; }
        public Action OnSelect { get; set; }
        public MenuUI SubMenu { get; set; }

        public MenuItem(string title, Action onSelect = null, MenuUI subMenu = null)
        {
            Title = title;
            OnSelect = onSelect;
            SubMenu = subMenu;
        }
    }


    public class MenuUI
    {
        private readonly List<MenuItem> _items;
        private int _selectedIndex;
        private readonly int _top;
        private readonly int _left;
        private readonly int _width;
        private readonly string _title;
        private readonly StringBuilder _buffer;
        public bool IsActive { get; private set; } = true;

        public MenuUI(string title, List<MenuItem> items, int left = 0, int top = 0, int width = 64, bool isSubMenu = false)
        {
            _title = title;
            // Nếu là submenu thì tự động thêm lựa chọn Back cuối cùng
            if (isSubMenu)
            {
                _items = new List<MenuItem>(items)
                {
                    new MenuItem(Share.Localization.T("Back"), () => { IsActive = false; })
                };
            }
            else
            {
                _items = items;
            }
            _selectedIndex = 0;
            _left = left;
            _top = top;
            _width = width;
            _buffer = new StringBuilder();
        }

        // Hàm tiện ích tạo submenu
        public static MenuUI CreateSubMenu(string title, List<MenuItem> items, int left = 0, int top = 0, int width = 64)
        {
            return new MenuUI(title, items, left, top, width, true);
        }

        public void DrawMenu()
        {
            _buffer.Clear();
            bool prevCursorVisible = Console.CursorVisible;
            Console.CursorVisible = false;
            Console.SetCursorPosition(_left, _top);
            // Top border
            _buffer.AppendLine($"╔{new string('═', _width - 2)}╗");
            // Title centered
            int titlePad = (_width - 2 - _title.Length) / 2;
            string titleLine = new string(' ', titlePad) + _title + new string(' ', _width - 2 - _title.Length - titlePad);
            _buffer.AppendLine($"║{titleLine}║");
            // Separator
            _buffer.AppendLine($"╠{new string('═', _width - 2)}╣");
            // Menu items
            for (int i = 0; i < _items.Count; i++)
            {
                string prefix;
                // Xác định nút Back hoặc Exit để đánh số 0
                var isBack = i == _items.Count - 1 && _items[i].Title == Share.Localization.T("Back");
                var isExit = i == _items.Count - 1 && _items[i].Title == Share.Localization.T("Exit");
                if (isBack || isExit)
                    prefix = "0. ";
                else
                    prefix = $"{i + 1}. ";
                if (i == _selectedIndex) prefix = "> " + prefix;
                else prefix = "  " + prefix;
                string title = _items[i].Title;
                string line = prefix + title;
                if (line.Length > _width - 4) line = line.Substring(0, _width - 4);
                line = line.PadRight(_width - 4);
                if (i == _selectedIndex)
                    _buffer.AppendLine($"║ \u001b[7m{line}\u001b[0m ║"); // highlight
                else
                    _buffer.AppendLine($"║ {line} ║");
            }
            // Bottom border
            _buffer.AppendLine($"╚{new string('═', _width - 2)}╝");
            Console.SetCursorPosition(_left, _top);
            Console.Write(_buffer.ToString());
            // Chỉ hiện lại con trỏ khi menu không còn active
            if (!IsActive)
                Console.CursorVisible = prevCursorVisible;
        }

        public void HandleInput(ConsoleKeyInfo key)
        {
            // Hỗ trợ chọn nhanh bằng phím số
            if (char.IsDigit(key.KeyChar))
            {
                int idx = -1;
                if (key.KeyChar == '0')
                {
                    // 0 luôn là Back hoặc Exit (cuối danh sách)
                    idx = _items.Count - 1;
                }
                else
                {
                    int num = key.KeyChar - '1';
                    if (num >= 0 && num < _items.Count - 1)
                        idx = num;
                }
                if (idx >= 0 && idx < _items.Count)
                {
                    _selectedIndex = idx;
                    // Giả lập Enter
                    HandleInput(new ConsoleKeyInfo('\r', ConsoleKey.Enter, false, false, false));
                    return;
                }
            }
            switch (key.Key)
            {
                case ConsoleKey.UpArrow:
                    _selectedIndex = (_selectedIndex - 1 + _items.Count) % _items.Count;
                    break;
                case ConsoleKey.DownArrow:
                    _selectedIndex = (_selectedIndex + 1) % _items.Count;
                    break;
                case ConsoleKey.Enter:
                    var item = _items[_selectedIndex];
                    if (item.SubMenu != null)
                    {
                        Console.Clear(); // Chỉ clear khi chuyển sang menu mới
                        item.SubMenu.Reset();
                        while (item.SubMenu.IsActive)
                        {
                            item.SubMenu.DrawMenu();
                            if (Console.KeyAvailable)
                            {
                                var subKey = Console.ReadKey(true);
                                item.SubMenu.HandleInput(subKey);
                            }
                            Thread.Sleep(10);
                        }
                        Console.Clear(); // Clear khi quay lại menu cha
                        DrawMenu();
                    }
                    else
                    {
                        item.OnSelect?.Invoke();
                        IsActive = false;
                    }
                    break;
                case ConsoleKey.Escape:
                    // Nếu là menu con thì chỉ back về menu cha, không exit toàn bộ
                    IsActive = false;
                    break;
            }
        }

        public void Reset()
        {
            _selectedIndex = 0;
            IsActive = true;
        }
    }
}
