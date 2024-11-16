using Dapper;
using Microsoft.Data.SqlClient;
using SnippetsLibraryWebApp.Models;
using SnippetsLibraryWebApp.Utils;

namespace SnippetsLibraryWebApp.Repository
{
    public class TagRepository
    {
        public IEnumerable<TagModel> GetAllTags()
        {
            try
            {
                string connectionString = ConfigurationHelper.GetConnectionString();
                using (var connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT ID, Name FROM Tag";
                    var result = connection.Query<TagModel>(query);
                    return result;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while getting tags: " + ex.Message);
                return null;
            }
        }

        public IEnumerable<TagModel> GetTagsBySnippetId(int snippetId)
        {
            try
            {
                string connectionString = ConfigurationHelper.GetConnectionString();
                using (var connection = new SqlConnection(connectionString))
                {
                    string query = @"
                        SELECT t.ID, t.Name
                        FROM Tag t
                        INNER JOIN SnippetTag st ON t.ID = st.TagID
                        WHERE st.SnippetID = @SnippetId";

                    return connection.Query<TagModel>(query, new { SnippetId = snippetId });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while getting tags: " + ex.Message);
                return null;
            }
        }

        public bool AddTagForSnippet(int snippetId, string tagName)
        {
            try
            {
                string connectionString = ConfigurationHelper.GetConnectionString();
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        // Перевірка, чи існує вже тег з таким ім'ям
                        string checkTagQuery = "SELECT ID FROM Tag WHERE Name = @Name";
                        int? tagId = connection.QueryFirstOrDefault<int>(checkTagQuery, new { Name = tagName }, transaction);

                        // Якщо тег не існує, додаємо його
                        if (tagId == 0)
                        {
                            string insertTagQuery = "INSERT INTO Tag (Name) VALUES (@Name); SELECT CAST(SCOPE_IDENTITY() as int)";
                            tagId = connection.QuerySingle<int>(insertTagQuery, new { Name = tagName }, transaction);
                        }

                        // Додавання зв’язку між тегом та сніпетом у таблицю SnippetTag
                        string insertSnippetTagQuery = "INSERT INTO SnippetTag (SnippetID, TagID) VALUES (@SnippetID, @TagID)";
                        connection.Execute(insertSnippetTagQuery, new { SnippetID = snippetId, TagID = tagId }, transaction);

                        // Завершення транзакції
                        transaction.Commit();
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while adding tag: " + ex.Message);
                return false;
            }
        }


    }
}
