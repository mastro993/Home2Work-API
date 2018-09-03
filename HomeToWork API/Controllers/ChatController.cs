using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.WebPages;
using data.Common.Utils;
using data.Repositories;
using domain.Entities;
using domain.Interfaces;
using HomeToWork_API.Auth;
using HomeToWork_API.Firebase;
using HomeToWork_API.Utils;
using Microsoft.Ajax.Utilities;

namespace HomeToWork_API.Controllers
{
    public class ChatController : ApiController
    {
        private readonly IChatRepository _chatRepo;

        public ChatController()
        {
            _chatRepo = new ChatRepository();
        }

        [HttpGet]
        [Route("chat/list")]
        public IHttpActionResult GetChatList()
        {
            if (!Session.Authorized)
            {
                return Unauthorized();
            }

            var chats = _chatRepo.GetUserChatList(Session.User.Id);
            return Ok(chats);
        }

        [HttpGet]
        [Route("chat/{chatId:int}")]
        public IHttpActionResult GetChatMessages(int chatId)
        {
            if (!Session.Authorized)
            {
                return Unauthorized();
            }

            var messages = _chatRepo.GetMessagesByChatId(Session.User.Id, chatId);
            _chatRepo.SetMessagesAsRead(Session.User.Id, chatId);

            return Ok(messages);
        }

        [HttpPost]
        [Route("chat/{chatId:int}")]
        public IHttpActionResult PostMessage(int chatId, FormDataCollection data)
        {
            if (!Session.Authorized)
            {
                return Unauthorized();
            }

            var chat = _chatRepo.GetByChatId(Session.User.Id, chatId);

            if (chat == null)
            {
                return NotFound();
            }

            var valueMap = FormDataConverter.Convert(data);
            var text = valueMap.Get("text");

            if (text.IsNullOrWhiteSpace())
            {
                return BadRequest("Corpo messaggio mancante o vuoto");
            }

            var messageId =_chatRepo.InsertMessage(Session.User.Id, chatId, text);

            var msgData = new Dictionary<string, string>
            {
                {"TYPE", "NEW_MESSAGE"},
                {"CHAT_ID", chatId.ToString()},
                {"MESSAGE_ID", messageId.ToString()}
            };

            #pragma warning disable 4014
            FirebaseCloudMessanger.SendMessage(
                chat.User,
                "Nuovo messaggio", Session.User + " ti ha inviato un nuovo messaggio",
                msgData,
                "it.gruppoinfor.hometowork.NEW_MESSAGE");
            #pragma warning restore 4014


            var message = _chatRepo.getMessageById(messageId);

            return Ok(message);
        }


        [HttpGet]
        [Route("message/{messageId:long}")]
        public IHttpActionResult GetMessage(long messageId)
        {
            if (!Session.Authorized)
            {
                return Unauthorized();
            }

            var message = _chatRepo.getMessageById(messageId);

            var chat = _chatRepo.GetByChatId(Session.User.Id, message.ChatId);

            if (chat == null)
                return NotFound();

            return Ok(message);
        }

        [HttpPost]
        [Route("chat/new")]
        public IHttpActionResult PostNewChat(FormDataCollection data)
        {
            if (!Session.Authorized)
            {
                return Unauthorized();
            }

            var valueMap = FormDataConverter.Convert(data);

            var recipientId = valueMap.Get("recipientId").AsInt();

            if (recipientId == Session.User.Id)
            {
                return BadRequest("L'id del destinatario è uguale all'id del mittente");
            }

            // Controllo se era già stata creata una chat tra i due utenti
            var chat = _chatRepo.GetByUserIds(Session.User.Id, recipientId);

            if (chat != null)
            {
                return Ok(chat);
            }

            // Se non esiste ne creo una nuova

            var newChatId = _chatRepo.NewChat(Session.User.Id, recipientId);
            chat = _chatRepo.GetByChatId(Session.User.Id, newChatId);

            return Ok(chat);
        }
    }
}