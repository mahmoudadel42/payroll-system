import { Injectable } from '@angular/core';
import { AuthSession } from './auth.models';

const STORAGE_KEY = 'payroll.session';

@Injectable({ providedIn: 'root' })
export class SessionStorageService {
  read(): AuthSession | null {
    try {
      const raw = localStorage.getItem(STORAGE_KEY);
      const session = raw ? (JSON.parse(raw) as AuthSession) : null;
      return session && new Date(session.refreshTokenExpiresAt) > new Date() ? session : null;
    } catch {
      return null;
    }
  }

  write(session: AuthSession): void {
    localStorage.setItem(STORAGE_KEY, JSON.stringify(session));
  }

  clear(): void {
    localStorage.removeItem(STORAGE_KEY);
  }
}
