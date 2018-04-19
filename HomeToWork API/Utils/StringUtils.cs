using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI.WebControls;

namespace HomeToWork_API.Utils
{
    public class StringUtils
    {
        /**
         * Controlla che l'email inserita sia nel formato valido
         */
        public static bool ValidateEmail(string emailString)
        {
            const string pattern =
                @"^[a-z][a-z|0-9|]*([_][a-z|0-9]+)*([.][a-z|0-9]+([_][a-z|0-9]+)*)?@[a-z][a-z|0-9|]*\.([a-z][a-z|0-9]*(\.[a-z][a-z|0-9]*)?)$";
            var match = Regex.Match(emailString.Trim(), pattern, RegexOptions.IgnoreCase);

            return match.Success;
        }

        /**
         * Controlla che la password inserita sia lunga almeno 8 caratteri, contanga un numero e un carratere speciale
         */
        public static bool ValidatePassword(string passwordString)
        {
           
            const string pattern = @"^.*(?=.{8,})(?=.*[\d])(?=.*[\W]).*$";

            var match = Regex.Match(passwordString.Trim(), pattern, RegexOptions.IgnoreCase);

            return match.Success;
        }
    }
}