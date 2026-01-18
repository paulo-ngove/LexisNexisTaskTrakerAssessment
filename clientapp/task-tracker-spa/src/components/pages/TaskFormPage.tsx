import React, { useState, useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import type { CreateTaskDto, UpdateTaskDto, TaskItem, ProblemDetails } from '../../api/types';
import { apiClient } from '../../api/client';
import TaskForm from '../../components/TaskForm';
import LoadingSpinner from '../../components/LoadingSpinner';

const TaskFormPage: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const isEdit = !!id;
  
  const [task, setTask] = useState<TaskItem | null>(null);
  const [isLoading, setIsLoading] = useState(isEdit);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState<string>('');

  useEffect(() => {
    if (isEdit) {
      fetchTask();
    }
  }, [id]);

  const fetchTask = async () => {
    if (!id) return;
    
    setIsLoading(true);
    try {
      const data = await apiClient.getTask(parseInt(id));
      setTask(data);
    } catch (err) {
      const problem = err as ProblemDetails;
      setError(problem.detail || 'Failed to fetch task');
    } finally {
      setIsLoading(false);
    }
  };

  const handleSubmit = async (formData: CreateTaskDto | UpdateTaskDto) => {
    setIsSubmitting(true);
    setError('');
    
    try {
      if (isEdit && id) {
        await apiClient.updateTask(parseInt(id), formData as UpdateTaskDto);
      } else {
        await apiClient.createTask(formData as CreateTaskDto);
      }
      navigate('/');
    } catch (err) {
      const problem = err as ProblemDetails;
      setError(problem.detail || 'Failed to save task');
      setIsSubmitting(false);
    }
  };

  if (isLoading) {
    return (
      <div className="container mx-auto px-4 py-8">
        <LoadingSpinner text="Loading task..." />
      </div>
    );
  }

  return (
    <div className="container mx-auto px-4 py-8">
      <button
        onClick={() => navigate('/')}
        className="mb-6 px-4 py-2 text-gray-600 hover:text-gray-900"
      >
        ← Back to Tasks
      </button>
      
      <h1 className="text-3xl font-bold mb-8">
        {isEdit ? 'Edit Task' : 'Create New Task'}
      </h1>
      
      <TaskForm
        initialData={task || undefined}
        onSubmit={handleSubmit}
        isLoading={isSubmitting}
        error={error}
        isEdit={isEdit}
      />
    </div>
  );
};

export default TaskFormPage;