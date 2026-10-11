import { PagedQuery } from '@shared/models/api.models';

export interface Absence {
  id: number;
  employeeId: number;
  employeeName: string;
  departmentName: string;
  date: string;
  reason: string | null;
}

export interface AbsencesQuery extends PagedQuery {
  employeeId?: number | null;
  departmentId?: number | null;
  from?: string | null;
  to?: string | null;
}

export interface CreateAbsencesRequest {
  employeeId: number;
  dates: string[];
  reason: string | null;
}

export interface UpdateAbsenceRequest {
  date: string;
  reason: string | null;
}
