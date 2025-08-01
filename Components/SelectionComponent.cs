using System;
using System.Collections.Generic;
using System.Text;

namespace Loda.Components;

    // Component chọn một mục từ danh sách (single selection, generic)
    public class SelectionComponent<T> : BaseSelectionComponent<T>
    {
        private int _cursor = 0;
        private bool _onBack = false;

        public SelectionComponent(List<T> items, Func<T, string> displaySelector, string title = "Select", int width = 64)
            : base(items, displaySelector, title, width) { }

        // Trả về index đã chọn, hoặc -1 nếu Esc
        public int Show()
        {
            ConsoleKeyInfo key;
            _onBack = false;
            do
            {
                Draw();
                key = Console.ReadKey(true);
                bool handled = false;
                if (char.IsDigit(key.KeyChar))
                {
                    int num = key.KeyChar - '0';
                    if (num == 0)
                    {
                        _onBack = true;
                        handled = true;
                    }
                    else if (num >= 1 && num <= PageSize)
                    {
                        int idx = StartIdx + num - 1;
                        if (idx < EndIdx)
                        {
                            _cursor = idx;
                            _onBack = false;
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
                                _cursor = _pageIndex * PageSize;
                                _onBack = false;
                            }
                            break;
                        case ConsoleKey.RightArrow:
                            if (_pageIndex < TotalPages - 1)
                            {
                                _pageIndex++;
                                _cursor = _pageIndex * PageSize;
                                _onBack = false;
                            }
                            break;
                        case ConsoleKey.UpArrow:
                            if (_onBack)
                            {
                                _cursor = Math.Min(_items.Count - 1, _pageIndex * PageSize + PageSize - 1);
                                if (_cursor >= _items.Count) _cursor = _items.Count - 1;
                                _onBack = false;
                            }
                            else if (_cursor > _pageIndex * PageSize)
                                _cursor--;
                            else
                                _onBack = true;
                            break;
                        case ConsoleKey.DownArrow:
                            if (_onBack)
                            {
                                _cursor = _pageIndex * PageSize;
                                _onBack = false;
                            }
                            else if (_cursor < Math.Min(_items.Count, (_pageIndex + 1) * PageSize) - 1)
                                _cursor++;
                            else
                                _onBack = true;
                            break;
                    }
                }
            } while (key.Key != ConsoleKey.Enter && key.Key != ConsoleKey.Escape);

            Console.Clear();
            if (key.Key == ConsoleKey.Enter)
                return _onBack ? -1 : _cursor;
            return -1;
        }

        private void Draw()
        {
            int lines = PageSize + 6;
            ClearMenuArea(lines);
            var buffer = DrawHeader();
            // Hiển thị các mục của trang hiện tại, đánh số 1-9
            for (int i = StartIdx; i < EndIdx; i++)
            {
                int displayIdx = i - StartIdx + 1;
                string prefix = (!_onBack && i == _cursor ? "> " : "  ") + $"{displayIdx}. ";
                string line = prefix + _displaySelector(_items[i]);
                if (line.Length > _width - 4) line = line.Substring(0, _width - 4);
                line = line.PadRight(_width - 4);
                if (!_onBack && i == _cursor)
                    buffer.AppendLine($"║ \u001b[7m{line}\u001b[0m ║");
                else
                    buffer.AppendLine($"║ {line} ║");
            }
            // Số 0 là Back
            string backPrefix = _onBack ? "> 0. " : "  0. ";
            string backLine = backPrefix + "Back";
            if (backLine.Length > _width - 4) backLine = backLine.Substring(0, _width - 4);
            backLine = backLine.PadRight(_width - 4);
            if (_onBack)
                buffer.AppendLine($"║ \u001b[7m{backLine}\u001b[0m ║");
            else
                buffer.AppendLine($"║ {backLine} ║");
            // Thông tin trang
            DrawPageInfo(buffer);
            DrawFooter(buffer);
            buffer.AppendLine();
            buffer.AppendLine("1-9: Select | 0: Back | ←/→: Page | ↑/↓: Move | Enter: Select | Esc: Cancel");
            Console.Write(buffer.ToString());
        }
    }
