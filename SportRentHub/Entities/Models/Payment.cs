namespace SportRentHub.Entities.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int BookingId { get; set; }
        public DateTime CreateDate { get; set; }
        public float Price { get; set; }
        public int Type { get; set; }
    }
}
