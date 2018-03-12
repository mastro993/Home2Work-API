using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeToWork.Database;

namespace HomeToWork.Chat
{
    public class ChatDao
    {
        public int InsertMessage(int userId, int chatId, string text)
        {
            var con = new SqlConnection(Config.ConnectionString);

            var cmd = new SqlCommand
            {
                CommandText = $@"INSERT INTO message (chat_id, sender_id, text)
                                OUTPUT Inserted.Id
                                VALUES ({chatId}, {userId}, '{text}')",
                Connection = con
            };

            try
            {
                con.Open();
                return (int) cmd.ExecuteScalar();
            }
            finally
            {
                con.Close();
            }
        }

        public int NewChat(int userId, int recipientUserId)
        {
            var con = new SqlConnection(Config.ConnectionString);

            var cmd = new SqlCommand
            {
                CommandText = $@"INSERT INTO chat (user1, user2)
                                OUTPUT Inserted.Id
                                VALUES ({userId}, {recipientUserId})",
                Connection = con
            };

            try
            {
                con.Open();
                return (int) cmd.ExecuteScalar();
            }
            finally
            {
                con.Close();
            }
        }

        public Chat GetByChatId(int chatId)
        {
            var con = new SqlConnection(Config.ConnectionString);
            var cmd = new SqlCommand
            {
                CommandText = $@"SELECT
                                     c.id,
                                    c.user1,
                                    c.user2,
                                    c.time,
                                COALESCE(c.last_message_id, 0) as last_message_id,
                                c.message_count,
  (SELECT COUNT(*)
   FROM message m
   WHERE m.chat_id = c.id AND m.[read] = 0) AS unread_count,
  COALESCE(m.sender_id, 0)                  AS last_message_sender_id,
  COALESCE(m.text, '')                      AS last_message_text,
  COALESCE(m.time, getdate())                                    AS last_message_time,
  COALESCE(m.[read], cast(0 as BIT))                                   AS last_message_read
FROM chat_info c
  LEFT JOIN message m ON c.last_message_id = m.id
WHERE c.id = {chatId}",
                Connection = con
            };

            con.Open();

            try
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        return Chat.Parse(reader);
                    }
                }
            }
            finally
            {
                con.Close();
            }

            return null;
        }

        public Chat GetByUserIds(int userId1, int userId2)
        {
            var con = new SqlConnection(Config.ConnectionString);

            var cmd = new SqlCommand
            {
                CommandText = $@"SELECT
                                c.id,
                                    c.user1,
                                    c.user2,
                                    c.time,
                                COALESCE(c.last_message_id, 0) as last_message_id,
                                c.message_count,
  (SELECT COUNT(*)
   FROM message m
   WHERE m.chat_id = c.id AND m.[read] = 0) AS unread_count,
  COALESCE(m.sender_id, 0)                  AS last_message_sender_id,
  COALESCE(m.text, '')                      AS last_message_text,
  COALESCE(m.time, getdate())                                    AS last_message_time,
 COALESCE(m.[read], cast(0 as BIT))                                 AS last_message_read
                                FROM chat_info c
                                     LEFT JOIN message m ON c.last_message_id = m.id
                                WHERE (c.user1 = {userId1} AND c.user2 = {userId2}) 
                                    OR (c.user1 = {userId2} AND c.user2 = {userId1})
                                ORDER BY last_message_read ASC, last_message_time DESC",
                Connection = con
            };


            con.Open();

            try
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        return Chat.Parse(reader);
                    }
                }
            }
            finally
            {
                con.Close();
            }

            return null;
        }

        public List<Chat> GetUserChatList(int userId)
        {
            var con = new SqlConnection(Config.ConnectionString);

            var cmd = new SqlCommand
            {
                CommandText = $@"SELECT
                                c.*,
                                (SELECT COUNT(*)
                                    FROM message m
                                WHERE m.chat_id = c.id AND m.[read] = 0 AND m.sender_id != {userId}) AS unread_count,
                                m.sender_id AS last_message_sender_id,
                                m.text      AS last_message_text,
                                m.time      AS last_message_time,
                                m.[read]    AS last_message_read
                                FROM chat_info c
                                     LEFT JOIN message m ON c.last_message_id = m.id
                                WHERE (c.user1 = {userId} OR  c.user2 = {userId}) AND message_count > 0
                                ORDER BY last_message_read ASC, last_message_time DESC",
                Connection = con
            };

            con.Open();

            var chats = new List<Chat>();

            try
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        chats.Add(Chat.Parse(reader));
                    }
                }

                return chats;
            }
            finally
            {
                con.Close();
            }
        }

        public void SetMessagesAsRead(int userId, int chatId)
        {
            var con = new SqlConnection(Config.ConnectionString);

            var cmd = new SqlCommand
            {
                CommandText = $@"UPDATE message
                                SET [read]=1
                                WHERE chat_id = {chatId} AND sender_id != {userId}",
                Connection = con
            };

            con.Open();

            try
            {
                cmd.ExecuteNonQuery();
            }
            finally
            {
                con.Close();
            }
        }

        public List<Message> GetMessagesByChatId(int chatId)
        {
            var con = new SqlConnection(Config.ConnectionString);

            var cmd = new SqlCommand
            {
                CommandText = $@"SELECT * FROM message 
                                WHERE chat_id = {chatId}",
                Connection = con
            };

            con.Open();

            var messages = new List<Message>();

            try
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        messages.Add(Message.Parse(reader));
                    }
                }

                return messages;
            }
            finally
            {
                con.Close();
            }
        }
    }
}