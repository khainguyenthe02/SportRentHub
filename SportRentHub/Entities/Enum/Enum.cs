namespace SportRentHub.Entities.Enum
{
    public enum RoleType
    {
        TECHNICAL,
        ACCOUNTANT,
        SALES,
        ADMIN = 999
    }
    public enum ComponentType
    {
        COURT = 1,
        USER = 2
    }
    public enum CourtStatus
    {
        ACTIVE, // đang hoạt động
        REPAIR_PART, // đang sửa chữa 1 phần
        REPAIR_ALL, // đang sửa chữa toàn bộ
        INACTIVE // ngừng hoạt động
    }
    public enum  BookingStatus
    {
        BOOKED, // đã đặt
        UNPAID, // chưa thanh toán
        PAID, // đã thanh toán
        CANCELED, // đã hủy
        REFUNDED // đã hoàn tiền

    }
    public enum PaymentStatus
    {
        UN_PAID, // chưa thanh toán
        PAID, // đã thanh toán
        CANCELED, // đã hủy
		UNPAID_FOR_ADMIN, // chưa thanh toán cho admin
		PAID_FOR_ADMIN, // đã thanh toán cho admin
	}
}
