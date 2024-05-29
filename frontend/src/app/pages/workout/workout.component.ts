import { AfterViewInit, Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { slideDownAnimation } from '../../shared/animation';
import { WorkoutResponseDto } from '../../models/workout.models';
import { WorkoutService } from '../../services/workout.service';
import { NotificationService } from '../../services/notification.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { WorkoutModalComponent } from '../../components/workout-modal/workout-modal.component';

@Component({
  selector: 'app-workout',
  templateUrl: './workout.component.html',
  styleUrl: './workout.component.css',
  animations: [slideDownAnimation],
})
export class WorkoutComponent implements OnInit, AfterViewInit {

  workouts: WorkoutResponseDto[] = [];
  showScrollIndicator: boolean = false;
  scrollTimeout: any;

  @ViewChild('searchInput') searchInput!: ElementRef;
  @ViewChild('workoutListContainer') workoutListContainer!: ElementRef;

  constructor(private workoutService: WorkoutService, private modalService: NgbModal, private notificationService: NotificationService) {}

  ngOnInit(): void {
    
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
    const searchValue = this.searchInput.nativeElement.value;
    if (searchValue) {
      this.workoutService.getWorkoutsByName(searchValue).subscribe(
        (data: WorkoutResponseDto[]) => {
          this.workouts = data;
        },
        (error) => {
          console.error('Failed to fetch workouts', error);
          this.notificationService.showError('Failed to fetch workouts');
        }
      );
    } else {
      this.workoutService.getAllWorkouts().subscribe(
        (data: WorkoutResponseDto[]) => {
          this.workouts = data;
        },
        (error) => {
          console.error('Failed to fetch workouts', error);
          this.notificationService.showError('Failed to fetch workouts');
        }
      );
    }
  }

  handleDeleteWorkout(workoutId: string): void {
    this.workoutService.deleteWorkout(workoutId).subscribe(
      () => {
        this.getWorkouts();
        this.notificationService.showSuccess('Workout deleted successfully');
      },
      (error) => {
        console.error('Failed to delete workout', error);
        this.notificationService.showError('Failed to delete workout');
      }
    );
  }

  handleCreateWorkout(): void {
    console.log('Opening modal to create a new workout');
    const modalRef = this.modalService.open(WorkoutModalComponent);

    modalRef.result.then(
      () => {
        console.log('Modal closed, refreshing workouts');
        this.getWorkouts();
      },
      () => {
        console.log('Modal dismissed');
      }
    );
  }

  handleEditWorkout(workoutId: string): void {
    console.log(`Opening modal to edit workout with ID: ${workoutId}`);
    const modalRef = this.modalService.open(WorkoutModalComponent);
    modalRef.componentInstance.workoutId = workoutId;

    modalRef.result.then(
      () => {
        console.log('Modal closed, refreshing workouts');
        this.getWorkouts();
      },
      () => {
        console.log('Modal dismissed');
      }
    );
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
