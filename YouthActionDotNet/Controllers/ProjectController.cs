using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
    public class ProjectController : ControllerBase, IUserInterfaceCRUD<Project>
    {
        private readonly DBContext _context;

        JsonSerializerSettings settings = new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };

        public ProjectController(DBContext context)
        {
            _context = context;
        }

        public bool Exists(string id)
        {
            return _context.Project.Any(e => e.ProjectId == id);
        }

        [HttpPost("Create")]
        public async Task<ActionResult<string>> Create(Project template)
        {

            template.ProjectId = Guid.NewGuid().ToString();

            var project = await _context.Project.Where(x => x.ProjectName == template.ProjectName).FirstOrDefaultAsync();
            if (project != null)
            {
                return JsonConvert.SerializeObject(new { success = false, message = "Project Already Exists" }, settings);
            }

            _context.Project.Add(template);
            await _context.SaveChangesAsync();

            CreatedAtAction("GetProject", new { id = template.ProjectId }, template);
            // return user in json format
            return JsonConvert.SerializeObject(new { success = true, message = "Project Created", data = template });
        }

        // GET: api/Project
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Project>>> GetProject()
        {
            return await _context.Project.ToListAsync();
        }

        // GET: api/Project/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Project>> GetProject(string id)
        {
            var project = await _context.Project.FindAsync(id);

            if (project == null)
            {
                return NotFound();
            }

            return project;
        }

        // PUT: api/Project/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProject(string id, Project project)
        {
            if (id != project.ProjectId)
            {
                return BadRequest();
            }

            _context.Entry(project).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Exists(id))
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

        // POST: api/Project
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Project>> PostProject(Project project)
        {
            _context.Project.Add(project);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (Exists(project.ProjectId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetProject", new { id = project.ProjectId }, project);
        }

        // DELETE: api/Project/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(string id)
        {
            var project = await _context.Project.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            _context.Project.Remove(project);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<string>> Get(string id)
        {
            var project = _context.Project.FindAsync(id);

            if (project == null)
            {
                return JsonConvert.SerializeObject(new { success = false, data = "", message = "Project Not Found" });

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
            _context.Entry(template).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
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
            var project = await _context.Project.FindAsync(id);
            if (project == null)
            {
                return JsonConvert.SerializeObject(new { success = false, data = "", message = "Project Not Found" });
            }
            _context.Project.Remove(project);
            await _context.SaveChangesAsync();
            return JsonConvert.SerializeObject(new { success = true, data = project, message = "Project Successfully Deleted" });
        }

        [HttpGet("All")]
        public async Task<ActionResult<string>> All()
        {
            var projects = await _context.Project.ToListAsync();
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

            settings.FieldSettings.Add("ProjectId", new InputType { type = "number", displayLabel = "Project Id", editable = false, primaryKey = true });
            settings.FieldSettings.Add("ProjectName", new InputType { type = "text", displayLabel = "Project Name", editable = true, primaryKey = false });
            settings.FieldSettings.Add("ProjectDescription", new InputType { type = "text", displayLabel = "EmaiProject Descriptionl", editable = true, primaryKey = false });
            settings.FieldSettings.Add("ProjectStartDate", new InputType { type = "text", displayLabel = "Project Start Date", editable = true, primaryKey = false });
            settings.FieldSettings.Add("ProjectEndDate", new InputType { type = "text", displayLabel = "Project End Date", editable = true, primaryKey = false });
            settings.FieldSettings.Add("ProjectCompletionDate", new InputType { type = "text", displayLabel = "Project Completion Date", editable = true, primaryKey = false });
            settings.FieldSettings.Add("ProjectStatus", new InputType { type = "text", displayLabel = "Project Status", editable = true, primaryKey = false });
            settings.FieldSettings.Add("ProjectBudget", new InputType { type = "number", displayLabel = "Project Budget", editable = true, primaryKey = false });

            var serviceCenters = _context.ServiceCenters.ToList();
            settings.FieldSettings.Add("ServiceCenterId", new DropdownInputType { type = "dropdown", displayLabel = "Service Center", editable = true, primaryKey = false, options = serviceCenters.Select(x => new DropdownOption { value = x.ServiceCenterId, label = x.ServiceCenterName }).ToList()});

            return JsonConvert.SerializeObject(new { success = true, data = settings, message = "Settings Successfully Retrieved" });
        }


    }
}
