using System.Text.RegularExpressions;

namespace ERBeeVisionMaster.Utils
{
    public class Validate
    {
        // Có tối thiểu 6 kí tự, bao gồm chữ hoa, chữ thường, số và ký tự đặc biệt
        public static bool ValidatePasword(string password)
        {

            var hasNumber = new Regex(@"[0-9]+");
            var hasUpperChar = new Regex(@"[A-Z]+");
            var hasLowerChar = new Regex(@"[a-z]+");
            var hasMinimum6Chars = new Regex(@".{6,}");

            var isValidated = hasNumber.IsMatch(password) && hasUpperChar.IsMatch(password) && hasMinimum6Chars.IsMatch(password) && HasSpecialChar(password) && hasLowerChar.IsMatch(password);

            return isValidated;

        }
        public static bool HasSpecialChar(string input)
        {
            string specialChar = @"\|!#$%&/()=?»«@£§€{}.-;'<>_,";
            foreach (var item in specialChar)
            {
                if (input.Contains(item)) return true;
            }

            return false;
        }
        public static string GenerateBase36Code()
        {
            return Guid.NewGuid().ToString("N").Substring(0, 8);
        }

        public static bool IsValidEmail(string email)
        {

            var hasEmail = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            var isValidated = hasEmail.IsMatch(email);
            return isValidated;
        }
    }
}
