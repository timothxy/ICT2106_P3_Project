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
using YouthActionDotNet.DAL;

namespace YouthActionDotNet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VolunteerWorkController : ControllerBase, IUserInterfaceCRUD<VolunteerWork>
    {
        private GenericRepository<VolunteerWork> VolunteerWorkRepository;
        private GenericRepository<Volunteer> VolunteerRepository;
        private GenericRepository<Employee> EmployeeRepository;
        private GenericRepository<Project> ProjectRepository;
        JsonSerializerSettings settings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        public VolunteerWorkController(DBContext context)
        {
            VolunteerWorkRepository = new GenericRepository<VolunteerWork>(context);
            VolunteerRepository = new GenericRepository<Volunteer>(context);
            EmployeeRepository = new GenericRepository<Employee>(context);
            ProjectRepository = new GenericRepository<Project>(context);
        }

        // GET: api/VolunteerWork
        [HttpPost("Create")]
        public async Task<ActionResult<string>> Create(VolunteerWork template)
        {
            var volunteerWork = await VolunteerWorkRepository.InsertAsync(template);
            return JsonConvert.SerializeObject(new { success = true, message = "Volunteer Work Created", data = volunteerWork }, settings);
        }

        [HttpGet("{$id}")]
        public async Task<ActionResult<string>> Get(string id)
        {
            var volunteerWork = await VolunteerWorkRepository.GetByIDAsync(id);
            if (volunteerWork == null)
            {
                return JsonConvert.SerializeObject(new { success = false, message = "Volunteer Work Not Found" }, settings);
            }
            return JsonConvert.SerializeObject(new { success = true, data = volunteerWork, message = "Volunteer Work Successfully Retrieved" }, settings);
        }
        
        [HttpGet("All")]
        public async Task<ActionResult<string>> All()
        {
            var volunteerWork = await VolunteerWorkRepository.GetAllAsync();
            return JsonConvert.SerializeObject(new { success = true, data = volunteerWork, message = "Volunteer Work Successfully Retrieved" }, settings);
        }

        [HttpPut("{$id}")]
        public async Task<ActionResult<string>> Update(string id, VolunteerWork template)
        {
            if (id != template.VolunteerWorkId)
            {
                return JsonConvert.SerializeObject(new { success = false, message = "Volunteer Work Not Found" }, settings);
            }
            await VolunteerWorkRepository.UpdateAsync(template);
            try
            {
                await VolunteerRepository.SaveAsync();
                return JsonConvert.SerializeObject(new { success = true, message = "Volunteer Work Successfully Updated" }, settings);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Exists(id))
                {
                    return JsonConvert.SerializeObject(new { success = false, message = "Volunteer Work Not Found" }, settings);
                }
                else
                {
                    throw;
                }
            }
        }

        [HttpPut("UpdateAndFetch/{$id}")]
        public async Task<ActionResult<string>> UpdateAndFetchAll(string id, VolunteerWork template)
        {
            if (id != template.VolunteerWorkId)
            {
                return JsonConvert.SerializeObject(new { success = false, message = "Volunteer Work Not Found" }, settings);
            }
            await VolunteerWorkRepository.UpdateAsync(template);
            try
            {
                await VolunteerRepository.SaveAsync();
                var volunteerWork = await VolunteerWorkRepository.GetAllAsync();
                return JsonConvert.SerializeObject(new { success = true, data = volunteerWork, message = "Volunteer Work Successfully Updated" }, settings);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Exists(id))
                {
                    return JsonConvert.SerializeObject(new { success = false, message = "Volunteer Work Not Found" }, settings);
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
            var volunteerWork = await VolunteerWorkRepository.GetByIDAsync(id);
            if (volunteerWork == null)
            {
                return JsonConvert.SerializeObject(new { success = false, message = "Volunteer Work Not Found" }, settings);
            }
            await VolunteerWorkRepository.DeleteAsync(volunteerWork);
            return JsonConvert.SerializeObject(new { success = true, message = "Volunteer Work Successfully Deleted" }, settings);
        }

        public bool Exists(string id)
        {
            return VolunteerWorkRepository.GetByID(id) != null;
        }

        [HttpGet("Settings")]
        public string Settings()
        {
            Settings settings = new Settings();
            settings.ColumnSettings = new Dictionary<string, ColumnHeader>();
            settings.FieldSettings = new Dictionary<string, InputType>();

            settings.ColumnSettings.Add("VolunteerWorkId", new ColumnHeader {displayHeader = "Volunteer Work Id"});
            settings.ColumnSettings.Add("ShiftStart", new ColumnHeader {displayHeader = "Shift Start"});
            settings.ColumnSettings.Add("ShiftEnd", new ColumnHeader {displayHeader = "Shift End"});
            settings.ColumnSettings.Add("SupervisingEmployee", new ColumnHeader {displayHeader = "Supervising Employee"});
            settings.ColumnSettings.Add("VolunteerId", new ColumnHeader {displayHeader = "Volunteer Id"});
            settings.ColumnSettings.Add("projectId", new ColumnHeader {displayHeader = "Project Id"});
            
            settings.FieldSettings.Add("VolunteerWorkId", new InputType {type="text", displayLabel = "Volunteer Work Id",editable = false,primaryKey=true});
            settings.FieldSettings.Add("ShiftStart", new InputType {type="datetime", displayLabel = "Shift Start",editable = true,primaryKey=false});
            settings.FieldSettings.Add("ShiftEnd", new InputType {type="datetime", displayLabel = "Shift End",editable = true, primaryKey=false});

            // Fetch Volunteers and use info as dropdown options
            var allVolunteers = VolunteerRepository.GetAll(filter: u => u.ApprovalStatus == "Approved");
            settings.FieldSettings.Add("VolunteerId", new DropdownInputType {
                type="dropdown",
                displayLabel = "Volunteer Id",
                editable = true, 
                primaryKey=false,
                options = allVolunteers.Select(
                    u => new DropdownOption {
                        value = u.UserId, label = u.username
                        }).ToList()
                    });
            
            // Fetch projects and use info as dropdown options
            var allProjects = ProjectRepository.GetAll();
            settings.FieldSettings.Add("projectId", new DropdownInputType {
                type="dropdown",
                displayLabel = "Project Id",
                editable = true, 
                primaryKey=false,
                options = allProjects.Select(
                    u => new DropdownOption {
                        value = u.ProjectId, label = u.ProjectName
                        }).ToList()
                    });

            // Fetch employees and use info as dropdown options
            var allEmployees = EmployeeRepository.GetAll();
            settings.FieldSettings.Add("SupervisingEmployee", new DropdownInputType {
                type="dropdown",
                displayLabel = "Supervising Employee",
                editable = true, 
                primaryKey=false,
                options = allEmployees.Select(
                    u => new DropdownOption {
                        value = u.UserId, label = u.username
                        }).ToList()
                    });

            return JsonConvert.SerializeObject(new {
                success = true,
                data = settings
            });
        }
    }
}