import { Injectable } from '@angular/core';
import { IValidationService } from './i-validation.service';

@Injectable({
  providedIn: 'root',
})
export class PriorityValidationService implements IValidationService {
  validate(task: any): boolean {
    return task.priority !== null && task.priority !== undefined;
  }
}