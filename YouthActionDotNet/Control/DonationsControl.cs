using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using YouthActionDotNet.Controllers;
using YouthActionDotNet.DAL;
using YouthActionDotNet.Data;
using YouthActionDotNet.Models;

namespace YouthActionDotNet.Control{

    public class DonationsControl: IUserInterfaceCRUD<Donations>
    {
        private GenericRepositoryIn<Donations> DonationsRepositoryIn;
        private GenericRepositoryOut<Donations> DonationsRepositoryOut;
        JsonSerializerSettings settings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        public DonationsControl(DBContext context)
        {
            DonationsRepositoryIn = new GenericRepositoryIn<Donations>(context);
            DonationsRepositoryOut = new GenericRepositoryOut<Donations>(context);
        }
        public async Task<ActionResult<string>> All()
        {
            var donations = await DonationsRepositoryOut.GetAllAsync();
            return JsonConvert.SerializeObject(new {success = true, data = donations}, settings);
        }
        public async Task<ActionResult<string>> Create(Donations template)
        {
            await DonationsRepositoryIn.InsertAsync(template);
            var createdDonations = await DonationsRepositoryOut.GetByIDAsync(template.DonationsId);
            return JsonConvert.SerializeObject(new {success = true, data = createdDonations}, settings);
        }
        public async Task<ActionResult<string>> Delete(string id)
        {
            var donations = await DonationsRepositoryOut.GetByIDAsync(id);
            if (donations == null)
            {
                return JsonConvert.SerializeObject(new {success = false, message = "Donations Not Found"}, settings);
            }

            await DonationsRepositoryIn.DeleteAsync(donations);
            return JsonConvert.SerializeObject(new {success = true, message = "Donations Successfully Deleted"}, settings);
        }
        public async Task<ActionResult<string>> Delete(Donations template)
        {
            var donations = await DonationsRepositoryOut.GetByIDAsync(template.DonationsId);
            if (donations == null)
            {
                return JsonConvert.SerializeObject(new {success = false, message = "Donations Not Found"}, settings);
            }

            await DonationsRepositoryIn.DeleteAsync(donations);
            return JsonConvert.SerializeObject(new {success = true, message = "Donations Successfully Deleted"}, settings);
        }

        public bool Exists(string id)
        {
            if (DonationsRepositoryOut.GetByIDAsync(id) != null)
            {
                return true;
            }
            return false;
        }
        public async Task<ActionResult<string>> Get(string id)
        {
            var donations = await DonationsRepositoryOut.GetByIDAsync(id);
            if (donations == null)
            {
                return JsonConvert.SerializeObject(new {success = false, message = "Donations Not Found"}, settings);
            }
            return JsonConvert.SerializeObject(new {success = true, data = donations}, settings);
        }

        public string Settings()
        {   
            //Todo: Add settings
            return JsonConvert.SerializeObject(new {success = true, data = settings}, settings);
        }
        public async Task<ActionResult<string>> Update(string id, Donations template)
        {
            if(id != template.DonationsId)
            {
                return JsonConvert.SerializeObject(new {success = false, message = "Donations ID Mismatch"}, settings);
            }
            DonationsRepositoryIn.Update(template);
            try{
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
        public async Task<ActionResult<string>> UpdateAndFetchAll(string id, Donations template)
        {
            if(id != template.DonationsId)
            {
                return JsonConvert.SerializeObject(new {success = false, message = "Donations ID Mismatch"}, settings);
            }
            DonationsRepositoryIn.Update(template);
            try{
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