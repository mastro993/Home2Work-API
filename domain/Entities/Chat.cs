﻿using System;

namespace domain.Entities
{
    [Serializable]
    public class Chat
    {
        public long Id { get; set; }
        public Author User { get; set; }
        public Message LastMessage { get; set; }
        public int UnreadCount { get; set; }
    }
}