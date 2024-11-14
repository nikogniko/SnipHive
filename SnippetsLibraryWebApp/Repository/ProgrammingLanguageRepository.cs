using Dapper;
using Microsoft.Data.SqlClient;
using SnippetsLibraryWebApp.Models;
using SnippetsLibraryWebApp.Utils;

namespace SnippetsLibraryWebApp.Repository
{
    public class ProgrammingLanguageRepository
    {
        public IEnumerable<ProgrammingLanguageModel> GetAllProgrammingLanguages()
        {
            try
            {
                string connectionString = ConfigurationHelper.GetConnectionString();
                using (var connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT ID, Name FROM ProgrammingLanguage";
                    return connection.Query<ProgrammingLanguageModel>(query);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while getting programming languages: " + ex.Message);
                return null;
            }
        }

        public ProgrammingLanguageModel GetLanguageById(int languageId)
        {
            try
            {
                string connectionString = ConfigurationHelper.GetConnectionString();
                using (var connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT * FROM ProgrammingLanguage WHERE ID = @LanguageId";
                    return connection.QueryFirstOrDefault<ProgrammingLanguageModel>(query, new { LanguageId = languageId });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while getting programming language by id: " + ex.Message);
                return null;
            }
        }
    }
}
