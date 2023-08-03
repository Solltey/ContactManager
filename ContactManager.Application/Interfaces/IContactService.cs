using ContactManager.Persistence.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContactManager.Application.Interfaces
{
    public interface IContactService
    {
        IEnumerable<Contact> GetByUserId(string userId);
        Task<Contact?> GetByIdAsync(int Id);
        Task<IEnumerable<Contact>> GetAllAsync();
        Task<Contact?> CreateAsync(Contact contact);
        Task UpdateUrlAsync(Contact contact);
        Task DeleteAsync(int id);
    }
}
