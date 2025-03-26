namespace SportRentHub.Entities.DTOs.Review
{
    public class ReviewSearchDto
    {
        public int? Id { get; set; }
        public int? UserId { get; set; }
        public int? CourtId { get; set; }
        public List<int>? IdLst { get; set; }
    }
}
