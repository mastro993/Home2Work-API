using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeToWork.Common;

namespace HomeToWork.User
{
    [Serializable]
    public class UserProfile
    {
        public UserExp Exp { get; set; }
        public UserStats Stats { get; set; }
        public List<MonthlyActivity> Activity { get; set; }
        public DateTime Regdate { get; set; }
    }
}