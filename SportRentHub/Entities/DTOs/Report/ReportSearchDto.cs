namespace SportRentHub.Entities.DTOs.Report
{
	public class ReportSearchDto
	{
		public int? Id { get; set; }
		public List<int>? IdLst { get; set; }
		public int? UserId { get; set; }
		public int? CourtId { get; set; }
		public int? Status { get; set; }
	}
}
