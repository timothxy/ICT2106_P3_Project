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
    public class ServiceCenterController : ControllerBase, IUserInterfaceCRUD<ServiceCenter>
    {
        private readonly DBContext _context;
        JsonSerializerSettings settings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };
        public ServiceCenterController(DBContext context)
        {
            _context = context;
        }
        // GET: api/ServiceCenter
        [HttpPost("Create")]
        public async Task<ActionResult<string>> Create(ServiceCenter template)
        {
            template.ServiceCenterId = Guid.NewGuid().ToString();
            _context.ServiceCenters.Add(template);
            await _context.SaveChangesAsync();
            return JsonConvert.SerializeObject(new { success = true, message = "Service Center Created", data = template });
        }
        [HttpGet("{$id}")]
        public async Task<ActionResult<string>> Get(string id)
        {
            var serviceCenter = await _context.ServiceCenters.FindAsync(id);
            if (serviceCenter == null)
            {
                return JsonConvert.SerializeObject(new { success = false, message = "Service Center Not Found" });
            }
            return JsonConvert.SerializeObject(new { success = true, data = serviceCenter, message = "Service Center Successfully Retrieved" }, settings);
        }

        [HttpGet("All")]
        public async Task<ActionResult<string>> All()
        {
            var serviceCenter = await _context.ServiceCenters.ToListAsync();
            return JsonConvert.SerializeObject(new { success = true, data = serviceCenter, message = "Service Center Successfully Retrieved" });
        }
        [HttpPut("{$id}")]
        public async Task<ActionResult<string>> Update(string id, ServiceCenter template)
        {
            if (id != template.ServiceCenterId)
            {
                return JsonConvert.SerializeObject(new { success = false, message = "Service Center Not Found" });
            }
            _context.Entry(template).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
                return JsonConvert.SerializeObject(new { success = true, message = "Service Center Successfully Updated", data = template },settings);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Exists(id))
                {
                    return JsonConvert.SerializeObject(new { success = false, message = "Service Center Not Found" });
                }
                else
                {
                    throw;
                }
            }
        }
        [HttpPut("UpdateAndFetchAll/{$id}")]
        public async Task<ActionResult<string>> UpdateAndFetchAll(string id, ServiceCenter template)
        {
            if (id != template.ServiceCenterId)
            {
                return JsonConvert.SerializeObject(new { success = false, message = "Service Center Not Found" });
            }
            _context.Entry(template).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
                return await All();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Exists(id))
                {
                    return JsonConvert.SerializeObject(new { success = false, message = "Service Center Not Found" });
                }
                else
                {
                    throw;
                }
            }
        }
        [HttpDelete("{$id}")]
        public async Task<ActionResult<string>> Delete(string id)
        {
            var serviceCenter = await _context.ServiceCenters.FindAsync(id);
            if (serviceCenter == null)
            {
                return JsonConvert.SerializeObject(new { success = false, message = "Service Center Not Found" });
            }
            _context.ServiceCenters.Remove(serviceCenter);
            await _context.SaveChangesAsync();
            return JsonConvert.SerializeObject(new { success = true, message = "Service Center Successfully Deleted" });
        }
        [HttpGet("Settings")]
        public string Settings()
        {
            Settings settings = new Settings();
            settings.ColumnSettings = new Dictionary<string, ColumnHeader>();
            settings.FieldSettings = new Dictionary<string, InputType>();

            settings.ColumnSettings.Add("id", new ColumnHeader { displayHeader = "ID" });
            settings.ColumnSettings.Add("ServiceCenterName", new ColumnHeader { displayHeader = "Service Center Name" });
            settings.ColumnSettings.Add("ServiceCenterAddress", new ColumnHeader { displayHeader = "Service Center Address" });
            settings.ColumnSettings.Add("RegionalDirectorName", new ColumnHeader { displayHeader = "Regional Director Name" });

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

            return JsonConvert.SerializeObject(new { success = true, data = settings, message = "Settings Successfully Retrieved" });
        }

        public bool Exists(string id)
        {
            return _context.ServiceCenters.Any(e => e.ServiceCenterId == id);
        }
    }
}
