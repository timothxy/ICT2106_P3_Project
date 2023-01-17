using Microsoft.EntityFrameworkCore;
using YouthActionDotNet.Models;

namespace YouthActionDotNet.Data{
    public class DBContext : DbContext{
        public DBContext(DbContextOptions<DBContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<ServiceCenter> ServiceCenters { get; set; }
        public DbSet<Employee> Employee { get; set; }
    }
}