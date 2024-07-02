using GymTracker.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymTracker.Infrastructure.Data.Config;

public class UserWorkoutTemplateConfiguration : IEntityTypeConfiguration<UserWorkoutTemplate>
{
    public void Configure(EntityTypeBuilder<UserWorkoutTemplate> builder)
    {
        // Set primary key
        builder.HasKey(uwt => uwt.Id);

        // Configure properties
        builder.Property(uwt => uwt.StartDate).IsRequired();

        // Configure relation many-to-one with User
        builder.HasOne(uwt => uwt.User)
            .WithMany(u => u.UserWorkoutTemplates)
            .HasForeignKey(uwt => uwt.UserId);

        // Configure relation many-to-one with WorkoutTemplate
        builder.HasOne(uwt => uwt.WorkoutTemplate)
            .WithMany()
            .HasForeignKey(uwt => uwt.WorkoutTemplateId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure relation one-to-many with UserExerciseProgress
        builder.HasMany(uwt => uwt.UserExerciseProgress)
            .WithOne(uep => uep.UserWorkoutTemplate)
            .HasForeignKey(uep => uep.UserWorkoutTemplateId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
