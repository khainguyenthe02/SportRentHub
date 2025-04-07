using System.Security.Cryptography;
using System.Text;

namespace SportRentHub.Utils
{
    public class Convert
    {
        public static string GetMD5Hash(string input)
        {
            try
            {
                System.Security.Cryptography.MD5CryptoServiceProvider x = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] bs = Encoding.UTF8.GetBytes(input);
                bs = x.ComputeHash(bs);

                var s = new StringBuilder();
                foreach (byte b in bs)
                {
                    s.Append(b.ToString("x2"));
                }

                string password = s.ToString();
                return password;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public static string ComputeSHA256Hash(string input, string salt)
        {
            try
            {
                using (var sha256 = SHA256.Create())
                {
                    // Kết hợp mật khẩu và salt
                    string combinedInput = input + salt;

                    // Chuyển mật khẩu đã kết hợp thành mảng byte
                    byte[] bytes = Encoding.UTF8.GetBytes(combinedInput);

                    // Băm mật khẩu kết hợp với salt
                    byte[] hashBytes = sha256.ComputeHash(bytes);

                    // Chuyển đổi mảng byte thành chuỗi hexadecimal
                    var s = new StringBuilder();
                    foreach (byte b in hashBytes)
                    {
                        s.Append(b.ToString("x2"));
                    }
                    return s.ToString();
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public static string GenerateSalt(int size = 16)
        {
            var rng = new RNGCryptoServiceProvider();
            byte[] saltBytes = new byte[size];
            rng.GetBytes(saltBytes);
            return System.Convert.ToBase64String(saltBytes);
        }
        public static string ConvertRole(int roleType)
        {
            switch (roleType)
            {
                case 0:
                    return "User";
                case 999:
                    return "Admin";
                default:
                    return "";

            }
        }        
    }
}
