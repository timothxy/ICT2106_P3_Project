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
    public class ExpenseController : ControllerBase, IUserInterfaceCRUD<Expense>
    {
        private GenericRepository<Employee> EmployeeRepository;
        private GenericRepository<Expense> ExpenseRepository;
        private GenericRepository<Project> ProjectRepository;
        private FileRepository FileRepository;
        JsonSerializerSettings settings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        public ExpenseController(DBContext context)
        {
            ExpenseRepository = new GenericRepository<Expense>(context);
            EmployeeRepository = new GenericRepository<Employee>(context);
            ProjectRepository = new GenericRepository<Project>(context);
            FileRepository = new FileRepository(context);
        }

        [HttpGet("All")]
        public async Task<ActionResult<string>> All()
        {
            var expense = await ExpenseRepository.GetAllAsync();
            return JsonConvert.SerializeObject(new { success = true, data = expense, message = "Expense Successfully Retrieved" }, settings);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<string>> Get(string id)
        {
            var expense = await ExpenseRepository.GetByIDAsync(id);
            if (expense == null)
            {
                return JsonConvert.SerializeObject(new { success = false, message = "Expense Not Found" }, settings);
            }
            return JsonConvert.SerializeObject(new { success = true, message = "Expense Successfully Retrieved", data = expense }, settings);
        }

        [HttpPost("Create")]
        public async Task<ActionResult<string>> Create(Expense template)
        {
            try{
                var expense = await ExpenseRepository.InsertAsync(template);
                return JsonConvert.SerializeObject(new { success = true, message = "Expense Created", data = expense }, settings);
        
            }catch(Exception e){
                return JsonConvert.SerializeObject(new { success = false, message = e.Message }, settings);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<string>> Update(string id, Expense template)
        {
            if (id != template.ExpenseId)
            {
                return JsonConvert.SerializeObject(new { success = false, message = "Expense ID Mismatch" }, settings);
            }
            await ExpenseRepository.UpdateAsync(template);
            try
            {
                await ExpenseRepository.SaveAsync();
                return JsonConvert.SerializeObject(new { success = true, message = "Expense Successfully Updated", data = template }, settings);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Exists(id))
                {
                    return JsonConvert.SerializeObject(new { success = false, message = "Expense Not Found" }, settings);
                }
                else
                {
                    throw;
                }
            }
        }

        [HttpPut("UpdateAndFetch/{id}")]
        public async Task<ActionResult<string>> UpdateAndFetchAll(string id, Expense template)
        {
            if (id != template.ExpenseId)
            {
                return JsonConvert.SerializeObject(new { success = false, message = "Expense ID Mismatch" }, settings);
            }
            await ExpenseRepository.UpdateAsync(template);
            try
            {
                await ExpenseRepository.SaveAsync();
                var expense = await ExpenseRepository.GetAllAsync();
                return JsonConvert.SerializeObject(new { success = true, message = "Expense Successfully Updated", data = expense }, settings);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Exists(id))
                {
                    return JsonConvert.SerializeObject(new { success = false, message = "Expense Not Found" }, settings);
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
            var expense = await ExpenseRepository.GetByIDAsync(id);
            if (expense == null)
            {
                return JsonConvert.SerializeObject(new { success = false, message = "Expense Not Found" }, settings);
            }
            await ExpenseRepository.DeleteAsync(expense);
            return JsonConvert.SerializeObject(new { success = true, message = "Expense Successfully Deleted" }, settings);
        }

        [HttpDelete("Delete")]
        public async Task<ActionResult<String>> Delete(Expense template)
        {
            var expense = await ExpenseRepository.GetByIDAsync(template.ExpenseId);
            if (expense == null)
            {
                return JsonConvert.SerializeObject(new { success = false, message = "Expense Not Found" }, settings);
            }
            await ExpenseRepository.DeleteAsync(expense);
            return JsonConvert.SerializeObject(new { success = true, message = "Expense Successfully Deleted" }, settings);
        }

        public bool Exists(string id)
        {
            return ExpenseRepository.GetByID(id) != null;
        }

        [HttpGet("Settings")]
        public string Settings()
        {
            Settings settings = new Settings();
            settings.ColumnSettings = new Dictionary<string, ColumnHeader>();
            settings.FieldSettings = new Dictionary<string, InputType>();

            settings.ColumnSettings.Add("ExpenseId", new ColumnHeader { displayHeader = "Expense Id" });
            settings.ColumnSettings.Add("ExpenseAmount", new ColumnHeader { displayHeader = "Expense Amount" });
            settings.ColumnSettings.Add("Status", new ColumnHeader { displayHeader = "Status" });
            settings.ColumnSettings.Add("ApprovalId", new ColumnHeader { displayHeader = "Approval Id" });
            settings.ColumnSettings.Add("ProjectId", new ColumnHeader { displayHeader = "Project Id" });

            settings.FieldSettings.Add("ExpenseId", new InputType { type = "text", displayLabel = "Expense Id", editable = false, primaryKey = true });
            settings.FieldSettings.Add("ExpenseAmount", new InputType { type = "number", displayLabel = "Expense Amount", editable = true, primaryKey = false });
            settings.FieldSettings.Add("ExpenseDesc", new InputType { type = "text", displayLabel = "Expense Description", editable = true, primaryKey = false });
            settings.FieldSettings.Add("ExpenseReceipt", new InputType { type = "file", displayLabel = "Expense Receipt", editable = true, primaryKey = false });

            settings.FieldSettings.Add("Status", new DropdownInputType
            {
                type = "dropdown",
                displayLabel = "Status",
                editable = true,
                primaryKey = false,
                options = new List<DropdownOption>{
                new DropdownOption { value = "Pending", label = "Pending" },
                new DropdownOption { value = "Sent", label = "Sent" },
                new DropdownOption { value = "Declined", label = "Declined" },
                new DropdownOption { value = "Approved", label = "Approved" }
            }
            });

            settings.FieldSettings.Add("DateOfExpense", new InputType { type = "datetime", displayLabel = "Date of Expense", editable = true, primaryKey = false });
            settings.FieldSettings.Add("DateOfSubmission", new InputType { type = "datetime", displayLabel = "Date of Submission", editable = true, primaryKey = false });
            settings.FieldSettings.Add("DateOfReimbursement", new InputType { type = "datetime", displayLabel = "Date of Reimbursement", editable = true, primaryKey = false });

            var employee = EmployeeRepository.GetAll();
            settings.FieldSettings.Add("ApprovalId", new DropdownInputType
            {
                type = "dropdown",
                displayLabel = "Approval Id",
                editable = true,
                primaryKey = false,
                options = employee.Select(x => new DropdownOption { value = x.UserId, label = x.username }).ToList()
            });

            var project = ProjectRepository.GetAll();
            settings.FieldSettings.Add("ProjectId", new DropdownInputType
            {
                type = "dropdown",
                displayLabel = "Project Id",
                editable = true,
                primaryKey = false,
                options = project.Select(x => new DropdownOption { value = x.ProjectId, label = x.ProjectName }).ToList()
            });

            return JsonConvert.SerializeObject(new { success = true, data = settings, message = "Settings Successfully Fetched" });

        }
    }
}
