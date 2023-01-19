using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
    public class ProjectController : ControllerBase, IUserInterfaceCRUD<Project>
    {
        private UnitOfWork unitOfWork;

        JsonSerializerSettings settings = new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };

        public ProjectController(DBContext context)
        {
            unitOfWork = new UnitOfWork(context);
        }

        public bool Exists(string id)
        {
            return unitOfWork.ProjectRepository.GetByID(id) != null;
        }

        [HttpPost("Create")]
        public async Task<ActionResult<string>> Create(Project template)
        {

            template.ProjectId = Guid.NewGuid().ToString();
            var project = await unitOfWork.ProjectRepository.InsertAsync(template);
            return JsonConvert.SerializeObject(new { success = true, message = "Project Created", data = project }, settings);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<string>> Get(string id)
        {
            var project = await unitOfWork.ProjectRepository.GetByIDAsync(id);
            if (project == null)
            {
                return JsonConvert.SerializeObject(new { success = false, message = "Project Not Found" });
            }
            return JsonConvert.SerializeObject(new { success = true, data = project, message = "Project Successfully Retrieved" });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<string>> Update(string id, Project template)
        {
            if (id != template.ProjectId)
            {
                return JsonConvert.SerializeObject(new { success = false, data = "", message = "Project Id Mismatch" });
            }
            await unitOfWork.ProjectRepository.UpdateAsync(template);
            try
            {
                unitOfWork.Commit();
                return JsonConvert.SerializeObject(new { success = true, data = template, message = "Project Successfully Updated" });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Exists(id))
                {
                    return JsonConvert.SerializeObject(new { success = false, data = "", message = "Project Not Found" });
                }
                else
                {
                    throw;
                }
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<string>> UpdateAndFetchAll(string id, Project template)
        {
            if (id != template.ProjectId)
            {
                return JsonConvert.SerializeObject(new { success = false, data = "", message = "Project Id Mismatch" });
            }
            await unitOfWork.ProjectRepository.UpdateAsync(template);
            try
            {
                unitOfWork.Commit();
                var projects = await unitOfWork.ProjectRepository.GetAllAsync();
                return JsonConvert.SerializeObject(new { success = true, data = projects, message = "Project Successfully Updated" });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Exists(id))
                {
                    return JsonConvert.SerializeObject(new { success = false, data = "", message = "Project Not Found" });
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
            var project = await unitOfWork.ProjectRepository.GetByIDAsync(id);
            if (project == null)
            {
                return JsonConvert.SerializeObject(new { success = false, data = "", message = "Project Not Found" });
            }
            await unitOfWork.ProjectRepository.DeleteAsync(id);
            unitOfWork.Commit();
            return JsonConvert.SerializeObject(new { success = true, data = "", message = "Project Successfully Deleted" });
        }

        [HttpGet("All")]
        public async Task<ActionResult<string>> All()
        {
            var projects = await unitOfWork.ProjectRepository.GetAllAsync();
            return JsonConvert.SerializeObject(new { success = true, data = projects, message = "Projects Successfully Retrieved" });
        }


        [HttpGet("Settings")]
        public string Settings()
        {
            Settings settings = new Settings();
            settings.ColumnSettings = new Dictionary<string, ColumnHeader>();
            settings.FieldSettings = new Dictionary<string, InputType>();

            settings.ColumnSettings.Add("ProjectId", new ColumnHeader { displayHeader = "Project Id" });
            settings.ColumnSettings.Add("ProjectName", new ColumnHeader { displayHeader = "Project Name" });
            settings.ColumnSettings.Add("ProjectDescription", new ColumnHeader { displayHeader = "Project Description" });
            settings.ColumnSettings.Add("ProjectStartDate", new ColumnHeader { displayHeader = "Project Start Date" });
            settings.ColumnSettings.Add("ProjectEndDate", new ColumnHeader { displayHeader = "Project End Date" });
            settings.ColumnSettings.Add("ProjectCompletionDate", new ColumnHeader { displayHeader = "Project Completion Date" });
            settings.ColumnSettings.Add("ProjectStatus", new ColumnHeader { displayHeader = "Project Status" });
            settings.ColumnSettings.Add("ProjectBudget", new ColumnHeader { displayHeader = "Project Budget" });
            settings.ColumnSettings.Add("ServiceCenterId", new ColumnHeader { displayHeader = "Service Center Id" });

            settings.FieldSettings.Add("ProjectId", new InputType { type = "text", displayLabel = "Project Id", editable = false, primaryKey = true });
            settings.FieldSettings.Add("ProjectName", new InputType { type = "text", displayLabel = "Project Name", editable = true, primaryKey = false });
            settings.FieldSettings.Add("ProjectDescription", new InputType { type = "text", displayLabel = "EmaiProject Descriptionl", editable = true, primaryKey = false });
            settings.FieldSettings.Add("ProjectStartDate", new InputType { type = "datetime", displayLabel = "Project Start Date", editable = true, primaryKey = false });
            settings.FieldSettings.Add("ProjectEndDate", new InputType { type = "datetime", displayLabel = "Project End Date", editable = true, primaryKey = false });
            settings.FieldSettings.Add("ProjectCompletionDate", new InputType { type = "datetime", displayLabel = "Project Completion Date", editable = true, primaryKey = false });
            settings.FieldSettings.Add("ProjectStatus", new InputType { type = "text", displayLabel = "Project Status", editable = true, primaryKey = false });
            settings.FieldSettings.Add("ProjectBudget", new InputType { type = "number", displayLabel = "Project Budget", editable = true, primaryKey = false });

            var serviceCenters = unitOfWork.ServiceCenterRepository.GetAll();
            settings.FieldSettings.Add("ServiceCenterId", new DropdownInputType { 
                type = "dropdown", 
                displayLabel = "Service Center", 
                editable = true, 
                primaryKey = false, 
                options = serviceCenters.Select(x => new DropdownOption { value = x.ServiceCenterId, label = x.ServiceCenterName }).ToList()});

            return JsonConvert.SerializeObject(new { success = true, data = settings, message = "Settings Successfully Retrieved" });
        }


    }
}
