using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using YouthActionDotNet.Data;
using YouthActionDotNet.Models;

namespace YouthActionDotNet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceCenterController : ControllerBase
    {
        private readonly DBContext _context;

        public ServiceCenterController(DBContext context)
        {
            _context = context;
        }

        // GET: api/ServiceCenter
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ServiceCenter>>> GetServiceCenters()
        {
            return await _context.ServiceCenters.ToListAsync();
        }

        // GET: api/ServiceCenter/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceCenter>> GetServiceCenter(int id)
        {
            var serviceCenter = await _context.ServiceCenters.FindAsync(id);

            if (serviceCenter == null)
            {
                return NotFound();
            }

            return serviceCenter;
        }

        [HttpGet("All")]
        public async Task<ActionResult<String>> GetAllServiceCenters()
        {
            var serviceCenters = await _context.ServiceCenters.ToListAsync();
            return JsonConvert.SerializeObject(new { success = true, message = "Service Centers Retrieved", data = serviceCenters });
        }

        [HttpPost("Create")]
        public async Task<ActionResult<String>> PostCenter(ServiceCenter serviceCenter)
        {
            _context.ServiceCenters.Add(serviceCenter);
            await _context.SaveChangesAsync();

            return JsonConvert.SerializeObject(new { success = true, message = "Service Center Created", data = serviceCenter });
        }

        // PUT: api/ServiceCenter/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutServiceCenter(int id, ServiceCenter serviceCenter)
        {
            if (id != serviceCenter.id)
            {
                return BadRequest();
            }

            _context.Entry(serviceCenter).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ServiceCenterExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/ServiceCenter
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ServiceCenter>> PostServiceCenter(ServiceCenter serviceCenter)
        {
            _context.ServiceCenters.Add(serviceCenter);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetServiceCenter", new { id = serviceCenter.id }, serviceCenter);
        }

        // DELETE: api/ServiceCenter/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteServiceCenter(int id)
        {
            var serviceCenter = await _context.ServiceCenters.FindAsync(id);
            if (serviceCenter == null)
            {
                return NotFound();
            }

            _context.ServiceCenters.Remove(serviceCenter);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/User/Settings/
        [HttpGet("Settings")]
        public string getServiceCenterSettings()
        {
            Settings settings = new Settings();
            settings.ColumnSettings = new Dictionary<string, ColumnHeader>();
            settings.FieldSettings = new Dictionary<string, InputType>();

            settings.ColumnSettings.Add("id", new ColumnHeader { displayHeader = "ID" });
            settings.ColumnSettings.Add("ServiceCenterName", new ColumnHeader { displayHeader = "Service Center Name" });
            settings.ColumnSettings.Add("ServiceCenterAddress", new ColumnHeader { displayHeader = "Service Center Address" });
            settings.ColumnSettings.Add("RegionalDirectorId", new ColumnHeader { displayHeader = "Regional Director ID" });

            settings.FieldSettings.Add("id", new InputType { type = "number", displayLabel = "ID", editable = false, primaryKey = true });
            settings.FieldSettings.Add("ServiceCenterName", new InputType { type = "text", displayLabel = "Service Center Name", editable = true, primaryKey = false });
            settings.FieldSettings.Add("ServiceCenterAddress", new InputType { type = "text", displayLabel = "Service Center Address", editable = true, primaryKey = false });

            var allEmployees = _context.Employee.Where(u => u.EmployeeRole == "Regional Director").ToList();
            settings.FieldSettings.Add("RegionalDirectorId", new DropdownInputType { 
                type = "dropdown", 
                displayLabel = "Regional Director", 
                editable = true, 
                primaryKey = false, 
                options = allEmployees.Select(
                    e => new DropdownOption { 
                        value = e.UserId, 
                        label = e.username }).ToList() 
                     });

            return JsonConvert.SerializeObject(settings);
        }

        private bool ServiceCenterExists(int id)
        {
            return _context.ServiceCenters.Any(e => e.id == id);
        }
    }
}
