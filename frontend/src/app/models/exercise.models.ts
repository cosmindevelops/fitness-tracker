import { SeriesDto, SeriesResponseDto } from './series.models';

export interface ExerciseDto {
  name: string;
  series?: SeriesDto[];
}

export interface ExerciseResponseDto {
  id: string;
  name: string;
  series: SeriesResponseDto[];
}
