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

            public AddSnippetController(SnippetRepository snippetRepository)
            {
                _snippetRepository = snippetRepository;
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
            //[Route("api/snippets/add")]
            public async Task<IActionResult> AddSnippetAsync( string title, string description, int languageID, string code, 
                string status,  List<CategoryModel> categories, List<TagModel> tags, int userID)
            {
                // Перевіряємо, чи запит містить необхідні дані
/*                if (request == null || string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.Code))
                {
                    return BadRequest("Invalid request data.");
                }*/

                //  ------метод валідації даних в жсі зробити--------

                try
                {
                    // Отримуємо ідентифікатор користувача (наприклад, з токена аутентифікації)
                    //int userId = GetUserIdFromToken(); // Реалізуйте метод для отримання ідентифікатора користувача

                    // Створюємо новий об'єкт SnippetModel з вхідними даними
                    var newSnippet = new SnippetModel
                    {
                        Title = title,
                        Description = description,
                        ProgrammingLanguageID = languageID,
                        Code = code,
                        Status = status,
                        AuthorID = userID,
                        Categories = categories,
                        Tags = tags
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


            // Дія для обробки додавання нового сніпета
            /*[HttpPost]
            public IActionResult AddSnippet(SnippetModel model)
            {
                if (ModelState.IsValid)
                {
                    // Логіка додавання сніпета в базу даних
                    // Після успішного додавання - перенаправлення на головну сторінку
                    return RedirectToAction("Index", "Home");
                }

                // Якщо модель невалідна - повернення до форми з валідаційними повідомленнями
                *//*model.Languages = GetProgrammingLanguages();
                model.Categories = GetAvailableCategories();
                model.Tags = GetAvailableTags();*//*
                return View(model);
            }*/




        }
    }

}
