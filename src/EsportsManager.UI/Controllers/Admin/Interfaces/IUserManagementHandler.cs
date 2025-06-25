using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EsportsManager.BL.DTOs;

namespace EsportsManager.UI.Controllers.Admin.Interfaces
{
    /// <summary>
    /// Interface for User Management operations
    /// Follows Interface Segregation Principle
    /// </summary>
    public interface IUserManagementHandler
    {
        Task HandleUserListAsync();
        Task HandleUserSearchAsync();
        Task HandleUserStatusToggleAsync();
        Task HandleUserDetailViewAsync();
        Task HandleUserDeleteAsync();
    }
}
