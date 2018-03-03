using System.Collections.Specialized;
using System.Net.Http.Formatting;

namespace HomeToWork_API.Utils
{
    public class FormDataConverter
    {
        public static NameValueCollection Convert(FormDataCollection formDataCollection)
        {
            var pairs = formDataCollection.GetEnumerator();
            var collection = new NameValueCollection();

            while (pairs.MoveNext())
            {
                var pair = pairs.Current;
                collection.Add(pair.Key, pair.Value);
            }

            return collection;
        }
    }
}