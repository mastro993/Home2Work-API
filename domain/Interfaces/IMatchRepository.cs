using System.Collections.Generic;

namespace domain.Interfaces
{
    public interface IMatchRepository
    {
        void Insert(Entities.Match match);

        List<Entities.Match> GetAll();

        List<Entities.Match> GetByUserId(long userId);

        Entities.Match GetById(int matchId);

        Entities.Match EditMatch(Entities.Match match);
    }
}