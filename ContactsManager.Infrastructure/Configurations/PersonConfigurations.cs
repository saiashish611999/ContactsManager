using ContactsManager.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContactsManager.Infrastructure.Configurations;
public sealed class PersonConfigurations : IEntityTypeConfiguration<Person>
{
    public void Configure(EntityTypeBuilder<Person> builder)
    {
        builder.ToTable("Persons");

        builder.HasKey(p => p.PersonId);

        builder.Property(p => p.PersonName)
            .HasMaxLength(50)
            .IsRequired(true);

        builder.Property(p => p.EmailAddress)
            .HasMaxLength(50)
            .IsRequired(true);

        builder.Property(p => p.Address)
            .HasMaxLength(500)
            .IsRequired(false);

        builder.Property(p => p.Gender)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired(false);

        builder.HasOne(p => p.Country)
            .WithMany(p => p.Persons)
            .HasForeignKey(p => p.CountryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(p => p.PersonName);

        builder.HasIndex(p => p.EmailAddress);

        builder.HasIndex(p => p.Address);


    }
}
