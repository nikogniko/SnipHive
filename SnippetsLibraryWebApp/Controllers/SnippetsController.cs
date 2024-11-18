using Duende.IdentityServer.Events;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SnippetsLibraryWebApp.Extensions;
using SnippetsLibraryWebApp.Models;
using SnippetsLibraryWebApp.Repository;
using SnippetsLibraryWebApp.Utils;
using SnippetsLibraryWebApp.ViewModels;
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
        private readonly ProgrammingLanguageRepository _programmingLanguageRepository;


        public SnippetsController(
            SnippetRepository snippetRepository, 
            TagRepository tagRepository, 
            CategoryRepository categoryRepository, 
            ProgrammingLanguageRepository programmingLanguageRepository)
        {
            _snippetsRepository = snippetRepository;
            _tagRepository = tagRepository;
            _categoryRepository = categoryRepository;
            _programmingLanguageRepository = programmingLanguageRepository;
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
            ViewBag.PageType = nameof(AllSnippets);
            return View(snippets);
        }

        // Дія для перегляду деталей сніпета
        [AllowAnonymous] // Дозволяє анонімний доступ до деталей, якщо сніпети публічні
        [HttpGet]
        [Route("Snippets/Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var snippet = await _snippetsRepository.GetSnippetByIdAsync(id);
            if (snippet == null)
            {
                return NotFound();
            }

            // Припустимо, що Tags та Categories вже завантажені в snippet
            return View(snippet);
        }

        // Додайте ці методи у SnippetsController

        // Метод для видалення сніпета (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSnippet(int ID)
        {
            var snippet = await _snippetsRepository.GetSnippetByIdAsync(ID);
            if (snippet == null)
            {
                return NotFound();
            }

            // Перевірка, чи користувач є власником сніпета
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (snippet.AuthorID != currentUserId)
            {
                return Forbid();
            }

            // Видалення сніпета
            await _snippetsRepository.DeleteSnippetAsync(ID, currentUserId);

            return RedirectToAction("AllSnippets");
        }

        // GET: Snippets/EditSnippet/5
        [HttpGet]
        public async Task<IActionResult> EditSnippet(int id)
        {
            var snippet = await _snippetsRepository.GetSnippetByIdAsync(id);
            if (snippet == null)
            {
                return NotFound();
            }

            // Перевірка, чи користувач є власником сніпета
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (snippet.AuthorID != currentUserId)
            {
                return Forbid();
            }

            var viewModel = new EditSnippetViewModel
            {
                ID = snippet.ID,
                Title = snippet.Title,
                ProgrammingLanguageID = snippet.ProgrammingLanguageID,
                Description = snippet.Description,
                Code = snippet.Code,
                Status = snippet.Status,
                Categories = snippet.Categories.ToList(),
                Tags = snippet.Tags.ToList(),
                ProgrammingLanguages = new SelectList(await _programmingLanguageRepository.GetAllProgrammingLanguagesAsync(), "ID", "Name", snippet.ProgrammingLanguageID),
                AllCategories = (await _categoryRepository.GetAllCategoriesAsync()).Select(c => new SelectListItem
                {
                    Value = c.ID.ToString(),
                    Text = c.Name,
                    Selected = snippet.Categories.Any(sc => sc.ID == c.ID)
                }).ToList(),
                AllTags = (await _tagRepository.GetAllTagsAsync()).Select(t => new SelectListItem
                {
                    Value = t.ID.ToString(),
                    Text = t.Name,
                    Selected = snippet.Tags.Any(st => st.ID == t.ID)
                }).ToList()
            };

            return View(viewModel);
        }

        // Метод для обробки редагування сніпета
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSnippet(EditSnippetViewModel model, int[] selectedCategories, int[] selectedTags)
        {
            if (!ModelState.IsValid)
            {
                // Повторно завантажити категорії та теги, якщо модель не валідна
                model.Categories = (await _categoryRepository.GetCategoriesBySnippetIdAsync(model.ID)).ToList();
                model.Tags = (await _tagRepository.GetTagsBySnippetIdAsync(model.ID)).ToList();
                return View(model);
            }

            var existingSnippet = await _snippetsRepository.GetSnippetByIdAsync(model.ID);
            if (existingSnippet == null)
            {
                return Forbid();
            }

            // Оновлення полів сніпета
            existingSnippet.Title = model.Title;
            existingSnippet.Description = model.Description;
            existingSnippet.ProgrammingLanguageID = model.ProgrammingLanguageID;
            existingSnippet.Status = model.Status;
            existingSnippet.Code = model.Code;
            existingSnippet.UpdatedAt = DateTime.UtcNow;

            // Оновлення категорій та тегів
            //await _snippetsRepository.UpdateCategoriesAsync(existingSnippet.ID, categories);
            //await _snippetsRepository.UpdateTagsAsync(existingSnippet.ID, tags);

            // Збереження змін
            await _snippetsRepository.UpdateSnippetAsync(existingSnippet, int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value));

            return RedirectToAction("Details", new { id = existingSnippet.ID });
        }

        [HttpPost]
        [Route("/Snippets/AddSnippetToSavedAsync")]
        public async Task<IActionResult> AddSnippetToSavedAsync(int userId, int snippetId)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized(); // Повертає статус 401
            }

            var isAlreadySaved = await _snippetsRepository.IsSnippedSavedByUser(userId, snippetId);

            if (isAlreadySaved)
            {
                var result = await _snippetsRepository.RemoveSnippetFromSavedAsync(userId, snippetId);

                if (!result)
                {
                    return Json(new { success = false, isSaved = true });
                }

                return Json(new { success = true, isSaved = false });
            }
            else
            {
                var result = await _snippetsRepository.AddSnippetToSavedAsync(userId, snippetId);

                if (!result)
                {
                    return Json(new { success = false, isSaved = false });
                }

                return Json(new { success = true, isSaved = true });
            }
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
            ViewBag.PageType = nameof(FavoriteSnippets);
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
            ViewBag.PageType = nameof(MySnippets);
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
    }
}
