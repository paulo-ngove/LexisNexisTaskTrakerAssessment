import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import type { TaskItem, ProblemDetails } from '../../api/types';
import { apiClient } from '../../api/client';
import TaskList from '../../components/TaskList';
import LoadingSpinner from '../../components/LoadingSpinner';
import { AppSwal } from '../AppSwal';

const TaskListView: React.FC = () => {
  const navigate = useNavigate();
  const [tasks, setTasks] = useState<TaskItem[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string>('');
  const [searchQuery, setSearchQuery] = useState('');
  const [sortOrder, setSortOrder] = useState<'dueDate:asc' | 'dueDate:desc'>('dueDate:asc');
  const [isDeleting, setIsDeleting] = useState<number | null>(null);

  const fetchTasks = async () => {
    setIsLoading(true);
    setError('');
    
    try {
      const params: any = {};
      if (searchQuery) params.q = searchQuery;
      if (sortOrder) params.sort = sortOrder;
      
      const data = await apiClient.getTasks(params);
      setTasks(data);
    } catch (err) {
      const problem = err as ProblemDetails;
      setError(problem.detail || 'Failed to fetch tasks');
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    fetchTasks();
  }, [searchQuery, sortOrder]);

  const handleSearch = (e: React.FormEvent) => {
    e.preventDefault();
    fetchTasks();
  };

  const handleEdit = (task: TaskItem) => {
    navigate(`/tasks/${task.id}/edit`);
  };

 const handleDelete = async (id: number) => {
  const result = await AppSwal.fire({
    title: 'Delete Task?',
    text: "This action cannot be undone.",
    icon: 'warning',
    showCancelButton: true,
    confirmButtonText: 'Confirm Delete',
    cancelButtonText: 'Cancel',
    reverseButtons: true
  });

  if (!result.isConfirmed) return;

  setIsDeleting(id);
  try {
    await apiClient.deleteTask(id);
    setTasks(prev => prev.filter(t => t.id !== id));

    // Simple toast for success
    AppSwal.fire({
      icon: 'success',
      title: 'Success',
      text: 'Task removed.',
      timer: 1500,
      showConfirmButton: false,
      toast: true,
      position: 'top-end'
    });
  } catch (err) {
    const problem = err as ProblemDetails;
    AppSwal.fire({
      icon: 'error',
      title: 'Oops...',
      text: problem.detail || 'Something went wrong!',
    });
  } finally {
    setIsDeleting(null);
  }
};

  const handleToggleComplete = async (task: TaskItem) => {
    try {
      const updatedTask = task.isCompleted
        ? await apiClient.markAsIncomplete(task.id)
        : await apiClient.markAsComplete(task.id);
      
      setTasks(tasks.map(t => t.id === task.id ? updatedTask : t));
    } catch (err) {
      const problem = err as ProblemDetails;
      AppSwal.fire({
      icon: 'error',
      title: 'Oops...',
      text: problem.detail || 'Failed to update task status!',
    });
    }
  };

  return (
    <div className="container mx-auto px-4 py-8">
      <div className="flex justify-between items-center mb-8">
        <h1 className="text-3xl font-bold">Task Manager</h1>
        <button
          onClick={() => navigate('/tasks/new')}
          className="px-4 py-2 bg-blue-500 text-white rounded-md hover:bg-blue-600"
        >
          + New Task
        </button>
      </div>

      <div className="bg-white rounded-lg shadow p-6 mb-6">
        <form onSubmit={handleSearch} className="space-y-4">
          <div className="flex gap-4">
            <div className="flex-1">
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Search Tasks
              </label>
              <input
                type="text"
                value={searchQuery}
                onChange={(e) => setSearchQuery(e.target.value)}
                placeholder="Search by title or description..."
                className="w-full px-3 py-2 border border-gray-300 rounded-md"
              />
            </div>
            
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Sort by Due Date
              </label>
              <select
                value={sortOrder}
                onChange={(e) => setSortOrder(e.target.value as any)}
                className="w-full px-3 py-2 border border-gray-300 rounded-md"
              >
                <option value="dueDate:asc">Ascending (Oldest First)</option>
                <option value="dueDate:desc">Descending (Newest First)</option>
              </select>
            </div>
            
            <div className="self-end">
              <button
                type="submit"
                className="px-4 py-2 bg-gray-800 text-white rounded-md hover:bg-gray-900"
              >
                Search
              </button>
            </div>
          </div>
        </form>
      </div>

      {isLoading && tasks.length === 0 ? (
        <LoadingSpinner text="Loading tasks..." />
      ) : (
        <TaskList
          tasks={tasks}
          isLoading={false}
          onEdit={handleEdit}
          onDelete={handleDelete}
          onToggleComplete={handleToggleComplete}
          error={error}
        />
      )}

      {isDeleting && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center">
          <div className="bg-white p-6 rounded-lg">
            <LoadingSpinner text="Deleting task..." />
          </div>
        </div>
      )}
    </div>
  );
};

export default TaskListView;