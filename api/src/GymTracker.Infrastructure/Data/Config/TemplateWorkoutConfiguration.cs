using GymTracker.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymTracker.Infrastructure.Data.Config;

public class TemplateWorkoutConfiguration : IEntityTypeConfiguration<TemplateWorkout>
{
    public void Configure(EntityTypeBuilder<TemplateWorkout> builder)
    {
        // Set primary key
        builder.HasKey(tw => tw.Id);

        // Configure properties
        builder.Property(tw => tw.Name).IsRequired().HasMaxLength(100);

        // Configure relation many-to-one with TemplateWeek
        builder.HasOne(tw => tw.TemplateWeek)
               .WithMany(tw => tw.TemplateWorkouts)
               .HasForeignKey(tw => tw.TemplateWeekId)
               .OnDelete(DeleteBehavior.Cascade);

        // Configure relation one-to-many with TemplateExercise
        builder.HasMany(tw => tw.TemplateExercises)
               .WithOne(te => te.TemplateWorkout)
               .HasForeignKey(te => te.TemplateWorkoutId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
