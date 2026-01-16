using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecruitmentApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateInterviewFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AllowFeedback",
                table: "Interviews",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AllowFeedback",
                table: "Interviews");
        }
    }
}
