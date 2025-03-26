namespace SportRentHub.Entities.DTOs.Payment
{
    public class PaymentDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int BookingId { get; set; }
        public DateTime CreateDate { get; set; }
        public float Price { get; set; }
        public int Type { get; set; }
        public string UserFullname { get; set; }
        public string UserPhoneNumber { get; set; }
        public int CourtId { get; set; }
        public string CourtName { get; set; }
    }
}
