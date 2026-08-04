using ContactsManager.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContactsManager.Infrastructure.Configurations;
public class CountryConfigurations : IEntityTypeConfiguration<Country>
{
    public void Configure(EntityTypeBuilder<Country> builder)
    {
        builder.ToTable("Countries");

        builder.HasKey(p => p.CountryId);

        builder.Property(p => p.CountryName)
            .IsRequired(true)
            .HasMaxLength(50);


        builder.HasIndex(p => p.CountryName);
    }
}
