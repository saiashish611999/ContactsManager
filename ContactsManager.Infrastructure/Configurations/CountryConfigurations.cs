using ContactsManager.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContactsManager.Infrastructure.Configurations;
public sealed class CountryConfigurations : IEntityTypeConfiguration<Country>
{
    public void Configure(EntityTypeBuilder<Country> builder)
    {
        builder.ToTable("Countries");

        builder.HasKey(c => c.CountryId);

        builder.Property(c => c.CountryName)
            .HasMaxLength(25)
            .IsRequired(false);

        builder.HasIndex(c => c.CountryId);

        builder.HasIndex(c => c.CountryName);
    }
}
