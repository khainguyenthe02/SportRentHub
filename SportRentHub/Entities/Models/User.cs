namespace SportRentHub.Entities.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Fullname { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public int? Status { get; set; } // 0: inactive, 1: active, 2: banned
		public int? Role { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? LastLogin { get; set; }
        public string Salt { get; set; }
        public string Token { get; set; }
        public string BankNumber { get; set; }
        public string BankName { get; set; }
        public string BankAccount { get; set; }


    }
}
