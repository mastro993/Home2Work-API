using System.Data.SqlClient;
using data.Common;
using data.Common.Utils;
using domain.Entities;

namespace data.Mappers
{
    class CompanySqlMapper : Mapper<SqlDataReader, Company>
    {
        public override Company MapFrom(SqlDataReader @from)
        {
            var id = @from["id"].ToLong();
            var name = @from["name"].ToString();
            var domain = @from["domain"].ToString();

            var lat = @from["latitude"].ToDouble();
            var lng = @from["longitude"].ToDouble();
            var street = @from["street"].ToString();
            var region = @from["region"].ToString();
            var cap = @from["cap"].ToString();
            var city = @from["city"].ToString();
            var district = @from["district"].ToString();

            return new Company()
            {
                Id = id,
                Name = name,
                Address = new Address()
                {
                    Latitude = lat,
                    Longitude = lng,
                    Street = street,
                    Region = region,
                    PostalCode = cap,
                    City = city,
                    District = district
                },
                Domain = domain
            };
        }
    }
}
