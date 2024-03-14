using GymTracker.Core.Entities;
using GymTracker.Infrastructure.Data;
using GymTracker.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GymTracker.Infrastructure.Repositories;

public class ExerciseRepository : IExerciseRepository
{
    private readonly ApplicationDbContext _context;

    public ExerciseRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Exercise>> GetAllExercisesForWorkoutAsync(Guid workoutId)
    {
        return await _context.Exercises
                             .Where(e => e.WorkoutId == workoutId)
                             .Include(e => e.Series)
                             .ToListAsync();
    }

    public async Task<Exercise> GetExerciseByIdAsync(Guid exerciseId)
    {
        return await _context.Exercises
                             .Include(e => e.Series)
                             .FirstOrDefaultAsync(e => e.Id == exerciseId);
    }

    public async Task<Exercise> CreateExerciseAsync(Exercise exercise)
    {
        await _context.Exercises.AddAsync(exercise);
        await _context.SaveChangesAsync();
        return exercise;
    }

    public async Task UpdateExerciseAsync(Exercise exercise)
    {
        _context.Exercises.Update(exercise);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteExerciseAsync(Guid exerciseId)
    {
        var exercise = await _context.Exercises.FindAsync(exerciseId);
        if (exercise != null)
        {
            _context.Exercises.Remove(exercise);
            await _context.SaveChangesAsync();
        }
    }
}
