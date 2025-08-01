using System;
using System.Collections.Generic;
using System.Text;

namespace Loda.Components;

    // Base class cho các component selection/pagination console
    public abstract class BaseSelectionComponent<T>
    {
        protected readonly List<T> _items;
        protected readonly Func<T, string> _displaySelector;
        protected int _pageIndex = 0;
        protected const int PageSize = 9;
        protected readonly string _title;
        protected readonly int _width;

        protected BaseSelectionComponent(List<T> items, Func<T, string> displaySelector, string title = "Select", int width = 64)
        {
            _items = items;
            _displaySelector = displaySelector;
            _title = title;
            _width = width;
        }

        protected int TotalPages => (_items.Count + PageSize - 1) / PageSize;
        protected int StartIdx => _pageIndex * PageSize;
        protected int EndIdx => Math.Min(StartIdx + PageSize, _items.Count);

        // Clear vùng menu (không clear toàn bộ màn hình)
        protected void ClearMenuArea(int lines)
        {
            int menuTop = 2;
            int menuLeft = 2;
            for (int i = 0; i < lines; i++)
            {
                int y = menuTop + i;
                if (y >= 0 && y < Console.BufferHeight)
                {
                    Console.SetCursorPosition(menuLeft, y);
                    Console.Write(new string(' ', _width));
                }
            }
            Console.SetCursorPosition(menuLeft, menuTop);
        }

        // Vẽ header, trả về StringBuilder đã có header
        protected StringBuilder DrawHeader()
        {
            var buffer = new StringBuilder();
            buffer.AppendLine($"╔{new string('═', _width - 2)}╗");
            int titlePad = (_width - 2 - _title.Length) / 2;
            string titleLine = new string(' ', titlePad) + _title + new string(' ', _width - 2 - _title.Length - titlePad);
            buffer.AppendLine($"║{titleLine}║");
            buffer.AppendLine($"╠{new string('═', _width - 2)}╣");
            return buffer;
        }

        // Vẽ footer, có thể override
        protected virtual void DrawFooter(StringBuilder buffer)
        {
            // Hướng dẫn phím, căn giữa
            string hint = "←/→:Page  ↑/↓:Move  1-9:Select  0:Back  Enter:OK  Esc:Cancel";
            int pad = Math.Max(0, (_width - 2 - hint.Length) / 2);
            string hintLine = new string(' ', pad) + hint + new string(' ', _width - 2 - hint.Length - pad);
            buffer.AppendLine($"║{hintLine}║");
            buffer.AppendLine($"╚{new string('═', _width - 2)}╝");
        }

        // Vẽ thông tin phân trang (chỉ hiển thị nếu có nhiều hơn 1 trang)
        protected void DrawPageInfo(StringBuilder buffer)
        {
            if (TotalPages > 1)
            {
                string pageInfo = $"Page {_pageIndex + 1}/{TotalPages} ";
                if (_pageIndex == 0) pageInfo += "[First] ";
                if (_pageIndex == TotalPages - 1) pageInfo += "[Last] ";
                pageInfo = pageInfo.PadRight(_width - 4);
                buffer.AppendLine($"║ {pageInfo} ║");
            }
        }
    }

