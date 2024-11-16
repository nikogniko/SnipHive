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
        private readonly CategoryRepository _categoryRepository;
        private readonly TagRepository _tagRepository;

        public SnippetRepository(CategoryRepository categoryRepository, TagRepository tagRepository)
        {
            _categoryRepository = categoryRepository;
            _tagRepository = tagRepository;
        }

        private async Task<bool> IsUserAuthorOfSnippetAsync(int snippetId, int userId)
        {
            string connectionString = ConfigurationHelper.GetConnectionString();
            using (var connection = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT COUNT(1) 
                    FROM Snippet
                    WHERE ID = @SnippetID AND AuthorID = @UserID;
                ";

                var isAuthor = await connection.ExecuteScalarAsync<bool>(query, new { SnippetID = snippetId, UserID = userId });
                return isAuthor;
            }
        }

        /*public async Task<IEnumerable<UserModel>> GetAllUsersAsync()
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
        }*/

        public async Task<IEnumerable<SnippetModel>> GetAllSnippetsAsync()
        {
            string connectionString = ConfigurationHelper.GetConnectionString();
            using (var connection = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT Snippet.ID, Title, Description, ProgrammingLanguageID, AuthorID, CreatedAt, UpdatedAt, Code, Status, 
                    COUNT(SavedSnippets.UserID) AS SavesCount 
                    FROM Snippet
                    INNER JOIN SavedSnippets ON SavedSnippets.SnippetID = Snippet.ID
                    GROUP BY Snippet.ID, Title, Description, ProgrammingLanguageID, AuthorID, CreatedAt, UpdatedAt, Code, Status;
                ";

                var snippets = await connection.QueryAsync<SnippetModel>(query);

                // Завантаження категорій і тегів для кожного сніпета
                foreach (var snippet in snippets)
                {
                    /*snippet.Categories = await GetCategoriesBySnippetIdAsync(snippet.ID);
                    snippet.Tags = await GetTagsBySnippetIdAsync(snippet.ID);*/
                    snippet.Categories = await _categoryRepository.GetCategoriesBySnippetIdAsync(snippet.ID);
                    snippet.Tags = await _tagRepository.GetTagsBySnippetIdAsync(snippet.ID);
                }
                return snippets;
            }
        }

        public async Task<SnippetModel> GetSnippetByIdAsync(int snippetId)
        {
            string connectionString = ConfigurationHelper.GetConnectionString();
            using (var connection = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT Snippet.ID, Title, Description, ProgrammingLanguageID, AuthorID, CreatedAt, UpdatedAt, Code, Status, 
                    COUNT(SavedSnippets.UserID) AS SavesCount
                    FROM Snippet
                    INNER JOIN SavedSnippets ON SavedSnippets.SnippetID = Snippet.ID
                    WHERE SnippetID = @CurrentSnippetID
                    GROUP BY Snippet.ID, Title, Description, ProgrammingLanguageID, AuthorID, CreatedAt, UpdatedAt, Code, Status;
                ";

                var snippet = await connection.QuerySingleOrDefaultAsync<SnippetModel>(query, new { SnippetID = snippetId });

                if (snippet != null)
                {
                    // Завантаження категорій і тегів для сніпета
                    snippet.Categories = await _categoryRepository.GetCategoriesBySnippetIdAsync(snippet.ID);
                    snippet.Tags = await _tagRepository.GetTagsBySnippetIdAsync(snippet.ID);
                }

                return snippet;
            }
        }

        public async Task<bool> DeleteSnippetAsync(int snippetId, int userId)
        {
            // Перевіряємо, чи є користувач автором сніпета
            var isAuthor = await IsUserAuthorOfSnippetAsync(snippetId, userId);
            if (!isAuthor)
            {
                return false; // Якщо користувач не є автором, видалення неможливе
            }

            string connectionString = ConfigurationHelper.GetConnectionString();
            using (var connection = new SqlConnection(connectionString))
            {
                string query = @"
                    DELETE FROM Snippet
                    WHERE ID = @SnippetID;
                ";

                var rowsAffected = await connection.ExecuteAsync(query, new { SnippetID = snippetId });
                return rowsAffected > 0;
            }
        }

        /*public async Task<bool> UpdateSnippetAsync(SnippetModel snippet, int userId)
        {
            // Перевіряємо, чи є користувач автором сніпета
            var isAuthor = await IsUserAuthorOfSnippetAsync(snippet.ID, userId);
            if (!isAuthor)
            {
                return false; // Якщо користувач не є автором, редагування неможливе
            }

            string connectionString = ConfigurationHelper.GetConnectionString();
            using (var connection = new SqlConnection(connectionString))
            {
                string sql = @"
                    UPDATE Snippet
                    SET Title = @Title,
                        Description = @Description,
                        ProgrammingLanguageID = @ProgrammingLanguageID,
                        Code = @Code,
                        Status = @Status
                    WHERE ID = @SnippetID;
                ";

                var rowsAffected = await connection.ExecuteAsync(sql, new
                {
                    snippet.Title,
                    snippet.Description,
                    snippet.ProgrammingLanguageID,
                    snippet.Code,
                    snippet.Status,
                    SnippetID = snippet.ID
                });

                return rowsAffected > 0;
            }
        }*/

        public async Task<bool> UpdateSnippetAsync(SnippetModel snippet, int userId)
        {
            // Перевіряємо, чи є користувач автором сніпета
            var isAuthor = await IsUserAuthorOfSnippetAsync(snippet.ID, userId);
            if (!isAuthor)
            {
                return false; // Якщо користувач не є автором, редагування неможливе
            }

            string connectionString = ConfigurationHelper.GetConnectionString();
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Оновлення основної інформації про сніпет
                        string updateSnippetSql = @"
                            UPDATE Snippet
                            SET Title = @Title,
                                Description = @Description,
                                ProgrammingLanguageID = @ProgrammingLanguageID,
                                Code = @Code,
                                Status = @Status
                            WHERE ID = @SnippetID;
                        ";

                        var rowsAffected = await connection.ExecuteAsync(updateSnippetSql, new
                        {
                            snippet.Title,
                            snippet.Description,
                            snippet.ProgrammingLanguageID,
                            snippet.Code,
                            snippet.Status,
                            SnippetID = snippet.ID
                        }, transaction);


                        // Перевірка та оновлення категорій, якщо необхідно
                        bool areCategoriesUpdated = await _categoryRepository.UpdateSnippetCategoriesAsync(snippet.ID, snippet.Categories, connection, transaction);

                        // Перевірка та оновлення тегів, якщо необхідно
                        bool areTagsUpdated = await _tagRepository.UpdateSnippetTagsAsync(snippet.ID, snippet.Tags, connection, transaction);

                        if (rowsAffected == 0 && !areCategoriesUpdated && !areTagsUpdated)
                        {
                            transaction.Rollback();
                            return false; // Якщо жодного рядка не було оновлено
                        }

                        // Фіксація транзакції
                        transaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        // Відкат транзакції у разі помилки
                        transaction.Rollback();
                        Console.WriteLine("Error while updating snippet: " + ex.Message);
                        return false;
                    }
                }
            }
        }

        public async Task<bool> AddSnippetAsync(SnippetModel snippet)
        {
            string connectionString = ConfigurationHelper.GetConnectionString();
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Додавання основної інформації про сніпет
                        string insertSnippetSql = @"
                            INSERT INTO Snippet (Title, Description, ProgrammingLanguageID, Code, Status, AuthorID)
                            VALUES (@Title, @Description, @ProgrammingLanguageID, @Code, @Status, @AuthorID);
                            SELECT CAST(SCOPE_IDENTITY() as int);
                        ";

                        int snippetId = await connection.QuerySingleAsync<int>(insertSnippetSql, new
                        {
                            snippet.Title,
                            snippet.Description,
                            snippet.ProgrammingLanguageID,
                            snippet.Code,
                            snippet.Status,
                            snippet.AuthorID
                        }, transaction);

                        // Додавання зв'язків для категорій
                        if (snippet.Categories != null && snippet.Categories.Any())
                        {
                            await _categoryRepository.AddSnippetCategoriesAsync(snippetId, snippet.Categories, connection, transaction);
                        }

                        // Додавання зв'язків для тегів
                        if (snippet.Tags != null && snippet.Tags.Any())
                        {
                            await _tagRepository.AddSnippetTagsAsync(snippetId, snippet.Tags, connection, transaction);
                        }

                        // Фіксація транзакції
                        transaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        // Відкат транзакції у разі помилки
                        transaction.Rollback();
                        Console.WriteLine("Error while adding snippet: " + ex.Message);
                        return false;
                    }
                }
            }
        }

        public async Task<IEnumerable<SnippetModel>> GetAllPublicSnippetsAsync()
        {
            string connectionString = ConfigurationHelper.GetConnectionString();
            using (var connection = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT Snippet.ID, Title, Description, ProgrammingLanguageID, AuthorID, CreatedAt, UpdatedAt, Code, Status,
                    COUNT(SavedSnippets.UserID) AS SavesCount
                    FROM Snippet
                    INNER JOIN SavedSnippets ON SavedSnippets.SnippetID = Snippet.ID
                    WHERE Status = 'public'
                    GROUP BY Snippet.ID, Title, Description, ProgrammingLanguageID, AuthorID, CreatedAt, UpdatedAt, Code, Status;
                ";

                var publicSnippets = await connection.QueryAsync<SnippetModel>(query);
                return publicSnippets;
            }
        }

        public async Task<IEnumerable<SnippetModel>> GetUserSnippetsAsync(int userId)
        {
            string connectionString = ConfigurationHelper.GetConnectionString();
            using (var connection = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT Snippet.ID, Title, Description, ProgrammingLanguageID, AuthorID, CreatedAt, UpdatedAt, Code, Status,
                    COUNT(SavedSnippets.UserID) AS SavesCount
                    FROM Snippet
                    INNER JOIN SavedSnippets ON SavedSnippets.SnippetID = Snippet.ID
                    WHERE AuthorID = @CurrentUserID
                    GROUP BY Snippet.ID, Title, Description, ProgrammingLanguageID, AuthorID, CreatedAt, UpdatedAt, Code, Status;
                ";

                var userSnippets = await connection.QueryAsync<SnippetModel>(query, new { CurrentUserID = userId });
                return userSnippets;
            }
        }

        public async Task<IEnumerable<SnippetModel>> GetSavedSnippetsByUserAsync(int userId)
        {
            string connectionString = ConfigurationHelper.GetConnectionString();
            using (var connection = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT Snippet.ID AS SnippetID, Title, Description, ProgrammingLanguageID, AuthorID, CreatedAt, UpdatedAt, 
                           Code, Status, COUNT(SavedSnippets.UserID) AS SavesCount
                    FROM SavedSnippets
                    INNER JOIN Snippet ON Snippet.ID = SavedSnippets.SnippetID
                    WHERE SavedSnippets.UserID = @CurrentUserID
                    GROUP BY Snippet.ID, Title, Description, ProgrammingLanguageID, AuthorID, CreatedAt, UpdatedAt, 
                             Code, Status;
                ";

                var savedSnippets = await connection.QueryAsync<SnippetModel>(query, new { CurrentUserID = userId });
                return savedSnippets;
            }
        }

        public async Task<bool> AddSnippetToSavedAsync(int userId, int snippetId)
        {
            string connectionString = ConfigurationHelper.GetConnectionString();
            using (var connection = new SqlConnection(connectionString))
            {
                string query = @"
                    INSERT INTO SavedSnippets (UserID, SnippetID)
                    VALUES (@CurrentUserID, @CurrentSnippetID);
                ";

                var rowsAffected = await connection.ExecuteAsync(query, new
                {
                    CurrentUserID = userId,
                    CurrentSnippetID = snippetId
                });

                return rowsAffected > 0;
            }
        }

        public async Task<bool> RemoveSnippetFromSavedAsync(int userId, int snippetId)
        {
            string connectionString = ConfigurationHelper.GetConnectionString();
            using (var connection = new SqlConnection(connectionString))
            {
                string query = @"
                    DELETE FROM SavedSnippets
                    WHERE UserID = @CurrentUserID AND SnippetID = @CurrentSnippetID;
                ";

                var rowsAffected = await connection.ExecuteAsync(query, new
                {
                    CurrentUserID = userId,
                    CurrentSnippetID = snippetId
                });

                return rowsAffected > 0;
            }
        }
    }
}

