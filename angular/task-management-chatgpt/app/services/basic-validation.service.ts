import { Injectable } from '@angular/core';
import { IValidationService } from './i-validation.service';

@Injectable({
  providedIn: 'root',
})
export class BasicValidationService implements IValidationService {
  validate(task: any): boolean {
    return task.text.trim().length > 0;
  }
}