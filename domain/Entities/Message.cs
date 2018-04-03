﻿using System;

namespace domain.Entities
{
    [Serializable]
    public class Message
    {
        public long Id { get; set; }
        public Author Sender { get; set; }
        public string Text { get; set; }
        public DateTime Time { get; set; }
        public bool Read { get; set; }
    }
}