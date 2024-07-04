using GymTracker.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymTracker.Infrastructure.Data.Config;

public class UserExerciseProgressConfiguration : IEntityTypeConfiguration<UserExerciseProgress>
{
    public void Configure(EntityTypeBuilder<UserExerciseProgress> builder)
    {
        // Set primary key
        builder.HasKey(uep => uep.Id);

        // Configure properties
        builder.Property(uep => uep.WorkoutCompleted).IsRequired();
        builder.Property(uep => uep.Set1Reps);
        builder.Property(uep => uep.Set2Reps);
        builder.Property(uep => uep.Set3Reps);
        builder.Property(uep => uep.Set4Reps);
        builder.Property(uep => uep.CompletionDate);

        // Configure relation many-to-one with UserWorkoutTemplate
        builder.HasOne(uep => uep.UserWorkoutTemplate)
            .WithMany(uwt => uwt.UserExerciseProgress)
            .HasForeignKey(uep => uep.UserWorkoutTemplateId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure relation many-to-one with TemplateExercise
        builder.HasOne(uep => uep.TemplateExercise)
            .WithMany()
            .HasForeignKey(uep => uep.TemplateExerciseId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}