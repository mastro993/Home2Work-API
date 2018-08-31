using System;

namespace domain.Entities
{
    [Serializable]
    public class User
    {
        public long Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }

        public Address Address { get; set; }

        public Company Company { get; set; }
        public Address JobAddress { get; set; }

        public TimeSpan? JobStartTime { get; set; }
        public TimeSpan? JobEndTime { get; set; }

        public DateTime? Birthday { get; set; }
        public DateTime Regdate { get; set; }

        public String FirebaseToken { get; set; }

        public override string ToString()
        {
            return $"{Name} {Surname}";
        }
    }
}