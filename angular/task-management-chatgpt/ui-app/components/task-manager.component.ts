import { Component, OnInit, Injector } from '@angular/core';
import { TaskService } from '../services/task.service';
import { Task } from '../models/task.model';
import { IValidationService } from '../services/i-validation.service';
import { ValidationServiceFactory } from '../services/validation-service.factory';
import { VALIDATION_SERVICE } from '../tokens/validation-service.token';

@Component({
  selector: 'app-task-manager',
  templateUrl: './task-manager.component.html',
  styleUrls: ['./task-manager.component.css'],
})
export class TaskManagerComponent implements OnInit {
  tasks: Task[] = [];
  newTask: string = '';
  validationService: IValidationService;

  constructor(
    private taskService: TaskService,
    private injector: Injector
  ) {
    this.validationService = ValidationServiceFactory.getValidationService('basic');
  }

  ngOnInit() {
    this.loadTasks();
  }

  loadTasks() {
    this.tasks = this.taskService.getTasks();
  }

  addTask() {
    const task: Task = {
      id: Date.now(),
      text: this.newTask,
      completed: false,
      category: 'General',
      priority: 'medium',
      dueDate: new Date(),
    };

    if (this.validationService.validate(task)) {
      this.taskService.addTask(task);
      this.loadTasks();
    } else {
      console.log('Task is invalid');
    }
  }

  switchValidationStrategy(strategy: 'basic' | 'priority') {
    this.validationService = ValidationServiceFactory.getValidationService(strategy);
  }
}