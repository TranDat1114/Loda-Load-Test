using System.Text;
using Loda.Components;

namespace Loda;

public class MenuItem
{
    public string Title { get; set; }
    public Action? OnSelect { get; set; }
    public MenuUI? SubMenu { get; set; }

    public MenuItem(string title, Action? onSelect = null, MenuUI? subMenu = null)
    {
        Title = title;
        if (onSelect != null) OnSelect = onSelect;
        if (subMenu != null) SubMenu = subMenu;
    }
}

// Kế thừa BaseSelectionComponent để đồng bộ UI selection/pagination
public class MenuUI : BaseSelectionComponent<MenuItem>
{
    private int _selectedIndex;
    private readonly int _top, _left;
    public bool IsActive { get; private set; } = true;

    public MenuUI(string title, List<MenuItem> items, int left = 0, int top = 0, int width = 64, bool isSubMenu = false)
        : base(isSubMenu ? new List<MenuItem>(items) { new MenuItem(Share.Localization.T("Back"), () => { }) } : items, x => x.Title, title, width)
    {
        _selectedIndex = 0;
        _pageIndex = 0;
        _left = left;
        _top = top;
        if (isSubMenu)
        {
            // Đảm bảo Back luôn là mục cuối cùng, OnSelect sẽ set IsActive=false
            _items[^1].OnSelect = () => { IsActive = false; };
        }
    }

    public static MenuUI CreateSubMenu(string title, List<MenuItem> items, int left = 0, int top = 0, int width = 64)
        => new MenuUI(title, items, left, top, width, true);

    public void DrawMenu()
    {
        var buffer = DrawHeader();
        int totalItems = _items.Count - 1;
        int startIdx = StartIdx;
        int endIdx = EndIdx > totalItems ? totalItems : EndIdx;
        for (int i = startIdx; i < endIdx; i++)
        {
            int displayIdx = i - startIdx + 1;
            string prefix = (_selectedIndex == i ? "> " : "  ") + $"{displayIdx}. ";
            string line = prefix + _items[i].Title;
            if (line.Length > _width - 4) line = line[..(_width - 4)];
            line = line.PadRight(_width - 4);
            if (_selectedIndex == i)
                buffer.AppendLine($"║ \u001b[7m{line}\u001b[0m ║");
            else
                buffer.AppendLine($"║ {line} ║");
        }
        // Back/Exit cuối cùng, số 0
        string backPrefix = _selectedIndex == _items.Count - 1 ? "> 0. " : "  0. ";
        string backLine = backPrefix + _items[^1].Title;
        if (backLine.Length > _width - 4) backLine = backLine[..(_width - 4)];
        backLine = backLine.PadRight(_width - 4);
        if (_selectedIndex == _items.Count - 1)
            buffer.AppendLine($"║ \u001b[7m{backLine}\u001b[0m ║");
        else
            buffer.AppendLine($"║ {backLine} ║");
        DrawPageInfo(buffer);
        DrawFooter(buffer);
        Console.SetCursorPosition(_left, _top);
        Console.Write(buffer.ToString());
    }

    public void HandleInput(ConsoleKeyInfo key)
    {
        int totalItems = _items.Count - 1;
        int startIdx = StartIdx;
        int endIdx = EndIdx > totalItems ? totalItems : EndIdx;
        DrawMenu();
        var keyPressed = key; 
        if (char.IsDigit(keyPressed.KeyChar))
        {
            int num = keyPressed.KeyChar - '0';
            if (num == 0)
            {
                _selectedIndex = _items.Count - 1;
            }
            else if (num >= 1 && num <= PageSize)
            {
                int idx = startIdx + num - 1;
                if (idx < endIdx)
                {
                    _selectedIndex = idx;
                }
            }
        }
        else
        {
            switch (keyPressed.Key)
            {
                case ConsoleKey.LeftArrow:
                    if (_pageIndex > 0)
                    {
                        _pageIndex--;
                        _selectedIndex = _pageIndex * PageSize;
                    }
                    break;
                case ConsoleKey.RightArrow:
                    if (_pageIndex < TotalPages - 1)
                    {
                        _pageIndex++;
                        _selectedIndex = _pageIndex * PageSize;
                    }
                    break;
                case ConsoleKey.UpArrow:
                    if (_selectedIndex == _items.Count - 1)
                        _selectedIndex = endIdx - 1;
                    else if (_selectedIndex > startIdx)
                        _selectedIndex--;
                    break;
                case ConsoleKey.DownArrow:
                    if (_selectedIndex == endIdx - 1)
                        _selectedIndex = _items.Count - 1;
                    else if (_selectedIndex < endIdx - 1)
                        _selectedIndex++;
                    break;
                case ConsoleKey.Enter:
                    var item = _items[_selectedIndex];
                    if (_selectedIndex == _items.Count - 1)
                    {
                        item.OnSelect?.Invoke();
                        // Chỉ set IsActive = false nếu là Exit
                        if (item.Title == Share.Localization.T("Exit"))
                            IsActive = false;
                    }
                    else if (item.SubMenu != null)
                    {
                        Console.Clear();
                        item.SubMenu.Reset();
                        while (item.SubMenu.IsActive)
                        {
                            item.SubMenu.DrawMenu();
                            if (Console.KeyAvailable)
                            {
                                var subKey = Console.ReadKey(true);
                                item.SubMenu.HandleInput(subKey);
                            }
                            System.Threading.Thread.Sleep(10);
                        }
                        Console.Clear();
                        DrawMenu();
                    }
                    else
                    {
                        item.OnSelect?.Invoke();
                        // Chỉ set IsActive = false nếu là Exit
                        if (item.Title == Share.Localization.T("Exit"))
                            IsActive = false;
                    }
                    break;
                case ConsoleKey.Escape:
                    IsActive = false;
                    break;
            }
        }
    }

    public void Reset()
    {
        _selectedIndex = 0;
        _pageIndex = 0;
        IsActive = true;
    }
}

