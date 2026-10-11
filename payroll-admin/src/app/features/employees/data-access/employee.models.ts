import { JobGrade, PagedQuery } from '@shared/models/api.models';

export interface Employee {
  id: number;
  fullName: string;
  birthDate: string;
  address: string;
  phone: string;
  email: string;
  grade: JobGrade;
  departmentId: number;
  departmentName: string;
  hireDate: string;
}

export interface EmployeeDetails {
  employee: Employee;
  baseSalary: number | null;
  departmentIncentivePercentage: number;
  serviceYears: number;
}

export interface EmployeesQuery extends PagedQuery {
  departmentId?: number | null;
  grade?: JobGrade | null;
}

export interface SaveEmployeeRequest {
  fullName: string;
  birthDate: string;
  address: string;
  phone: string;
  email: string;
  grade: JobGrade;
  departmentId: number;
  hireDate: string;
}
