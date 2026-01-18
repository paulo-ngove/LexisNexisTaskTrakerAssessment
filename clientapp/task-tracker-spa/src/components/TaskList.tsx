import React from 'react';
import type { TaskItem } from '../api/types';
import { formatDate, getPriorityColor, getStatusColor } from '../utils/formatters';
import LoadingSpinner from './LoadingSpinner';

interface TaskListProps {
  tasks: TaskItem[];
  isLoading: boolean;
  onEdit: (task: TaskItem) => void;
  onDelete: (id: number) => void;
  onToggleComplete: (task: TaskItem) => void;
  error?: string;
}

const TaskList: React.FC<TaskListProps> = ({
  tasks,
  isLoading,
  onEdit,
  onDelete,
  onToggleComplete,
  error
}) => {
  if (isLoading) {
    return <LoadingSpinner text="Loading tasks..." />;
  }

  if (error) {
    return (
      <div className="bg-red-50 border border-red-200 rounded p-4">
        <p className="text-red-800">{error}</p>
      </div>
    );
  }

  if (tasks.length === 0) {
    return (
      <div className="text-center py-8">
        <p className="text-gray-500">No tasks found. Create your first task!</p>
      </div>
    );
  }

  return (
    <div className="space-y-4">
      {tasks.map((task) => (
        <div
          key={task.id}
          className="border rounded-lg p-4 hover:shadow-md transition-shadow"
        >
          <div className="flex justify-between items-start">
            <div className="flex-1">
              <div className="flex items-center gap-2 mb-2">
                <h3 className="text-lg font-semibold">{task.title}</h3>
                <span className={`px-2 py-1 rounded text-xs font-medium ${getPriorityColor(task.priority)}`}>
                  {task.priority}
                </span>
                <span className={`px-2 py-1 rounded text-xs font-medium ${getStatusColor(task.status)}`}>
                  {task.status}
                </span>
              </div>
              
              {task.description && (
                <p className="text-gray-600 mb-3">{task.description}</p>
              )}
              
              <div className="flex items-center gap-4 text-sm text-gray-500">
                <span>Due: {formatDate(task.dueDate)}</span>
                <span>Created: {formatDate(task.createdAt)}</span>
              </div>
            </div>
            
            <div className="flex gap-2">
              <button
                onClick={() => onToggleComplete(task)}
                className={`px-3 py-1 text-sm rounded ${
                  task.isCompleted
                    ? 'bg-gray-200 text-gray-800 hover:bg-gray-300'
                    : 'bg-green-500 text-white hover:bg-green-600'
                }`}
              >
                {task.isCompleted ? 'Mark Incomplete' : 'Mark Complete'}
              </button>
              
              <button
                onClick={() => onEdit(task)}
                className="px-3 py-1 text-sm bg-blue-500 text-white rounded hover:bg-blue-600"
              >
                Edit
              </button>
              
              <button
                onClick={() => onDelete(task.id)}
                className="px-3 py-1 text-sm bg-red-500 text-white rounded hover:bg-red-600"
              >
                Delete
              </button>
            </div>
          </div>
        </div>
      ))}
    </div>
  );
};

export default TaskList;