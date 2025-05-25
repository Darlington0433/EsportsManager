using System;
using System.Collections.Generic;
using EsportManager.Models;
using EsportManager.DAL.Interfaces;
using EsportManager.BLL.Interfaces;
using EsportManager.Utils;
using System.Linq;

namespace EsportManager.BLL
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public User Login(string username, string password)
        {
            Console.WriteLine($"Đang đăng nhập với username: {username}");

            User fakeUser = null;

            if (username != null)
            {
                if (username.StartsWith("@admin", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Đăng nhập với vai trò Admin");
                    fakeUser = new User
                    {
                        UserID = 1,
                        Username = "@admin",
                        DisplayName = "Quản trị viên",
                        Role = "Admin",
                        Phone = "0987654321",
                        Email = "admin@example.com",
                        Password = password,
                        SecurityQuestion = "Tên trường học đầu tiên?",
                        SecurityAnswer = "truonghoc",
                        Status = "Approved"
                    };
                }
                else if (username.StartsWith("@player", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Đăng nhập với vai trò Player");
                    fakeUser = new User
                    {
                        UserID = 2,
                        Username = "@player",
                        DisplayName = "Người chơi",
                        Role = "Player",
                        Phone = "0987654322",
                        Email = "player@example.com",
                        Password = password,
                        SecurityQuestion = "Tên trường học đầu tiên?",
                        SecurityAnswer = "truonghoc",
                        Status = "Approved"
                    };
                }
                else if (username.StartsWith("@viewer", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Đăng nhập với vai trò Viewer");
                    fakeUser = new User
                    {
                        UserID = 3,
                        Username = "@viewer",
                        DisplayName = "Người xem",
                        Role = "Viewer",
                        Phone = "0987654323",
                        Email = "viewer@example.com",
                        Password = password,
                        SecurityQuestion = "Tên trường học đầu tiên?",
                        SecurityAnswer = "truonghoc",
                        Status = "Approved"
                    };
                }
                else
                {
                    Console.WriteLine("Không nhận diện được vai trò từ username, đăng nhập mặc định với vai trò Admin");
                    fakeUser = new User
                    {
                        UserID = 1,
                        Username = "@admin",
                        DisplayName = "Quản trị viên Mặc Định",
                        Role = "Admin",
                        Phone = "0987654321",
                        Email = "admin@example.com",
                        Password = password,
                        SecurityQuestion = "Tên trường học đầu tiên?",
                        SecurityAnswer = "truonghoc",
                        Status = "Approved"
                    };
                }
            }
            else
            {
                Console.WriteLine("Username null, đăng nhập mặc định với vai trò Admin");
                fakeUser = new User
                {
                    UserID = 1,
                    Username = "@admin",
                    DisplayName = "Quản trị viên Tự động",
                    Role = "Admin",
                    Phone = "0987654321",
                    Email = "admin@example.com",
                    Password = "admin123",
                    SecurityQuestion = "Tên trường học đầu tiên?",
                    SecurityAnswer = "truonghoc",
                    Status = "Approved"
                };
            }

            return fakeUser;
        }

        public bool Register(User user)
        {
            if (_userRepository.IsUsernameExists(user.Username))
            {
                return false;
            }

            if (_userRepository.IsEmailExists(user.Email))
            {
                return false;
            }

            user.Password = SecurityHelper.HashPassword(user.Password);
            user.SecurityAnswer = SecurityHelper.HashString(user.SecurityAnswer);
            user.Status = "Pending";

            return _userRepository.Add(user);
        }

        public bool ApproveUser(int userID)
        {
            return _userRepository.ChangeStatus(userID, "Approved");
        }

        public bool RejectUser(int userID)
        {
            return _userRepository.ChangeStatus(userID, "Rejected");
        }

        public bool DeleteUser(int userID)
        {
            return _userRepository.Delete(userID);
        }

        public bool DeleteUserByUsername(string username)
        {
            User user = _userRepository.GetByUsername(username);
            if (user != null)
            {
                return _userRepository.Delete(user.UserID);
            }
            return false;
        }

        public bool DeleteUserByEmail(string email)
        {
            User user = _userRepository.GetByEmail(email);
            if (user != null)
            {
                return _userRepository.Delete(user.UserID);
            }
            return false;
        }

        public bool ChangePassword(int userID, string oldPassword, string newPassword)
        {
            User user = _userRepository.GetByID(userID);
            if (user != null && SecurityHelper.VerifyPassword(oldPassword, user.Password))
            {
                return _userRepository.ChangePassword(userID, newPassword);
            }
            return false;
        }

        public bool ResetPassword(string username, string securityAnswer)
        {
            if (_userRepository.VerifySecurityAnswer(username, securityAnswer))
            {
                User user = _userRepository.GetByUsername(username);
                if (user != null)
                {
                    return _userRepository.ChangePassword(user.UserID, "player123");
                }
            }
            return false;
        }

        public string GetSecurityQuestion(string username)
        {
            return _userRepository.GetSecurityQuestion(username);
        }

        public bool RequestRoleChange(int userID, string newRole)
        {
            // TODO: Lưu yêu cầu vào bảng RoleChangeRequests
            return true;
        }

        public bool ApproveRoleChange(int requestID)
        {
            // TODO: Cập nhật trạng thái yêu cầu và cập nhật vai trò người dùng
            return true;
        }

        public bool RejectRoleChange(int requestID)
        {
            // TODO: Cập nhật trạng thái yêu cầu thành Rejected
            return true;
        }

        public List<User> GetPendingUsers()
        {
            return _userRepository.GetPendingUsers();
        }

        public List<User> GetAllUsers()
        {
            return _userRepository.GetAllUsers();
        }

        public bool UpdateProfile(User user)
        {
            return _userRepository.Update(user);
        }
    }
}