using System.ComponentModel.DataAnnotations;

namespace EventTicketSystem.Models
{
    public class Event
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required]
        public string Location { get; set; }

        public DateTime Date { get; set; }

        public string ImageUrl { get; set; }

        public int TicketCount { get; set; }

        public string Category { get; set; }
    }
}