namespace SportRentHub.Entities.Models
{
    public class Court
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string CourtName { get; set; }
        public string CourtDescription { get; set; }
        public string District { get; set; }
        public string Ward {  get; set; }
        public string Street { get; set; }
        public string Images { get; set; }
        public float? MinPrice { get; set; }
        public float? MaxPrice { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string ContactPerson { get; set; }
        public string ContactPhone { get; set; }
        public int? Status { get; set; }
    }
}
