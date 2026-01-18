export const TaskStatus = {
  New: 'New',
  InProgress: 'InProgress',
  Done: 'Done',
} as const;

 export const Priority = {
  Low: 'Low',
  Medium: 'Medium',
  High: 'High',
} as const;

export type TaskStatus = (typeof TaskStatus)[keyof typeof TaskStatus];
export type Priority = (typeof Priority)[keyof typeof Priority];

export interface TaskItem {
  id: number;
  title: string;
  description: string;
  status: TaskStatus;
  priority: Priority;
  dueDate?: string;
  isCompleted: boolean;
  createdAt: string;
  updatedAt?: string;
}

export interface CreateTaskDto {
  title: string;
  description: string;
  status: TaskStatus;
  priority: Priority;
  dueDate?: string;
}

export interface UpdateTaskDto {
  title?: string;
  description?: string;
  status?: TaskStatus;
  priority?: Priority;
  dueDate?: string | null;
}

export interface ProblemDetails {
  type?: string;
  title?: string;
  status?: number;
  detail?: string;
  instance?: string;
  errors?: Record<string, string[]>;
  [key: string]: any;
}

export interface ApiResponse<T> {
  data?: T;
  error?: ProblemDetails;
  isLoading: boolean;
}