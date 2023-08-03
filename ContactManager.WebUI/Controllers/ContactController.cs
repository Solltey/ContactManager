using ContactManager.Application.Interfaces;
using ContactManager.Persistence.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ContactManager.WebUI.Controllers
{
    [Authorize]
    public class ContactController : Controller
    {
        private readonly IContactService _contactService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICsvService _csvService;

        public ContactController(IContactService contactService,
            UserManager<ApplicationUser> userManager,
            ICsvService csvService)
        {
            _contactService = contactService;
            _userManager = userManager;
            _csvService = csvService;
        }

        public async Task<IActionResult> Table()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            var contacts = _contactService.GetByUserId(user.Id);

            return View(contacts);
        }

        [HttpPost]
        public async Task<IActionResult> Add(IFormFile file)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            var contacts = await _csvService.ReadContactsFromCsv(file);

            foreach (var contact in contacts)
            {
                await _contactService.CreateAsync(contact);
            }

            return Ok();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Contact>>> GetAll()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            var contacts = _contactService.GetByUserId(user.Id);

            return Ok(contacts);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Contact>> GetById(int id)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            var contacts = await _contactService.GetByIdAsync(id);
            if (contacts == null)
                return NotFound();

            return Ok(contacts);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteById(int id)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            var contact = await _contactService.GetByIdAsync(id);
            if (contact == null)
                return NotFound();

            await _contactService.DeleteAsync(contact.Id);

            return Ok();
        }

        [HttpPatch]
        public async Task<IActionResult> Update([FromBody] Contact contact)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            await _contactService.UpdateUrlAsync(contact);

            return Ok();
        }
    }
}
