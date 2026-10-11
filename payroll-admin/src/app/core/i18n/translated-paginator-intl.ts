import { Injectable, inject } from '@angular/core';
import { MatPaginatorIntl } from '@angular/material/paginator';
import { TranslateService } from '@ngx-translate/core';

@Injectable()
export class TranslatedPaginatorIntl extends MatPaginatorIntl {
  private readonly translate = inject(TranslateService);

  constructor() {
    super();
    this.translate.onLangChange.subscribe(() => this.update());
    this.update();
  }

  override getRangeLabel = (page: number, pageSize: number, length: number): string => {
    if (length === 0) return this.translate.instant('paginator.empty');
    const start = page * pageSize + 1;
    const end = Math.min(start + pageSize - 1, length);
    return this.translate.instant('paginator.range', { start, end, length });
  };

  private update(): void {
    this.itemsPerPageLabel = this.translate.instant('paginator.itemsPerPage');
    this.nextPageLabel = this.translate.instant('paginator.next');
    this.previousPageLabel = this.translate.instant('paginator.previous');
    this.firstPageLabel = this.translate.instant('paginator.first');
    this.lastPageLabel = this.translate.instant('paginator.last');
    this.changes.next();
  }
}
