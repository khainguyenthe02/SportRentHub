namespace SportRentHub.Entities.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CourtId { get; set; }
        public string Content { get; set; }
        public int? RatingStar { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
