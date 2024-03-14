using GymTracker.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymTracker.Infrastructure.Data.Config
{
    public class ExerciseConfiguration : IEntityTypeConfiguration<Exercise>
    {
        public void Configure(EntityTypeBuilder<Exercise> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Name).IsRequired().HasMaxLength(100);

            // Configurarea relației one-to-many cu Workout
            builder.HasOne(e => e.Workout)
                   .WithMany(w => w.Exercises)
                   .HasForeignKey(e => e.WorkoutId);
        }
    }
}
