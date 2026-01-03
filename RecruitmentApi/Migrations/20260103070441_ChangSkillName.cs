using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecruitmentApi.Migrations
{
    /// <inheritdoc />
    public partial class ChangSkillName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "skill_name",
                table: "Skills",
                newName: "SkillName");

            migrationBuilder.RenameColumn(
                name: "skill_id",
                table: "Skills",
                newName: "SkillId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SkillName",
                table: "Skills",
                newName: "skill_name");

            migrationBuilder.RenameColumn(
                name: "SkillId",
                table: "Skills",
                newName: "skill_id");
        }
    }
}
