namespace SportRentHub.Entities.DTOs.ChildCourt
{
	public class ChildCourtSearchDto
	{
		public int? Id { get; set; }
		public int? CourtId { get; set; }
		public string? ChildCourtName { get; set; }
		public List<int>? IdLst { get; set; }
	}
}
