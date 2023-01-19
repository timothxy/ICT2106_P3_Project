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
using YouthActionDotNet.DAL;

namespace YouthActionDotNet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase, IUserInterfaceCRUD<Employee>
    {
        private UnitOfWork unitOfWork;


        public EmployeeController(DBContext context)
        {
            unitOfWork = new UnitOfWork(context);
        
        }

        [HttpPost("Create")]
        public async Task<ActionResult<string>> Create(Employee template)
        {
            var employees = await unitOfWork.EmployeeRepository.GetAllAsync();
            var existingEmployee = employees.FirstOrDefault(e => e.username == template.username);
            if(existingEmployee != null){
                return JsonConvert.SerializeObject(new { success = false, message = "Employee Already Exists" });
            }
            template.Password = u.hashpassword(template.Password);
            await unitOfWork.EmployeeRepository.InsertAsync(template);
            unitOfWork.Commit();
            var createdEmployee = await unitOfWork.EmployeeRepository.GetByIDAsync(template.UserId);
            return JsonConvert.SerializeObject(new { success = true, data = template, message = "Employee Successfully Created" });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<string>> Get(string id)
        {
            var employee = await unitOfWork.EmployeeRepository.GetByIDAsync(id);
            if (employee == null)
            {
                return JsonConvert.SerializeObject(new { success = false, data = "", message = "Employee Not Found" });
            }
            return JsonConvert.SerializeObject(new { success = true, data = employee, message = "Employee Successfully Retrieved" });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<string>> Update(string id, Employee template)
        {
            if(id != template.UserId){
                return JsonConvert.SerializeObject(new { success = false, data = "", message = "Employee Id Mismatch" });
            }
            await unitOfWork.EmployeeRepository.UpdateAsync(template);
            try
            {
                await unitOfWork.EmployeeRepository.SaveAsync();
                return JsonConvert.SerializeObject(new { success = true, data = template, message = "Employee Successfully Updated" });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Exists(id))
                {
                    return JsonConvert.SerializeObject(new { success = false, data = "", message = "Employee Not Found" });
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
                return JsonConvert.SerializeObject(new { success = false, data = "", message = "Employee Id Mismatch" });
            }
            await unitOfWork.EmployeeRepository.UpdateAsync(template);
            try
            {
                await unitOfWork.EmployeeRepository.SaveAsync();
                var employees = await unitOfWork.EmployeeRepository.GetAllAsync();
                return JsonConvert.SerializeObject(new { success = true, data = employees, message = "Employee Successfully Updated" });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Exists(id))
                {
                    return JsonConvert.SerializeObject(new { success = false, data = "", message = "Employee Not Found" });
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
            var employee = await unitOfWork.EmployeeRepository.GetByIDAsync(id);
            if (employee == null)
            {
                return JsonConvert.SerializeObject(new { success = false, data = "", message = "Employee Not Found" });
            }
            await unitOfWork.EmployeeRepository.DeleteAsync(employee);
            unitOfWork.Commit();
            return JsonConvert.SerializeObject(new { success = true, data = "", message = "Employee Successfully Deleted" });    
        }

        [HttpGet("All")]
        public async Task<ActionResult<string>> All()
        {
            var employees = await unitOfWork.EmployeeRepository.GetAllAsync();
            return JsonConvert.SerializeObject(new { success = true, data = employees, message = "Employees Successfully Retrieved" });
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
                new DropdownOption { value = "Service Center Manager", label = "Service Center Manager" },
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
            return unitOfWork.EmployeeRepository.GetByIDAsync(id) != null;
        }
    }
}
