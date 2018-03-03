using System;
using System.CodeDom.Compiler;
using System.Data.SqlClient;
using HomeToWork.Common;
using HomeToWork.Company;

namespace HomeToWork.Auth
{
    public class AuthUser : User.User
    {
        public string AccessToken { get; set; }
        public bool Configured { get; set; }

        public static AuthUser Parse(SqlDataReader reader)
        {
            var companyDao = new CompanyDao();

            var userId = (int) reader["id"];
            var email = reader["email"].ToString();
            var accessToken = reader["access_token"].ToString();
            var name = reader["name"].ToString();
            var surname = reader["surname"].ToString();
            var company = companyDao.GetById((int) reader["company_id"]);

            var cap = reader["cap"].ToString();
            var city = reader["city"].ToString();
            var addressLine = reader["address"].ToString();

            var regdate = DateTime.Parse(reader["regdate"].ToString());
            var homeLatLng = LatLng.Parse(reader["location"].ToString());
            var configured = (bool) reader["configured"];

            return new AuthUser()
            {
                Id = userId,
                Email = email,
                AccessToken = accessToken,
                Name = name,
                Surname = surname,
                HomeLatLng = homeLatLng,
                Address = new Address()
                {
                    Cap = cap,
                    City = city,
                    AddressLine = addressLine,
                },
                Company = company,
                Regdate = regdate,
                Configured = configured
            };
        }
    }
}