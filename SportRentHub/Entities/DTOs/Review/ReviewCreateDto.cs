namespace SportRentHub.Entities.DTOs.Review
{
    public class ReviewCreateDto
    {
        public int UserId { get; set; }
        public int CourtId { get; set; }
        public string? Content { get; set; }
        public int? RatingStar { get; set; }
    }
}
