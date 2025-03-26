namespace SportRentHub.Entities.DTOs.Review
{
    public class ReviewDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CourtId { get; set; }
        public string Content { get; set; }
        public int? RatingStar { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string Username { get; set; }
        public string UserFullname { get; set; }
    }
}
