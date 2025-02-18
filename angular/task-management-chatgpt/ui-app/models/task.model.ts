export interface Task {
  id: number;
  text: string;
  completed: boolean;
  category: string;
  dueDate: Date;
  priority: 'low' | 'medium' | 'high';
}