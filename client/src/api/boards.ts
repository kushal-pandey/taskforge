import api from "./client";

export interface Board {
  id: string;
  projectId: string;
  name: string;
  createdAt: string;
}

export interface CreateBoardPayload {
  name: string;
}

export const getBoards = (projectId: string) =>
  api.get<Board[]>(`/projects/${projectId}/boards`).then((res) => res.data);

export const getBoardById = (projectId: string, boardId: string) =>
  api.get<Board>(`/projects/${projectId}/boards/${boardId}`).then((res) => res.data);

export const createBoard = (projectId: string, payload: CreateBoardPayload) =>
  api.post<Board>(`/projects/${projectId}/boards`, payload).then((res) => res.data);