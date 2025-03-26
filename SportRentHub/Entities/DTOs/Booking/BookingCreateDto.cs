namespace SportRentHub.Entities.DTOs.Booking
{
    public class BookingCreateDto
    {
        public int UserId { get; set; }
        public int CourtId { get; set; }
        public DateTime BookingDate { get; set; }
        public float StartTime { get; set; }
        public float EndTime { get; set; }
        public int? Status { get; set; }
        public float Price { get; set; }
    }
}
