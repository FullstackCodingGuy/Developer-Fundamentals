import { Injectable } from '@angular/core';
import { Task } from '../models/task.model';

@Injectable({
  providedIn: 'root',
})
export class TaskService {
  private tasks: Task[] = [];

  addTask(task: Task) {
    this.tasks.push(task);
  }

  getTasks(): Task[] {
    return [...this.tasks];
  }

  removeTask(taskId: number): void {
    this.tasks = this.tasks.filter(task => task.id !== taskId);
  }
}