import { InjectionToken } from '@angular/core';
import { IValidationService } from '../services/i-validation.service';

export const VALIDATION_SERVICE = new InjectionToken<IValidationService>('IValidationService');