namespace SportRentHub.Entities.DTOs.Booking
{
    public class BookingDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
		public int ChildCourtId { get; set; }
		public DateTime CreateDate { get; set; }
		public DateTime StartTime { get; set; }
		public DateTime EndTime { get; set; }
		public int Status { get; set; }
        public float Price { get; set; }
        public string UserFullName { get; set; }
        public string UserPhoneNumber { get; set; }
        public string ChildCourtName { get; set; }
    }
}
