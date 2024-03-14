using GymTracker.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymTracker.Infrastructure.Data.Config
{
    public class WorkoutConfiguration : IEntityTypeConfiguration<Workout>
    {
        public void Configure(EntityTypeBuilder<Workout> builder)
        {
            builder.HasKey(w => w.Id);
            builder.Property(w => w.Notes).HasColumnType("nvarchar(max)");
            builder.Property(w => w.Date).IsRequired();

            // Configurarea relației one-to-many cu User
            builder.HasOne(w => w.User)
                   .WithMany(u => u.Workouts)
                   .HasForeignKey(w => w.UserId);
        }
    }
}
