import { SeriesResponseDto } from './series.models';

export interface ExerciseDto {
  name: string;
}

export interface ExerciseResponseDto {
  id: string;
  name: string;
  series: SeriesResponseDto[];
}
