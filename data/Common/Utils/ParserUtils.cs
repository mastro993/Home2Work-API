namespace data.Common.Utils
{
    public static class ParserUtils
    {
        public static int ToInt(this object o)
        {
            if (int.TryParse(o.ToString(), out int i)) return i;
            return 0;
        }

        public static double ToDouble(this object o)
        {
            if (double.TryParse(o.ToString(), out double i)) return i;
            return 0;
        }

        public static long ToLong(this object o)
        {
            if (long.TryParse(o.ToString(), out long i)) return i;
            return 0;
        }

        public static float ToFloat(this object o)
        {
            if (float.TryParse(o.ToString(), out float i)) return i;
            return 0;
        }
    }
}