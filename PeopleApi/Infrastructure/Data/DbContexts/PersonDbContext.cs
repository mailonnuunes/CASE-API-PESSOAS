using Microsoft.EntityFrameworkCore;
using PeopleApi.Domain;

namespace PeopleApi.Infrastructure.Data.DbContexts
{
    public class PersonDbContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public PersonDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public DbSet<Person> People { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(
                _configuration.GetValue<string>("ConnectionStrings:ConnectionString"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(PersonDbContext).Assembly);
        }

    }
}
