// src/app/interfaces/validation-service.interface.ts
export interface IValidationService {
  validateTaskText(text: string): ValidationResult;
  getName(): string; // Added to identify the validation service
}

export interface ValidationResult {
  isValid: boolean;
  error?: string;
}

// src/app/services/validation/basic-validation.service.ts
import { Injectable } from '@angular/core';
import { IValidationService, ValidationResult } from '../../interfaces/validation-service.interface';

@Injectable()
export class BasicValidationService implements IValidationService {
  validateTaskText(text: string): ValidationResult {
    if (!text || !text.trim()) {
      return {
        isValid: false,
        error: 'Task text cannot be empty'
      };
    }
    return { isValid: true };
  }

  getName(): string {
    return 'Basic Validation';
  }
}

// src/app/services/validation/strict-validation.service.ts
import { Injectable } from '@angular/core';
import { IValidationService, ValidationResult } from '../../interfaces/validation-service.interface';

@Injectable()
export class StrictValidationService implements IValidationService {
  validateTaskText(text: string): ValidationResult {
    if (!text || !text.trim()) {
      return {
        isValid: false,
        error: 'Task text cannot be empty'
      };
    }
    
    if (text.trim().length < 5) {
      return {
        isValid: false,
        error: 'Task must be at least 5 characters long'
      };
    }

    if (!/^[A-Z]/.test(text)) {
      return {
        isValid: false,
        error: 'Task must start with a capital letter'
      };
    }

    return { isValid: true };
  }

  getName(): string {
    return 'Strict Validation';
  }
}

// src/app/services/validation/permissive-validation.service.ts
import { Injectable } from '@angular/core';
import { IValidationService, ValidationResult } from '../../interfaces/validation-service.interface';

@Injectable()
export class PermissiveValidationService implements IValidationService {
  validateTaskText(text: string): ValidationResult {
    // Accepts any non-null input
    if (text === null || text === undefined) {
      return {
        isValid: false,
        error: 'Task text must not be null'
      };
    }
    return { isValid: true };
  }

  getName(): string {
    return 'Permissive Validation';
  }
}

// src/app/services/validation/validation-factory.service.ts
import { Injectable, inject } from '@angular/core';
import { IValidationService } from '../../interfaces/validation-service.interface';
import { BasicValidationService } from './basic-validation.service';
import { StrictValidationService } from './strict-validation.service';
import { PermissiveValidationService } from './permissive-validation.service';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ValidationFactoryService {
  private basicValidation = new BasicValidationService();
  private strictValidation = new StrictValidationService();
  private permissiveValidation = new PermissiveValidationService();

  private currentValidationService = new BehaviorSubject<IValidationService>(this.basicValidation);
  currentValidation$ = this.currentValidationService.asObservable();

  private validationServices = new Map<string, IValidationService>([
    ['basic', this.basicValidation],
    ['strict', this.strictValidation],
    ['permissive', this.permissiveValidation]
  ]);

  setValidationType(type: string): void {
    const service = this.validationServices.get(type);
    if (service) {
      this.currentValidationService.next(service);
    }
  }

  getCurrentValidator(): IValidationService {
    return this.currentValidationService.value;
  }

  getAvailableValidators(): string[] {
    return Array.from(this.validationServices.keys());
  }
}

// src/app/components/validation-selector/validation-selector.component.ts
import { Component, inject } from '@angular/core';
import { ValidationFactoryService } from '../../services/validation/validation-factory.service';

@Component({
  selector: 'app-validation-selector',
  standalone: true,
  template: `
    <div class="validation-selector">
      <label>Select Validation Type: </label>
      <select (change)="changeValidationType($event)" class="validation-select">
        @for (type of validationTypes; track type) {
          <option [value]="type">{{ type }}</option>
        }
      </select>
      <p class="current-validator">
        Current Validator: {{ (validationFactory.currentValidation$ | async)?.getName() }}
      </p>
    </div>
  `,
  styles: [`
    .validation-selector {
      margin: 20px 0;
      padding: 10px;
      background-color: #f8f9fa;
      border-radius: 4px;
    }
    .validation-select {
      margin-left: 10px;
      padding: 5px;
    }
    .current-validator {
      margin-top: 10px;
      font-style: italic;
    }
  `]
})
export class ValidationSelectorComponent {
  validationFactory = inject(ValidationFactoryService);
  validationTypes = this.validationFactory.getAvailableValidators();

  changeValidationType(event: Event): void {
    const select = event.target as HTMLSelectElement;
    this.validationFactory.setValidationType(select.value);
  }
}

// Update task-manager.component.ts to include the selector
import { Component } from '@angular/core';
import { TaskFormComponent } from '../task-form/task-form.component';
import { TaskListComponent } from '../task-list/task-list.component';
import { ValidationSelectorComponent } from '../validation-selector/validation-selector.component';

@Component({
  selector: 'app-task-manager',
  standalone: true,
  imports: [TaskFormComponent, TaskListComponent, ValidationSelectorComponent],
  template: `
    <div class="task-manager">
      <header>
        <h1>Task Manager</h1>
      </header>
      <app-validation-selector />
      <app-task-form />
      <app-task-list />
    </div>
  `
})
export class TaskManagerComponent {}

// Update task-form.component.ts and task-item.component.ts to use the factory
// Example for task-form.component.ts:
import { Component, signal, inject, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { ValidationFactoryService } from '../../services/validation/validation-factory.service';

@Component({
  // ... other metadata
})
export class TaskFormComponent implements OnInit, OnDestroy {
  private validationFactory = inject(ValidationFactoryService);
  private validationSubscription?: Subscription;

  ngOnInit() {
    this.validationSubscription = this.validationFactory.currentValidation$
      .subscribe(validator => {
        // Revalidate current input if needed
        if (this.newTask()) {
          const result = validator.validateTaskText(this.newTask());
          this.error.set(result.isValid ? '' : (result.error || ''));
        }
      });
  }

  ngOnDestroy() {
    this.validationSubscription?.unsubscribe();
  }

  submitTask(): void {
    const validation = this.validationFactory.getCurrentValidator()
      .validateTaskText(this.newTask());
    // ... rest of the submission logic
  }
}
