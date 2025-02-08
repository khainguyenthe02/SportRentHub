namespace SportRentHub.Entities.Const
{
    public class MessageError
    {
        public const string ErrorCreate = "Có lỗi trong quá trình thêm mới";
        public const string ErrorUpdate = "có lỗi trong quá trình cập nhật";

        public const string NoContent = "Không tìm thấy dữ liệu";
        public const string LoginError = "Tên đăng nhập hoặc mật khẩu không đúng";
        public const string ComparePasswordError = "Mật khẩu gõ lại không trùng.";
        public const string InvalidPasswordError = "Mật khẩu không được để trống";
        public const string InvalidRePasswordError = "Mật khẩu nhập lại không được để trống";
        public const string PasswordNotEquals = " Mật khẩu mới không được trùng với mật khẩu cũ";
        public const string TypingPasswordError = "Mật khẩu cần có tối thiểu 6 kí tự, bao gồm chữ hoa, chữ thường, số và kí tự đặc biệt";
        public const string AccountInActive = "Tài khoản của bạn đã bị khóa, vui lòng liên hệ quản trị viên để được trợ giúp";
        public const string InvalidPasswordCount = "Tài khoản của bạn đã bị khóa do nhập sai mật khẩu quá số lần quy định, vui lòng liên hệ quản trị viên để được trợ giúp";

        public const string TypeEmailError = "Email không đúng định dạng";
        public const string EmailExist = "Email đã tồn tại trên hệ thống";
        public const string EmailNotExist = "Email không tồn tại trên hệ thống";

        public const string IDNotExist = "ID không tồn tại trên hệ thống";

        public const string PostImageError = "Hình ảnh không khả dụng";

        public const string UserOrPostNotExist = "Bài viết hoặc người dùng không tồn tại";

        public const string PostIsFavorited = "Bài viết đã được yêu thích trước đó, không thể tạo mới";
        public const string ReportLessThan24h = "Vui lòng đợi 24h tiếp theo để có thể báo cáo tiếp.";
        public const string UserHasReportedYet = "Bạn đã báo cáo trước đó vào lúc: ";

        public const string CreatedOrUpdatedPriceNotEqualsZero = "Giá đăng bài với giá cập nhật không thể bằng 0";
        public const string CountPostToSaleNotEqualsZero = "Số lượng bài đăng giảm giá không thể bằng 0";
        public const string PercentNotEqualsZero = "Phần trăm giảm giá không thể bằng 0";
    }
}
