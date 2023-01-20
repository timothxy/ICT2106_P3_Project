using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using YouthActionDotNet.Data;
using YouthActionDotNet.Models;

namespace YouthActionDotNet.DAL{
    public class FileRepository: GenericRepository<File>{

        public FileRepository(DBContext context): base(context){
            this.context = context;
            this.dbSet = context.Set<File>();            
        }

        public async Task<string> UploadFile(string fileName, string fileUrl){
            File template = new File();
            template.FileName = fileName;
            template.FileUrl = fileUrl;
            await dbSet.AddAsync(template);
            context.SaveChanges();
            return template.FileId;
        }
    }
}