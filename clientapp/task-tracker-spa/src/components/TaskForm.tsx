import React, { useState } from 'react';
import { type CreateTaskDto, type UpdateTaskDto, TaskStatus, Priority } from '../api/types';
import { validateCreateTask, validateUpdateTask, type ValidationErrors } from '../utils/validation';
import LoadingSpinner from './LoadingSpinner';

interface TaskFormProps {
  initialData?: Partial<CreateTaskDto>;
  onSubmit: (data: CreateTaskDto | UpdateTaskDto) => Promise<void>;
  isLoading: boolean;
  error?: string;
  isEdit?: boolean;
}

const TaskForm: React.FC<TaskFormProps> = ({
  initialData = {},
  onSubmit,
  isLoading,
  error,
  isEdit = false
}) => {
  const [formData, setFormData] = useState<CreateTaskDto | UpdateTaskDto>({
    title: initialData.title || '',
    description: initialData.description || '',
    status: initialData.status || TaskStatus.New,
    priority: initialData.priority || Priority.Medium,
    dueDate: initialData.dueDate || '',
  });

  const [validationErrors, setValidationErrors] = useState<ValidationErrors>({});

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement>) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: value
    }));
    
   if (validationErrors[name]) {
    setValidationErrors(({ [name]: _, ...rest }) => rest)};
    
 };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    // Validate
    const errors = isEdit 
      ? validateUpdateTask(formData as UpdateTaskDto)
      : validateCreateTask(formData as CreateTaskDto);
    
    if (errors) {
      setValidationErrors(errors);
      return;
    }
    
    try {
      await onSubmit(formData);
    } catch {
      // Error is handled by parent component
    }
  };

  if (isLoading) {
    return <LoadingSpinner text={isEdit ? 'Updating task...' : 'Creating task...'} />;
  }

  return (
    <form onSubmit={handleSubmit} className="space-y-6 max-w-2xl mx-auto">
      {error && (
        <div className="bg-red-50 border border-red-200 rounded p-4">
          <p className="text-red-800">{error}</p>
        </div>
      )}
      
      <div>
        <label className="block text-sm font-medium text-gray-700 mb-1">
          Title *
        </label>
        <input
          type="text"
          name="title"
          value={formData.title || ''}
          onChange={handleChange}
          className={`w-full px-3 py-2 border rounded-md ${
            validationErrors.title ? 'border-red-500' : 'border-gray-300'
          }`}
          disabled={isLoading}
        />
        {validationErrors.title && (
          <p className="mt-1 text-sm text-red-600">{validationErrors.title.join(', ')}</p>
        )}
      </div>
      
      <div>
        <label className="block text-sm font-medium text-gray-700 mb-1">
          Description
        </label>
        <textarea
          name="description"
          value={formData.description || ''}
          onChange={handleChange}
          rows={4}
          className={`w-full px-3 py-2 border rounded-md ${
            validationErrors.description ? 'border-red-500' : 'border-gray-300'
          }`}
          disabled={isLoading}
        />
        {validationErrors.description && (
          <p className="mt-1 text-sm text-red-600">{validationErrors.description.join(', ')}</p>
        )}
      </div>
      
      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
        <div>
          <label className="block text-sm font-medium text-gray-700 mb-1">
            Status *
          </label>
          <select
            name="status"
            value={formData.status || ''}
            onChange={handleChange}
            className={`w-full px-3 py-2 border rounded-md ${
              validationErrors.status ? 'border-red-500' : 'border-gray-300'
            }`}
            disabled={isLoading}
          >
            <option value="">Select Status</option>
            {Object.values(TaskStatus).map(status => (
              <option key={status} value={status}>
                {status}
              </option>
            ))}
          </select>
          {validationErrors.status && (
            <p className="mt-1 text-sm text-red-600">{validationErrors.status.join(', ')}</p>
          )}
        </div>
        
        <div>
          <label className="block text-sm font-medium text-gray-700 mb-1">
            Priority *
          </label>
          <select
            name="priority"
            value={formData.priority || ''}
            onChange={handleChange}
            className={`w-full px-3 py-2 border rounded-md ${
              validationErrors.priority ? 'border-red-500' : 'border-gray-300'
            }`}
            disabled={isLoading}
          >
            <option value="">Select Priority</option>
            {Object.values(Priority).map(priority => (
              <option key={priority} value={priority}>
                {priority}
              </option>
            ))}
          </select>
          {validationErrors.priority && (
            <p className="mt-1 text-sm text-red-600">{validationErrors.priority.join(', ')}</p>
          )}
        </div>
      </div>
      
      <div>
        <label className="block text-sm font-medium text-gray-700 mb-1">
          Due Date
        </label>
        <input
          type="datetime-local"
          name="dueDate"
          value={formData.dueDate || ''}
          onChange={handleChange}
          className={`w-full px-3 py-2 border rounded-md ${
            validationErrors.dueDate ? 'border-red-500' : 'border-gray-300'
          }`}
          disabled={isLoading}
        />
        {validationErrors.dueDate && (
          <p className="mt-1 text-sm text-red-600">{validationErrors.dueDate.join(', ')}</p>
        )}
      </div>
      
      <div className="flex gap-3">
        <button
          type="submit"
          disabled={isLoading}
          className="px-4 py-2 bg-blue-500 text-white rounded-md hover:bg-blue-600 disabled:opacity-50 disabled:cursor-not-allowed"
        >
          {isEdit ? 'Update Task' : 'Create Task'}
        </button>
        <button
          type="button"
          onClick={() => window.history.back()}
          className="px-4 py-2 bg-gray-200 text-gray-800 rounded-md hover:bg-gray-300"
          disabled={isLoading}
        >
          Cancel
        </button>
      </div>
    </form>
  );
};

export default TaskForm;