using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        // GET: /Events/DetailsJson/5 — returns event data as JSON for the modal
        [Authorize]
        public IActionResult DetailsJson(int id)
        {
            var eventItem = _context.Events.FirstOrDefault(x => x.Id == id);
            if (eventItem == null)
                return NotFound();

            return Json(new
            {
                id = eventItem.Id,
                title = eventItem.Title,
                description = eventItem.Description,
                location = eventItem.Location,
                date = eventItem.Date,
                imageUrl = eventItem.ImageUrl,
                ticketCount = eventItem.TicketCount,
                category = eventItem.Category
            });
        }

        // POST: /Events/Buy/5
        [HttpPost]
        [Authorize]
        public IActionResult Buy(int id)
        {
            var eventItem = _context.Events.FirstOrDefault(x => x.Id == id);
            if (eventItem == null)
                return NotFound();

            if (eventItem.TicketCount < 1)
            {
                TempData["ErrorMessage"] = "Sorry, no tickets left for this event.";
                return RedirectToAction("Index", "Home");
            }

            // Get user info from claims
            var userIdStr = User.FindFirstValue("UserId");
            if (!int.TryParse(userIdStr, out int userId))
                return Unauthorized();

            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
                return Unauthorized();

            var ticket = new Ticket
            {
                CustomerName = user.FullName,
                CustomerEmail = user.Email,
                Quantity = 1,
                EventId = id,
                Event = eventItem,
                UserId = userId,
                User = user,
                PurchaseDate = DateTime.Now
            };

            eventItem.TicketCount -= 1;
            _context.Tickets.Add(ticket);
            _context.SaveChanges();

            TempData["SuccessMessage"] = $"🎉 Ticket purchased successfully for \"{eventItem.Title}\"!";
            return RedirectToAction("MyTickets");
        }

        [Authorize]
        public IActionResult MyTickets()
        {
            var userIdStr = User.FindFirstValue("UserId");
            if (!int.TryParse(userIdStr, out int userId))
                return RedirectToAction("Index", "Home");

            var tickets = _context.Tickets
                .Where(t => t.UserId == userId)
                .Include(t => t.Event)
                .OrderByDescending(t => t.PurchaseDate)
                .ToList();

            return View(tickets);
        }
    }
}