export type Role = 'Admin' | 'HR';

export interface LoginRequest {
  userName: string;
  password: string;
}

export interface AuthSession {
  accessToken: string;
  accessTokenExpiresAt: string;
  refreshToken: string;
  refreshTokenExpiresAt: string;
  userName: string;
  displayName: string;
  roles: Role[];
}
