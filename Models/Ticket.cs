namespace EventTicketSystem.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public required string CustomerName { get; set; }
        public required string CustomerEmail { get; set; }
        public int Quantity { get; set; }
        public DateTime PurchaseDate { get; set; }
        public int EventId { get; set; }
        public required Event Event { get; set; }
    }
}