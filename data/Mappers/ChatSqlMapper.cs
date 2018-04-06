using System;
using System.Data.SqlClient;
using data.Common;
using data.Common.Utils;
using data.Repositories;
using domain.Entities;
using domain.Interfaces;

namespace data.Mappers
{
    class ChatSqlMapper : Mapper<SqlDataReader, Chat>
    {

        private readonly IUserRepository _userRepo = new UserRepository();

        public override Chat MapFrom(SqlDataReader @from)
        { 

            var userId = @from["userId"].ToLong();
            var user = _userRepo.GetById(userId);

            var lastMsgSenderId = @from["last_message_sender_id"].ToLong();
            var lastMsgSender = _userRepo.GetById(lastMsgSenderId);

            var chatId = @from["id"].ToLong();
            var unreadCount = @from["unread_count"].ToInt();

            var lastMessageId = @from["last_message_id"].ToLong();
            var lastMsgText = @from["last_message_text"].ToString();
            var lastMsgTime = DateTime.Parse(@from["last_message_time"].ToString());
            var lastMsgNew = (bool)@from["last_message_new"];


            return new Chat()
            {
                Id = chatId,
                User = new Author()
                {
                    Id = user.Id,
                    Name = user.Name,
                    Surname = user.Surname
                },
                LastMessage = new Message()
                {
                    Id = lastMessageId,
                    Sender = new Author()
                    {
                        Id = lastMsgSender?.Id ?? 0,
                        Name = lastMsgSender?.Name ?? "",
                        Surname = lastMsgSender?.Surname ?? ""
                    },
                    Text = lastMsgText,
                    Time = lastMsgTime,
                    New = lastMsgNew
                },
                UnreadCount = unreadCount
            };
        }
    }
}
