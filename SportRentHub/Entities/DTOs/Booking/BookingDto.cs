using SportRentHub.Entities.DTOs.ChildCourt;

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
        public int CourtId { get; set; }
        public string CourtName { get; set; }
		public string CourtDistrict { get; set; }
		public string CourtWard { get; set; }
		public string CourtStreet { get; set; }
		public int Type { get; set; }
		public float Quantity { get; set; }
	}
}
