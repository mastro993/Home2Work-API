using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using data.Common;
using data.Common.Utils;
using domain.Entities;

namespace data.Mappers
{
    class UserRankingSqlMapper : Mapper<SqlDataReader, UserRanking>

    {
        public override UserRanking MapFrom(SqlDataReader @from)
        {
            var position = @from["position"].ToInt();
            var userId = @from["user_id"].ToLong();
            var name = @from["name"].ToString();
            var surname = @from["surname"].ToString();
            var companyId = @from["company_id"].ToLong();
            var companyName = @from["company_name"].ToString();
            var value = @from["value"].ToLong();

            return new UserRanking()
            {
                Position = position,
                UserId = userId,
                UserName = name + " " + surname,
                CompanyId = companyId,
                CompanyName = companyName,
                Amount = value
            };
        }
    }
}