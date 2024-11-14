using Dapper;
using Microsoft.Data.SqlClient;
using SnippetsLibraryWebApp.Models;
using SnippetsLibraryWebApp.Utils;
using System.Text;

namespace SnippetsLibraryWebApp.Repository
{
    public class UserRepository
    {  
        public string? GetUsernameById(int userId)
        {
            try
            {
                string connectionString = ConfigurationHelper.GetConnectionString();
                using (var connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT Username FROM [User] WHERE ID = @UserId";
                    var username = connection.QueryFirstOrDefault<string>(query, new { UserId = userId });

                    return username;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error fetching username: " + ex.Message);
                return null;
            }
        }

        // Метод для реєстрації нового користувача з використанням Dapper
        public int? AddUser(string username, string email, string password)
        {
            try
            {
                string connectionString = ConfigurationHelper.GetConnectionString(); // Метод для отримання рядка підключення
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "INSERT INTO [User] (Username, Email, Password) VALUES (@Username, @Email, @PasswordHash)";
                    var parameters = new { Username = username, Email = email, PasswordHash = HashHelper.HashPassword(password) };
                    int rowsAffected = connection.Execute(query, parameters);

                    if (rowsAffected > 0)
                    {
                        query = "SELECT ID FROM [User] WHERE Email = @Email";
                        int? userID = connection.QueryFirstOrDefault<UserModel>(query, new { Email = email }).Id;

                        if (userID != null)
                        {
                            return userID;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error registering user: " + ex.Message);
                return null;
            }
            return null;
        }

        // метод для авторизації (з Dapper)
        public int? GetUserId(string email, string password)
        {
            try
            {
                string connectionString = ConfigurationHelper.GetConnectionString();
                using (var connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT ID, Username, Password AS PasswordHash FROM [User] WHERE Email = @Email";
                    var user = connection.QueryFirstOrDefault<UserModel>(query, new { Email = email });

                    if (user != null && user.PasswordHash == HashHelper.HashPassword(password))
                    {
                        return user.Id;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error during login: " + ex.Message);
                return null;
            }
            // Якщо авторизація неуспішна
            return null;
        }

        // TODO: GetUserById(int userId) якщо буде вікно профіля користувача

        // TODO: EditUser(int userId) якщо буде профілm користувача має редагуватися

    }
}
