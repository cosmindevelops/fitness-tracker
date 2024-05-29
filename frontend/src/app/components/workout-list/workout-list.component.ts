import { Component, ElementRef, EventEmitter, HostListener, Input, Output } from '@angular/core';
import { WorkoutResponseDto } from '../../models/workout.models';
import { SeriesDto } from '../../models/series.models';
import { slideDownAnimation } from '../../shared/animation';

@Component({
  selector: 'app-workout-list',
  templateUrl: './workout-list.component.html',
  styleUrl: './workout-list.component.css',
  animations: [slideDownAnimation],
})
export class WorkoutListComponent {
  dropdownOpen: boolean[] = [];

  @Input() workouts: WorkoutResponseDto[] = [];
  @Output() editWorkout = new EventEmitter<string>();
  @Output() deleteWorkout = new EventEmitter<string>();

  constructor(private elRef: ElementRef) {}

  ngOnChanges(): void {
    this.dropdownOpen = new Array(this.workouts.length).fill(false);
  }

  toggleDropdown(index: number): void {
    this.dropdownOpen = this.dropdownOpen.map(() => false);
    this.dropdownOpen[index] = true;
  }

  closeDropdown(index: number): void {
    this.dropdownOpen[index] = false;
  }

  getRepRange(series: SeriesDto[]): string {
    if (!series || series.length === 0) {
      return '0 reps';
    }
    const minReps = Math.min(...series.map((s) => s.repetitions));
    const maxReps = Math.max(...series.map((s) => s.repetitions));

    if (minReps === maxReps) {
      return `${minReps} reps`;
    }

    return `${minReps}-${maxReps} reps`;
  }

  @HostListener('document:click', ['$event'])
  handleOutsideClick(event: Event) {
    const clickedOutsideDropdown = !(event.target as HTMLElement).closest('.dropdown');

    if (clickedOutsideDropdown) {
      this.dropdownOpen = this.dropdownOpen.map(() => false);
    }
  }

  @HostListener('click', ['$event'])
  expandCard(event: MouseEvent) {
    const clickedElement = event.currentTarget as HTMLElement;
    if (this.elRef.nativeElement.contains(clickedElement)) {
      if (clickedElement.classList.contains('expanded')) {
        clickedElement.classList.remove('expanded');
      } else {
        clickedElement.classList.add('expanded');
      }
    }
  }
}
