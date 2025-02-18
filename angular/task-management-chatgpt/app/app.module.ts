import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppComponent } from './app.component';
import { TaskManagerComponent } from './components/task-manager.component';
import { TaskService } from './services/task.service';
import { BasicValidationService } from './services/basic-validation.service';
import { PriorityValidationService } from './services/priority-validation.service';
import { IValidationService } from './services/i-validation.service';
import { VALIDATION_SERVICE } from './tokens/validation-service.token';

@NgModule({
  declarations: [AppComponent, TaskManagerComponent],
  imports: [BrowserModule],
  providers: [
    TaskService,
    { provide: VALIDATION_SERVICE, useClass: BasicValidationService },
  ],
  bootstrap: [AppComponent],
})
export class AppModule {}