using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Payroll.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AbsenceRuleThreshold : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MinDays",
                table: "AbsenceRules",
                newName: "ThresholdDays");

            migrationBuilder.RenameIndex(
                name: "IX_AbsenceRules_Type_MinDays",
                table: "AbsenceRules",
                newName: "IX_AbsenceRules_Type_ThresholdDays");

            migrationBuilder.Sql("UPDATE [AbsenceRules] SET [ThresholdDays] = 1 WHERE [Type] = 2 AND [ThresholdDays] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE [AbsenceRules] SET [ThresholdDays] = 0 WHERE [Type] = 2 AND [ThresholdDays] = 1");

            migrationBuilder.RenameColumn(
                name: "ThresholdDays",
                table: "AbsenceRules",
                newName: "MinDays");

            migrationBuilder.RenameIndex(
                name: "IX_AbsenceRules_Type_ThresholdDays",
                table: "AbsenceRules",
                newName: "IX_AbsenceRules_Type_MinDays");
        }
    }
}
