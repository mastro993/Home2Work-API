using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading;
using HomeToWork.Utils;

namespace HomeToWork.Common
{
    [Serializable]
    public class LatLng
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public LatLng(double lat, double lng)
        {
            Latitude = lat;
            Longitude = lng;
        }

        public static LatLng Parse(string latLngString)
        {
            if (latLngString.Equals("")) return null;
            var ci = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
            var strings = latLngString.Split(',');
            var lat = double.Parse(strings[0]);
            var lng = double.Parse(strings[1]);
            return new LatLng(lat, lng);
        }

        public override string ToString()
        {
            var nfi = new NumberFormatInfo {NumberDecimalSeparator = "."};
            return Latitude.ToString(nfi) + "," + Longitude.ToString(nfi);
        }

        public double GetDistanceTo(LatLng latLng)
        {
            return MapUtils.Haversine(Latitude, latLng.Latitude, Longitude, latLng.Longitude);
        }

        public double GetDistanceTo(double lat, double lng)
        {
            return MapUtils.Haversine(Latitude, lat, Longitude, lng);
        }

        public static double GetDistance(LatLng a, LatLng b)
        {
            return MapUtils.Haversine(a.Latitude, b.Latitude, a.Longitude, b.Longitude);
        }
    }
}