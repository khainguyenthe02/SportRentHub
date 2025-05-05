namespace SportRentHub.Entities.DTOs.User
{
    public class UserSearchDto
    {
        public int? Id { get; set; }
        public string? Username { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public int? Role { get; set; }
        public List<int>? IdLst { get; set; }   
        public int? Status { get; set; }
	}
}
