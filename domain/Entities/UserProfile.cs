using System;
using System.Collections.Generic;

namespace domain.Entities
{
    [Serializable]
    public class UserProfile
    {
        public ProfileStatus Status { get; set; }
        public UserKarma Karma { get; set; }
        public UserStats Stats { get; set; }
        public Dictionary<string, SharingActivity> Activity { get; set; }
        public DateTime Regdate { get; set; }
    }
}