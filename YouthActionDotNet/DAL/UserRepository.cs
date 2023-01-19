using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using YouthActionDotNet.Data;
using YouthActionDotNet.Models;

namespace YouthActionDotNet.DAL
{
    public class UserRepository : GenericRepository<User>
    {   
        internal DBContext context;
        internal DbSet<User> dbSet;

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
    
    }
}