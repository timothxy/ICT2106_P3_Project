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
            string hashedPassword = u.hashpassword(password);
            var user = await dbSet.FirstOrDefaultAsync(u => u.username == username && u.Password == hashedPassword);
            if (user == null)
                return null;
            return user;
        }

        public virtual async Task<User> Register(string username, string password){
            
            System.Diagnostics.Debug.WriteLine(username);
            System.Diagnostics.Debug.WriteLine(password);

            Console.WriteLine(username);
            Console.WriteLine(password);

            string hashedPassword = u.hashpassword(password);
            User template = new User();
            template.username = username;
            template.Password = hashedPassword;
            
            System.Diagnostics.Debug.WriteLine(template.UserId);
            System.Diagnostics.Debug.WriteLine(template.username);
            System.Diagnostics.Debug.WriteLine(template.Password);

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