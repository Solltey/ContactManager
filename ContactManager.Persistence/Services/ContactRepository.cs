using ContactManager.Persistence.Context;
using ContactManager.Persistence.Entities;
using ContactManager.Persistence.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContactManager.Persistence.Services
{
    public class ContactRepository : IContactRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public ContactRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public async Task<Contact?> AddAsync(Contact contact)
        {
            await _dbContext.Contacts.AddAsync(contact);
            await _dbContext.SaveChangesAsync();

            return contact;
        }

        public async Task DeleteAsync(Contact contact)
        {
            _dbContext.Contacts.Remove(contact);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Contact>> GetAllAsync()
        {
            return await _dbContext.Contacts.ToListAsync();
        }

        public async Task<Contact?> GetByIdAsync(int id)
        {
            return await _dbContext.Contacts.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task UpdateAsync(Contact contact)
        {
            _dbContext.Entry(contact).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        public IEnumerable<Contact> GetByUserIdAsync(string userId)
        {
            return _dbContext.Contacts.Where(u => u.UserId == userId).ToList();
        }
    }
}
