using System;

namespace domain.Entities
{
    [Serializable]
    public class Author
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
    }
}