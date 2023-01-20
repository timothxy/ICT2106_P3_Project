using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using YouthActionDotNet.DAL;
using YouthActionDotNet.Data;
using YouthActionDotNet.Models;

namespace YouthActionDotNet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceCenterController : ControllerBase, IUserInterfaceCRUD<ServiceCenter>
    {
        private GenericRepository<ServiceCenter> ServiceCenterRepository;
        private GenericRepository<User> UserRepository;
        JsonSerializerSettings settings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };
        public ServiceCenterController(DBContext context)
        {
            ServiceCenterRepository = new GenericRepository<ServiceCenter>(context);
            UserRepository = new GenericRepository<User>(context);
        }
        
        public bool Exists(string id)
        {
            return ServiceCenterRepository.GetByID(id) != null;
        }
        // GET: api/ServiceCenter
        [HttpPost("Create")]
        public async Task<ActionResult<string>> Create(ServiceCenter template)
        {
            var serviceCenter = await ServiceCenterRepository.InsertAsync(template);
            return JsonConvert.SerializeObject(new { success = true, message = "Service Center Created", data = serviceCenter }, settings);
        }
        [HttpGet("{$id}")]
        public async Task<ActionResult<string>> Get(string id)
        {
            var serviceCenter = await ServiceCenterRepository.GetByIDAsync(id);
            if (serviceCenter == null)
            {
                return JsonConvert.SerializeObject(new { success = false, message = "Service Center Not Found" });
            }
            return JsonConvert.SerializeObject(new { success = true, data = serviceCenter, message = "Service Center Successfully Retrieved" });
        }

        [HttpGet("All")]
        public async Task<ActionResult<string>> All()
        {
            var serviceCenter = await ServiceCenterRepository.GetAllAsync();
            return JsonConvert.SerializeObject(new { success = true, data = serviceCenter, message = "Service Center Successfully Retrieved" });
        }
        [HttpPut("{$id}")]
        public async Task<ActionResult<string>> Update(string id, ServiceCenter template)
        {
            if (id != template.ServiceCenterId)
            {
                return JsonConvert.SerializeObject(new { success = false, message = "Service Center Not Found" });
            }
            await ServiceCenterRepository.UpdateAsync(template);
            try
            {
                await ServiceCenterRepository.SaveAsync();
                return JsonConvert.SerializeObject(new { success = true, message = "Service Center Updated", data = template }, settings);

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
        [HttpPut("UpdateAndFetch/{id}")]
        public async Task<ActionResult<string>> UpdateAndFetchAll(string id, ServiceCenter template)
        {
            if (id != template.ServiceCenterId)
            {
                return JsonConvert.SerializeObject(new { success = false, message = "Service Center Not Found" });
            }
            await ServiceCenterRepository.UpdateAsync(template);
            try
            {
                await ServiceCenterRepository.SaveAsync();
                var serviceCenter = await ServiceCenterRepository.GetAllAsync();
                return JsonConvert.SerializeObject(new { success = true, data = serviceCenter, message = "Service Center Updated" });
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
            var serviceCenter = await ServiceCenterRepository.GetByIDAsync(id);
            if (serviceCenter == null)
            {
                return JsonConvert.SerializeObject(new { success = false, message = "Service Center Not Found" });
            }
            await ServiceCenterRepository.DeleteAsync(serviceCenter);
            return JsonConvert.SerializeObject(new { success = true, message = "Service Center Deleted" });
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

            var allEmployees = UserRepository.GetAll(filter: e => e.Role == "Employee");
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

    }
}
