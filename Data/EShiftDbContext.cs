using Microsoft.EntityFrameworkCore;
using eShift.Models;

namespace eShift.Data
{
    public class EShiftDbContext : DbContext
    {
        public EShiftDbContext(DbContextOptions<EShiftDbContext> options) : base(options) { }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Assistant> Assistants { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Container> Containers { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<JobStop> JobStops { get; set; }
        public DbSet<Load> Loads { get; set; }
        public DbSet<Lorry> Lorries { get; set; }
        public DbSet<Trip> Trips { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Admin>().ToTable("admin");
            modelBuilder.Entity<Branch>().ToTable("branch");
            modelBuilder.Entity<Driver>().ToTable("driver");
            modelBuilder.Entity<Container>().ToTable("container");
            modelBuilder.Entity<Lorry>().ToTable("lorry");
            modelBuilder.Entity<Trip>().ToTable("trip");
            modelBuilder.Entity<Job>().ToTable("job");
            modelBuilder.Entity<Assistant>().ToTable("assistant");
        }
    }
}
