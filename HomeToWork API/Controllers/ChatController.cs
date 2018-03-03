using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.WebPages;
using HomeToWork.Chat;
using HomeToWork.Firebase;
using HomeToWork_API.Auth;
using HomeToWork_API.Utils;

namespace HomeToWork_API.Controllers
{
    public class ChatController : ApiController
    {
        [HttpGet]
        [Route("api/chat/{chatId:int}")]
        public IHttpActionResult GetChatMessages(int chatId)
        {
            if (!Session.Authorized) return Unauthorized();

            var chatDao = new ChatDao();
            var messages = chatDao.GetMessagesByChatId(chatId);
            chatDao.SetMessagesAsRead(Session.User.Id, chatId);

            return Ok(messages);
        }

        [HttpPost]
        [Route("api/chat/{chatId:int}")]
        public IHttpActionResult PostMessage(int chatId, FormDataCollection data)
        {
            if (!Session.Authorized) return Unauthorized();

            var chatDao = new ChatDao();
            var chat = chatDao.GetByChatId(chatId);

            if (chat == null)
                return NotFound();

            var valueMap = FormDataConverter.Convert(data);
            var text = valueMap.Get("text");

            chatDao.InsertMessage(Session.User.Id, chatId, text);

            var msgData = new Dictionary<string, string>
            {
                {"TYPE", "NEW_MESSAGE"},
                {"CHAT_ID", chatId.ToString()},
                {"TEXT", text}
            };
            var recipientId = chat.User1.Id == Session.User.Id ? chat.User2.Id : chat.User1.Id;
            FirebaseCloudMessanger.SendMessage(
                recipientId,
                "Nuovo messaggio", Session.User + " ti ha inviato un nuovo messaggio",
                msgData,
                "it.gruppoinfor.hometowork.NEW_MESSAGE");

            return Ok();
        }

        [HttpPost]
        [Route("api/chat/new")]
        public IHttpActionResult PostNewChat(FormDataCollection data)
        {
            if (!Session.Authorized) return Unauthorized();

            var valueMap = FormDataConverter.Convert(data);

            var recipientId = valueMap.Get("recipientId").AsInt();

            var chatDao = new ChatDao();

            // Controllo se era già stata creata una chat tra i due utenti
            var chat = chatDao.GetByUserIds(Session.User.Id, recipientId);

            if (chat != null) return Ok(chat);

            // Se non esiste ne creo una nuova

            var newChatId = chatDao.NewChat(Session.User.Id, recipientId);
            chat = chatDao.GetByChatId(newChatId);

            return Ok(chat);
        }
    }
}