export interface Department {
  id: number;
  nameAr: string;
  nameEn: string;
  incentivePercentage: number;
  employeesCount: number;
}

export interface SaveDepartmentRequest {
  nameAr: string;
  nameEn: string;
  incentivePercentage: number;
}
