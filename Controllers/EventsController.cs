using Microsoft.AspNetCore.Mvc;
using EventTicketSystem.Data;
using EventTicketSystem.Models;

namespace EventTicketSystem.Controllers
{
    public class EventsController : Controller
    {
        private readonly AppDbContext _context;

        public EventsController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var events = _context.Events.ToList();
            return View(events);
        }

        public IActionResult Details(int id)
        {
            var eventItem = _context.Events.FirstOrDefault(x => x.Id == id);
            if (eventItem == null)
            {
                return NotFound();
            }
            return View(eventItem);
        }
    }
}