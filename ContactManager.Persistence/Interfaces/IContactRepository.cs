using ContactManager.Persistence.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContactManager.Persistence.Interfaces
{
    public interface IContactRepository
    {
        Task<IEnumerable<Contact>> GetAllAsync();
        Task<Contact?> GetByIdAsync(int id);
        Task<Contact?> AddAsync(Contact url);
        Task UpdateAsync(Contact url);
        Task DeleteAsync(Contact url);
        IEnumerable<Contact> GetByUserIdAsync(string userId);
    }
}
