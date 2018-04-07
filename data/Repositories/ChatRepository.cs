using System.Collections.Generic;
using System.Data.SqlClient;
using data.Common;
using data.Common.Utils;
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

        public long InsertMessage(long userId, long chatId, string text)
        {
            var con = new SqlConnection(Config.ConnectionString);

            var cmd = new SqlCommand
            {
                CommandText = $"insert_message_to_chat {chatId}, {userId}, '{text}'",
                Connection = con
            };

            try
            {
                con.Open();
                return cmd.ExecuteScalar().ToLong();
            }
            catch (SqlException e)
            {
                System.Console.WriteLine(e);
                return 0;
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
                CommandText = $"get_chat_message {messageId}",
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
                CommandText = $"insert_new_chat {userId}, {recipientUserId}",
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
                CommandText = $@"get_user_chat {userId}, {chatId}",
                Connection = con
            };


            try
            {
                con.Open();
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
                CommandText = $"get_users_chat {userId}, {recipientId}",
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
                CommandText = $"get_user_chat_list {userId}",
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
                CommandText = $@"set_messages_as_read {chatId}, {userId}",
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

        public List<Message> GetMessagesByChatId(long userId, long chatId)
        {
            var con = new SqlConnection(Config.ConnectionString);

            var cmd = new SqlCommand
            {
                CommandText = $"get_chat_messages {chatId}, {userId}",
                Connection = con
            };


            var messages = new List<Message>();

            try
            {
                con.Open();
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