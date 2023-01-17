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
    public class EmployeeController : ControllerBase
    {
        private readonly DBContext _context;
        JsonSerializerSettings settings = new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };


        public EmployeeController(DBContext context)
        {
            _context = context;
        }

        // To get a single employee
        // GET: api/Employee/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> GetEmployee(int id)
        {
            var employee = await _context.Employee.FindAsync(id);

            if (employee == null)
            {
                return NotFound();
            }

            return employee;
        }

        // To get all employees
        // GET: api/Employee/All
        [HttpGet("All")]
        public async Task<ActionResult<String>> GetAllEmployees()
        {
            var employees = await _context.Employee.ToListAsync();
            return JsonConvert.SerializeObject(new {success = true, data = employees, message = "Employees Successfully Retrieved"},settings);
        }

        //To update the employee
        // PUT: api/Employee/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmployee(string id, Employee employee)
        {
            if (id != employee.UserId)
            {
                return BadRequest();
            }

            _context.Entry(employee).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(id))
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

        // To to create a new employee
        // GET: api/Employee/Create
        [HttpPost("Create")]
        public async Task<ActionResult<String>> PostUser(Employee employee)
        {   
            
            //check if user exists
            employee.UserId = Guid.NewGuid().ToString();
            SHA256 sha256 = SHA256.Create();
            var secretPw = Convert.ToHexString(sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(employee.Password)));

            employee.Password = secretPw;
            sha256.Dispose();
            var userPL = await _context.Users.Where(u => u.username == employee.username).FirstOrDefaultAsync();
            if(userPL != null){
                return JsonConvert.SerializeObject(new {success=false,message="User Already Exists"});
            }
            
            _context.Employee.Add(employee);
            await _context.SaveChangesAsync();

            CreatedAtAction("GetUser", new { id = employee.UserId }, employee);
            //return the user in json format
            return JsonConvert.SerializeObject(new {success=true,message="User Successfully Created", data = employee},settings);
        }

        // To delete an employee
        // DELETE: api/Employee/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var employee = await _context.Employee.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            _context.Employee.Remove(employee);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EmployeeExists(string id)
        {
            return _context.Employee.Any(e => e.UserId == id);
        }

        // To provide front end which fields are to be displayed
        // GET: api/Employee/Settings
        [HttpGet("Settings")]
        public string GetEmployeeSettings(){

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
            return JsonConvert.SerializeObject(settings);
        }
    }
}
