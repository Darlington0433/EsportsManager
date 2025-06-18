using System;

namespace EsportsManager.UI.Legacy.Menus
{
    public abstract class BaseMenu
    {
        protected readonly string MenuTitle;
        protected readonly string[] MenuOptions;

        protected BaseMenu(string menuTitle, string[] menuOptions)
        {
            MenuTitle = menuTitle;
            MenuOptions = menuOptions;
        }

        public virtual void Show()
        {
            while (true)
            {
                int selected = EsportsManager.UI.Legacy.LegacyUIHelper.ShowMenu(MenuTitle, MenuOptions);
                
                if (selected == -1 || selected == MenuOptions.Length - 1) // Escape hoặc Đăng xuất
                    return;
                    
                HandleMenuSelection(selected);
            }
        }

        protected abstract void HandleMenuSelection(int selected);
    }
}
