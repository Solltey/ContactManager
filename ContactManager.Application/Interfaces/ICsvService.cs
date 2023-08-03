using ContactManager.Persistence.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContactManager.Application.Interfaces
{
    public interface ICsvService
    {
        Task<IEnumerable<Contact>> ReadContactsFromCsv(IFormFile file);
    }
}
