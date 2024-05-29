import { AfterViewInit, Component, ElementRef, Input, OnInit, ViewChild } from '@angular/core';
import { AbstractControl, FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NotificationService } from '../../services/notification.service';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { WorkoutService } from '../../services/workout.service';
import { WorkoutDto, WorkoutResponseDto } from '../../models/workout.models';
import { ExerciseDto } from '../../models/exercise.models';
import { SeriesDto } from '../../models/series.models';

@Component({
  selector: 'app-workout-modal',
  templateUrl: './workout-modal.component.html',
  styleUrl: './workout-modal.component.css',
})
export class WorkoutModalComponent implements OnInit {
  workoutForm!: FormGroup;
  workoutNameHasInput?: boolean;
  exerciseNameHasInput?: boolean;
  @Input() workoutId?: string;

  constructor(
    private fb: FormBuilder,
    private notificationService: NotificationService,
    private activeModal: NgbActiveModal,
    private workoutService: WorkoutService
  ) {}

  ngOnInit(): void {
    console.log('WorkoutModalComponent initialized');
    this.workoutNameHasInput = false;
    this.exerciseNameHasInput = false;

    this.workoutForm = this.fb.group({
      workoutName: ['', Validators.required],
      exercises: this.fb.array([]),
    });

    if (this.workoutId) {
      console.log(`Loading workout with ID: ${this.workoutId}`);
      this.loadWorkout(this.workoutId);
    } else {
      this.addExercise(); // Add an initial empty exercise if creating a new workout
    }

    let workoutNameControl = this.workoutForm.get('workoutName');
    if (workoutNameControl) {
      workoutNameControl.valueChanges.subscribe((value) => {
        this.workoutNameHasInput = !!value;
      });
    }

    let exerciseNameControl = this.workoutForm.get('exercises');
    if (exerciseNameControl) {
      exerciseNameControl.valueChanges.subscribe((value) => {
        this.exerciseNameHasInput = !!value;
      });
    }
  }

  get exercises(): FormArray {
    return this.workoutForm.get('exercises') as FormArray;
  }

  newExercise(): FormGroup {
    return this.fb.group({
      exerciseName: ['', Validators.required],
      sets: this.fb.array([this.newSet()]),
    });
  }

  newSet(): FormGroup {
    return this.fb.group({
      kg: ['', Validators.required],
      reps: ['', Validators.required],
      rpe: ['', Validators.required],
    });
  }

  addExercise(): void {
    if (this.isExerciseValid()) {
      this.exercises.push(this.newExercise());
    } else {
      this.notificationService.showWarning('Please fill out all exercise fields before adding a new exercise.');
    }
  }

  addSet(exerciseIndex: number): void {
    const sets = this.exercises.at(exerciseIndex).get('sets') as FormArray;
    if (this.isSetValid(sets)) {
      sets.push(this.newSet());
    } else {
      this.notificationService.showWarning('Please fill out all set fields before adding a new set.');
    }
  }

  deleteSet(exerciseIndex: number, setIndex: number): void {
    const sets = this.exercises.at(exerciseIndex).get('sets') as FormArray;
    sets.removeAt(setIndex);
  }

  deleteExercise(exerciseIndex: number): void {
    this.exercises.removeAt(exerciseIndex);
  }

  getSets(exerciseIndex: number): FormArray {
    return this.exercises.at(exerciseIndex).get('sets') as FormArray;
  }

  isExerciseValid(): boolean {
    return !this.exercises.controls.some(
      (exercise: AbstractControl) =>
        (exercise as FormGroup).get('exerciseName')?.invalid ||
        ((exercise as FormGroup).get('sets') as FormArray).controls.some((set: AbstractControl) => (set as FormGroup).invalid)
    );
  }

  isSetValid(sets: FormArray): boolean {
    return !sets.controls.some((set: AbstractControl) => (set as FormGroup).invalid);
  }

  saveWorkout(): void {
    if (this.workoutForm.valid) {
      const workoutDto: WorkoutDto = this.createWorkoutDto();
      if (this.workoutId) {
        this.workoutService.updateWorkout(this.workoutId, workoutDto).subscribe(
          (response: WorkoutResponseDto) => {
            this.notificationService.showSuccess('Workout updated successfully!');
            this.workoutForm.reset();
            this.exercises.clear();
            this.closeModal();
          },
          (error) => {
            this.notificationService.showError('Failed to update workout.');
          }
        );
      } else {
        this.workoutService.createWorkout(workoutDto).subscribe(
          (response: WorkoutResponseDto) => {
            this.notificationService.showSuccess('Workout saved successfully!');
            this.workoutForm.reset();
            this.exercises.clear();
            this.closeModal();
          },
          (error) => {
            this.notificationService.showError('Failed to save workout.');
          }
        );
      }
    } else {
      this.notificationService.showWarning('Please fill out all required fields.');
    }
  }

  createWorkoutDto(): WorkoutDto {
    const workoutDto: WorkoutDto = {
      notes: this.workoutForm.get('workoutName')?.value,
      date: new Date(),
      exercises: this.exercises.controls.map((exerciseControl) => {
        const exercise = exerciseControl as FormGroup;
        const exerciseDto: ExerciseDto = {
          name: exercise.get('exerciseName')?.value,
          series: (exercise.get('sets') as FormArray).controls.map((setControl) => {
            const set = setControl as FormGroup;
            const seriesDto: SeriesDto = {
              repetitions: set.get('reps')?.value,
              rpe: set.get('rpe')?.value,
              weight: set.get('kg')?.value,
            };
            return seriesDto;
          }),
        };
        return exerciseDto;
      }),
    };
    return workoutDto;
  }

  loadWorkout(workoutId: string): void {
    this.workoutService.getWorkoutById(workoutId).subscribe(
      (workout: WorkoutResponseDto) => {
        console.log('Workout data fetched successfully', workout);
        this.workoutForm.patchValue({
          workoutName: workout.notes,
        });
        const exerciseArray = this.exercises;
        workout.exercises.forEach((exercise) => {
          const exerciseGroup = this.fb.group({
            exerciseName: [exercise.name, Validators.required],
            sets: this.fb.array([]),
          });
          const setArray = exerciseGroup.get('sets') as FormArray;
          exercise.series.forEach((set) => {
            const setGroup = this.fb.group({
              kg: [set.weight, Validators.required],
              reps: [set.repetitions, Validators.required],
              rpe: [set.rpe, Validators.required],
            });
            setArray.push(setGroup);
          });
          exerciseArray.push(exerciseGroup);
        });
      },
      (error) => {
        console.error('Failed to load workout', error);
      }
    );
  }

  deleteWorkout(): void {
    if (this.workoutId) {
      // If it's an edit operation, delete the workout from the database
      this.workoutService.deleteWorkout(this.workoutId).subscribe(
        () => {
          this.notificationService.showSuccess('Workout deleted successfully.');
          this.closeModal();
        },
        (error) => {
          this.notificationService.showError('Failed to delete workout.');
          console.error('Failed to delete workout', error);
        }
      );
    } else {
      // If it's a new workout, just reset the form
      this.workoutForm.reset();
      this.exercises.clear();
      this.notificationService.showInfo('Workout cleared.');
      this.closeModal();
    }
  }

  closeModal(): void {
    console.log('Closing modal');
    this.activeModal.dismiss();
  }
}
