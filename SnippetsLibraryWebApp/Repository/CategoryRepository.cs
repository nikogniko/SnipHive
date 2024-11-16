using Dapper;
using Microsoft.Data.SqlClient;
using SnippetsLibraryWebApp.Models;
using SnippetsLibraryWebApp.Utils;

namespace SnippetsLibraryWebApp.Repository
{
    public class CategoryRepository
    {
        public IEnumerable<CategoryModel> GetAllCategories()
        {
            try
            {
                string connectionString = ConfigurationHelper.GetConnectionString();
                using (var connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT ID, Name FROM Category";
                    return connection.Query<CategoryModel>(query);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while getting categories: " + ex.Message);
                return null;
            }
        }

        public CategoryModel GetCategoryById(int categoryId)
        {
            try
            {
                string connectionString = ConfigurationHelper.GetConnectionString();
                using (var connection = new SqlConnection(connectionString))
                {
                    string query = @"
                        SELECT *
                        FROM Category c
                        WHERE sc.ID = @cId";

                    return connection.Query<CategoryModel>(query, new { cId = categoryId }).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while getting categories: " + ex.Message);
                return null;
            }
        }

        public IEnumerable<CategoryModel> GetCategoriesBySnippetId(int snippetId)
        {
            try
            {
                string connectionString = ConfigurationHelper.GetConnectionString();
                using (var connection = new SqlConnection(connectionString))
                {
                    string query = @"
                        SELECT c.ID, c.Name
                        FROM Category c
                        INNER JOIN SnippetCategory sc ON c.ID = sc.CategoryID
                        WHERE sc.SnippetID = @SnippetId";

                    return connection.Query<CategoryModel>(query, new { SnippetId = snippetId });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while getting categories: " + ex.Message);
                return null;
            }
        }
    }
}
