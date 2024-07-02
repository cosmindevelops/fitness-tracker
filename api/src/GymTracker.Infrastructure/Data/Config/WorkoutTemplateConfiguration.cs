using GymTracker.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymTracker.Infrastructure.Data.Config;

public class WorkoutTemplateConfiguration : IEntityTypeConfiguration<WorkoutTemplate>
{
    public void Configure(EntityTypeBuilder<WorkoutTemplate> builder)
    {
        // Set primary key
        builder.HasKey(wt => wt.Id);

        // Configure properties
        builder.Property(wt => wt.Name).IsRequired().HasMaxLength(100);
        builder.Property(wt => wt.DurationWeeks).IsRequired();
        builder.Property(wt => wt.Description).IsRequired().HasMaxLength(500);

        // Configure relation one-to-many with TemplateWeek
        builder.HasMany(wt => wt.TemplateWeeks)
               .WithOne(wt => wt.WorkoutTemplate)
               .HasForeignKey(wt => wt.WorkoutTemplateId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}