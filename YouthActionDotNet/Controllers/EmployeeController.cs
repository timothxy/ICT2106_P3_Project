using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using YouthActionDotNet.Data;
using System.Security.Cryptography;
using YouthActionDotNet.Models;

namespace YouthActionDotNet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase, IUserInterfaceCRUD<Employee>
    {
        private readonly DBContext _context;
        JsonSerializerSettings settings = new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };


        public EmployeeController(DBContext context)
        {
            _context = context;
        }

        [HttpPost("Create")]
        public async Task<ActionResult<string>> Create(Employee template)
        {
            template.UserId = Guid.NewGuid().ToString();
            //check if user exists
            SHA256 sha256 = SHA256.Create();
            var secretPw = Convert.ToHexString(sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(template.Password)));

            template.Password = secretPw;
            sha256.Dispose();
            var userPL = await _context.Users.Where(u => u.username == template.username).FirstOrDefaultAsync();
            if(userPL != null){
                return JsonConvert.SerializeObject(new {success=false,message="Volunteer Already Exists"},settings);
            }
            
            _context.Employee.Add(template);
            await _context.SaveChangesAsync();

            CreatedAtAction("GetUser", new { id = template.UserId }, template);
            //return the user in json format
            return JsonConvert.SerializeObject(new {success=true,message="User Successfully Created", data = template},settings);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<string>> Get(string id)
        {
            var employee = await _context.Employee.FindAsync(id);
            if (employee == null)
            {
                return JsonConvert.SerializeObject(new {success = false, message = "Employee Not Found"});
            }
            return JsonConvert.SerializeObject(new {success = true, data = employee, message = "Employee Successfully Retrieved"});
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<string>> Update(string id, Employee template)
        {
            if(id != template.UserId){
                return JsonConvert.SerializeObject(new { success = false, data = "", message = "Volunteer Id Mismatch" });
            }
            _context.Entry(template).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
                return JsonConvert.SerializeObject(new { success = true, data = template, message = "Volunteer Successfully Updated" });
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
        public async Task<ActionResult<string>> UpdateAndFetchAll(string id, Employee template)
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
            var employee = await _context.Employee.FindAsync(id);
            if (employee == null)
            {
                return JsonConvert.SerializeObject(new {success = false, message = "Employee Not Found"});
            }
        
            _context.Employee.Remove(employee);
            await _context.SaveChangesAsync();
            return JsonConvert.SerializeObject(new {success = true, message = "Employee Successfully Deleted"});
        }

        [HttpGet("All")]
        public async Task<ActionResult<string>> All()
        {
            var employees = await _context.Employee.ToListAsync();
            return JsonConvert.SerializeObject(new {success = true, data = employees, message = "Employees Successfully Retrieved"},settings);
        
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
            settings.FieldSettings.Add("Role", new DropdownInputType { type = "dropdown", displayLabel = "Role", editable = true, primaryKey = false, options = new List<DropdownOption> {
                new DropdownOption { value = "Admin", label = "Admin" },
                new DropdownOption { value = "Employee", label = "Employee" },
                new DropdownOption { value = "Volunteer", label = "Volunteer" },
                new DropdownOption { value = "Donor", label = "Donor" },
            } });
            settings.FieldSettings.Add("EmployeeNationalId", new InputType { type = "text", displayLabel = "National Id", editable = true, primaryKey = false, toolTip = "E.g. AB123456C" });
            settings.FieldSettings.Add("BankName", new InputType { type = "text", displayLabel = "Bank Name", editable = true, primaryKey = false });
            settings.FieldSettings.Add("BankAccountNumber", new InputType { type = "text", displayLabel = "Bank Account Number", editable = true, primaryKey = false });
            settings.FieldSettings.Add("PAYE", new InputType { type = "text", displayLabel = "PAYE Number", editable = true, primaryKey = false });
            settings.FieldSettings.Add("DateJoined", new InputType { type = "datetime", displayLabel = "Date Joined", editable = true, primaryKey = false });
            settings.FieldSettings.Add("EmployeeType", new DropdownInputType { type = "dropdown", displayLabel = "Employee Type", editable = true, primaryKey = false, options = new List<DropdownOption> {
                new DropdownOption { value = "Full Time", label = "Full Time" },
                new DropdownOption { value = "Part Time", label = "Part Time" },
                new DropdownOption { value = "Contract", label = "Contract" }, 
                new DropdownOption { value = "Intern", label = "Intern" },
                new DropdownOption { value = "Temp", label = "Temp" },
            } });
            settings.FieldSettings.Add("EmployeeRole", new DropdownInputType { type = "dropdown", displayLabel = "Role", editable = true, primaryKey = false, options = new List<DropdownOption> {
                new DropdownOption { value = "Regional Director", label = "Regional Director" },
                new DropdownOption { value = "Full Time Worker", label = "Full Time Worker"},
                new DropdownOption { value = "Cheif Executive", label = "Cheif Executive"},
                new DropdownOption { value = "Finance Director", label = "Finance Director"},
                new DropdownOption { value = "Operating Manager", label = "Operating Manager"},
                new DropdownOption { value = "Marketing Manager", label = "Marketing Manager"},
                new DropdownOption { value = "Director of new Business", label = "Director of new Business"},
            } });
            return JsonConvert.SerializeObject(new { success = true, data = settings, message = "Settings Successfully Retrieved" });
        }

        public bool Exists(string id)
        {
            return _context.Employee.Any(e => e.UserId == id);
        }
    }
}
