using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using data.Common;
using data.Common.Utils;
using domain.Entities;

namespace data.Mappers
{
    class ProfileStatusSqlMapper : Mapper<SqlDataReader, ProfileStatus>
    {
        public override ProfileStatus MapFrom(SqlDataReader @from)
        {
            var status = @from["status"].ToString();
            var date = LocalDateTime.Parse(@from["date"].ToString());
            var hidden = (bool)@from["hidden"];

            return new ProfileStatus()
            {
                Status = status,
                Date = date,
                Hidden = hidden
            };
        }
    }
}