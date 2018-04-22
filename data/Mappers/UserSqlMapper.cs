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
            var userId = @from["id"].ToLong();
            var email = @from["email"].ToString();
            var name = @from["name"].ToString();
            var surname = @from["surname"].ToString();
            var companyId = @from["company_id"].ToLong();
            var homeLat = @from["home_latitude"].ToDouble();
            var homeLng = @from["home_longitude"].ToDouble();
            var homeStreet = @from["home_address"].ToString();
            var homeRegion = @from["home_region"].ToString();
            var homeCity = @from["home_city"].ToString();
            var homeCap = @from["home_cap"].ToString();
            var homeDistrict = @from["home_district"].ToString();
            var regdate = LocalDateTime.Parse(@from["registration_date"].ToString());

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
                    Street = homeStreet,
                    Region = homeRegion,
                    City = homeCity,
                    PostalCode = homeCap,
                    District = homeDistrict
                },
                Company = company,
                Regdate = regdate
            };
        }
    }
}