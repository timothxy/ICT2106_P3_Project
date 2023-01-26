using System;
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
    public class PermissionsController : ControllerBase, IUserInterfaceCRUD<Permissions>
    {
        private PermissionsRepository PermissionRepository;

        public PermissionsController(DBContext context)
        {
            PermissionRepository = new PermissionsRepository(context);
        }
        [HttpGet("All")]
        public async Task<ActionResult<string>> All()
        {
            var permissions = await PermissionRepository.GetAllAsync();
            return JsonConvert.SerializeObject(new { success = true, data = permissions, message = "Permissions Successfully Retrieved" });
        }
        [HttpPost("Create")]
        public async Task<ActionResult<string>> Create(Permissions template)
        {
            Console.WriteLine(template.Role);
            Console.WriteLine(template.Permission);
            var existingRole = await PermissionRepository.GetByRole(template.Role);
            if (existingRole != null)
            {
                return JsonConvert.SerializeObject(new { success = false, message = "Role Already Exists" });
            }
            var permission = await PermissionRepository.InsertAsync(template);
            return JsonConvert.SerializeObject(new { success = true, message = "Permission Created", data = permission });
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult<string>> Delete(string id)
        {
            var permission = PermissionRepository.GetByID(id);
            if (permission == null)
            {
                return JsonConvert.SerializeObject(new { success = false, message = "Permission Not Found" });
            }
            await PermissionRepository.DeleteAsync(permission);
            return JsonConvert.SerializeObject(new { success = true, message = "Permission Deleted" });
        }

        [HttpDelete("delete")]
        public async Task<ActionResult<string>> Delete(Permissions template)
        {
            var permission = PermissionRepository.GetByID(template.Id);
            if (permission == null)
            {
                return JsonConvert.SerializeObject(new { success = false, message = "Permission Not Found" });
            }
            await PermissionRepository.DeleteAsync(permission);
            return JsonConvert.SerializeObject(new { success = true, message = "Permission Deleted" });
        }

        public bool Exists(string id)
        {
            if (PermissionRepository.GetByID(id) != null)
            {
                return true;
            }
            return false;
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<string>> Get(string id)
        {
            var permission = await PermissionRepository.GetByIDAsync(id);
            if (permission == null)
            {
                return JsonConvert.SerializeObject(new { success = false, message = "Permission Not Found" });
            }
            return JsonConvert.SerializeObject(new { success = true, message = "Permission Successfully Retrieved", data = permission });
        }

        [HttpGet("GetPermissions/{role}")]
        public async Task<ActionResult<string>> GetByRole(string role)
        {
            var permission = await PermissionRepository.GetByRole(role);
            if (permission == null)
            {
                return JsonConvert.SerializeObject(new { success = false, message = "Permission Not Found" });
            }
            return JsonConvert.SerializeObject(new { success = true, message = "Permission Successfully Retrieved", data = permission });
        }
        [HttpGet("Settings")]
        public string Settings()
        {
            Settings settings = new Settings();
            settings.ColumnSettings = new Dictionary<string, ColumnHeader>();
            settings.FieldSettings = new Dictionary<string, InputType>();

            settings.ColumnSettings.Add("Id", new ColumnHeader{ displayHeader = "Permission ID"});
            settings.ColumnSettings.Add("Role", new ColumnHeader{ displayHeader = "Role"});
            
            settings.FieldSettings.Add("Id", new InputType{ type = "text", displayLabel="Permission ID", editable = false, primaryKey = true});
            settings.FieldSettings.Add("Role", new InputType{ type = "text", displayLabel="Role", editable = true, primaryKey = false});
        
            return JsonConvert.SerializeObject(new { success = true, message = "Settings Retrieved", data = settings });
        }
        [HttpPut("{id}")]
        public async Task<ActionResult<string>> Update(string id, Permissions template)
        {
            if(id != template.Id){
                return JsonConvert.SerializeObject(new { success = false, message = "Permission Id Mismatch" });
            }
            await PermissionRepository.UpdateAsync(template);
            try{
                await PermissionRepository.SaveAsync();
                return JsonConvert.SerializeObject(new { success = true, message = "Permission Updated", data = template });
            }catch(DbUpdateConcurrencyException){
                if(!Exists(id)){
                    return JsonConvert.SerializeObject(new { success = false, message = "Permission Not Found" });
                }else{
                    throw;
                }
            }
        }
        [HttpPut("UpdateAndFetch/{id}")]
        public async Task<ActionResult<string>> UpdateAndFetchAll(string id, Permissions template)
        {
            if(id != template.Id){
                return JsonConvert.SerializeObject(new { success = false, message = "Permission Id Mismatch" });
            }
            await PermissionRepository.UpdateAsync(template);
            try{
                await PermissionRepository.SaveAsync();
                var permissions = await PermissionRepository.GetAllAsync();
                return JsonConvert.SerializeObject(new { success = true, message = "Permission Updated", data = permissions });
            }catch(DbUpdateConcurrencyException){
                if(!Exists(id)){
                    return JsonConvert.SerializeObject(new { success = false, message = "Permission Not Found" });
                }else{
                    throw;
                }
            }
        }
    }
}