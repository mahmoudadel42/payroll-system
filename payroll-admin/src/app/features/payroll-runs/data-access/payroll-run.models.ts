import { JobGrade, PagedQuery, PayrollRunStatus } from '@shared/models/api.models';

export interface PayrollRun {
  id: number;
  year: number;
  month: number;
  status: PayrollRunStatus;
  totalNet: number;
  employeesCount: number;
  createdAt: string;
  approvedAt: string | null;
}

export interface Payslip {
  id: number;
  payrollRunId: number;
  employeeId: number;
  employeeName: string;
  departmentId: number;
  departmentName: string;
  grade: JobGrade;
  serviceYears: number;
  absenceDays: number;
  baseSalary: number;
  departmentIncentivePercentage: number;
  departmentIncentive: number;
  serviceYearsIncentivePercentage: number;
  serviceYearsIncentive: number;
  absenceBonusPercentage: number;
  absenceBonus: number;
  absenceDeductionPercentage: number;
  absenceDeduction: number;
  netSalary: number;
}

export interface PayrollRunDetails {
  run: PayrollRun;
  payslips: Payslip[];
}

export interface PayrollRunsQuery extends PagedQuery {
  year?: number | null;
  status?: PayrollRunStatus | null;
}

export interface GeneratePayrollRunRequest {
  year: number;
  month: number;
}
