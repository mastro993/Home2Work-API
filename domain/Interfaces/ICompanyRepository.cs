using System.Collections.Generic;
using domain.Entities;

namespace domain.Interfaces
{
    public interface ICompanyRepository
    {

        List<Company> GetAll();

        Company GetById(long companyId);

    }
}
