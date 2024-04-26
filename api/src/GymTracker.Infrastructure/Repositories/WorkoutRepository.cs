using GymTracker.Core.Entities;
using GymTracker.Infrastructure.Data;
using GymTracker.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GymTracker.Infrastructure.Repositories;

public class WorkoutRepository : IWorkoutRepository
{
    private readonly ApplicationDbContext _context;

    public WorkoutRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Workout>> GetAllWorkoutsAsync(Guid userId)
    {
        return await _context.Workouts
                         .Include(w => w.Exercises)
                         .ThenInclude(e => e.Series)
                         .Where(w => w.UserId == userId)
                         .ToListAsync();
    }

    public async Task<Workout> GetWorkoutByIdAsync(Guid userId, Guid workoutId)
    {
        return await _context.Workouts
                             .Include(w => w.Exercises)
                             .ThenInclude(e => e.Series)
                             .FirstOrDefaultAsync(w => w.Id == workoutId && w.UserId == userId);
    }

    public async Task<Workout> CreateWorkoutAsync(Workout workout)
    {
        await _context.Workouts.AddAsync(workout);
        await _context.SaveChangesAsync();
        return workout;
    }

    public async Task UpdateWorkoutAsync(Workout workout)
    {
        _context.Workouts.Update(workout);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteWorkoutAsync(Guid workoutId)
    {
        var workout = await _context.Workouts.FindAsync(workoutId);
        if (workout != null)
        {
            _context.Workouts.Remove(workout);
            await _context.SaveChangesAsync();
        }
    }
}