using ContactManager.Application.Interfaces;
using ContactManager.Application.Models;
using ContactManager.Persistence.Entities;
using CsvHelper;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContactManager.Application.Services
{
    public class CsvService : ICsvService
    {
        public async Task<IEnumerable<Contact>> ReadContactsFromCsv(IFormFile file)
        {
            List<Contact> contacts = new List<Contact>();

            if (file == null || file.Length <= 0)
            {
                return contacts;
            }

            // Read the CSV content using CsvHelper
            using (var reader = new StreamReader(file.OpenReadStream()))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                // Map CSV headers to Contact properties
                csv.Context.RegisterClassMap<ContactCsvMap>();

                // Read all records and add them to the contacts list
                while (csv.Read())
                {
                    contacts.Add(csv.GetRecord<Contact>());
                }
            }

            return contacts;
        }
    }
}
