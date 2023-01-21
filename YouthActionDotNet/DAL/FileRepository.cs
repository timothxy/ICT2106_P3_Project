using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using YouthActionDotNet.Data;
using YouthActionDotNet.Models;

namespace YouthActionDotNet.DAL{
    public class FileRepository: GenericRepository<Models.File>{

        public FileRepository(DBContext context): base(context){
            this.context = context;
            this.dbSet = context.Set<Models.File>();            
        }

        public async Task<string> UploadFile(string fileName, string fileUrl){
            Models.File template = new Models.File();
            template.FileName = fileName;
            template.FileUrl = fileUrl;
            await dbSet.AddAsync(template);
            context.SaveChanges();
            return template.FileId;
        }

        public IActionResult getFilePath(string fileId){
            var file = (Models.File)dbSet.Find(fileId);
            var filePath = Path.Combine("uploads", file.FileName);
            if(file == null){
                return null;
            }else{
                var FileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                string mimeType = new
                Microsoft.AspNetCore.StaticFiles.FileExtensionContentTypeProvider().Mappings[Path.GetExtension(file.FileName)];
                return new FileStreamResult(FileStream, mimeType);

            }
        }
    }
}