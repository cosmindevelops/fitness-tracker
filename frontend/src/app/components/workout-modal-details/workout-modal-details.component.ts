import { Component, Input } from '@angular/core';
import { WorkoutResponseDto } from '../../models/workout.models';
import { ExerciseDto } from '../../models/exercise.models';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-workout-modal-details',
  templateUrl: './workout-modal-details.component.html',
  styleUrl: './workout-modal-details.component.css',
})
export class WorkoutModalDetailsComponent {
  @Input() workout!: WorkoutResponseDto;

  constructor(public activeModal: NgbActiveModal) {}

  closeModal(): void {
    this.activeModal.close();
  }

  getTotalSets(exercises: ExerciseDto[] = []): number {
    return exercises.reduce((total, exercise) => total + (exercise.series?.length ?? 0), 0);
  }

  getRPERange(exercises: ExerciseDto[] = []): string {
    let minRPE = Infinity;
    let maxRPE = -Infinity;

    exercises.forEach(exercise => {
      exercise.series?.forEach(set => {
        if (set.rpe < minRPE) minRPE = set.rpe;
        if (set.rpe > maxRPE) maxRPE = set.rpe;
      });
    });

    if (minRPE === Infinity || maxRPE === -Infinity) {
      return 'N/A';
    }

    if (minRPE === maxRPE) {
      return `${minRPE}`;
    }

    return `${minRPE}-${maxRPE}`;
  }
}