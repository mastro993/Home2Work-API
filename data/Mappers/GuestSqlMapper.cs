using System.Data.SqlClient;
using data.Common;
using data.Common.Utils;
using data.Repositories;
using domain.Entities;
using domain.Interfaces;

namespace data.Mappers
{
    class GuestSqlMapper: Mapper<SqlDataReader, Guest>
    {

        private readonly IUserRepository _userRepo = new UserRepository();


        public override Guest MapFrom(SqlDataReader @from)
        {
            var shareId =@from["share_id"].ToLong();
            var guestId = @from["user_id"].ToLong();
            var startLat = @from["start_latitude"].ToDouble();
            var startLng = @from["start_longitude"].ToDouble();
            var endLat = @from["end_latitude"].ToDouble();
            var endLng = @from["end_longitude"].ToDouble();
            var status = @from["status"].ToInt();
            var distance =@from["distance"].ToInt();

            return new Guest()
            {
                ShareId = shareId,
                User = _userRepo.GetById(guestId),
                StartLat = startLat,
                StartLng = startLng,
                EndLat = endLat,
                EndLng = endLng,
                Status = status,
                Distance = distance,
            };
        }
    }
}
