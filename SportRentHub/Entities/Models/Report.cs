namespace SportRentHub.Entities.Models
{
	public class Report
	{
		public int Id { get; set; }
		public int UserId { get; set; }
		public int CourtId { get; set; }
		public string Content { get; set; }
		public DateTime CreateDate { get; set; }
		public int Status { get; set; }
	}
}
