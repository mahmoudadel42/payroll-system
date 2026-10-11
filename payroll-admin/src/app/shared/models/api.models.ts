export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export interface PagedQuery {
  page: number;
  pageSize: number;
  search?: string | null;
}

export interface Lookup {
  id: number;
  name: string;
}

export const JOB_GRADES = ['First', 'Second', 'Third'] as const;
export type JobGrade = (typeof JOB_GRADES)[number];

export const ABSENCE_RULE_TYPES = ['Deduction', 'Bonus'] as const;
export type AbsenceRuleType = (typeof ABSENCE_RULE_TYPES)[number];

export const PAYROLL_RUN_STATUSES = ['Draft', 'Approved'] as const;
export type PayrollRunStatus = (typeof PAYROLL_RUN_STATUSES)[number];

export type AbsenceEffect = 'None' | 'Bonus' | 'Deduction';

export const DEFAULT_PAGE_SIZE = 10;
export const PAGE_SIZE_OPTIONS = [10, 25, 50, 100];

export const emptyPage = <T>(): PagedResult<T> => ({
  items: [],
  totalCount: 0,
  page: 1,
  pageSize: DEFAULT_PAGE_SIZE,
  totalPages: 0,
});
