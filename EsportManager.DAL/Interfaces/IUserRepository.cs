using System;
using System.Collections.Generic;
using EsportManager.Models;

namespace EsportManager.DAL.Interfaces
{
    public interface IUserRepository
    {
        User GetByUsername(string username);
        User GetByID(int userID);
        User GetByEmail(string email);
        List<User> GetPendingUsers();
        List<User> GetAllUsers();
        bool Add(User user);
        bool Update(User user);
        bool Delete(int userID);
        bool ChangePassword(int userID, string newPassword);
        bool ChangeRole(int userID, string newRole);
        bool ChangeStatus(int userID, string newStatus);
        bool IsUsernameExists(string username);
        bool IsEmailExists(string email);
        string GetSecurityQuestion(string username);
        bool VerifySecurityAnswer(string username, string answer);
    }
}