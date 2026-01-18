import axios, { AxiosError, type AxiosInstance, type AxiosResponse } from 'axios';
import type { TaskItem, CreateTaskDto, UpdateTaskDto, ProblemDetails, Priority } from './types';

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL;

class ApiClient {
  private client: AxiosInstance;

  constructor() {
    this.client = axios.create({
      baseURL: API_BASE_URL,
      headers: {
        'Content-Type': 'application/json',
      },
    });

    // Response interceptor for error handling
    this.client.interceptors.response.use(
      (response: AxiosResponse) => response,
      (error: AxiosError<ProblemDetails>) => {
        if (error.response) {
          // The request was made and the server responded with a status code
          // that falls out of the range of 2xx
          console.error('API Error:', error.response.data);
          return Promise.reject(error.response.data);
        } else if (error.request) {
          // The request was made but no response was received
          console.error('Network Error:', error.request);
          return Promise.reject({
            title: 'Network Error',
            detail: 'Unable to connect to the server. Please check your connection.',
          });
        } else {
          // Something happened in setting up the request that triggered an Error
          console.error('Request Error:', error.message);
          return Promise.reject({
            title: 'Request Error',
            detail: error.message,
          });
        }
      }
    );
  }

  async getTasks(params?: { q?: string; sort?: string }): Promise<TaskItem[]> {
    const response = await this.client.get<TaskItem[]>('/api/tasks', { params });
    return response.data;
  }

  async getTask(id: number): Promise<TaskItem> {
    const response = await this.client.get<TaskItem>(`/api/tasks/${id}`);
    return response.data;
  }

  async createTask(task: CreateTaskDto): Promise<TaskItem> {
    const response = await this.client.post<TaskItem>('/api/tasks', task);
    return response.data;
  }

  async updateTask(id: number, task: UpdateTaskDto): Promise<void> {
    await this.client.put(`/api/tasks/${id}`, task);
  }

  async deleteTask(id: number): Promise<void> {
    await this.client.delete(`/api/tasks/${id}`);
  }

  async markAsComplete(id: number): Promise<TaskItem> {
    const response = await this.client.patch<TaskItem>(`/api/tasks/${id}/complete`);
    return response.data;
  }

  async markAsIncomplete(id: number): Promise<TaskItem> {
    const response = await this.client.patch<TaskItem>(`/api/tasks/${id}/incomplete`);
    return response.data;
  }

  async getTasksByPriority(priority: Priority): Promise<TaskItem[]> {
    const response = await this.client.get<TaskItem[]>(`/api/tasks/priority/${priority}`);
    return response.data;
  }
}

export const apiClient = new ApiClient();