using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using YouthActionDotNet.Data;
using YouthActionDotNet.Models;

namespace YouthActionDotNet.DAL
{
    public class UserRepository : GenericRepository<User>
    {   
        public UserRepository(DBContext context) : base(context)
        {
            this.context = context;
            this.dbSet = context.Set<User>();
        }

        public virtual async Task<User> Login(string username, string password)
        {
            string hashedPassword = Utils.hashpassword(password);
            var user = await dbSet.FirstOrDefaultAsync(u => u.username == username && u.Password == hashedPassword);
            if (user == null)
                return null;
            return user;
        }

        public virtual async Task<User> Register(string username, string password){
            
            string hashedPassword = Utils.hashpassword(password);
            User template = new User();
            template.username = username;
            template.Password = hashedPassword;

            var user = await dbSet.FirstOrDefaultAsync(u => u.username == template.username);
            if (user == null){
                dbSet.Add(template);
                context.SaveChanges();
                return await dbSet.FirstOrDefaultAsync(u => u.username == template.username && u.Password == hashedPassword);
            }
            return null;
        }
    }
}