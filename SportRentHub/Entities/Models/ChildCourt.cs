namespace SportRentHub.Entities.Models
{
	public class ChildCourt
	{
		public int Id { get; set; }
		public int CourtId { get; set; }
		public string ChildCourtName { get; set; }
		public string ChildCourtDescription { get; set; }
		public string Position { get; set; }
		public float RentCost	{ get; set; }
	}
}
