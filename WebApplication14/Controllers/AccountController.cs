using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using WebApplication14.Models;

namespace WebApplication14.Controllers
{
    public class AccountController : Controller
    {
        private readonly string _conn;

        public AccountController(IConfiguration config)
        {
            _conn = config.GetConnectionString("Default");
        }

        // хеш пароля
        public static string Hash(string password)
        {
            byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
            return Convert.ToHexString(bytes);
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterModel model)
        {
            if (!ModelState.IsValid) return View(model);

            using (var con = new SqlConnection(_conn))
            {
                con.Open();

                // проверка: есть ли такой email
                var check = new SqlCommand("SELECT COUNT(*) FROM Users WHERE Email = @Email", con);
                check.Parameters.AddWithValue("@Email", model.Email);
                if ((int)check.ExecuteScalar() > 0)
                {
                    ModelState.AddModelError("Email", "Такой email уже есть");
                    return View(model);
                }

                var cmd = new SqlCommand("INSERT INTO Users (Name, Email, PasswordHash) VALUES (@Name, @Email, @Hash)", con);
                cmd.Parameters.AddWithValue("@Name", model.Name);
                cmd.Parameters.AddWithValue("@Email", model.Email);
                cmd.Parameters.AddWithValue("@Hash", Hash(model.Password));
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Login");
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid) return View(model);

            int id = 0;
            string name = "";
            string role = "";

            using (var con = new SqlConnection(_conn))
            {
                con.Open();
                var cmd = new SqlCommand("SELECT Id, Name, Role FROM Users WHERE Email = @Email AND PasswordHash = @Hash", con);
                cmd.Parameters.AddWithValue("@Email", model.Email);
                cmd.Parameters.AddWithValue("@Hash", Hash(model.Password));

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        id = reader.GetInt32(0);
                        name = reader.GetString(1);
                        role = reader.GetString(2);
                    }
                }
            }

            if (id == 0)
            {
                ModelState.AddModelError("", "Неверный email или пароль");
                return View(model);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, id.ToString()),
                new Claim(ClaimTypes.Name, name),
                new Claim(ClaimTypes.Role, role)
            };
            var identity = new ClaimsIdentity(claims, "Cookies");
            await HttpContext.SignInAsync("Cookies", new ClaimsPrincipal(identity));

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("Cookies");
            return RedirectToAction("Index", "Home");
        }
    }
}