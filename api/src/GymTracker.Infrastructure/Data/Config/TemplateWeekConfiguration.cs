using GymTracker.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymTracker.Infrastructure.Data.Config;

public class TemplateWeekConfiguration : IEntityTypeConfiguration<TemplateWeek>
{
    public void Configure(EntityTypeBuilder<TemplateWeek> builder)
    {
        // Set primary key
        builder.HasKey(tw => tw.Id);

        // Configure properties
        builder.Property(tw => tw.WeekNumber).IsRequired();

        // Configure relation many-to-one with WorkoutTemplate
        builder.HasOne(tw => tw.WorkoutTemplate)
               .WithMany(wt => wt.TemplateWeeks)
               .HasForeignKey(tw => tw.WorkoutTemplateId)
               .OnDelete(DeleteBehavior.Cascade);

        // Configure relation one-to-many with TemplateWorkout
        builder.HasMany(tw => tw.TemplateWorkouts)
               .WithOne(tw => tw.TemplateWeek)
               .HasForeignKey(tw => tw.TemplateWeekId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}