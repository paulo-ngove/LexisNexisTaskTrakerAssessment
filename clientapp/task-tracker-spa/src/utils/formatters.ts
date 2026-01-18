import { format } from 'date-fns';

export function formatDate(dateString?: string): string {
  if (!dateString) return 'No due date';
  
  try {
    const date = new Date(dateString);
    return format(date, 'MMM dd, yyyy');
  } catch {
    return 'Invalid date';
  }
}

export function formatDateTime(dateString?: string): string {
  if (!dateString) return '';
  
  try {
    const date = new Date(dateString);
    return format(date, 'MMM dd, yyyy HH:mm');
  } catch {
    return '';
  }
}

export function getPriorityColor(priority: string): string {
  switch (priority) {
    case 'High': return 'bg-red-100 text-red-800';
    case 'Medium': return 'bg-yellow-100 text-yellow-800';
    case 'Low': return 'bg-green-100 text-green-800';
    default: return 'bg-gray-100 text-gray-800';
  }
}

export function getStatusColor(status: string): string {
  switch (status) {
    case 'Done': return 'bg-green-100 text-green-800';
    case 'InProgress': return 'bg-blue-100 text-blue-800';
    case 'New': return 'bg-gray-100 text-gray-800';
    default: return 'bg-gray-100 text-gray-800';
  }
}