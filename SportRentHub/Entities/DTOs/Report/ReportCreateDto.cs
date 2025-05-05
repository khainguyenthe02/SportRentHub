namespace SportRentHub.Entities.DTOs.Report
{
	public class ReportCreateDto
	{
		public int UserId { get; set; }
		public int CourtId { get; set; }
		public string? Content { get; set; }
	}
}
