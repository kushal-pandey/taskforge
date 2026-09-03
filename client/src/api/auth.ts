import api from "./client";

export interface RegisterPayload {
  tenantName: string;
  fullName: string;
  email: string;
  password: string;
}

export interface LoginPayload {
  email: string;
  password: string;
}

export interface AuthResponse {
  accessToken: string;
  refreshToken: string;
  accessTokenExpiresAt: string;
}

export const registerUser = (payload: RegisterPayload) =>
  api.post<AuthResponse>("/auth/register", payload).then((res) => res.data);

export const loginUser = (payload: LoginPayload) =>
  api.post<AuthResponse>("/auth/login", payload).then((res) => res.data);