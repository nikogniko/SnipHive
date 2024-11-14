using SnippetsLibraryWebApp.Models;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using SnippetsLibraryWebApp.Utils;

namespace SnippetsLibraryWebApp.Repository
{
    public class SnippetRepository
    {
        public async Task<IEnumerable<UserModel>> GetAllUsersAsync()
        {
            string connectionString = ConfigurationHelper.GetConnectionString();
            using (var connection = new SqlConnection(connectionString))
            {
                var query = "SELECT ID, Username, Email FROM [User]";
                return await connection.QueryAsync<UserModel>(query);
            }
        }

        public async Task<UserModel> GetUserByEmailAsync(string email)
        {
            string connectionString = ConfigurationHelper.GetConnectionString();
            using (var connection = new SqlConnection(connectionString))
            {
                var query = "SELECT ID, Username, Email FROM [User] WHERE Email = @Email";
                return await connection.QueryFirstOrDefaultAsync<UserModel>(query, new { Email = email });
            }
        }
    }
}

