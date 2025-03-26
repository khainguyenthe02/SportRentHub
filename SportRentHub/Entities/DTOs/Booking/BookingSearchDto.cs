namespace SportRentHub.Entities.DTOs.Booking
{
    public class BookingSearchDto
    {
        public int? Id { get; set; }
        public int? UserId { get; set; }
        public int? CourtId { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? BookingDate { get; set; }
        public float? StartTime { get; set; }
        public float? EndTime { get; set; }
        public int? Status { get; set; }
        public List<int>? IdLst { get; set; }
    }
}
