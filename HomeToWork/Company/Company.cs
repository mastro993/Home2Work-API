using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeToWork.Common;
using HomeToWork.Location;

namespace HomeToWork.Company
{
    [Serializable]
    public class Company
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public LatLng LatLng { get; set; }
        public Address Address { get; set; }

        public static Company Parse(SqlDataReader reader)
        {
            var id = (int) reader["id"];
            var name = reader["name"].ToString();
            var latLng = LatLng.Parse(reader["location"].ToString());
            var cap = reader["cap"].ToString();
            var city = reader["city"].ToString();
            var addressLine = reader["address"].ToString();

            return new Company()
            {
                Id = id,
                Name = name,
                LatLng = latLng,
                Address = new Address()
                {
                    Cap = cap,
                    City = city,
                    AddressLine = addressLine,
                }
            };
        }
    }
}