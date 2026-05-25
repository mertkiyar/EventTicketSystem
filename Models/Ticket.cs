namespace EventTicketSystem.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerEmail { get; set; }
        public int Quantity { get; set; } = 1;
        public DateTime PurchaseDate { get; set; }
        public int EventId { get; set; }
        public required Event Event { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
    }
}