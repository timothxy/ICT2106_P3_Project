using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using YouthActionDotNet.DAL;
using YouthActionDotNet.Data;
using YouthActionDotNet.Models;

namespace YouthActionDotNet.Controllers{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController: ControllerBase{
        
        private FileRepository FileRepository;
        public FileController(DBContext context){
            FileRepository = new FileRepository(context);
        }

        [HttpPost("Upload")]
        public async Task<string> UploadFile(){
            try{
                var form = await Request.ReadFormAsync();
                var file = form.Files.FirstOrDefault();
                System.Diagnostics.Debug.WriteLine(file.FileName);

                Console.WriteLine(file.FileName);

                if(file != null)
                {
                    var filePath = Path.Combine("uploads", file.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create)){
                        await file.OpenReadStream().CopyToAsync(stream);
                    }
                    
                    var fileId = await FileRepository.UploadFile(file.FileName,filePath);
                    return JsonConvert.SerializeObject(new { success = true, message = "File uploaded successfully", data = fileId });
                }
                return JsonConvert.SerializeObject(new { success = false, message = "No file found" });
            }catch(Exception e){
                System.Diagnostics.Debug.WriteLine(e.Message);
                Console.WriteLine(e.Message);
                return JsonConvert.SerializeObject(new { success = false, message = e.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<string> GetFilePath(string id){
            try{
                var filePath = FileRepository.getFilePath(id);
                if(filePath == null)
                    return JsonConvert.SerializeObject(new { success = false, message = "File Does not exist" });
                else
                    return JsonConvert.SerializeObject(new { success = true, message = "File path retrieved successfully", data = filePath });
            }catch(Exception e){
                return JsonConvert.SerializeObject(new { success = false, message = e.Message });
            }
        }
    }
}