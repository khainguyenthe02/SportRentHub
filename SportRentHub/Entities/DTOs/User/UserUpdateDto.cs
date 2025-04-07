namespace SportRentHub.Entities.DTOs.User
{
    public class UserUpdateDto
    {
        public int Id { get; set; }
        public string? Fullname { get; set; }
        public string? Password { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public int? Role { get; set; }
        public string? Token { get; set; }
    }
}
