using Entities.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class RelationshipsMap : IEntityTypeConfiguration<Relationships>
{
    public void Configure(EntityTypeBuilder<Relationships> builder)
    {
        builder.HasKey(r => r.Id);

        // Configuración de la relación con Persons como padre
        builder.HasOne(r => r.Father)
            .WithMany(p => p.FatherRelationships)
            .HasForeignKey(r => r.FatherId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configuración de la relación con Persons como madre
        builder.HasOne(r => r.Mother)
            .WithMany(p => p.MotherRelationships)
            .HasForeignKey(r => r.MotherId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configuración de la relación con Persons como hijo
        builder.HasOne(r => r.Child)
            .WithMany(p => p.ChildRelationships)
            .HasForeignKey(r => r.ChildId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.ToTable("Relationships");
    }
}
