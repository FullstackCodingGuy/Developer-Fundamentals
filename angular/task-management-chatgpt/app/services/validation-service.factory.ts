import { Injectable } from '@angular/core';
import { IValidationService } from './i-validation.service';
import { BasicValidationService } from './basic-validation.service';
import { PriorityValidationService } from './priority-validation.service';

@Injectable({
  providedIn: 'root',
})
export class ValidationServiceFactory {
  static getValidationService(strategy: 'basic' | 'priority'): IValidationService {
    if (strategy === 'priority') {
      return new PriorityValidationService();
    } else {
      return new BasicValidationService();
    }
  }
}