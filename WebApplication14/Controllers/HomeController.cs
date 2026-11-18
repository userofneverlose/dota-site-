using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApplication14.Models;

namespace WebApplication14.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var config = HttpContext.RequestServices.GetRequiredService<IConfiguration>();
            var repo = new ItemRepository(config.GetConnectionString("Default"));
            var items = repo.GetAll().Take(3).ToList();
            return View(items);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contacts()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Contacts(ContactModel model)
        {
            if (!ModelState.IsValid) return View(model);

            TempData["Message"] = "Спасибо! Сообщение отправлено.";
            return RedirectToAction("Contacts");
        }   

        public IActionResult Faq()
        {
            return View();
        }

        public IActionResult Mark()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult Error404()
        {
            return View();
        }
    }
}
