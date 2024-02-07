﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Monstersoft.VennWms.Main.Domain.Entities.DepositorEntities;

namespace Monstersoft.VennWms.Main.Persistance.EntityConfigurations.Configurations.DepositorEntityConfigurations;

public class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        #region Tablo Tanımları
        builder.ToTable("Companies").HasKey(p => p.Id);
        #endregion

        #region Alan Tanımları
        builder.Property(p => p.Id).HasColumnName("Id").IsRequired();
        builder.Property(p => p.Code).HasColumnName("Code").HasMaxLength(30).IsRequired();
        builder.Property(p => p.Name).HasColumnName("Name").HasMaxLength(120).IsRequired();
        builder.Property(p => p.Description).HasColumnName("Description").HasMaxLength(120);
        builder.Property(p => p.TaxOffice).HasColumnName("TaxOffice").HasMaxLength(30);
        builder.Property(p => p.TaxNumber).HasColumnName("TaxNumber").HasMaxLength(30);
        builder.Property(p => p.PhoneNumber).HasColumnName("PhoneNumber").HasMaxLength(15);
        builder.Property(p => p.FaxNumber).HasColumnName("FaxNumber").HasMaxLength(15);
        builder.Property(p => p.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(p => p.UpdatedDate).HasColumnName("UpdatedDate");
        builder.Property(p => p.DeletedDate).HasColumnName("DeletedDate");
        #endregion

        #region Indexler
        builder.HasIndex(indexExpression: p => p.Code, name: "UK_Companies_Code").IsUnique();
        builder.HasIndex(indexExpression: p => p.Name, name: "UK_Companies_Name").IsUnique();
        #endregion

        #region İlişki Tanımları
        builder.HasMany(p => p.Customers);
        builder.HasMany(p => p.Disturbitors);
        builder.HasMany(p => p.Suppliers);
        #endregion

        #region Filtreler
        builder.HasQueryFilter(p => !p.DeletedDate.HasValue);
        #endregion
    }
}