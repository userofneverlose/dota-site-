using Microsoft.AspNetCore.Mvc;
using WebApplication14.Models;

namespace WebApplication14.Controllers
{
    public class ItemsController : Controller
    {
        private readonly ItemRepository _repo;

        public ItemsController(IConfiguration config)
        {
            _repo = new ItemRepository(config.GetConnectionString("Default"));
        }

        public IActionResult Index()
        {
            return View(_repo.GetAll());
        }

        public IActionResult Details(int id)
        {
            var item = _repo.GetById(id);
            if (item == null) return NotFound();
            return View(item);
        }
    }
}