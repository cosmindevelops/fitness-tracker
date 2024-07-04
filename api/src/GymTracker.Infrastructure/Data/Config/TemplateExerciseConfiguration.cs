using GymTracker.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymTracker.Infrastructure.Data.Config;

public class TemplateExerciseConfiguration : IEntityTypeConfiguration<TemplateExercise>
{
    public void Configure(EntityTypeBuilder<TemplateExercise> builder)
    {
        // Set primary key
        builder.HasKey(te => te.Id);

        // Configure properties
        builder.Property(te => te.ExerciseName).IsRequired().HasMaxLength(100);
        builder.Property(te => te.LastSetIntensity).HasMaxLength(100);
        builder.Property(te => te.WarmupSets).HasMaxLength(50);
        builder.Property(te => te.WorkingSets).HasMaxLength(50);
        builder.Property(te => te.Reps).HasMaxLength(50);
        builder.Property(te => te.Rpe).HasMaxLength(50);
        builder.Property(te => te.Rest).HasMaxLength(50);
        builder.Property(te => te.Substitution1).HasMaxLength(100);
        builder.Property(te => te.Substitution2).HasMaxLength(100);
        builder.Property(te => te.Notes).HasMaxLength(500);

        // Configure relation many-to-one wiyh TemplateWorkout
        builder.HasOne(te => te.TemplateWorkout)
               .WithMany(tw => tw.TemplateExercises)
               .HasForeignKey(te => te.TemplateWorkoutId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}