using System;

namespace data.Common.Utils
{
    public class StringUtils
    {
        private const string Chars = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

        public static string RandomString()
        {
            var lenght = Chars.Length;
            var randomString = "";
            var rnd = new Random();

            for (var i = 0; i < lenght; i++)
            {
                randomString += Chars[rnd.Next(0, lenght - 1)];
            }

            return randomString;
        }
    }
}