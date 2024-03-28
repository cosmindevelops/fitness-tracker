using GymTracker.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymTracker.Infrastructure.Data.Config;

public class SeriesConfiguration : IEntityTypeConfiguration<Series>
{
    public void Configure(EntityTypeBuilder<Series> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Repetitions).IsRequired();
        builder.Property(s => s.RPE).IsRequired();

        builder.HasOne(s => s.Exercise)
               .WithMany(e => e.Series)
               .HasForeignKey(s => s.ExerciseId);
    }
}