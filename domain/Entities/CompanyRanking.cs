using System;
using System.Collections.Generic;
using System.Text;

namespace domain.Entities
{
    public class CompanyRanking
    {
        public int Position { get; set; }
        public long CompanyId { get; set; }
        public string CompanyName { get; set; }
        public double Float { get; set; }
    }
}
