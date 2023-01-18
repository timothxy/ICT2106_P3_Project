using Microsoft.EntityFrameworkCore;
using YouthActionDotNet.Models;

namespace YouthActionDotNet.Data{
    public class DBContext : DbContext{
        public DBContext(DbContextOptions<DBContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<ServiceCenter> ServiceCenters { get; set; }
        public DbSet<Employee> Employee { get; set; }
        public DbSet<Volunteer> Volunteer { get; set; }
        public DbSet<Donor> Donor { get; set; }
        public DbSet<VolunteerWork> VolunteerWork { get; set; }
        public DbSet<Project> Project { get; set; }
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
        modelBuilder.Entity<Donor>().ToTable("Donor")
            .HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId);
        modelBuilder.Entity<ServiceCenter>().ToTable("ServiceCenter")
            .HasOne(e => e.RegionalDirector)
            .WithMany()
            .HasForeignKey(e => e.RegionalDirectorId);
        modelBuilder.Entity<VolunteerWork>().ToTable("VolunteerWork")
            .HasOne(e => e.employee)
            .WithMany()
            .HasForeignKey(e => e.SupervisingEmployee);
        modelBuilder.Entity<VolunteerWork>().ToTable("VolunteerWork")
            .HasOne(e => e.volunteer)
            .WithMany()
            .HasForeignKey(e => e.VolunteerId);
        modelBuilder.Entity<VolunteerWork>().ToTable("VolunteerWork")
            .HasOne(e => e.project)
            .WithMany()
            .HasForeignKey(e => e.projectId);
        modelBuilder.Entity<Project>().ToTable("Project")
            .HasOne(e => e.ServiceCenter)
            .WithMany()
            .HasForeignKey(e => e.ServiceCenterId);
        }
    }
}