export interface LoginRequest {
  username: string;
  password: string;
}

export interface UserProfile {
  id: number;
  username: string;
  fullName: string;
}

export interface LoginResponse {
  token: string;
  expiresAtUtc: string;
  user: UserProfile;
}
