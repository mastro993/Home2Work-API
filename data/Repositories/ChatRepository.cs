using System.Collections.Generic;
using System.Data.SqlClient;
using data.Common;
using data.Database;
using data.Mappers;
using domain.Entities;
using domain.Interfaces;

namespace data.Repositories
{
    public class ChatRepository : IChatRepository
    {
        private readonly Mapper<SqlDataReader, Chat> _mapper = new ChatSqlMapper();
        private readonly Mapper<SqlDataReader, Message> _messageMapper = new MessageSqlMapper();

        public int InsertMessage(long userId, long chatId, string text)
        {
            var con = new SqlConnection(Config.ConnectionString);

            var cmd = new SqlCommand
            {
                CommandText = $@"INSERT INTO message (chat_id, sender_id, text) VALUES ({chatId}, {userId}, '{text}')",
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

        public Message getMessageById(long messageId)
        {
            var con = new SqlConnection(Config.ConnectionString);

            var cmd = new SqlCommand
            {
                CommandText = $@"SELECT * FROM message WHERE id = {messageId}",
                Connection = con
            };

            try
            {
                con.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        return _messageMapper.MapFrom(reader);
                    }
                }
            }
            finally
            {
                con.Close();
            }

            return null;
        }

        public int NewChat(long userId, long recipientUserId)
        {
            var con = new SqlConnection(Config.ConnectionString);

            var cmd = new SqlCommand
            {
                CommandText = $@"INSERT INTO chat(user1, user2) VALUES({userId}, {recipientUserId})",
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

        public Chat GetByChatId(long userId, long chatId)
        {
            var con = new SqlConnection(Config.ConnectionString);

            var cmd = new SqlCommand
            {
                CommandText = $@"SELECT
  c.id,
  (
    SELECT CASE
           WHEN c.user1 = {userId}
             THEN c.user2
           WHEN c.user2 = {userId}
             THEN c.user1
           END
  )                                                              AS userId,
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
                        return _mapper.MapFrom(reader);
                    }
                }
            }
            finally
            {
                con.Close();
            }

            return null;
        }

        public Chat GetByUserIds(long userId, long recipientId)
        {
            var con = new SqlConnection(Config.ConnectionString);

            var cmd = new SqlCommand
            {
                CommandText = $@"SELECT
  c.id,
  (
    SELECT CASE
           WHEN c.user1 = {userId}
             THEN c.user2
           WHEN c.user2 = {userId}
             THEN c.user1
           END
  )                                                              AS userId,
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
                                WHERE (c.user1 = {userId} AND c.user2 = {recipientId}) 
                                    OR (c.user1 = {recipientId} AND c.user2 = {userId})
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
                        return _mapper.MapFrom(reader);
                    }
                }
            }
            finally
            {
                con.Close();
            }

            return null;
        }

        public List<Chat> GetUserChatList(long userId)
        {
            var con = new SqlConnection(Config.ConnectionString);

            var cmd = new SqlCommand
            {
                CommandText = $@"SELECT
  c.id,
  (
    SELECT CASE
           WHEN c.user1 = {userId}
             THEN c.user2
           WHEN c.user2 = {userId}
             THEN c.user1
           END
  )                                                              AS userId,
  c.time,
  c.last_message_id,
  c.message_count,
  (SELECT COUNT(*)
   FROM message m
   WHERE m.chat_id = c.id AND m.[read] = 0 AND m.sender_id != {userId}) AS unread_count,
  m.sender_id                                                    AS last_message_sender_id,
  m.text                                                         AS last_message_text,
  m.time                                                         AS last_message_time,
  m.[read]                                                       AS last_message_read
FROM chat_info c
  LEFT JOIN message m ON c.last_message_id = m.id
WHERE (c.user1 = {userId} OR c.user2 = {userId}) AND message_count > 0
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
                        chats.Add(_mapper.MapFrom(reader));
                    }
                }

                return chats;
            }
            finally
            {
                con.Close();
            }
        }

        public void SetMessagesAsRead(long userId, long chatId)
        {
            var con = new SqlConnection(Config.ConnectionString);

            var cmd = new SqlCommand
            {
                CommandText = $@"UPDATE message SET [read]=1 WHERE chat_id = {chatId} AND sender_id != {userId}",
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

        public List<Message> GetMessagesByChatId(long chatId)
        {
            var con = new SqlConnection(Config.ConnectionString);

            var cmd = new SqlCommand
            {
                CommandText = $@"SELECT * WHERE chat_id = {chatId}",
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
                        messages.Add(_messageMapper.MapFrom(reader));
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