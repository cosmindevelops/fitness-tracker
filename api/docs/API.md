# API Documentation

This document provides detailed information about the API endpoints available in the GymTracker application. Each endpoint is described with its route, HTTP method, request parameters, and responses.

## Table of Contents

- [Authentication](#authentication)
  - [Register](#register)
  - [Login](#login)
- [Workouts](#workouts)
  - [Get All Workouts](#get-all-workouts)
  - [Get Workout by ID](#get-workout-by-id)
  - [Search Workouts by Name](#search-workouts-by-name)
  - [Create Workout](#create-workout)
  - [Update Workout](#update-workout)
  - [Delete Workout](#delete-workout)
- [Exercises](#exercises)
  - [Get All Exercises](#get-all-exercises)
  - [Get Exercise by ID](#get-exercise-by-id)
  - [Create Exercise](#create-exercise)
  - [Update Exercise](#update-exercise)
  - [Delete Exercise](#delete-exercise)
- [Series](#series)
  - [Get All Series](#get-all-series)
  - [Get Series by ID](#get-series-by-id)
  - [Create Series](#create-series)
  - [Update Series](#update-series)
  - [Delete Series](#delete-series)
- [Users](#users)
  - [Get User by ID](#get-user-by-id)

## Authentication

### Register

- **Route**: `POST api/auth/register`
- **Description**: Register a new user.
- **Request**:
  - Body:
    ```json
    {
      "username": "string",
      "email": "string",
      "password": "string"
    }
    ```
- **Response**:
  - `200 OK`: 
    ```json
    {
      "message": "User registered successfully."
    }
    ```
  - `400 Bad Request`: Invalid registration request.

### Login

- **Route**: `POST api/auth/login`
- **Description**: Log in an existing user.
- **Request**:
  - Body:
    ```json
    {
      "email": "string",
      "password": "string"
    }
    ```
- **Response**:
  - `200 OK`: 
    ```json
    {
      "userId": "Guid",
      "token": "string"
    }
    ```
  - `400 Bad Request`: Invalid login request.
  - `401 Unauthorized`: Invalid username or password.

## Workouts

### Get All Workouts

- **Route**: `GET api/workouts`
- **Description**: Retrieve all workouts for the authenticated user.
- **Response**:
  - `200 OK`: 
    ```json
    [
      {
        "id": "Guid",
        "notes": "string",
        "date": "date",
        "exercises": [...]
      }
    ]
    ```

### Get Workout by ID

- **Route**: `GET api/workouts/{workoutId}`
- **Description**: Retrieve a specific workout by ID for the authenticated user.
- **Parameters**:
  - `workoutId` (Guid): The ID of the workout.
- **Response**:
  - `200 OK`: 
    ```json
    {
      "id": "Guid",
      "notes": "string",
      "date": "date",
      "exercises": [...]
    }
    ```
  - `404 Not Found`: Workout not found.

### Search Workouts by Name

- **Route**: `GET api/workouts/search`
- **Description**: Search for workouts by name for the authenticated user.
- **Parameters**:
  - Query parameter `name` (string): The name of the workout.
- **Response**:
  - `200 OK`: 
    ```json
    [
      {
        "id": "Guid",
        "notes": "string",
        "date": "date",
        "exercises": [...]
      }
    ]
    ```

### Create Workout

- **Route**: `POST api/workouts`
- **Description**: Create a new workout for the authenticated user.
- **Request**:
  - Body:
    ```json
    {
      "notes": "string",
      "date": "date",
      "exercises": [...]
    }
    ```
- **Response**:
  - `201 Created`: 
    ```json
    {
      "notes": "string",
      "date": "date",
      "exercises": [...]
    }
    ```

### Update Workout

- **Route**: `PUT api/workouts/{workoutId}`
- **Description**: Update an existing workout for the authenticated user.
- **Parameters**:
  - `workoutId` (Guid): The ID of the workout.
- **Request**:
  - Body:
    ```json
    {
      "notes": "string",
      "date": "date"
    }
    ```
- **Response**:
  - `204 No Content`

### Delete Workout

- **Route**: `DELETE api/workouts/{workoutId}`
- **Description**: Delete a workout for the authenticated user.
- **Parameters**:
  - `workoutId` (Guid): The ID of the workout.
- **Response**:
  - `204 No Content`

## Exercises

### Get All Exercises

- **Route**: `GET api/workouts/{workoutId}/exercises`
- **Description**: Retrieve all exercises for a specific workout.
- **Parameters**:
  - `workoutId` (Guid): The ID of the workout.
- **Response**:
  - `200 OK`: 
    ```json
    [
      {
        "id": "Guid",
        "name": "string",
        "series": [...]
      }
    ]
    ```

### Get Exercise by ID

- **Route**: `GET api/workouts/{workoutId}/exercises/{exerciseId}`
- **Description**: Retrieve a specific exercise by ID.
- **Parameters**:
  - `workoutId` (Guid): The ID of the workout.
  - `exerciseId` (Guid): The ID of the exercise.
- **Response**:
  - `200 OK`: 
    ```json
    {
      "id": "Guid",
      "name": "string",
      "series": [...]
    }
    ```

### Create Exercise

- **Route**: `POST api/workouts/{workoutId}/exercises`
- **Description**: Create a new exercise for a specific workout.
- **Parameters**:
  - `workoutId` (Guid): The ID of the workout.
- **Request**:
  - Body:
    ```json
    {
      "name": "string",
    }
    ```
- **Response**:
  - `201 Created`: 
    ```json
    {
      "id": "Guid",
      "name": "string",
      "series": [...]
    }
    ```

### Update Exercise

- **Route**: `PUT api/workouts/{workoutId}/exercises/{exerciseId}`
- **Description**: Update an existing exercise.
- **Parameters**:
  - `workoutId` (Guid): The ID of the workout.
  - `exerciseId` (Guid): The ID of the exercise.
- **Request**:
  - Body:
    ```json
    {
      "name": "string",
    }
    ```
- **Response**:
  - `204 No Content`

### Delete Exercise

- **Route**: `DELETE api/workouts/{workoutId}/exercises/{exerciseId}`
- **Description**: Delete an exercise from a specific workout.
- **Parameters**:
  - `workoutId` (Guid): The ID of the workout.
  - `exerciseId` (Guid): The ID of the exercise.
- **Response**:
  - `204 No Content`

## Series

### Get All Series

- **Route**: `GET api/workouts/{workoutId}/exercises/{exerciseId}/series`
- **Description**: Retrieve all series for a specific exercise.
- **Parameters**:
  - `workoutId` (Guid): The ID of the workout.
  - `exerciseId` (Guid): The ID of the exercise.
- **Response**:
  - `200 OK`: 
    ```json
    [
      {
        "id": "Guid",
        "repetitions": "int",
        "weight": "double",
        "rpe": "int"
      }
    ]
    ```

### Get Series by ID

- **Route**: `GET api/workouts/{workoutId}/exercises/{exerciseId}/series/{seriesId}`
- **Description**: Retrieve a specific series by ID.
- **Parameters**:
  - `workoutId` (Guid): The ID of the workout.
  - `exerciseId` (Guid): The ID of the exercise.
  - `seriesId` (Guid): The ID of the series.
- **Response**:
  - `200 OK`: 
    ```json
    {
      "id": "Guid",
      "repetitions": "int",
      "weight": "double",
      "rpe": "int"
    }
    ```

### Create Series

- **Route**: `POST api/workouts/{workoutId}/exercises/{exerciseId}/series`
- **Description**: Create a new series for a specific exercise.
- **Parameters**:
  - `workoutId` (Guid): The ID of the workout.
  - `exerciseId` (Guid): The ID of the exercise.
- **Request**:
  - Body:
    ```json
    {
      "repetitions": "int",
      "weight": "double",
      "rpe": "int"
    }
    ```
- **Response**:
  - `201 Created`: 
    ```json
    {
      "id": "Guid",
      "repetitions": "int",
      "weight": "double",
      "rpe": "int"
    }
    ```

### Update Series

- **Route**: `PUT api/workouts/{workoutId}/exercises/{exerciseId}/series/{seriesId}`
- **Description**: Update an existing series.
- **Parameters**:
  - `workoutId` (Guid): The ID of the workout.
  - `exerciseId` (Guid): The ID of the exercise.
  - `seriesId` (Guid): The ID of the series.
- **Request**:
  - Body:
    ```json
    {
      "repetitions": "int",
      "weight": "double",
      "rpe": "int"
    }
    ```
- **Response**:
  - `204 No Content`

### Delete Series

- **Route**: `DELETE api/workouts/{workoutId}/exercises/{exerciseId}/series/{seriesId}`
- **Description**: Delete a series from a specific exercise.
- **Parameters**:
  - `workoutId` (Guid): The ID of the workout.
  - `exerciseId` (Guid): The ID of the exercise.
  - `seriesId` (Guid): The ID of the series.
- **Response**:
  - `204 No Content`

## Users

### Get User by ID

- **Route**: `GET api/users/{userId}`
- **Description**: Retrieve a specific user by ID.
- **Parameters**:
  - `userId` (Guid): The ID of the user.
- **Response**:
  - `200 OK`: 
    ```json
    {
      "userId": "Guid",
      "username": "string",
      "email": "string",
    }
    ```
  - `404 Not Found`: User not found.
