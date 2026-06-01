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
        public async Task<IActionResult> Create(Event model, IFormFile imageFile)
        {
            if (!IsUserAdmin())
                return UnauthorizedAdminAccess();

            if (!ModelState.IsValid)
                return View(model);

            // Handle image upload
            if (imageFile != null && imageFile.Length > 0)
            {
                // Generate unique filename
                string filename = $"{Guid.NewGuid()}_{Path.GetFileName(imageFile.FileName)}";
                string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", filename);

                // Create directory if it doesn't exist
                Directory.CreateDirectory(Path.GetDirectoryName(uploadPath));

                // Save file
                using (var stream = new FileStream(uploadPath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                // Set the image URL
                model.ImageUrl = $"/uploads/{filename}";
            }
            else if (string.IsNullOrEmpty(model.ImageUrl))
            {
                // Provide a default image if none is provided
                model.ImageUrl = "/images/concert_banner.png";
            }

            _context.Events.Add(model);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            if (!IsUserAdmin())
                return UnauthorizedAdminAccess();

            var eventItem = _context.Events.FirstOrDefault(x => x.Id == id);
            if (eventItem == null)
                return NotFound();

            return View(eventItem);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Event model, IFormFile imageFile)
        {
            if (!IsUserAdmin())
                return UnauthorizedAdminAccess();

            var eventItem = _context.Events.FirstOrDefault(x => x.Id == id);
            if (eventItem == null)
                return NotFound();

            if (!ModelState.IsValid)
                return View(model);

            // Update basic properties
            eventItem.Title = model.Title;
            eventItem.Description = model.Description;
            eventItem.Location = model.Location;
            eventItem.Date = model.Date;
            eventItem.Time = model.Time;
            eventItem.TicketCount = model.TicketCount;
            eventItem.Category = model.Category;

            // Handle image upload (if provided)
            if (imageFile != null && imageFile.Length > 0)
            {
                // Generate unique filename
                string filename = $"{Guid.NewGuid()}_{Path.GetFileName(imageFile.FileName)}";
                string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", filename);

                // Create directory if it doesn't exist
                Directory.CreateDirectory(Path.GetDirectoryName(uploadPath));

                // Save file
                using (var stream = new FileStream(uploadPath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                // Update image URL
                eventItem.ImageUrl = $"/uploads/{filename}";
            }

            _context.Events.Update(eventItem);
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