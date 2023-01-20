using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using YouthActionDotNet.Data;
using YouthActionDotNet.Models;
using YouthActionDotNet.DAL;

namespace YouthActionDotNet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase, IUserInterfaceCRUD<User>
    {
        private RepositoryReference repositoryReference;

        public UserController(DBContext context)
        {
            repositoryReference = new RepositoryReference(context);
        }
        
        public bool Exists(string id)
        {
            return repositoryReference.DonorRepository.GetByID(id) != null;
        }

        // To login the user
        // POST: api/User/Login
        [HttpPost("Login")]
        public async Task<ActionResult<String>> LoginUser(User user)
        {
            var validLoginUser = await repositoryReference.UserRepository.Login(user.username, user.Password);
            if (validLoginUser == null)
            {
                return JsonConvert.SerializeObject(new { success = false, message = "Invalid Username or Password" });
            }
            return JsonConvert.SerializeObject(new { success = true, message = "Login Successful", data=validLoginUser });
        }
        

        [HttpPost("Create")]
        public async Task<ActionResult<string>> Create(User template)
        {
            var users = await repositoryReference.UserRepository.GetAllAsync();
            var existingUser = users.FirstOrDefault(u => u.username == template.username);
            if(existingUser != null){
                return JsonConvert.SerializeObject(new { success = false, message = "User Already Exists" });
            }
            
            var createdUser = await repositoryReference.UserRepository.Register(template.username, template.Password);
            if(createdUser == null){
                return JsonConvert.SerializeObject(new { success = false, message = "Unexpected Error" });
            }
            return JsonConvert.SerializeObject(new { success = true, data = template, message = "User Successfully Created" });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<string>> Get(string id)
        {
            var user = await repositoryReference.UserRepository.GetByIDAsync(id);
            if (user == null)
            {
                return JsonConvert.SerializeObject(new { success = false, message = "User Not Found" });
            }
            return JsonConvert.SerializeObject(new { success = true, data = user, message = "User Successfully Retrieved" });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<string>> Update(string id, User template)
        {
            if (id != template.UserId)
            {
                return JsonConvert.SerializeObject(new { success = false, data = "", message = "Volunteer Id Mismatch" });
            }
            repositoryReference.UserRepository.Update(template);
            try
            {
                await repositoryReference.UserRepository.SaveAsync();
                return await Get(id);
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
        public async Task<ActionResult<string>> UpdateAndFetchAll(string id, User template)
        {
            if (id != template.UserId)
            {
                return JsonConvert.SerializeObject(new { success = false, data = "", message = "Volunteer Id Mismatch" });
            }
            repositoryReference.UserRepository.Update(template);
            try
            {
                await repositoryReference.UserRepository.SaveAsync();
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
            var user = await repositoryReference.UserRepository.GetByIDAsync(id);
            if (user == null)
            {
                return JsonConvert.SerializeObject(new { success = false, data = "", message = "User Not Found" });
            }
            repositoryReference.UserRepository.Delete(user);
            repositoryReference.Commit();
            return JsonConvert.SerializeObject(new { success = true, data = "", message = "User Successfully Deleted" });
        }

        [HttpGet("All")]
        public async Task<ActionResult<string>> All()
        {
            var users = await repositoryReference.UserRepository.GetAllAsync();
            return JsonConvert.SerializeObject(new { success = true, data = users, message = "Users Successfully Retrieved" });
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

            return JsonConvert.SerializeObject(new { success = true, data = settings, message = "Settings Successfully Retrieved" });
        }
    }
}
