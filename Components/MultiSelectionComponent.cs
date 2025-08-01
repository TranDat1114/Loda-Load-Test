using System;
using System.Collections.Generic;
using System.Text;

namespace Loda.Components
{
    // Component cho phép chọn nhiều mục trong danh sách (console)
    public class MultiSelectionComponent<T>
    {
        private readonly List<T> _items;
        private readonly Func<T, string> _displaySelector;
        private readonly HashSet<int> _selectedIndexes = new();
        private int _cursor = 0;
        private readonly string _title;
        private readonly int _width;

        public MultiSelectionComponent(List<T> items, Func<T, string> displaySelector, string title = "Select items", int width = 64)
        {
            _items = items;
            _displaySelector = displaySelector;
            _title = title;
            _width = width;
        }

        public List<T> Show()
        {
            ConsoleKeyInfo key;
            do
            {
                Draw();
                key = Console.ReadKey(true);
                switch (key.Key)
                {
                    case ConsoleKey.UpArrow:
                        _cursor = (_cursor - 1 + _items.Count) % _items.Count;
                        break;
                    case ConsoleKey.DownArrow:
                        _cursor = (_cursor + 1) % _items.Count;
                        break;
                    case ConsoleKey.Spacebar:
                        if (_selectedIndexes.Contains(_cursor))
                            _selectedIndexes.Remove(_cursor);
                        else
                            _selectedIndexes.Add(_cursor);
                        break;
                }
            } while (key.Key != ConsoleKey.Enter && key.Key != ConsoleKey.Escape);

            Console.Clear();
            if (key.Key == ConsoleKey.Enter)
            {
                var result = new List<T>();
                foreach (var idx in _selectedIndexes)
                    result.Add(_items[idx]);
                return result;
            }
            // Nếu nhấn Esc thì trả về danh sách rỗng
            return new List<T>();
        }

        private void Draw()
        {
            // Chỉ clear vùng menu, không clear toàn bộ màn hình
            int menuTop = 2; // Vị trí cố định trên màn hình
            int menuLeft = 2;
            for (int i = 0; i < _items.Count + 7; i++)
            {
                int y = menuTop + i;
                if (y >= 0 && y < Console.BufferHeight)
                {
                    Console.SetCursorPosition(menuLeft, y);
                    Console.Write(new string(' ', _width));
                }
            }
            Console.SetCursorPosition(menuLeft, menuTop);
            var buffer = new StringBuilder();
            buffer.AppendLine($"╔{new string('═', _width - 2)}╗");
            int titlePad = (_width - 2 - _title.Length) / 2;
            string titleLine = new string(' ', titlePad) + _title + new string(' ', _width - 2 - _title.Length - titlePad);
            buffer.AppendLine($"║{titleLine}║");
            buffer.AppendLine($"╠{new string('═', _width - 2)}╣");
            for (int i = 0; i < _items.Count; i++)
            {
                string prefix = _selectedIndexes.Contains(i) ? "[x] " : "[ ] ";
                string cursor = i == _cursor ? "> " : "  ";
                string line = cursor + prefix + _displaySelector(_items[i]);
                if (line.Length > _width - 4) line = line.Substring(0, _width - 4);
                line = line.PadRight(_width - 4);
                if (i == _cursor)
                    buffer.AppendLine($"║ \u001b[7m{line}\u001b[0m ║");
                else
                    buffer.AppendLine($"║ {line} ║");
            }
            buffer.AppendLine($"╚{new string('═', _width - 2)}╝");
            buffer.AppendLine();
            buffer.AppendLine("Space: Toggle | Enter: Confirm | Esc: Cancel");
            Console.Write(buffer.ToString());
        }
    }
}
