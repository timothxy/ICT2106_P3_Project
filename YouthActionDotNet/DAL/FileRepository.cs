using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
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
            var fileExtention = Path.GetExtension(template.FileName);
            var mime = new FileExtensionContentTypeProvider();
            mime.TryGetContentType(fileExtention, out string contentType);
            template.FileMIME = contentType;
            template.FileExt = fileExtention;
            await dbSet.AddAsync(template);
            context.SaveChanges();
            return template.FileId;
        }

        public async Task<Models.File> getFilePath(string fileId){
            var file = await dbSet.FindAsync(fileId);
            if(file == null){
                return null;
            }else{
                file.FileUrl = Path.Combine("uploads", file.FileName);
                return file;
            }
        }
    }
}