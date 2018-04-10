using System;
using System.Collections.Generic;
using System.Text;

namespace domain.Entities
{
    public class Ranks
    {
        public Rank GlobalRanks { get; set; }
        public Rank GlobalMonthlyRanks { get; set; }
        public Rank CompanyRanks { get; set; }
        public Rank CompanyMonthlyRanks { get; set; }
    }
}