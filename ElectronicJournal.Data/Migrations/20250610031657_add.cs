using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElectronicJournal.Domain.Migrations
{
    /// <inheritdoc />
    public partial class add : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subjects_SubjectAssignments_SubjectAssignmentId",
                table: "Subjects");

            migrationBuilder.DropIndex(
                name: "IX_Subjects_SubjectAssignmentId",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "SubjectAssignmentId",
                table: "Subjects");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SubjectAssignmentId",
                table: "Subjects",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Subjects_SubjectAssignmentId",
                table: "Subjects",
                column: "SubjectAssignmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Subjects_SubjectAssignments_SubjectAssignmentId",
                table: "Subjects",
                column: "SubjectAssignmentId",
                principalTable: "SubjectAssignments",
                principalColumn: "Id");
        }
    }
}
