using System;
using System.Data.SqlClient;
using data.Common;
using data.Common.Utils;
using data.Repositories;
using domain.Entities;
using domain.Interfaces;

namespace data.Mappers
{
    class ShareSqlMapper: Mapper<SqlDataReader, Share>
    {

        private readonly IUserRepository _userRepo = new UserRepository();

        public override Share MapFrom(SqlDataReader @from)
        {

            var shareId = @from["id"].ToLong();
            var hostId = @from["host_id"].ToLong();
            var status = @from["status"].ToInt();
            var time = LocalDateTime.Parse(@from["time"].ToString());

            return new Share()
            {
                Id = shareId,
                Host = _userRepo.GetById(hostId),
                Status = (ShareStatus)status,
                Time = time
            };
        }
    }
}
