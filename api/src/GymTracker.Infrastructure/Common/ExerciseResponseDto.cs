﻿namespace GymTracker.Infrastructure.Common;

public class ExerciseResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public List<SeriesResponseDto> Series { get; set; }
}