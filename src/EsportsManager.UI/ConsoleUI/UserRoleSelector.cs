// Test role hiện tại của user để demo chức năng menu

using System;
using EsportsManager.UI.ConsoleUI.Utilities;

namespace EsportsManager.UI.ConsoleUI;

public class UserRoleSelector
{
    public static string SelectUserRole()
    {
        var roleOptions = new[]
        {
            "Player - Người chơi",
            "Admin - Quản trị viên", 
            "Viewer - Người xem"
        };

        int selection = InteractiveMenuService.DisplayInteractiveMenu("CHỌN VAI TRÒ ĐỂ DEMO", roleOptions);
        
        return selection switch
        {
            0 => "Player",
            1 => "Admin",
            2 => "Viewer",
            _ => "Viewer"
        };
    }
}
