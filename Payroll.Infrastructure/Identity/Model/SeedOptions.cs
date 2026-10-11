namespace Payroll.Infrastructure.Identity.Model
{
    public class SeedOptions
    {
        public const string Section = "Seed";

        public string AdminUserName { get; set; } = "admin";
        public string AdminPassword { get; set; } = string.Empty;
        public string HrUserName { get; set; } = "hr";
        public string HrPassword { get; set; } = string.Empty;
        public bool SampleData { get; set; }
    }

}
