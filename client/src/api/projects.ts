import api from "./client";

export interface Project {
  id: string;
  name: string;
  description: string | null;
  createdAt: string;
}

export interface CreateProjectPayload {
  name: string;
  description?: string;
}

export const getProjects = () => api.get<Project[]>("/projects").then((res) => res.data);

export const createProject = (payload: CreateProjectPayload) =>
  api.post<Project>("/projects", payload).then((res) => res.data);

export const getProjectById = (id: string) =>
  api.get<Project>(`/projects/${id}`).then((res) => res.data);