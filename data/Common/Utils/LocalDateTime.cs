using System;
using System.Collections.Generic;
using System.Text;

namespace data.Common.Utils
{
    class LocalDateTime
    {

        public static DateTime Parse(string s)
        {
            var t = DateTime.SpecifyKind(
                DateTime.Parse(s),
                DateTimeKind.Utc);

            var tz = TimeZoneInfo.FindSystemTimeZoneById("Central Europe Standard Time");
            var tdt = TimeZoneInfo.ConvertTimeFromUtc(t, tz);

            return tdt;
        }
    }
}
