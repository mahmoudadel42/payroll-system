import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { LanguageService } from '@core/i18n/language.service';
import { API_BASE_URL } from './api.config';

/** The API localizes names and error messages based on `Accept-Language`. */
export const languageInterceptor: HttpInterceptorFn = (req, next) => {
  if (!req.url.startsWith(inject(API_BASE_URL))) return next(req);
  const language = inject(LanguageService).current();
  return next(req.clone({ setHeaders: { 'Accept-Language': language } }));
};
