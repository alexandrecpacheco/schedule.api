using System.Security.Cryptography;
using System.Text;

namespace Schedule.Domain.Utils
{
    public class Cryptography
    {
        public static string SHA256(string text)
        {
            var sha256Managed = new SHA256Managed();
            var hash = new StringBuilder();
            var computeHash = sha256Managed.ComputeHash(Encoding.UTF8.GetBytes(text));
            foreach (var theByte in computeHash) hash.Append(theByte.ToString("x2"));

            return hash.ToString();
        }
    }
}
