using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<string> UploadFile(File file){
            return await FileRepository.UploadFile(file.FileName,file.FileUrl);
        }
    }
}