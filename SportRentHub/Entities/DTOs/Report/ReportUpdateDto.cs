namespace SportRentHub.Entities.DTOs.Report
{
	public class ReportUpdateDto
	{
		public int Id { get; set; }
		public int? UserId { get; set; }
		public int? CourtId { get; set; }
		public string? Content { get; set; }
		public DateTime? CreateDate { get; set; }
		public int? Status { get; set; }
	}
}
