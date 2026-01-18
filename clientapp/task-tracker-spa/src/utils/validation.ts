import { type CreateTaskDto, type UpdateTaskDto, Priority, TaskStatus } from '../api/types';

export interface ValidationErrors {
  [key: string]: string[];
}

export function validateCreateTask(task: CreateTaskDto): ValidationErrors | null {
  const errors: ValidationErrors = {};

  if (!task.title || task.title.trim() === '') {
    errors.title = ['Title is required'];
  } else if (task.title.length > 200) {
    errors.title = ['Title cannot exceed 200 characters'];
  }

  if (task.description && task.description.length > 1000) {
    errors.description = ['Description cannot exceed 1000 characters'];
  }

  if (!task.status || !Object.values(TaskStatus).includes(task.status)) {
    errors.status = ['Status is required and must be one of: New, InProgress, Done'];
  }

  if (!task.priority || !Object.values(Priority).includes(task.priority)) {
    errors.priority = ['Priority is required and must be one of: Low, Medium, High'];
  }

  if (task.dueDate) {
    const dueDate = new Date(task.dueDate);
    if (isNaN(dueDate.getTime())) {
      errors.dueDate = ['Due date must be a valid date'];
    }
  }

  return Object.keys(errors).length > 0 ? errors : null;
}

export function validateUpdateTask(task: UpdateTaskDto): ValidationErrors | null {
  const errors: ValidationErrors = {};

  if (task.title !== undefined) {
    if (task.title.trim() === '') {
      errors.title = ['Title cannot be empty'];
    } else if (task.title.length > 200) {
      errors.title = ['Title cannot exceed 200 characters'];
    }
  }

  if (task.description !== undefined && task.description.length > 1000) {
    errors.description = ['Description cannot exceed 1000 characters'];
  }

  // Status validation
  if (task.status !== undefined && !Object.values(TaskStatus).includes(task.status)) {
    errors.status = ['Status must be one of: New, InProgress, Done'];
  }

  if (task.priority !== undefined && !Object.values(Priority).includes(task.priority)) {
    errors.priority = ['Priority must be one of: Low, Medium, High'];
  }

  if (task.dueDate !== undefined && task.dueDate !== null) {
    const dueDate = new Date(task.dueDate);
    if (isNaN(dueDate.getTime())) {
      errors.dueDate = ['Due date must be a valid date'];
    }
  }

  return Object.keys(errors).length > 0 ? errors : null;
}

export function formatValidationErrors(errors: ValidationErrors): string[] {
  const messages: string[] = [];
  
  for (const [field, fieldErrors] of Object.entries(errors)) {
    messages.push(`${field}: ${fieldErrors.join(', ')}`);
  }
  
  return messages;
}