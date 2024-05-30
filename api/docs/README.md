# GymTracker

GymTracker is a web application designed to help fitness enthusiasts track their workouts in a more personalized and comprehensive way. This application allows users to create an account, authenticate, and manage their workout routines, exercises, and series, including tracking stats like repetitions, weight, and RPE (Rate of Perceived Exertion).

## Table of Contents

- [Features](#features)
- [Tech Stack](#tech-stack)
- [Usage](#usage)
- [Project Structure](#project-structure)

## Features

- User authentication and authorization
- Create, read, update, and delete (CRUD) operations for workouts
- Each workout can have multiple exercises
- Each exercise can have multiple series
- Track stats for each series: repetitions, weight, and RPE

## Tech Stack

- **Backend**: C#
- **Frontend**: Angular
- **Database**: Microsoft SQL Server

## Usage

1. Register a new account.
2. Log in with your credentials.
3. Create a new workout.
4. Add exercises to your workout.
5. Add series to each exercise and track your stats.

## Project Structure

### Backend

- **GymTracker.API**
    - `Controllers`
        - AuthController.cs
        - BaseController.cs
        - ExerciseController.cs
        - SeriesController.cs
        - UserController.cs
        - WorkoutController.cs
    - `Exceptions`
        - ErrorHandlingMiddleware.cs
    - `Logs`
    - `Program.cs`
    - `appsettings.json`

- **GymTracker.Core**
    - `Entities`
        - Exercise.cs
        - Role.cs
        - Series.cs
        - User.cs
        - UserRole.cs
        - Workout.cs

- **GymTracker.Infrastructure**
    - `Authentication`
        - JwtTokenService.cs
    - `Common`
        - `Exceptions`
        - `Mapping`
            - WorkoutMappingProfile.cs
        - `Utility`
            - EntityValidator.cs
            - GuidValidator.cs
    - `Data`
        - ApplicationDbContext.cs
    - `Repositories`
        - ExerciseRepository.cs
        - SeriesRepository.cs
        - UserRepository.cs
        - WorkoutRepository.cs
    - `Services`
        - AuthService.cs
        - ExerciseService.cs
        - SeriesService.cs
        - UserService.cs
        - WorkoutService.cs

### Frontend

- **app**
    - `components`
        - `navbar`
        - `workout-list`
        - `workout-modal`
    - `models`
        - auth.models.ts
        - exercise.models.ts
        - series.models.ts
        - workout.models.ts
    - `pages`
        - `auth`
        - `home`
        - `workout`
    - `services`
        - auth-guard.service.ts
        - auth.service.ts
        - jwt.interceptor.ts
        - notification.service.ts
        - workout.service.ts
    - `shared`
        - animation.ts
- **assets**
    - images
