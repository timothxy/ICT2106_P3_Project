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

namespace YouthActionDotNet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VolunteerController : ControllerBase
    {
        private readonly DBContext _context;
        JsonSerializerSettings settings = new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };

        public VolunteerController(DBContext context)
        {
            _context = context;
        }

        // GET: api/Volunteer/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Volunteer>> GetVolunteer(string id)
        {
            var volunteer = await _context.Volunteer.FindAsync(id);

            if (volunteer == null)
            {
                return NotFound();
            }

            return volunteer;
        }

        // To get all employees
        // GET: api/Volunteer/All
        [HttpGet("All")]
        public async Task<ActionResult<String>> GetAllVolunteers()
        {
            var volunteers = await _context.Volunteer.ToListAsync();
            return JsonConvert.SerializeObject(new {success = true, data = volunteers, message = "Volunteers Successfully Retrieved"});
        }

        //To update the volunteer
        // PUT: api/Volunteer/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutVolunteer(string id, Volunteer volunteer)
        {
            if (id != volunteer.UserId)
            {
                return BadRequest();
            }

            _context.Entry(volunteer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VolunteerExists(id))
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

        // POST: api/Volunteer
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("Create")]
        public async Task<ActionResult<String>> PostVolunteer(Volunteer volunteer)
        {

            volunteer.UserId = Guid.NewGuid().ToString();
            //check if user exists
            SHA256 sha256 = SHA256.Create();
            var secretPw = Convert.ToHexString(sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(volunteer.Password)));

            volunteer.Password = secretPw;
            sha256.Dispose();
            var userPL = await _context.Users.Where(u => u.username == volunteer.username).FirstOrDefaultAsync();
            if(userPL != null){
                return JsonConvert.SerializeObject(new {success=false,message="Volunteer Already Exists"},settings);
            }
            
            _context.Volunteer.Add(volunteer);
            await _context.SaveChangesAsync();

            CreatedAtAction("GetUser", new { id = volunteer.UserId }, volunteer);
            //return the user in json format
            return JsonConvert.SerializeObject(new {success=true,message="User Successfully Created", data = volunteer},settings);}

        // DELETE: api/Volunteer/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVolunteer(int id)
        {
            var volunteer = await _context.Volunteer.FindAsync(id);
            if (volunteer == null)
            {
                return NotFound();
            }

            _context.Volunteer.Remove(volunteer);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool VolunteerExists(string id)
        {
            return _context.Volunteer.Any(e => e.UserId == id);
        }

        [HttpGet("Settings")]
        public string GetVolunteerSettings(){
            Settings settings = new Settings();
            settings.ColumnSettings = new Dictionary<string, ColumnHeader>();
            settings.FieldSettings = new Dictionary<string, InputType>();
            
            settings.ColumnSettings.Add("UserId", new ColumnHeader { displayHeader = "User Id" });
            settings.ColumnSettings.Add("username", new ColumnHeader { displayHeader = "Username" });
            settings.ColumnSettings.Add("Email", new ColumnHeader { displayHeader = "Email" });
            settings.ColumnSettings.Add("Password", new ColumnHeader { displayHeader = "Password" });
            settings.ColumnSettings.Add("Role", new ColumnHeader { displayHeader = "Role" });

            settings.FieldSettings.Add("UserId", new InputType { type = "number", displayLabel = "User Id", editable = false, primaryKey = true });
            settings.FieldSettings.Add("username", new InputType { type = "text", displayLabel = "Username", editable = true, primaryKey = false });
            settings.FieldSettings.Add("Email", new InputType { type = "text", displayLabel = "Email", editable = true, primaryKey = false });
            settings.FieldSettings.Add("Password", new InputType { type = "text", displayLabel = "Password", editable = true, primaryKey = false });
            settings.FieldSettings.Add("Role", new DropdownInputType { type = "dropdown", displayLabel = "Role", editable = true, primaryKey = false, options = new List<DropdownOption> {
                new DropdownOption { value = "Admin", label = "Admin" },
                new DropdownOption { value = "Employee", label = "Employee" },
                new DropdownOption { value = "Volunteer", label = "Volunteer" },
                new DropdownOption { value = "Donor", label = "Donor" },
            } });
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

            var allEmployees = _context.Users.Where(u => u.Role == "Employee").ToList();
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
