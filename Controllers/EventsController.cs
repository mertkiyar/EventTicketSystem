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
        public IActionResult Buy(int id)
        {
            var eventItem = _context.Events.FirstOrDefault(x => x.Id == id);
            if (eventItem == null)

                return NotFound();
            return View(eventItem);
        }

        [HttpPost]

        public IActionResult Buy(int id, string name, string email, int quantity)
        {
            var eventItem = _context.Events.FirstOrDefault(x => x.Id == id);
            if (eventItem == null)
                return NotFound();
            if (eventItem.TicketCount < quantity)
                return BadRequest("Not enough tickets");
            var ticket = new Ticket
            {
                CustomerName = name,
                CustomerEmail = email,
                Quantity = quantity,
                EventId = id,
                Event = eventItem,
                PurchaseDate = DateTime.Now
            };
            eventItem.TicketCount -= quantity;
            _context.Tickets.Add(ticket);
            _context.SaveChanges();
            return RedirectToAction("Details", new { id = id });
        }
    }
}