namespace SportRentHub.Entities.DTOs.Payment
{
    public class PaymentSearchDto
    {
        public int? Id { get; set; }
        public int? UserId { get; set; }
        public int? BookingId { get; set; }
        public int? Type { get; set; }
		public int? Status { get; set; }
		public List<int>? IdLst { get; set; }
    }
}
