using ContactsManager.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContactsManager.Infrastructure.Configurations;
public sealed class PersonConfigurations : IEntityTypeConfiguration<Person>
{
    public void Configure(EntityTypeBuilder<Person> builder)
    {
        builder.ToTable("Persons");

        builder.HasIndex(prop => prop.PersonId);

        builder.Property(prop => prop.PersonName)
            .HasMaxLength(25)
            .IsRequired(false);

        builder.Property(prop => prop.Gender)
            .HasConversion<string>()
            .IsRequired(false);

        builder.Property(prop => prop.DateOfBirth)
            .IsRequired(false);

        builder.Property(prop => prop.Address)
            .HasMaxLength(500)
            .IsRequired(false);

        builder.HasOne(prop => prop.Country)
            .WithMany(prop => prop.Persons)
            .HasForeignKey(prop => prop.CountryId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(prop => prop.PersonName);

        builder.HasIndex(prop => prop.Email);

        builder.HasIndex(prop => prop.Gender);

        builder.HasIndex(prop => prop.Address);

        builder.HasIndex(prop => prop.PersonId);

    }
}
