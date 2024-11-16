using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SnippetsLibraryWebApp.Models;
using SnippetsLibraryWebApp.Repository;
using System.Diagnostics;

namespace SnippetsLibraryWebApp.Controllers
{
    namespace SnippetsLibraryWebApp.Controllers
    {
        public class AddSnippetController : Controller
        {
            private readonly SnippetRepository _snippetRepository;
            private readonly TagRepository _tagRepository;
            private readonly CategoryRepository _categoryRepository;

            public AddSnippetController(SnippetRepository snippetRepository, TagRepository tagRepository, CategoryRepository categoryRepository)
            {
                _snippetRepository = snippetRepository;
                _tagRepository = tagRepository;
                _categoryRepository=categoryRepository;
            }

            // Дія для відкриття сторінки додавання сніпета
            //[HttpGet]
            public IActionResult AddSnippet()
            {
                return View();
            }

            [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
            public IActionResult Error()
            {
                return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }

            [HttpPost]
            [Route("api/tags/add")]
            public async Task<IActionResult> AddTag([FromBody] string tagName)
            {
                if (string.IsNullOrWhiteSpace(tagName))
                {
                    return BadRequest("Invalid tag name.");
                }

                try
                {
                    // Викликаємо метод репозиторія для додавання нового тега
                    var tagId = await _tagRepository.AddNewTagAsync(tagName);

                    // Якщо не вдалося додати тег
                    if (tagId == null)
                    {
                        return StatusCode(500, "Failed to add the tag.");
                    }

                    // Повертаємо ID нового тега
                    return Ok(tagId);
                }
                catch (Exception ex)
                {
                    // Логування помилки
                    Console.WriteLine($"Error while adding tag: {ex.Message}");
                    return StatusCode(500, "An error occurred while adding the tag.");
                }
            }



            [HttpPost]
            public async Task<IActionResult> AddSnippetAsync(string title, string description, int languageID, string code, 
                string status,  int[] categories, int[] tags, int userID)
            {
                try
                {
                    List<CategoryModel> categoriesModel = new List<CategoryModel>();

                    foreach(var category in categories)
                    {
                        categoriesModel.Add(await _categoryRepository.GetCategoryByIdAsync(category));
                    }

                    List<TagModel> tagsModel = new List<TagModel>();
                    foreach (var tag in tags)
                    {
                        tagsModel.Add(await _tagRepository.GetTagsByIdAsync(tag));
                    }

                    // Створюємо новий об'єкт SnippetModel з вхідними даними
                    var newSnippet = new SnippetModel
                    {
                        Title = title,
                        Description = description,
                        ProgrammingLanguageID = languageID,
                        Code = code,
                        Status = status,
                        AuthorID = userID,
                        Categories = categoriesModel,
                        Tags = tagsModel
                    };

                    // Додаємо сніпет до бази даних
                    var snippetId = await  _snippetRepository.AddSnippetAsync(newSnippet);

                    // Повертаємо успішну відповідь з ID нового сніпета
                    return Ok(new { SnippetID = snippetId });
                }
                catch (Exception ex)
                {
                    // Логування помилки
                    Console.WriteLine($"Error while adding snippet: {ex.Message}");
                    return StatusCode(500, "An error occurred while adding the snippet.");
                }
            }
        }
    }

}
