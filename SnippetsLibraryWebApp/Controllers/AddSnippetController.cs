using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SnippetsLibraryWebApp.Models;
using System.Diagnostics;

namespace SnippetsLibraryWebApp.Controllers
{
    namespace SnippetsLibraryWebApp.Controllers
    {
        public class AddSnippetController : Controller
        {
            // Дія для відкриття сторінки додавання сніпета
            //[HttpGet]
            public IActionResult AddSnippet()
            {
                // Підготовка даних для заповнення форми
                /*var model = new AddSnippet
                {
                    Languages = GetProgrammingLanguages(),
                    Categories = GetAvailableCategories(),
                    Tags = GetAvailableTags()
                };*/
                return View();
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

            [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
            public IActionResult Error()
            {
                return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }

            // Приклад методів отримання списків мов, категорій та тегів
            private List<SelectListItem> GetProgrammingLanguages()
            {
                // Отримання списку мов (можна змінити на запит до БД)
                return new List<SelectListItem>
            {
                new SelectListItem { Value = "1", Text = "C#" },
                new SelectListItem { Value = "2", Text = "JavaScript" },
                new SelectListItem { Value = "3", Text = "Python" }
            };
            }

            private List<SelectListItem> GetAvailableCategories()
            {
                // Отримання списку категорій (можна змінити на запит до БД)
                return new List<SelectListItem>
            {
                new SelectListItem { Value = "1", Text = "Web Development" },
                new SelectListItem { Value = "2", Text = "Data Science" }
            };
            }

            private List<SelectListItem> GetAvailableTags()
            {
                // Отримання списку тегів (можна змінити на запит до БД)
                return new List<SelectListItem>
            {
                new SelectListItem { Value = "1", Text = "Machine Learning" },
                new SelectListItem { Value = "2", Text = "Frontend" }
            };
            }
        }
    }

}
