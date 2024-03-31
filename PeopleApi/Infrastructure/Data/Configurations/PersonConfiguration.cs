using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PeopleApi.Domain;

namespace PeopleApi.Infrastructure.Data.Configurations
{
    public class PersonConfiguration : IEntityTypeConfiguration<Person>
    {
        public void Configure(EntityTypeBuilder<Person> builder)
        {
            builder.ToTable("People");
            builder.Property(x => x.Id).HasColumnType("INTEGER").ValueGeneratedOnAdd();
            builder.Property(p => p.Name).HasColumnType("VARCHAR(50)").IsRequired();
            builder.Property(p => p.Age).HasColumnType("INT").IsRequired().HasMaxLength(150);
            builder.Property(p => p.Email).IsRequired().HasColumnType("VARCHAR(150)");

        }

        
    }
}
