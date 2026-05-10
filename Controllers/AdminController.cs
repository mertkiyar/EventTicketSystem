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
        public IActionResult Index()
        {
            var events = _context.Events.ToList();
            return View(events);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Event model)
        {
            if (!ModelState.IsValid)
                return View(model);
            _context.Events.Add(model);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult Delete(int id)
        {
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