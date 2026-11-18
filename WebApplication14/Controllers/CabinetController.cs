using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using WebApplication14.Models;

namespace WebApplication14.Controllers
{
    [Authorize]
    public class CabinetController : Controller
    {
        private readonly string _conn;
        private readonly ItemRepository _repo;

        public CabinetController(IConfiguration config)
        {
            _conn = config.GetConnectionString("Default");
            _repo = new ItemRepository(_conn);
        }

        // id текущего пользователя
        private int UserId()
        {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        }

        public IActionResult Index()
        {
            ViewBag.Count = _repo.CountByUser(UserId());
            return View();
        }

        public IActionResult MyItems()
        {
            return View(_repo.GetByUser(UserId()));
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(ItemFormModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var item = new Item
            {
                Title = model.Title,
                Description = model.Description,
                Price = model.Price,
                UserId = UserId()
            };
            _repo.Add(item);
            return RedirectToAction("MyItems");
        }

        public IActionResult Edit(int id)
        {
            var item = _repo.GetById(id);
            if (item == null || item.UserId != UserId()) return NotFound();

            var model = new ItemFormModel
            {
                Id = item.Id,
                Title = item.Title,
                Description = item.Description,
                Price = item.Price
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(ItemFormModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var item = new Item
            {
                Id = model.Id,
                Title = model.Title,
                Description = model.Description,
                Price = model.Price,
                UserId = UserId()
            };
            if (!_repo.Update(item)) return NotFound();
            return RedirectToAction("MyItems");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            _repo.Delete(id, UserId());
            return RedirectToAction("MyItems");
        }

        public IActionResult Profile()
        {
            var model = new ProfileModel();
            using (var con = new SqlConnection(_conn))
            {
                con.Open();
                var cmd = new SqlCommand("SELECT Name, Email, RegisteredAt FROM Users WHERE Id = @Id", con);
                cmd.Parameters.AddWithValue("@Id", UserId());
                using (var r = cmd.ExecuteReader())
                {
                    if (r.Read())
                    {
                        model.Name = r.GetString(0);
                        model.Email = r.GetString(1);
                        model.RegisteredAt = r.GetDateTime(2);
                    }
                }
            }
            return View(model);
        }

        [HttpPost]
        public IActionResult Profile(ProfileModel model)
        {
            if (!ModelState.IsValid) return View(model);

            using (var con = new SqlConnection(_conn))
            {
                con.Open();

                // email не должен совпадать с чужим
                var check = new SqlCommand("SELECT COUNT(*) FROM Users WHERE Email = @Email AND Id <> @Id", con);
                check.Parameters.AddWithValue("@Email", model.Email);
                check.Parameters.AddWithValue("@Id", UserId());
                if ((int)check.ExecuteScalar() > 0)
                {
                    ModelState.AddModelError("Email", "Такой email уже занят");
                    return View(model);
                }

                var cmd = new SqlCommand("UPDATE Users SET Name = @Name, Email = @Email WHERE Id = @Id", con);
                cmd.Parameters.AddWithValue("@Name", model.Name);
                cmd.Parameters.AddWithValue("@Email", model.Email);
                cmd.Parameters.AddWithValue("@Id", UserId());
                cmd.ExecuteNonQuery();
            }

            TempData["Message"] = "Профиль сохранён";
            return RedirectToAction("Profile");
        }

        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ChangePassword(ChangePasswordModel model)
        {
            if (!ModelState.IsValid) return View(model);

            using (var con = new SqlConnection(_conn))
            {
                con.Open();

                // проверяем старый пароль
                var check = new SqlCommand("SELECT COUNT(*) FROM Users WHERE Id = @Id AND PasswordHash = @Hash", con);
                check.Parameters.AddWithValue("@Id", UserId());
                check.Parameters.AddWithValue("@Hash", AccountController.Hash(model.OldPassword));
                if ((int)check.ExecuteScalar() == 0)
                {
                    ModelState.AddModelError("OldPassword", "Старый пароль неверный");
                    return View(model);
                }

                var cmd = new SqlCommand("UPDATE Users SET PasswordHash = @Hash WHERE Id = @Id", con);
                cmd.Parameters.AddWithValue("@Hash", AccountController.Hash(model.NewPassword));
                cmd.Parameters.AddWithValue("@Id", UserId());
                cmd.ExecuteNonQuery();
            }

            TempData["Message"] = "Пароль изменён";
            return RedirectToAction("ChangePassword");
        }
    }
}