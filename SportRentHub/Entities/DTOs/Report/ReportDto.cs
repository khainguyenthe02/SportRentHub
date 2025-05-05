namespace SportRentHub.Entities.DTOs.Report
{
	public class ReportDto
	{
		public int Id { get; set; }
		public int UserId { get; set; }
		public int CourtId { get; set; }
		public string Content { get; set; }
		public DateTime CreateDate { get; set; }
		public int Status { get; set; }
		public string UserFullname { get; set; }
		public string CourtName { get; set; }
		public string CourtWard { get; set; }
		public string CourtStreet { get; set; }
		public string CourtDistrict { get; set; }
	}
}
