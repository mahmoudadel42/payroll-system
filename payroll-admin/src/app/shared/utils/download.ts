import { HttpResponse } from '@angular/common/http';

/** Saves a blob response, honoring the server's `Content-Disposition` file name when present. */
export function saveBlobResponse(response: HttpResponse<Blob>, fallbackName: string): void {
  if (!response.body) return;
  const disposition = response.headers.get('Content-Disposition') ?? '';
  const match = /filename\*?=(?:UTF-8'')?"?([^";]+)"?/i.exec(disposition);
  const fileName = match ? decodeURIComponent(match[1]) : fallbackName;

  const url = URL.createObjectURL(response.body);
  const link = document.createElement('a');
  link.href = url;
  link.download = fileName;
  link.click();
  URL.revokeObjectURL(url);
}
