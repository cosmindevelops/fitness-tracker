import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { NavbarComponent } from './components/navbar/navbar.component';
import { HomeComponent } from './pages/home/home.component';
import { WorkoutComponent } from './pages/workout/workout.component';
import { AuthComponent } from './pages/auth/auth.component';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { JwtInterceptor } from './services/jwt.interceptor';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { ToastrModule } from 'ngx-toastr';
import { WorkoutListComponent } from './components/workout-list/workout-list.component';
import { WorkoutModalComponent } from './components/workout-modal/workout-modal.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { WorkoutModalDetailsComponent } from './components/workout-modal-details/workout-modal-details.component';

@NgModule({
  declarations: [AppComponent, NavbarComponent, HomeComponent, WorkoutComponent, AuthComponent, WorkoutListComponent, WorkoutModalComponent, WorkoutModalDetailsComponent],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    ReactiveFormsModule,
    BrowserModule,
    BrowserAnimationsModule,
    FormsModule,
    NgbModule,
    ToastrModule.forRoot({
      timeOut: 3000,
      positionClass: 'toast-bottom-center',
      maxOpened: 1,
      autoDismiss: true,
      preventDuplicates: true,
    }),
  ],
  providers: [{ provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true }],
  bootstrap: [AppComponent]
})
export class AppModule {}
