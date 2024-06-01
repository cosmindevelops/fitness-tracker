import { AfterViewInit, Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { slideDownAnimation } from '../../shared/animation';
import { WorkoutResponseDto } from '../../models/workout.models';
import { WorkoutService } from '../../services/workout.service';
import { NotificationService } from '../../services/notification.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { WorkoutModalComponent } from '../../components/workout-modal/workout-modal.component';
import { WorkoutModalDetailsComponent } from '../../components/workout-modal-details/workout-modal-details.component';

@Component({
  selector: 'app-workout',
  templateUrl: './workout.component.html',
  styleUrl: './workout.component.css',
  animations: [slideDownAnimation],
})
export class WorkoutComponent implements OnInit, AfterViewInit {
  workouts: WorkoutResponseDto[] = [];
  showScrollIndicator: boolean = false;
  searchByDate: boolean = false;
  initialLoad: boolean = true;
  scrollTimeout!: ReturnType<typeof setTimeout>;
  noWorkoutsFound: boolean = false;

  @ViewChild('searchInput') searchInput!: ElementRef;
  @ViewChild('dateInput') dateInput!: ElementRef;
  @ViewChild('workoutListContainer') workoutListContainer!: ElementRef;

  constructor(private workoutService: WorkoutService, private modalService: NgbModal, private notificationService: NotificationService) {}

  ngOnInit(): void {
    this.workoutService.getWorkoutUpdateListener().subscribe(() => {
      this.getWorkouts();
    });
  }

  ngAfterViewInit(): void {
    this.checkScrollIndicator();
    this.workoutListContainer.nativeElement.addEventListener('scroll', () => {
      clearTimeout(this.scrollTimeout);
      this.checkScrollIndicator();
      this.hideScrollIndicatorWithDelay();
    });
    this.hideScrollIndicatorWithDelay();
  }

  getWorkouts(): void {
    const searchValue = this.searchInput?.nativeElement?.value || '';
    const dateValue = this.searchByDate ? this.dateInput?.nativeElement?.value : '';

    if (searchValue || dateValue) {
      this.workoutService.getWorkouts(searchValue, dateValue).subscribe(
        (data: WorkoutResponseDto[]) => {
          this.workouts = data;
          this.noWorkoutsFound = data.length === 0; // Update property
        },
        (error) => {
          this.notificationService.showError('Failed to fetch workouts');
        }
      );
    } else {
      this.workoutService.getAllWorkouts().subscribe(
        (data: WorkoutResponseDto[]) => {
          this.workouts = data;
          this.noWorkoutsFound = data.length === 0; // Update property
        },
        (error) => {
          this.notificationService.showError('Failed to fetch workouts');
        }
      );
    }
  }

  handleDeleteWorkout(workoutId: string): void {
    this.workoutService.deleteWorkout(workoutId).subscribe(
      () => {
        this.notificationService.showSuccess('Workout deleted successfully');
      },
      (error) => {
        this.notificationService.showError('Failed to delete workout');
      }
    );
  }

  handleCreateWorkout(): void {
    const modalRef = this.modalService.open(WorkoutModalComponent);

    modalRef.result.then(
      (result) => {
        if (result) {
          this.getWorkouts();
        }
      },
      () => {}
    );
  }

  handleEditWorkout(workoutId: string): void {
    const modalRef = this.modalService.open(WorkoutModalComponent);
    modalRef.componentInstance.workoutId = workoutId;

    modalRef.result.then(
      (result) => {
        if (result) {
          this.getWorkouts();
        }
      },
      () => {}
    );
  }

  showWorkoutDetails(workout: WorkoutResponseDto): void {
    const modalRef = this.modalService.open(WorkoutModalDetailsComponent, {
      backdrop: true,
      keyboard: true,
      centered: true,
    });

    modalRef.componentInstance.workout = workout;

    modalRef.result.catch(() => {});
  }

  checkScrollIndicator(): void {
    const element = this.workoutListContainer.nativeElement;
    this.showScrollIndicator = element.scrollHeight > element.clientHeight && element.scrollTop + element.clientHeight < element.scrollHeight;
  }

  hideScrollIndicatorWithDelay(): void {
    this.scrollTimeout = setTimeout(() => {
      this.showScrollIndicator = false;
    }, 1500);
  }
}
