namespace SportRentHub.Entities.DTOs.Booking
{
    public class BookingSearchDto
    {
        public int? Id { get; set; }
        public int? UserId { get; set; }
		public int? ChildCourtId { get; set; }
		public int? Status { get; set; }
        public DateTime? EndTime { get; set; }
        public List<int>? IdLst { get; set; }
    }
}
