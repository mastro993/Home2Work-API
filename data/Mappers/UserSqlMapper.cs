using System;
using System.Data.SqlClient;
using data.Common;
using data.Common.Utils;
using data.Repositories;
using domain.Entities;
using domain.Interfaces;

namespace data.Mappers
{
    class UserSqlMapper : Mapper<SqlDataReader, User>
    {
        private readonly ICompanyRepository _companyRepo = new CompanyRepository();

        public override User MapFrom(SqlDataReader @from)
        {
            var userId = @from["user_id"].ToLong();
            var email = @from["email"].ToString();
            var name = @from["name"].ToString();
            var surname = @from["surname"].ToString();
            var companyId = @from["company_id"].ToLong();
            var homeLat = @from["latitude"].ToDouble();
            var homeLng = @from["longitude"].ToDouble();
            var street = @from["street"].ToString();
            var region = @from["region"].ToString();
            var city = @from["city"].ToString();
            var cap = @from["cap"].ToString();
            var district = @from["district"].ToString();
            var regdate = DateTime.Parse(@from["regdate"].ToString());
            //var accessToken = @from["access_token"].ToString();

            var company = _companyRepo.GetById(companyId);

            return new User()
            {
                Id = userId,
                Email = email,
                Name = name,
                Surname = surname,
                Address = new Address()
                {
                    Latitude = homeLat,
                    Longitude = homeLng,
                    Street = street,
                    Region = region,
                    City = city,
                    PostalCode = cap,
                    District = district
                },
                Company = company,
                Regdate = regdate
            };
        }
    }
}