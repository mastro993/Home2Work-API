using System.Collections.Generic;
using domain.Entities;

namespace domain.Interfaces
{
    public interface IChatRepository
    {
        long InsertMessage(long userId, long chatId, string text);

        Message getMessageById(long messageId);

        int NewChat(long userId, long recipientUserId);

        Chat GetByChatId(long userId, long chatId);

        Chat GetByUserIds(long userId1, long userId2);

        List<Chat> GetUserChatList(long userId);

        void SetMessagesAsRead(long userId, long chatId);

        List<Message> GetMessagesByChatId(long usaerId, long chatId);
    }
}