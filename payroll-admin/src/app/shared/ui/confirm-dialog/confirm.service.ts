import { Injectable, inject } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Observable, filter, map } from 'rxjs';
import { ConfirmDialog, ConfirmDialogData } from './confirm-dialog';

@Injectable({ providedIn: 'root' })
export class ConfirmService {
  private readonly dialog = inject(MatDialog);

  /** Emits once, and only if the user confirms. */
  confirm(data: ConfirmDialogData): Observable<true> {
    return this.dialog
      .open<ConfirmDialog, ConfirmDialogData, boolean>(ConfirmDialog, { data, width: '420px' })
      .afterClosed()
      .pipe(
        filter((confirmed) => confirmed === true),
        map(() => true as const),
      );
  }

  confirmDelete(name: string): Observable<true> {
    return this.confirm({
      title: 'common.deleteTitle',
      message: 'common.deleteMessage',
      confirmText: 'common.delete',
      params: { name },
      danger: true,
    });
  }
}
