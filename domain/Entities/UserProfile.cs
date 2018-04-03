using System;
using System.Collections.Generic;

namespace domain.Entities
{
    [Serializable]
    public class UserProfile
    {
        public UserExp Exp { get; set; }
        public UserStats Stats { get; set; }
        public Dictionary<int, SharingActivity> Activity { get; set; }
        public DateTime Regdate { get; set; }
    }
}