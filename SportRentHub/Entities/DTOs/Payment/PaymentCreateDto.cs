namespace SportRentHub.Entities.DTOs.Payment
{
    public class PaymentCreateDto
    {
        public int? UserId { get; set; }
        public int? BookingId { get; set; }
        public float? Price { get; set; }
        public int? Type { get; set; }
    }
}
