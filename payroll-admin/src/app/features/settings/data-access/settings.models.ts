import { AbsenceRuleType, JobGrade } from '@shared/models/api.models';

export interface GradeSalary {
  grade: JobGrade;
  /** `null` when no salary has been configured for the grade yet. */
  baseSalary: number | null;
  updatedAt: string | null;
}

export interface ServiceYearsRule {
  id: number;
  minYears: number;
  percentage: number;
}

export interface SaveServiceYearsRuleRequest {
  minYears: number;
  percentage: number;
}

export interface AbsenceRule {
  id: number;
  type: AbsenceRuleType;
  thresholdDays: number;
  percentage: number;
}

export interface SaveAbsenceRuleRequest {
  type: AbsenceRuleType;
  thresholdDays: number;
  percentage: number;
}
