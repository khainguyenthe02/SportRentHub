namespace SportRentHub.Entities.DTOs.Court
{
    public class CourtSearchDto
    {
        public int? Id { get; set; }
        public int? UserId { get; set; }
        public string? CourtName { get; set; }
        public string? District { get; set; }
        public string? Ward { get; set; }
        public string? Street { get; set; }
        public float? MinPrice { get; set; }
        public float? MaxPrice { get; set; }
        public int? Status { get; set; }
        public List<int>? IdLst { get; set; }
    }
}
