using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecruitmentApi.Migrations
{
    /// <inheritdoc />
    public partial class NewMigrations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HR_Review_Interviews_InterviewId",
                table: "HR_Review");

            migrationBuilder.DropColumn(
                name: "candidate_id1",
                table: "Interviews");

            migrationBuilder.DropColumn(
                name: "interview_type_id1",
                table: "Interviews");

            migrationBuilder.DropColumn(
                name: "job_id1",
                table: "Interviews");

            migrationBuilder.DropColumn(
                name: "scheduled_by1",
                table: "Interviews");

            migrationBuilder.DropColumn(
                name: "candidate_id1",
                table: "Candidate_Documents");

            migrationBuilder.RenameColumn(
                name: "InterviewId",
                table: "HR_Review",
                newName: "interview_id1");

            migrationBuilder.RenameIndex(
                name: "IX_HR_Review_InterviewId",
                table: "HR_Review",
                newName: "IX_HR_Review_interview_id1");

            migrationBuilder.AddForeignKey(
                name: "FK_HR_Review_Interviews_interview_id1",
                table: "HR_Review",
                column: "interview_id1",
                principalTable: "Interviews",
                principalColumn: "interview_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HR_Review_Interviews_interview_id1",
                table: "HR_Review");

            migrationBuilder.RenameColumn(
                name: "interview_id1",
                table: "HR_Review",
                newName: "InterviewId");

            migrationBuilder.RenameIndex(
                name: "IX_HR_Review_interview_id1",
                table: "HR_Review",
                newName: "IX_HR_Review_InterviewId");

            migrationBuilder.AddColumn<string>(
                name: "candidate_id1",
                table: "Interviews",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "interview_type_id1",
                table: "Interviews",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "job_id1",
                table: "Interviews",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "scheduled_by1",
                table: "Interviews",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "candidate_id1",
                table: "Candidate_Documents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_HR_Review_Interviews_InterviewId",
                table: "HR_Review",
                column: "InterviewId",
                principalTable: "Interviews",
                principalColumn: "interview_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
