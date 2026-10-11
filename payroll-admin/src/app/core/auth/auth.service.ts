import { HttpClient, HttpContext } from '@angular/common/http';
import { Injectable, computed, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, catchError, finalize, map, shareReplay, tap, throwError } from 'rxjs';
import { API_BASE_URL } from '@core/http/api.config';
import { SKIP_AUTH, SKIP_ERROR_TOAST } from '@core/http/http-context';
import { NotificationService } from '@core/notifications/notification.service';
import { AuthSession, LoginRequest, Role } from './auth.models';
import { SessionStorageService } from './session-storage.service';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);
  private readonly storage = inject(SessionStorageService);
  private readonly notifications = inject(NotificationService);
  private readonly baseUrl = `${inject(API_BASE_URL)}/auth`;

  private readonly session = signal<AuthSession | null>(this.storage.read());
  private refreshInFlight$: Observable<string> | null = null;
  private expiring = false;

  readonly isAuthenticated = computed(() => this.session() !== null);
  readonly displayName = computed(() => this.session()?.displayName ?? '');
  readonly userName = computed(() => this.session()?.userName ?? '');
  readonly roles = computed<Role[]>(() => this.session()?.roles ?? []);
  readonly isAdmin = computed(() => this.roles().includes('Admin'));

  get accessToken(): string | null {
    return this.session()?.accessToken ?? null;
  }

  hasAnyRole(roles: readonly Role[]): boolean {
    return roles.length === 0 || roles.some((role) => this.roles().includes(role));
  }

  login(request: LoginRequest): Observable<void> {
    const context = new HttpContext().set(SKIP_AUTH, true).set(SKIP_ERROR_TOAST, true);
    return this.http.post<AuthSession>(`${this.baseUrl}/login`, request, { context }).pipe(
      tap((session) => this.setSession(session)),
      map(() => undefined),
    );
  }

  /** Exchanges the refresh token for a new access token; concurrent callers share one request. */
  refreshAccessToken(): Observable<string> {
    const refreshToken = this.session()?.refreshToken;
    if (!refreshToken) return throwError(() => new Error('No refresh token available.'));

    this.refreshInFlight$ ??= this.http
      .post<AuthSession>(
        `${this.baseUrl}/refresh`,
        { refreshToken },
        { context: new HttpContext().set(SKIP_AUTH, true).set(SKIP_ERROR_TOAST, true) },
      )
      .pipe(
        tap((session) => this.setSession(session)),
        map((session) => session.accessToken),
        catchError((error) => {
          this.clearSession();
          return throwError(() => error);
        }),
        finalize(() => (this.refreshInFlight$ = null)),
        shareReplay({ bufferSize: 1, refCount: false }),
      );
    return this.refreshInFlight$;
  }

  logout(): void {
    const refreshToken = this.session()?.refreshToken;
    if (refreshToken) {
      this.http
        .post(`${this.baseUrl}/logout`, { refreshToken }, {
          context: new HttpContext().set(SKIP_AUTH, true).set(SKIP_ERROR_TOAST, true),
        })
        .subscribe({ error: () => undefined });
    }
    this.clearSession();
    void this.router.navigate(['/login']);
  }

  /** Called when the API rejects the session and it cannot be refreshed. */
  expireSession(): void {
    if (this.expiring || this.router.url.startsWith('/login')) return;
    this.expiring = true;
    this.clearSession();
    this.notifications.error('auth.sessionExpired');
    void this.router
      .navigate(['/login'], { queryParams: { returnUrl: this.router.url } })
      .finally(() => (this.expiring = false));
  }

  private setSession(session: AuthSession): void {
    this.storage.write(session);
    this.session.set(session);
  }

  private clearSession(): void {
    this.storage.clear();
    this.session.set(null);
  }
}
