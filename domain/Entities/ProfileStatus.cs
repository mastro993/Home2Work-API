using System;
using System.Collections.Generic;
using System.Text;

namespace domain.Entities
{
    public class ProfileStatus
    {
        public string Status { get; set; }
        public DateTime Date { get; set; }
        public bool Hidden { get; set; }
    }
}