using System.Collections.Generic;
using System.Threading.Tasks;
using data.Repositories;
using domain.Interfaces;
using FirebaseNet.Messaging;

namespace HomeToWork_API.Firebase
{
    public class FirebaseCloudMessanger
    {
        private static readonly IFirebaseRepository _firebaseRepo = new FcmTokenRepository();


        private const string FcmServerKey =
            "AAAAWufn6zM:APA91bGnlQ5Lho_wrWIzgvZf-1yzowMn0lGeunTSmPvpmeOy4Bmltq0rp-Rb2EHwwDqZmcEvXnk3Y2_LP8ZFcm_XI8jkrkNlTb_7I2EFvbeYqJImAIvAB_CrEApqPxRnd2VQefLXNkcj";

        public static async Task SendMessage(long toUserId, string title, string body,
            IDictionary<string, string> data = null, string clickAction = null, string sound = "default",
            string icon = "default")
        {
            var client = new FCMClient(FcmServerKey);
            var token = _firebaseRepo.GetUserToken(toUserId);

            var message = new Message()
            {
                To = token,
                Notification = new AndroidNotification()
                {
                    Body = body,
                    Title = title,
                    Icon = icon,
                    ClickAction = clickAction,
                    Sound = sound,
                },
                Data = data
            };

            await client.SendMessageAsync(message);
        }

        public static async Task SendMessage(long toUserId, IDictionary<string, string> data = null)
        {
            var client = new FCMClient(FcmServerKey);
            var token = _firebaseRepo.GetUserToken(toUserId);

            var message = new Message()
            {
                To = token,
                Data = data
            };

            await client.SendMessageAsync(message);
        }
    }
}