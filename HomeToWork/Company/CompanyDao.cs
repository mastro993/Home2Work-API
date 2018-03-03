using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeToWork.Database;
using HomeToWork.Location;

namespace HomeToWork.Company
{
    public class CompanyDao
    {
        public int Insert(Company company)
        {
            var con = new SqlConnection(Config.ConnectionString);
            var cmd = new SqlCommand
            {
                CommandText = $@"INSERT INTO 
                                dbo.company (name, location, cap, city, address) 
                                VALUES ('{company.Name}', '{company.LatLng}', '{company.Address.Cap}', 
                                        '{company.Address.City}', '{company.Address.AddressLine}')",
                Connection = con
            };


            con.Open();

            int companyId;

            try
            {
                companyId = cmd.ExecuteNonQuery();
            }
            finally
            {
                con.Close();
            }

            return companyId;
        }

        public List<Company> GetAll()
        {
            var con = new SqlConnection(Config.ConnectionString);
            var cmd = new SqlCommand
            {
                CommandText = @"SELECT * FROM company",
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
                        companies.Add(Company.Parse(reader));
                    }
                }
            }
            finally
            {
                con.Close();
            }


            return companies;
        }

        public Company GetById(int companyId)
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
                        return Company.Parse(reader);
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