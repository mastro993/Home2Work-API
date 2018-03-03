using System.Text;

namespace HomeToWork.Utils
{
    public class HashingUtils
    {
        public static string Sha256(string stringToHash)
        {
            var sb = new StringBuilder();

            using (var hash = System.Security.Cryptography.SHA256.Create())
            {
                var enc = Encoding.UTF8;
                var result = hash.ComputeHash(enc.GetBytes(stringToHash));

                foreach (var b in result)
                    sb.Append(b.ToString("x2"));
            }

            return sb.ToString();
        }
    }
}