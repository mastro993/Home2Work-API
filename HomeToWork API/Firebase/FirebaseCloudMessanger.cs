using System.Collections.Generic;
using System.Threading.Tasks;
using data.Repositories;
using domain.Entities;
using domain.Interfaces;
using FirebaseNet.Messaging;
using Message = FirebaseNet.Messaging.Message;

namespace HomeToWork_API.Firebase
{
    public class FirebaseCloudMessanger
    {
        private const string FcmServerKey =
            "AAAAWufn6zM:APA91bGnlQ5Lho_wrWIzgvZf-1yzowMn0lGeunTSmPvpmeOy4Bmltq0rp-Rb2EHwwDqZmcEvXnk3Y2_LP8ZFcm_XI8jkrkNlTb_7I2EFvbeYqJImAIvAB_CrEApqPxRnd2VQefLXNkcj";

        public static async Task SendMessage(User recipient, string title, string body,
            IDictionary<string, string> data = null, string clickAction = null, string sound = "default",
            string icon = "default")
        {
            var client = new FCMClient(FcmServerKey);

            var message = new Message()
            {
                To = recipient.FirebaseToken,
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

        public static async Task SendMessage(User recipient, IDictionary<string, string> data = null)
        {
            var client = new FCMClient(FcmServerKey);

            var message = new Message()
            {
                To = recipient.FirebaseToken,
                Data = data
            };

            await client.SendMessageAsync(message);
        }
    }
}