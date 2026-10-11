import { AbsenceEffect, JobGrade, PayrollRunStatus } from '@shared/models/api.models';
import { Payslip } from '@features/payroll-runs/data-access/payroll-run.models';

export type ReportKind = 'salaries' | 'incentives-deductions' | 'attendance' | 'employees';

export interface MonthlyReportFilter {
  year: number;
  month: number;
  departmentId?: number | null;
}

export function currentMonthFilter(): MonthlyReportFilter {
  const now = new Date();
  return { year: now.getFullYear(), month: now.getMonth() + 1, departmentId: null };
}

export interface EmployeesReportFilter {
  departmentId?: number | null;
  grade?: JobGrade | null;
  asOf?: string | null;
}

export interface SalariesReport {
  year: number;
  month: number;
  isPreview: boolean;
  runStatus: PayrollRunStatus | null;
  rows: Payslip[];
  totals: {
    employeesCount: number;
    baseSalary: number;
    departmentIncentive: number;
    serviceYearsIncentive: number;
    absenceBonus: number;
    absenceDeduction: number;
    netSalary: number;
  };
}

export interface IncentivesDeductionsRow {
  employeeId: number;
  employeeName: string;
  departmentName: string;
  serviceYears: number;
  absenceDays: number;
  departmentIncentive: number;
  serviceYearsIncentive: number;
  absenceBonus: number;
  absenceDeduction: number;
  totalIncentives: number;
  netEffect: number;
}

export interface IncentivesDeductionsReport {
  year: number;
  month: number;
  isPreview: boolean;
  runStatus: PayrollRunStatus | null;
  rows: IncentivesDeductionsRow[];
  totals: {
    departmentIncentive: number;
    serviceYearsIncentive: number;
    absenceBonus: number;
    absenceDeduction: number;
    totalIncentives: number;
    netEffect: number;
  };
}

export interface AttendanceReport {
  year: number;
  month: number;
  rows: {
    employeeId: number;
    employeeName: string;
    departmentName: string;
    absenceDays: number;
    dates: string;
    effect: AbsenceEffect;
    effectPercentage: number;
  }[];
  totalAbsenceDays: number;
  employeesWithBonus: number;
  employeesWithDeduction: number;
}

export interface CountItem {
  name: string;
  count: number;
}

export interface EmployeesReport {
  asOf: string;
  rows: {
    employeeId: number;
    fullName: string;
    email: string;
    phone: string;
    departmentName: string;
    grade: JobGrade;
    birthDate: string;
    hireDate: string;
    serviceYears: number;
    baseSalary: number | null;
  }[];
  totalEmployees: number;
  byDepartment: CountItem[];
  byGrade: CountItem[];
}
