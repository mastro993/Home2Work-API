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
    public class Message
    {
        public long Id { get; set; }
        public Author Sender { get; set; }
        public string Text { get; set; }
        public DateTime Time { get; set; }
        public bool Read { get; set; }

        public static Message Parse(SqlDataReader reader)
        {
            var userDao = new UserDao();

            var senderId = Int32.Parse(reader["sender_id"].ToString());
            var sender = userDao.GetById(senderId);

            var messageId = Int64.Parse(reader["id"].ToString());
            var messageText = reader["text"].ToString();
            var messageTime = DateTime.Parse(reader["time"].ToString());
            var messageRead = (bool) reader["read"];


            return new Message()
            {
                Id = messageId,
                Sender = new Author() {Id = sender.Id, Name = sender.ToString()},
                Text = messageText,
                Time = messageTime,
                Read = messageRead
            };
        }
    }
}