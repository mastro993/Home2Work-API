using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeToWork.User;

namespace HomeToWork.Chat
{
    [Serializable]
    public class Chat
    {
        public long Id { get; set; }
        public Author User1 { get; set; }
        public Author User2 { get; set; }
        public Message LastMessage { get; set; }
        public int UnreadCount { get; set; }

        public static Chat Parse(SqlDataReader reader)
        {
            var userDao = new UserDao();

            var user1Id = int.Parse(reader["user1"].ToString());
            var user2Id = int.Parse(reader["user2"].ToString());
            var user1 = userDao.GetById(user1Id);
            var user2 = userDao.GetById(user2Id);

            var lastMsgSenderId = int.Parse(reader["last_message_sender_id"].ToString());
            var lastMsgSender = userDao.GetById(lastMsgSenderId);

            var chatId = long.Parse(reader["id"].ToString());
            var unreadCount = int.Parse(reader["unread_count"].ToString());

            var lastMessageId = long.Parse(reader["last_message_id"].ToString());
            var lastMsgText = reader["last_message_text"].ToString();
            var lastMsgTime = DateTime.Parse(reader["last_message_time"].ToString());
            var lastMsgRead = (bool) reader["last_message_read"];


            return new Chat()
            {
                Id = chatId,
                User1 = new Author() {Id = user1.Id, Name = user1.ToString()},
                User2 = new Author() {Id = user2.Id, Name = user2.ToString()},
                LastMessage = new Message()
                {
                    Id = lastMessageId,
                    Sender = new Author()
                    {
                        Id = lastMsgSender.Id,
                        Name = lastMsgSender.ToString()
                    },
                    Text = lastMsgText,
                    Time = lastMsgTime,
                    Read = lastMsgRead
                },
                UnreadCount = unreadCount
            };
        }
    }
}