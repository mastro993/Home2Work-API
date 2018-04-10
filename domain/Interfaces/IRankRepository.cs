using System;
using System.Collections.Generic;
using System.Text;
using domain.Entities;

namespace domain.Interfaces
{
    public interface IRankRepository
    {
        Ranks GetRanks();
    }
}