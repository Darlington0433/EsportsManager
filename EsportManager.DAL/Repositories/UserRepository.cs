using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using EsportManager.Models;
using EsportManager.DAL.Interfaces;
using EsportManager.Utils;

namespace EsportManager.DAL
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;

        public UserRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public User GetByUsername(string username)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM Users WHERE Username = @Username";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new User
                            {
                                UserID = reader.GetInt32(reader.GetOrdinal("UserID")),
                                Username = reader.GetString(reader.GetOrdinal("Username")),
                                DisplayName = reader.GetString(reader.GetOrdinal("DisplayName")),
                                Role = reader.GetString(reader.GetOrdinal("Role")),
                                Email = reader.GetString(reader.GetOrdinal("Email")),
                                Phone = reader.GetString(reader.GetOrdinal("Phone")),
                                Password = reader.GetString(reader.GetOrdinal("Password")),
                                SecurityQuestion = reader.GetString(reader.GetOrdinal("SecurityQuestion")),
                                SecurityAnswer = reader.GetString(reader.GetOrdinal("SecurityAnswer")),
                                Status = reader.GetString(reader.GetOrdinal("Status"))
                            };
                        }
                    }
                }
            }
            return null;
        }

        public User GetByID(int userID)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM Users WHERE UserID = @UserID";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", userID);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new User
                            {
                                UserID = reader.GetInt32(reader.GetOrdinal("UserID")),
                                Username = reader.GetString(reader.GetOrdinal("Username")),
                                DisplayName = reader.GetString(reader.GetOrdinal("DisplayName")),
                                Role = reader.GetString(reader.GetOrdinal("Role")),
                                Email = reader.GetString(reader.GetOrdinal("Email")),
                                Phone = reader.GetString(reader.GetOrdinal("Phone")),
                                Password = reader.GetString(reader.GetOrdinal("Password")),
                                SecurityQuestion = reader.GetString(reader.GetOrdinal("SecurityQuestion")),
                                SecurityAnswer = reader.GetString(reader.GetOrdinal("SecurityAnswer")),
                                Status = reader.GetString(reader.GetOrdinal("Status"))
                            };
                        }
                    }
                }
            }
            return null;
        }

        public User GetByEmail(string email)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM Users WHERE Email = @Email";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Email", email);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new User
                            {
                                UserID = reader.GetInt32(reader.GetOrdinal("UserID")),
                                Username = reader.GetString(reader.GetOrdinal("Username")),
                                DisplayName = reader.GetString(reader.GetOrdinal("DisplayName")),
                                Role = reader.GetString(reader.GetOrdinal("Role")),
                                Email = reader.GetString(reader.GetOrdinal("Email")),
                                Phone = reader.GetString(reader.GetOrdinal("Phone")),
                                Password = reader.GetString(reader.GetOrdinal("Password")),
                                SecurityQuestion = reader.GetString(reader.GetOrdinal("SecurityQuestion")),
                                SecurityAnswer = reader.GetString(reader.GetOrdinal("SecurityAnswer")),
                                Status = reader.GetString(reader.GetOrdinal("Status"))
                            };
                        }
                    }
                }
            }
            return null;
        }

        public List<User> GetPendingUsers()
        {
            List<User> users = new List<User>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM Users WHERE Status = 'Pending'";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            users.Add(new User
                            {
                                UserID = reader.GetInt32(reader.GetOrdinal("UserID")),
                                Username = reader.GetString(reader.GetOrdinal("Username")),
                                DisplayName = reader.GetString(reader.GetOrdinal("DisplayName")),
                                Role = reader.GetString(reader.GetOrdinal("Role")),
                                Email = reader.GetString(reader.GetOrdinal("Email")),
                                Phone = reader.GetString(reader.GetOrdinal("Phone")),
                                Password = reader.GetString(reader.GetOrdinal("Password")),
                                SecurityQuestion = reader.GetString(reader.GetOrdinal("SecurityQuestion")),
                                SecurityAnswer = reader.GetString(reader.GetOrdinal("SecurityAnswer")),
                                Status = reader.GetString(reader.GetOrdinal("Status"))
                            });
                        }
                    }
                }
            }
            return users;
        }

        public List<User> GetAllUsers()
        {
            List<User> users = new List<User>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM Users";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            users.Add(new User
                            {
                                UserID = reader.GetInt32(reader.GetOrdinal("UserID")),
                                Username = reader.GetString(reader.GetOrdinal("Username")),
                                DisplayName = reader.GetString(reader.GetOrdinal("DisplayName")),
                                Role = reader.GetString(reader.GetOrdinal("Role")),
                                Email = reader.GetString(reader.GetOrdinal("Email")),
                                Phone = reader.GetString(reader.GetOrdinal("Phone")),
                                Password = reader.GetString(reader.GetOrdinal("Password")),
                                SecurityQuestion = reader.GetString(reader.GetOrdinal("SecurityQuestion")),
                                SecurityAnswer = reader.GetString(reader.GetOrdinal("SecurityAnswer")),
                                Status = reader.GetString(reader.GetOrdinal("Status"))
                            });
                        }
                    }
                }
            }
            return users;
        }

        public bool Add(User user)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = @"INSERT INTO Users (Username, DisplayName, Role, Email, Phone, Password, SecurityQuestion, SecurityAnswer, Status)
                                VALUES (@Username, @DisplayName, @Role, @Email, @Phone, @Password, @SecurityQuestion, @SecurityAnswer, @Status)";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", user.Username);
                    command.Parameters.AddWithValue("@DisplayName", user.DisplayName);
                    command.Parameters.AddWithValue("@Role", user.Role);
                    command.Parameters.AddWithValue("@Email", user.Email);
                    command.Parameters.AddWithValue("@Phone", user.Phone);
                    command.Parameters.AddWithValue("@Password", user.Password);
                    command.Parameters.AddWithValue("@SecurityQuestion", user.SecurityQuestion);
                    command.Parameters.AddWithValue("@SecurityAnswer", user.SecurityAnswer);
                    command.Parameters.AddWithValue("@Status", user.Status);
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool Update(User user)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = @"UPDATE Users SET Username = @Username, DisplayName = @DisplayName, Role = @Role, 
                                Email = @Email, Phone = @Phone, SecurityQuestion = @SecurityQuestion, 
                                SecurityAnswer = @SecurityAnswer, Status = @Status 
                                WHERE UserID = @UserID";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", user.UserID);
                    command.Parameters.AddWithValue("@Username", user.Username);
                    command.Parameters.AddWithValue("@DisplayName", user.DisplayName);
                    command.Parameters.AddWithValue("@Role", user.Role);
                    command.Parameters.AddWithValue("@Email", user.Email);
                    command.Parameters.AddWithValue("@Phone", user.Phone);
                    command.Parameters.AddWithValue("@SecurityQuestion", user.SecurityQuestion);
                    command.Parameters.AddWithValue("@SecurityAnswer", user.SecurityAnswer);
                    command.Parameters.AddWithValue("@Status", user.Status);
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool Delete(int userID)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "DELETE FROM Users WHERE UserID = @UserID";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", userID);
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool ChangePassword(int userID, string newPassword)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "UPDATE Users SET Password = @Password WHERE UserID = @UserID";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", userID);
                    command.Parameters.AddWithValue("@Password", SecurityHelper.HashPassword(newPassword));
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool ChangeRole(int userID, string newRole)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "UPDATE Users SET Role = @Role WHERE UserID = @UserID";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", userID);
                    command.Parameters.AddWithValue("@Role", newRole);
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool ChangeStatus(int userID, string newStatus)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "UPDATE Users SET Status = @Status WHERE UserID = @UserID";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", userID);
                    command.Parameters.AddWithValue("@Status", newStatus);
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool IsUsernameExists(string username)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT COUNT(*) FROM Users WHERE Username = @Username";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    return (int)command.ExecuteScalar() > 0;
                }
            }
        }

        public bool IsEmailExists(string email)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT COUNT(*) FROM Users WHERE Email = @Email";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Email", email);
                    return (int)command.ExecuteScalar() > 0;
                }
            }
        }

        public string GetSecurityQuestion(string username)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT SecurityQuestion FROM Users WHERE Username = @Username";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    object result = command.ExecuteScalar();
                    return result?.ToString();
                }
            }
        }

        public bool VerifySecurityAnswer(string username, string answer)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT SecurityAnswer FROM Users WHERE Username = @Username";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    object result = command.ExecuteScalar();
                    if (result != null)
                    {
                        string storedAnswer = result.ToString();
                        return SecurityHelper.HashString(answer) == storedAnswer;
                    }
                    return false;
                }
            }
        }
    }
}