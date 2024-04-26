﻿using GymTracker.Core.Entities;

namespace GymTracker.Infrastructure.Repositories.Interfaces;

public interface IWorkoutRepository
{
    Task<IEnumerable<Workout>> GetAllWorkoutsAsync(Guid userId);

    Task<Workout> GetWorkoutByIdAsync(Guid userId, Guid workoutId);

    Task<Workout> CreateWorkoutAsync(Workout workout);

    Task UpdateWorkoutAsync(Workout workout);

    Task DeleteWorkoutAsync(Guid workoutId);
}