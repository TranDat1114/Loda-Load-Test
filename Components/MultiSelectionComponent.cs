using System;
using System.Collections.Generic;
using System.Text;


namespace Loda.Components;

// Component cho phép chọn nhiều mục trong danh sách (console), đồng bộ UI selection/pagination
public class MultiSelectionComponent<T> : BaseSelectionComponent<T>
{
    private readonly HashSet<int> _selectedIndexes = new();
    private int _cursor = 0;

    public MultiSelectionComponent(List<T> items, Func<T, string> displaySelector, string title = "Select items", int width = 64)
        : base(items, displaySelector, title, width) { }

    public List<T> Show()
    {
        ConsoleKeyInfo key;
        while (true)
        {
            DrawMenu();
            key = Console.ReadKey(true);
            bool handled = false;
            if (char.IsDigit(key.KeyChar))
            {
                int num = key.KeyChar - '0';
                if (num == 0)
                {
                    Console.Clear();
                    return new List<T>();
                }
                else if (num >= 1 && num <= PageSize)
                {
                    int idx = StartIdx + num - 1;
                    if (idx < EndIdx)
                    {
                        _cursor = idx;
                        handled = true;
                    }
                }
            }
            if (!handled)
            {
                switch (key.Key)
                {
                    case ConsoleKey.LeftArrow:
                        if (_pageIndex > 0)
                        {
                            _pageIndex--;
                            _cursor = StartIdx;
                        }
                        handled = true;
                        break;
                    case ConsoleKey.RightArrow:
                        if (_pageIndex < TotalPages - 1)
                        {
                            _pageIndex++;
                            _cursor = StartIdx;
                        }
                        handled = true;
                        break;
                    case ConsoleKey.UpArrow:
                        if (_cursor > StartIdx)
                            _cursor--;
                        else
                            _cursor = EndIdx - 1;
                        handled = true;
                        break;
                    case ConsoleKey.DownArrow:
                        if (_cursor < EndIdx - 1)
                            _cursor++;
                        else
                            _cursor = StartIdx;
                        handled = true;
                        break;
                    case ConsoleKey.Spacebar:
                        if (_selectedIndexes.Contains(_cursor))
                            _selectedIndexes.Remove(_cursor);
                        else
                            _selectedIndexes.Add(_cursor);
                        handled = true;
                        break;
                    case ConsoleKey.Enter:
                        Console.Clear();
                        var result = new List<T>();
                        foreach (var idx in _selectedIndexes)
                            result.Add(_items[idx]);
                        return result;
                    case ConsoleKey.Escape:
                        Console.Clear();
                        return new List<T>();
                }
            }
        }
    }

    private void DrawMenu()
    {
        int lines = PageSize + 7;
        ClearMenuArea(lines);
        var buffer = DrawHeader();
        for (int i = StartIdx; i < EndIdx; i++)
        {
            int displayIdx = i - StartIdx + 1;
            string prefix = _selectedIndexes.Contains(i) ? "[x] " : "[ ] ";
            string cursor = i == _cursor ? "> " : "  ";
            string line = cursor + prefix + $"{displayIdx}. " + _displaySelector(_items[i]);
            if (line.Length > _width - 4) line = line[..(_width - 4)];
            line = line.PadRight(_width - 4);
            if (i == _cursor)
                buffer.AppendLine($"║ \u001b[7m{line}\u001b[0m ║");
            else
                buffer.AppendLine($"║ {line} ║");
        }
        // Số 0 là Back
        string backPrefix = (_cursor < StartIdx || _cursor >= EndIdx) ? "> 0. " : "  0. ";
        string backLine = backPrefix + "Back";
        if (backLine.Length > _width - 4) backLine = backLine[..(_width - 4)];
        backLine = backLine.PadRight(_width - 4);
        if (_cursor < StartIdx || _cursor >= EndIdx)
            buffer.AppendLine($"║ \u001b[7m{backLine}\u001b[0m ║");
        else
            buffer.AppendLine($"║ {backLine} ║");
        DrawPageInfo(buffer);
        DrawFooter(buffer);
        buffer.AppendLine();
        buffer.AppendLine("Space: Toggle | 1-9: Select | 0: Back | ←/→: Page | ↑/↓: Move | Enter: Confirm | Esc: Cancel");
        Console.Write(buffer.ToString());
    }
}


