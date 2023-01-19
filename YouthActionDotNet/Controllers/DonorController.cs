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
    public class DonorController : ControllerBase,IUserInterfaceCRUD<Donor>
    {
        private UnitOfWork unitOfWork;
        JsonSerializerSettings settings = new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };


        public DonorController(DBContext context)
        {
            unitOfWork = new UnitOfWork(context);
        }

        public bool Exists(string id)
        {
            return unitOfWork.DonorRepository.GetByID(id) != null;
        }
        
        [HttpPost("Create")]
        public async Task<ActionResult<string>> Create(Donor donor)
        {
            var donors = await unitOfWork.DonorRepository.GetAllAsync();
            var existingDonor = donors.FirstOrDefault(d => d.UserId == donor.UserId);
            if(existingDonor != null){
                return JsonConvert.SerializeObject(new { success = false, message = "Donor Already Exists" });
            }
            donor.UserId = Guid.NewGuid().ToString();
            donor.Password = u.hashpassword(donor.Password);
            await unitOfWork.DonorRepository.InsertAsync(donor);
            unitOfWork.Commit();
            var createdDonor = await unitOfWork.DonorRepository.GetByIDAsync(donor.UserId);
            return JsonConvert.SerializeObject(new { success = true, data = donor, message = "Donor Successfully Created" });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<string>> Get(string id)
        {   
            var donor = await unitOfWork.DonorRepository.GetByIDAsync(id);
            if (donor == null)
            {
                return JsonConvert.SerializeObject(new {success=false,message="Donor Not Found"},settings);
            }
            return JsonConvert.SerializeObject(new {success=true,data=donor},settings);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<string>> Update(string id, Donor donor)
        {
            if(id != donor.UserId){
                return JsonConvert.SerializeObject(new { success = false, data = "", message = "Donor Id Mismatch" });
            }
            unitOfWork.DonorRepository.Update(donor);
            try{
                unitOfWork.Commit();
                return await Get(id);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Exists(id))
                {
                    return JsonConvert.SerializeObject(new { success = false, data = "", message = "Donor Not Found" });
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
                return JsonConvert.SerializeObject(new { success = false, data = "", message = "Donor Id Mismatch" });
            }
            unitOfWork.DonorRepository.Update(template);
            try{
                unitOfWork.Commit();
                return await All();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Exists(id))
                {
                    return JsonConvert.SerializeObject(new { success = false, data = "", message = "Donor Not Found" });
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
            var donor = await unitOfWork.DonorRepository.GetByIDAsync(id);
            if (donor == null)
            {
                return JsonConvert.SerializeObject(new { success = false, data = "", message = "Donor Not Found" });
            }
            unitOfWork.DonorRepository.Delete(donor);
            unitOfWork.Commit();
            return JsonConvert.SerializeObject(new { success = true, data = "", message = "Donor Successfully Deleted" });
        }

        [HttpGet("All")]
        public async Task<ActionResult<string>> All()
        {
            var donors = await unitOfWork.DonorRepository.GetAllAsync();
            return JsonConvert.SerializeObject(new { success = true, data = donors }, settings);
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

            settings.FieldSettings.Add("donorName", new InputType { type = "text", displayLabel = "Donor Name", editable = true, primaryKey = false });
            settings.FieldSettings.Add("donorType", new DropdownInputType { type = "dropdown", displayLabel = "Donor Type", editable = true, primaryKey = false, options = new List<DropdownOption> {
                new DropdownOption { value = "Individual", label = "Individual" },
                new DropdownOption { value = "Organization", label = "Organization" },
            } });
            return JsonConvert.SerializeObject(new {success=true,message="Settings Fetched", data = settings});

        }
    }
}

