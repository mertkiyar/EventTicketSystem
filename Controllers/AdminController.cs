using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EventTicketSystem.Data;
using EventTicketSystem.Models;

namespace EventTicketSystem.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;
        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        private bool IsUserAdmin()
        {
            var userIdStr = User.FindFirstValue("UserId");
            if (!int.TryParse(userIdStr, out int userId))
                return false;

            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            return user?.IsAdmin ?? false;
        }

        private IActionResult UnauthorizedAdminAccess()
        {
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Index()
        {
            if (!IsUserAdmin())
                return UnauthorizedAdminAccess();

            var events = _context.Events.ToList();
            return View(events);
        }

        public IActionResult Create()
        {
            if (!IsUserAdmin())
                return UnauthorizedAdminAccess();

            return View();
        }

        [HttpPost]
        public IActionResult Create(Event model)
        {
            if (!IsUserAdmin())
                return UnauthorizedAdminAccess();

            if (!ModelState.IsValid)
                return View(model);
            _context.Events.Add(model);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            if (!IsUserAdmin())
                return UnauthorizedAdminAccess();

            var ev = _context.Events.FirstOrDefault(x => x.Id == id);
            if (ev != null)
            {
                _context.Events.Remove(ev);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}