using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using YouthActionDotNet.DAL;
using YouthActionDotNet.Data;
using YouthActionDotNet.Models;

namespace YouthActionDotNet.Controllers{

    [Route("api/[controller]")]
    [ApiController]
    public class DonationsController : ControllerBase, IUserInterfaceCRUD<Donations>
    {
        private GenericRepository<Donations> DonationsRepository;
        JsonSerializerSettings settings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        public DonationsController(DBContext context)
        {
            DonationsRepository = new GenericRepository<Donations>(context);
        }
        [HttpGet("All")]
        public async Task<ActionResult<string>> All()
        {
            var donations = await DonationsRepository.GetAllAsync();
            return JsonConvert.SerializeObject(new {success = true, data = donations}, settings);
        }

        [HttpPost("Create")]
        public async Task<ActionResult<string>> Create(Donations template)
        {
            await DonationsRepository.InsertAsync(template);
            var createdDonations = await DonationsRepository.GetByIDAsync(template.DonationId);
            return JsonConvert.SerializeObject(new {success = true, data = createdDonations}, settings);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<string>> Delete(string id)
        {
            var donations = await DonationsRepository.GetByIDAsync(id);
            if (donations == null)
            {
                return JsonConvert.SerializeObject(new {success = false, message = "Donations Not Found"}, settings);
            }

            await DonationsRepository.DeleteAsync(donations);
            return JsonConvert.SerializeObject(new {success = true, message = "Donations Successfully Deleted"}, settings);
        }

        [HttpDelete("Delete")]
        public async Task<ActionResult<string>> Delete(Donations template)
        {
            var donations = await DonationsRepository.GetByIDAsync(template.DonationId);
            if (donations == null)
            {
                return JsonConvert.SerializeObject(new {success = false, message = "Donations Not Found"}, settings);
            }

            await DonationsRepository.DeleteAsync(donations);
            return JsonConvert.SerializeObject(new {success = true, message = "Donations Successfully Deleted"}, settings);
        }

        public bool Exists(string id)
        {
            if (DonationsRepository.GetByIDAsync(id) != null)
            {
                return true;
            }
            return false;
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult<string>> Get(string id)
        {
            var donations = await DonationsRepository.GetByIDAsync(id);
            if (donations == null)
            {
                return JsonConvert.SerializeObject(new {success = false, message = "Donations Not Found"}, settings);
            }
            return JsonConvert.SerializeObject(new {success = true, data = donations}, settings);
        }

        public string Settings()
        {
            return JsonConvert.SerializeObject(new {success = true, data = settings}, settings);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult<string>> Update(string id, Donations template)
        {
            if(id != template.DonationId)
            {
                return JsonConvert.SerializeObject(new {success = false, message = "Donations ID Mismatch"}, settings);
            }
            DonationsRepository.Update(template);
            try{
                await DonationsRepository.SaveAsync();
                return await Get(id);
            }catch (DbUpdateConcurrencyException)
            {
                if (!Exists(id))
                {
                    return JsonConvert.SerializeObject(new {success = false, message = "Donations Not Found"}, settings);
                }
                else
                {
                    throw;
                }
            }
        }
        [HttpPut("UpdateAndFetch/{id}")]
        public async Task<ActionResult<string>> UpdateAndFetchAll(string id, Donations template)
        {
            if(id != template.DonationId)
            {
                return JsonConvert.SerializeObject(new {success = false, message = "Donations ID Mismatch"}, settings);
            }
            DonationsRepository.Update(template);
            try{
                await DonationsRepository.SaveAsync();
                return await All();
            }catch (DbUpdateConcurrencyException)
            {
                if (!Exists(id))
                {
                    return JsonConvert.SerializeObject(new {success = false, message = "Donations Not Found"}, settings);
                }
                else
                {
                    throw;
                }
            }
        }
    }
}