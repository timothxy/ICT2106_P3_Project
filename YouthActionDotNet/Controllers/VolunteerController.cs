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
        private UnitOfWork unitOfWork;
        JsonSerializerSettings settings = new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };

        public VolunteerController(DBContext context)
        {
            unitOfWork = new UnitOfWork(context);
        }

        public bool Exists(string id)
        {
            return unitOfWork.VolunteerRepository.GetByID(id) != null;
        }

        [HttpPost("Create")]
        public async Task<ActionResult<string>> Create(Volunteer template)
        {
            var volunteers = await unitOfWork.VolunteerRepository.GetAllAsync();
            var existingVolunteer = volunteers.FirstOrDefault(d => d.UserId == template.UserId);
            if(existingVolunteer != null){
                return JsonConvert.SerializeObject(new { success = false, message = "Volunteer Already Exists" });
            }
            template.UserId = Guid.NewGuid().ToString();
            template.Password = u.hashpassword(template.Password);
            await unitOfWork.VolunteerRepository.InsertAsync(template);
            unitOfWork.Commit();
            var createdVolunteer = await unitOfWork.VolunteerRepository.GetByIDAsync(template.UserId);
            return JsonConvert.SerializeObject(new { success = true, data = template, message = "Volunteer Successfully Created" });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<string>> Get(string id)
        {
            var volunteer = await unitOfWork.VolunteerRepository.GetByIDAsync(id);
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
            unitOfWork.VolunteerRepository.Update(template);
            try{
                unitOfWork.Commit();
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
        [HttpPut("{id}")]
        public async Task<ActionResult<string>> UpdateAndFetchAll(string id, Volunteer template)
        {
            if(id != template.UserId){
                return JsonConvert.SerializeObject(new { success = false, data = "", message = "Volunteer Id Mismatch" });
            }
            unitOfWork.VolunteerRepository.Update(template);
            try{
                unitOfWork.Commit();
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
            var volunteer = await unitOfWork.VolunteerRepository.GetByIDAsync(id);
            if (volunteer == null)
            {
                return JsonConvert.SerializeObject(new { success = false, data = "", message = "Volunteer Not Found" });
            }
            unitOfWork.VolunteerRepository.Delete(volunteer);
            unitOfWork.Commit();
            return JsonConvert.SerializeObject(new { success = true, data = "", message = "Volunteer Successfully Deleted" });
        }

        [HttpGet("All")]
        public async Task<ActionResult<string>> All()
        {
            var volunteers = await unitOfWork.VolunteerRepository.GetAllAsync();
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

            var allEmployees = unitOfWork.EmployeeRepository.GetAll(filter: e=>e.Role == "Employee");
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
