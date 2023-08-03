using ContactManager.Application.Interfaces;
using ContactManager.Persistence.Entities;
using ContactManager.Persistence.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Dropbox.Api.TeamLog.LoginMethod;

namespace ContactManager.Application.Services
{
    public class ContactService : IContactService
    {
        private readonly IContactRepository _contactRepository;

        public ContactService(IContactRepository contactRepository)
        {
            _contactRepository = contactRepository;
        }

        public async Task<Contact?> CreateAsync(Contact contact)
        {
            var result = await _contactRepository.AddAsync(contact);

            return result;
        }

        public async Task DeleteAsync(int id)
        {
            await _contactRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<Contact>> GetAllAsync()
        {
            return await _contactRepository.GetAllAsync();
        }

        public async Task<Contact?> GetByIdAsync(int id)
        {
            return await _contactRepository.GetByIdAsync(id);
        }

        public IEnumerable<Contact> GetByUserId(string userId)
        {
            return _contactRepository.GetByUserIdAsync(userId);
        }

        public async Task UpdateUrlAsync(Contact contact)
        {
            await _contactRepository.UpdateAsync(contact);
        }
    }
}
