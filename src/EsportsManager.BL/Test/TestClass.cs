using EsportsManager.BL.DTOs;

namespace EsportsManager.BL.Test;

public class TestClass
{
    public void TestMethod()
    {
        var login = new LoginDto();
        var register = new RegisterDto(); 
        var create = new CreateUserDto();
        var update = new UpdatePasswordDto();
        var reset = new ResetPasswordDto();
    }
}
