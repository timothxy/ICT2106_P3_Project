using Microsoft.EntityFrameworkCore;
using YouthActionDotNet.Models;

namespace YouthActionDotNet.Data{
    public class DBContext : DbContext{
        public DBContext(DbContextOptions<DBContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<ServiceCenter> ServiceCenters { get; set; }
        public DbSet<Employee> Employee { get; set; }
        public DbSet<YouthActionDotNet.Models.Volunteer> Volunteer { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        modelBuilder.Entity<Employee>().ToTable("Employee")
            .HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId);
        modelBuilder.Entity<Volunteer>().ToTable("Volunteer")
            .HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId);
        modelBuilder.Entity<Volunteer>()
            .HasOne(e=> e.User)
            .WithMany()
            .HasForeignKey(e => e.ApprovedBy);
        modelBuilder.Entity<ServiceCenter>().ToTable("ServiceCenter")
            .HasOne(e => e.RegionalDirector)
            .WithMany()
            .HasForeignKey(e => e.RegionalDirectorId);
        }
    }
}