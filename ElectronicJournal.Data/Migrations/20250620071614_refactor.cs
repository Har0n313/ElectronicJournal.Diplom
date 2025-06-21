using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElectronicJournal.Domain.Migrations
{
    /// <inheritdoc />
    public partial class refactor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "GradeValue",
                table: "Assessments",
                newName: "MarkValue");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MarkValue",
                table: "Assessments",
                newName: "GradeValue");
        }
    }
}
