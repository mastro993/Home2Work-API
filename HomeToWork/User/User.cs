using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeToWork.Common;
using HomeToWork.Company;
using HomeToWork.Location;

namespace HomeToWork.User
{
    [Serializable]
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public LatLng HomeLatLng { get; set; }
        public Address Address { get; set; }
        public Company.Company Company { get; set; }
        public DateTime Regdate { get; set; }

        public static User Parse(SqlDataReader reader)
        {
            var companyDao = new CompanyDao();

            var userId = (int) reader["id"];
            var email = reader["email"].ToString();
            var name = reader["name"].ToString();
            var surname = reader["surname"].ToString();
            var companyId = (int) reader["company_id"];
            var cap = reader["cap"].ToString();
            var city = reader["city"].ToString();
            var addressLine = reader["address"].ToString();
            var regdate = DateTime.Parse(reader["regdate"].ToString());
            var location = LatLng.Parse(reader["location"].ToString());

            var company = companyDao.GetById(companyId);

            return new User()
            {
                Id = userId,
                Email = email,
                Name = name,
                Surname = surname,
                HomeLatLng = location,
                Address = new Address()
                {
                    Cap = cap,
                    City = city,
                    AddressLine = addressLine,
                },
                Company = company,
                Regdate = regdate
            };
        }
    }
}