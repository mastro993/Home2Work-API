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

            var lat = @from["home_latitude"].ToDouble();
            var lng = @from["home_longitude"].ToDouble();
            var street = @from["home_address"].ToString();
            var region = @from["home_region"].ToString();
            var city = @from["home_city"].ToString();
            var cap = @from["home_cap"].ToString();
            var district = @from["home_district"].ToString();

            var jobLat = @from["job_latitude"].ToDouble();
            var jobLng = @from["job_longitude"].ToDouble();
            var jobStreet = @from["job_address"].ToString();
            var jobRegion = @from["job_region"].ToString();
            var jobCity = @from["job_city"].ToString();
            var jobCap = @from["job_cap"].ToString();
            var jobDistrict = @from["job_district"].ToString();


            var jobStartString = @from["job_start_time"].ToString();
            var jobEndString = @from["job_end_time"].ToString();

            TimeSpan? jobStartTime;
            TimeSpan? jobEndtime;

            if (jobStartString.Equals(""))
            {
                jobStartTime = null;
            }
            else
            {
                jobStartTime = TimeSpan.Parse(jobStartString);
            }

            if (jobEndString.Equals(""))
            {
                jobEndtime = null;
            }
            else
            {
                jobEndtime = TimeSpan.Parse(jobEndString);
            }


            var birthdayString = @from["birthday"].ToString();
            DateTime? birthday;

            if (birthdayString.Equals(""))
            {
                birthday = null;
            }
            else
                birthday = LocalDateTime.Parse(birthdayString);


            var regdate = LocalDateTime.Parse(@from["registration_date"].ToString());

            var firebaseToken = @from["firebase_token"].ToString();

            var company = _companyRepo.GetById(companyId);

            return new User()
            {
                Id = userId,
                Email = email,
                Name = name,
                Surname = surname,
                Address = new Address()
                {
                    Latitude = lat,
                    Longitude = lng,
                    Street = street,
                    Region = region,
                    City = city,
                    PostalCode = cap,
                    District = district
                },
                JobAddress = new Address()
                {
                    Latitude = jobLat,
                    Longitude = jobLng,
                    Street = jobStreet,
                    Region = jobRegion,
                    City = jobCity,
                    PostalCode = jobCap,
                    District = jobDistrict
                },
                JobStartTime = jobStartTime,
                JobEndTime = jobEndtime,
                Company = company,
                Regdate = regdate,
                Birthday = birthday,
                FirebaseToken = firebaseToken
            };
        }
    }
}