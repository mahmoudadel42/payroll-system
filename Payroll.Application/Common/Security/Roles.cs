namespace Payroll.Application.Common.Security
{
    public static class Roles
    {
        public const string Admin = "Admin";
        public const string HR = "HR";
        public const string AdminOrHR = Admin + "," + HR;
    }

}
