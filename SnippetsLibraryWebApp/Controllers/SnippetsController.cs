using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SnippetsLibraryWebApp.Extensions;
using SnippetsLibraryWebApp.Models;
using SnippetsLibraryWebApp.Repository;
using SnippetsLibraryWebApp.Utils;
using System.Diagnostics;
using System.Security.Claims;

namespace SnippetsLibraryWebApp.Controllers
{
    [Authorize(AuthenticationSchemes = "CookieAuth")]
    public class SnippetsController : Controller
    {
        private readonly SnippetRepository _snippetsRepository;
        private readonly TagRepository _tagRepository;
        private readonly CategoryRepository _categoryRepository;

        public SnippetsController(SnippetRepository snippetRepository, TagRepository tagRepository, CategoryRepository categoryRepository)
        {
            _snippetsRepository = snippetRepository;
            _tagRepository = tagRepository;
            _categoryRepository = categoryRepository;
        }

        // Дія для відкриття сторінки додавання сніпета
        [HttpGet]
        public IActionResult AddSnippet()
        {
            return View("~/Views/AddSnippet/AddSnippet.cshtml");
        }

        // Сторінка "Усі сніпети"
        [AllowAnonymous]
        public async Task<IActionResult> AllSnippets()
        {
            var snippets = await _snippetsRepository.GetAllPublicSnippetsAsync();
            return View(snippets);
        }

        // Дія для відображення обраних сніпетів користувача
        [HttpGet]
        [Route("Snippets/FavoriteSnippets")]
        public async Task<IActionResult> FavoriteSnippets()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized(); // Повертає статус 401
            }

            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var favoriteSnippets = await _snippetsRepository.GetSavedSnippetsByUserAsync(userId);
            if (Request.IsAjaxRequest())
            {
                return PartialView("_SnippetList", favoriteSnippets);
            }
            return View(favoriteSnippets);
        }

        // Дія для відображення авторських сніпетів користувача
        [HttpGet]
        [Route("Snippets/MySnippets")]
        public async Task<IActionResult> MySnippets()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized(); // Повертає статус 401
            }

            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var userSnippets = await _snippetsRepository.GetUserSnippetsAsync(userId);
            if (Request.IsAjaxRequest())
            {
                return PartialView("_SnippetList", userSnippets);
            }
            return View(userSnippets);
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
        [Route("/Snippets/CreateSnippetAsync")]
        public async Task<IActionResult> CreateSnippetAsync(string title, string description, int languageID, string code,
                string status, int[] categories, int[] tags, int userID)
        {
            try
            {
                List<CategoryModel> categoriesModel = new List<CategoryModel>();

                foreach (var category in categories)
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
                var snippetId = await _snippetsRepository.AddSnippetAsync(newSnippet);

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

        // Детальна сторінка сніпета
        public async Task<IActionResult> Details(int id)
        {
            var snippet = await _snippetsRepository.GetSnippetByIdAsync(id);
            if (snippet == null)
            {
                return NotFound();
            }
            return View(snippet);
        }
    }
}
