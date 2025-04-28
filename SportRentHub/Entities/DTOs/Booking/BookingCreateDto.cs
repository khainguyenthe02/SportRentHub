namespace SportRentHub.Entities.DTOs.Booking
{
    public class BookingCreateDto
    {
        public int UserId { get; set; }
		public int ChildCourtId { get; set; }
		public DateTime StartTime { get; set; }
		public DateTime EndTime { get; set; }
		public int? Status { get; set; }
        public float? Price { get; set; }
		public int? Type { get; set; }
		public float? Quantity { get; set; }
	}
}
