using System;
using System.Data.SqlClient;
using data.Common;
using data.Common.Utils;
using data.Repositories;
using domain.Entities;
using domain.Interfaces;

namespace data.Mappers
{
    class ShareSqlMapper : Mapper<SqlDataReader, Share>
    {
        private readonly IUserRepository _userRepo = new UserRepository();

        public override Share MapFrom(SqlDataReader @from)
        {
            var shareId = @from["id"].ToLong();
            var hostId = @from["host_id"].ToLong();
            var status = @from["status"].ToInt();
            var startLat = @from["start_latitude"].ToDouble();
            var startLng = @from["start_longitude"].ToDouble();
            var endLat = @from["end_latitude"].ToDouble();
            var endLng = @from["end_longitude"].ToDouble();
            var distance = @from["distance"].ToInt();
            var time = LocalDateTime.Parse(@from["time"].ToString());

            return new Share()
            {
                Id = shareId,
                Host = _userRepo.GetById(hostId),
                Status = (ShareStatus) status,
                Time = time,
                StartLng = startLng,
                StartLat = startLat,
                EndLat = endLat,
                EndLng = endLng,
                SharedDistance = distance
            };
        }
    }
}