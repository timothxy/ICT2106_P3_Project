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
    public class DonorController : ControllerBase,IUserInterfaceCRUD<Donor>
    {
        private readonly DBContext _context;
        JsonSerializerSettings settings = new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };


        public DonorController(DBContext context)
        {
            _context = context;
        }

        public bool Exists(string id)
        {
            return _context.Donor.Any(e => e.UserId == id);
        }
        
        [HttpPost("Create")]
        public async Task<ActionResult<string>> Create(Donor donor)
        {
            donor.UserId = Guid.NewGuid().ToString();
            SHA256 sha256 = SHA256.Create();
            var secretPw = Convert.ToHexString(sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(donor.Password)));

            donor.Password = secretPw;
            sha256.Dispose();
            var userPL = await _context.Users.Where(u => u.username == donor.username).FirstOrDefaultAsync();
            if(userPL != null){
                return JsonConvert.SerializeObject(new {success=false,message="Donor Already Exists"},settings);
            }
            
            _context.Donor.Add(donor);
            await _context.SaveChangesAsync();

            CreatedAtAction("Get", new { id = donor.UserId }, donor);
            //return the user in json format
            return JsonConvert.SerializeObject(new {success=true,message="User Successfully Created", data = donor},settings);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<string>> Get(string id)
        {   
            var donor = await _context.Donor.FindAsync(id);
            if (donor == null)
            {
                return JsonConvert.SerializeObject(new {success=false,message="Donor Not Found"},settings);
            }
            return JsonConvert.SerializeObject(new {success=true,message="Donor Found", data = donor},settings); 
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<string>> Update(string id, Donor donor)
        {
            if (id != donor.UserId){
                return JsonConvert.SerializeObject(new {success=false,message="Bad Request"},settings);
            }
            _context.Entry(donor).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
                return JsonConvert.SerializeObject(new {success=true,message="Donor Updated", data = donor},settings);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Exists(id))
                {
                    return JsonConvert.SerializeObject(new {success=false,message="Donor Not Found"},settings);
                }
                else
                {
                    throw;
                }
            }
        }

        [HttpPut("UpdateAndFetch/{id}")]
        public async Task<ActionResult<string>> UpdateAndFetchAll(string id, Donor template)
        {
            if(id != template.UserId){
                return JsonConvert.SerializeObject(new { success = false, data = "", message = "Volunteer Id Mismatch" });
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
            var donor = await _context.Donor.FindAsync(id);
            if (donor == null)
            {
                return JsonConvert.SerializeObject(new {success=false,message="Donor Not Found"},settings);
            }
            _context.Donor.Remove(donor);
            await _context.SaveChangesAsync();
            return JsonConvert.SerializeObject(new {success=true,message="Donor Deleted", data = donor},settings);
        }

        [HttpGet("All")]
        public async Task<ActionResult<string>> All()
        {
            var donors = await _context.Donor.ToListAsync();
            return JsonConvert.SerializeObject(new {success=true,message="All Donors fetched", data = donors},settings);
        }

        [HttpGet("Settings")]
        public string Settings()
        {
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
            settings.FieldSettings.Add("Role", new DropdownInputType { type = "dropdown", displayLabel = "Role", editable = true, primaryKey = false, 
            options = new List<DropdownOption> {
                new DropdownOption { value = "Admin", label = "Admin" },
                new DropdownOption { value = "Employee", label = "Employee" },
                new DropdownOption { value = "Volunteer", label = "Volunteer" },
                new DropdownOption { value = "Donor", label = "Donor" },
            } });

            settings.FieldSettings.Add("donorName", new InputType { type = "text", displayLabel = "Donor Name", editable = true, primaryKey = false });
            settings.FieldSettings.Add("donorType", new DropdownInputType { type = "dropdown", displayLabel = "Donor Type", editable = true, primaryKey = false, options = new List<DropdownOption> {
                new DropdownOption { value = "Individual", label = "Individual" },
                new DropdownOption { value = "Organization", label = "Organization" },
            } });
            return JsonConvert.SerializeObject(new {success=true,message="Settings Fetched", data = settings});

        }
    }
}

