using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecruitmentApi.Migrations
{
    /// <inheritdoc />
    public partial class CandidateColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "doc_upload",
                table: "Candidates",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "doc_upload",
                table: "Candidates");
        }
    }
}
