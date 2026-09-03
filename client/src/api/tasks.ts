import api from "./client";

export interface TaskItem {
  id: string;
  columnId: string;
  title: string;
  description: string | null;
  assigneeUserId: string | null;
  dueDate: string | null;
  position: number;
  createdAt: string;
}

export interface CreateTaskPayload {
  title: string;
  description?: string;
}

export const getTasks = (columnId: string) =>
  api.get<TaskItem[]>(`/columns/${columnId}/tasks`).then((res) => res.data);

export const createTask = (columnId: string, payload: CreateTaskPayload) =>
  api.post<TaskItem>(`/columns/${columnId}/tasks`, payload).then((res) => res.data);