using System;
using System.Collections.Generic;
using System.Text;

namespace domain.Entities
{
    public class UserRanking
    {
        public int Position { get; set; }
        public long UserId { get; set; }
        public string UserName { get; set; }
        public long CompanyId { get; set; }
        public string CompanyName { get; set; }
        public long Amount { get; set; }
    }
}
