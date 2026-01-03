using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecruitmentApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEmailRecipient : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmailRecipients_Users_user_id",
                table: "EmailRecipients");

            migrationBuilder.DropIndex(
                name: "IX_EmailRecipients_user_id",
                table: "EmailRecipients");

            migrationBuilder.DropColumn(
                name: "user_id",
                table: "EmailRecipients");

            migrationBuilder.AddColumn<string>(
                name: "email",
                table: "EmailRecipients",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "email",
                table: "EmailRecipients");

            migrationBuilder.AddColumn<string>(
                name: "user_id",
                table: "EmailRecipients",
                type: "char(8)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_EmailRecipients_user_id",
                table: "EmailRecipients",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_EmailRecipients_Users_user_id",
                table: "EmailRecipients",
                column: "user_id",
                principalTable: "Users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
