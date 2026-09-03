import api from "./client";

export interface Column {
  id: string;
  boardId: string;
  name: string;
  order: number;
  createdAt: string;
}

export interface CreateColumnPayload {
  name: string;
  order: number;
}

export const getColumns = (boardId: string) =>
  api.get<Column[]>(`/boards/${boardId}/columns`).then((res) => res.data);

export const createColumn = (boardId: string, payload: CreateColumnPayload) =>
  api.post<Column>(`/boards/${boardId}/columns`, payload).then((res) => res.data);