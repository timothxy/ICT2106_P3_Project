using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YouthActionDotNet.Data;
using YouthActionDotNet.Models;
using Newtonsoft.Json;
using System.Security.Cryptography;
using YouthActionDotNet.DAL;

namespace YouthActionDotNet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VolunteerController : ControllerBase, IUserInterfaceCRUD<Volunteer>
    {
        private GenericRepository<Volunteer> VolunteerRepository;
        private GenericRepository<Employee> EmployeeRepository;
        JsonSerializerSettings settings = new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };

        public VolunteerController(DBContext context)
        {
            VolunteerRepository = new GenericRepository<Volunteer>(context);
            EmployeeRepository = new GenericRepository<Employee>(context);
        }

        public bool Exists(string id)
        {
            return VolunteerRepository.GetByID(id) != null;
        }

        [HttpPost("Create")]
        public async Task<ActionResult<string>> Create(Volunteer template)
        {
            var volunteers = await VolunteerRepository.GetAllAsync();
            var existingVolunteer = volunteers.FirstOrDefault(d => d.UserId == template.UserId);
            if(existingVolunteer != null){
                return JsonConvert.SerializeObject(new { success = false, message = "Volunteer Already Exists" });
            }
            template.Password = Utils.hashpassword(template.Password);
            await VolunteerRepository.InsertAsync(template);
            var createdVolunteer = await VolunteerRepository.GetByIDAsync(template.UserId);
            return JsonConvert.SerializeObject(new { success = true, data = template, message = "Volunteer Successfully Created" });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<string>> Get(string id)
        {
            var volunteer = await VolunteerRepository.GetByIDAsync(id);
            if (volunteer == null)
            {
                return JsonConvert.SerializeObject(new { success = false, data = "", message = "Volunteer Not Found" });
            }
            return JsonConvert.SerializeObject(new { success = true, data = volunteer, message = "Volunteer Successfully Retrieved" });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<string>> Update(string id, Volunteer template)
        {
            if(id != template.UserId){
                return JsonConvert.SerializeObject(new { success = false, data = "", message = "Volunteer Id Mismatch" });
            }
            VolunteerRepository.Update(template);
            try{
                return await Get(id);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Exists(id))
                {
                    return JsonConvert.SerializeObject(new { success = false, data = "", message = "Volunteer Not Found" });
                }
                else
                {
                    throw;
                }
            }
        }
        [HttpPut("UpdateAndFetch/{id}")]
        public async Task<ActionResult<string>> UpdateAndFetchAll(string id, Volunteer template)
        {
            if(id != template.UserId){
                return JsonConvert.SerializeObject(new { success = false, data = "", message = "Volunteer Id Mismatch" });
            }
            VolunteerRepository.Update(template);
            try{
                VolunteerRepository.Save();
                return await All();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Exists(id))
                {
                    return JsonConvert.SerializeObject(new { success = false, data = "", message = "Volunteer Not Found" });
                }
                else
                {
                    throw;
                }
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<string>> Delete(string id)
        {
            var volunteer = await VolunteerRepository.GetByIDAsync(id);
            if (volunteer == null)
            {
                return JsonConvert.SerializeObject(new { success = false, data = "", message = "Volunteer Not Found" });
            }
            VolunteerRepository.Delete(volunteer);
            VolunteerRepository.Save();
            return JsonConvert.SerializeObject(new { success = true, data = "", message = "Volunteer Successfully Deleted" });
        }

        [HttpDelete("Delete")]
        public async Task<ActionResult<string>> Delete(Volunteer template)
        {
            var volunteer = await VolunteerRepository.GetByIDAsync(template.UserId);
            if (volunteer == null)
            {
                return JsonConvert.SerializeObject(new { success = false, data = "", message = "Volunteer Not Found" });
            }
            VolunteerRepository.Delete(volunteer);
            VolunteerRepository.Save();
            return JsonConvert.SerializeObject(new { success = true, data = "", message = "Volunteer Successfully Deleted" });
        }

        [HttpGet("All")]
        public async Task<ActionResult<string>> All()
        {
            var volunteers = await VolunteerRepository.GetAllAsync();
            return JsonConvert.SerializeObject(new { success = true, data = volunteers, message = "Volunteers Successfully Retrieved" }, settings);
        }

        [HttpGet("Settings")]
        public string Settings()
        {
            Settings settings = new UserSettings();
            settings.ColumnSettings.Add("UserId", new ColumnHeader { displayHeader = "User Id" });
            settings.ColumnSettings.Add("username", new ColumnHeader { displayHeader = "Username" });
            settings.ColumnSettings.Add("Email", new ColumnHeader { displayHeader = "Email" });
            settings.ColumnSettings.Add("Password", new ColumnHeader { displayHeader = "Password" });
            settings.ColumnSettings.Add("Role", new ColumnHeader { displayHeader = "Role" });

            settings.FieldSettings.Add("VolunteerNationalId", new InputType { type = "text", displayLabel = "Volunteer National Id", editable = true, primaryKey = false });
            settings.FieldSettings.Add("VolunteerDateJoined", new InputType { type = "datetime", displayLabel = "Volunteer Date Joined", editable = true, primaryKey = false });
            settings.FieldSettings.Add("VolunteerDateBirth", new InputType { type = "datetime", displayLabel = "Volunteer Date Birth", editable = true, primaryKey = false });
            settings.FieldSettings.Add("Qualifications", new InputType { type = "text", displayLabel = "Qualifications", editable = true, primaryKey = false });
            settings.FieldSettings.Add("CriminalHistory", new DropdownInputType { type = "dropdown", displayLabel = "Criminal History", editable = true, primaryKey = false, options = new List<DropdownOption> {
                new DropdownOption { value = "Yes", label = "Yes" },
                new DropdownOption { value = "No", label = "No" },
            } });
            settings.FieldSettings.Add("CriminalHistoryDesc", new InputType { type = "text", displayLabel = "Criminal History Description", editable = true, primaryKey = false });
            settings.FieldSettings.Add("ApprovalStatus", new DropdownInputType { type = "dropdown", displayLabel = "Volunteer Status", editable = true, primaryKey = false, options = new List<DropdownOption> {
                new DropdownOption { value = "Approved", label = "Approved" },
                new DropdownOption { value = "Pending", label = "Pending" },
            } });

            var allEmployees = EmployeeRepository.GetAll(filter: e=>e.Role == "Employee");
            settings.FieldSettings.Add("ApprovedBy", new DropdownInputType { 
                type = "dropdown", 
                displayLabel = "Approved By", 
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
