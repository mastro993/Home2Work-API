using System;
using System.Data.SqlClient;
using System.Globalization;
using data.Common;
using data.Common.Utils;
using data.Repositories;
using domain.Entities;
using domain.Interfaces;

namespace data.Mappers
{
    class MessageSqlMapper : Mapper<SqlDataReader, Message>
    {
        private readonly IUserRepository _userRepo = new UserRepository();

        public override Message MapFrom(SqlDataReader @from)
        {
            var senderId = @from["sender_id"].ToLong();
            var sender = _userRepo.GetById(senderId);
            var messageId = @from["id"].ToLong();
            var chatId = @from["chat_id"].ToLong();
            var messageText = @from["text"].ToString();
            var messageTime = LocalDateTime.Parse(@from["time"].ToString());


            var messageNew = (bool) @from["new"];

            return new Message()
            {
                Id = messageId,
                ChatId = chatId,
                Sender = new Author()
                {
                    Id = sender.Id,
                    Name = sender.Name,
                    Surname = sender.Surname
                },
                Text = messageText,
                Time = messageTime,
                New = messageNew
            };
        }
    }
}