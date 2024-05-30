# Architecture

## Overview

The GymTracker application follows the principles of Clean Architecture. This architecture pattern helps in creating systems that are easy to maintain, test, and extend. It achieves this by organizing the project into several layers, each with distinct responsibilities.

## Clean Architecture

Clean Architecture is a software design philosophy that emphasizes the separation of concerns. It divides the system into layers, each with a specific role. The core idea is to keep the business logic independent of external factors such as databases, frameworks, and user interfaces.

### Layers in Clean Architecture

1. **Core** (Entities)
2. **Use Cases** (Application)
3. **Interface Adapters** (Adapters)
4. **Frameworks & Drivers** (Infrastructure)

## Project Structure

The project is organized into the following main directories:

### Backend

- **GymTracker.API**: This layer contains the web API controllers and middleware. It acts as the entry point for HTTP requests.
  - `Controllers`
    - AuthController.cs
    - BaseController.cs
    - ExerciseController.cs
    - SeriesController.cs
    - UserController.cs
    - WorkoutController.cs
  - `Exceptions`
    - ErrorHandlingMiddleware.cs
  - `Program.cs`
  - `appsettings.json`

- **GymTracker.Core**: This layer contains the core business logic and domain entities.
  - `Entities`
    - Exercise.cs
    - Role.cs
    - Series.cs
    - User.cs
    - UserRole.cs
    - Workout.cs

- **GymTracker.Infrastructure**: This layer contains implementations for data access, external services, and other infrastructure concerns.
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
  - app-routing.module.ts
  - app.component.css
  - app.component.html
  - app.component.ts
  - app.module.ts
- **assets**
  - images

## Detailed Description of Layers

### Core (Entities)

This layer contains the core business logic and domain entities. These entities are plain C# classes that represent the business model.

### Use Cases (Application)

This layer defines the use cases of the application. It contains the application-specific business rules and orchestrates the flow of data between the entities and the external layers.

### Interface Adapters (Adapters)

This layer contains the code that converts data from the format most convenient for the use cases and entities to the format most convenient for external agencies such as databases, web APIs, etc.

### Frameworks & Drivers (Infrastructure)

This layer contains the implementation details. It includes frameworks and tools such as the database access code (Entity Framework Core), web APIs, and other external services.

## Design Patterns

### Dependency Injection

The project uses dependency injection to decouple the instantiation of services and repositories from their usage. This is achieved using the built-in dependency injection provided by ASP.NET Core.

### Repository Pattern

The repository pattern is used to abstract the data access logic. This helps in keeping the business logic agnostic of the data source, making it easier to switch databases if needed.

### AutoMapper

AutoMapper is used to map between domain models and data transfer objects (DTOs). This helps in keeping the layers clean and focused on their respective responsibilities.

## Conclusion

The Clean Architecture pattern helps in creating a maintainable and testable codebase by clearly separating concerns and dependencies. By following this architecture, the GymTracker application is structured to be scalable, flexible, and easy to understand.

