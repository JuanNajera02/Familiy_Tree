using Entities.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class PersonsMap : IEntityTypeConfiguration<Persons>
{
    public void Configure(EntityTypeBuilder<Persons> builder)
    {
        builder.HasKey(p => p.Id);

        // Configuración de la relación uno a uno con Partner
        builder.HasOne(p => p.Partner)
            .WithOne()
            .HasForeignKey<Persons>(p => p.PartnerId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configuración de la relación uno a muchos con Relationships como padre
        builder.HasMany(p => p.FatherRelationships)
            .WithOne(r => r.Father)
            .HasForeignKey(r => r.FatherId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configuración de la relación uno a muchos con Relationships como madre
        builder.HasMany(p => p.MotherRelationships)
            .WithOne(r => r.Mother)
            .HasForeignKey(r => r.MotherId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configuración de la relación uno a muchos con Relationships como hijo
        builder.HasMany(p => p.ChildRelationships)
            .WithOne(r => r.Child)
            .HasForeignKey(r => r.ChildId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.ToTable("Persons");
    }
}
