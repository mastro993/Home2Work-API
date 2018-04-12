using System.Collections.Generic;
using System.Data.SqlClient;
using data.Common;
using data.Database;
using data.Mappers;
using domain.Entities;
using domain.Interfaces;

namespace data.Repositories
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly Mapper<SqlDataReader, Company> _mapper = new CompanySqlMapper();
        

        public List<Company> GetAll()
        {
            var con = new SqlConnection(Config.ConnectionString);
            var cmd = new SqlCommand
            {
                CommandText = "SELECT * FROM company",
                Connection = con
            };

            con.Open();

            var companies = new List<Company>();

            try
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        companies.Add(_mapper.MapFrom(reader));
                    }
                }
            }
            finally
            {
                con.Close();
            }


            return companies;
        }

        public Company GetById(long companyId)
        {
            var con = new SqlConnection(Config.ConnectionString);
            var cmd = new SqlCommand
            {
                CommandText = $@"SELECT * FROM company WHERE id = {companyId}",
                Connection = con
            };

            con.Open();

            try
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        return _mapper.MapFrom(reader);
                    }
                }
            }
            finally
            {
                con.Close();
            }


            return null;
        }

    }
}