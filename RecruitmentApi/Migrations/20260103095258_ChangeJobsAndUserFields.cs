using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecruitmentApi.Migrations
{
    /// <inheritdoc />
    public partial class ChangeJobsAndUserFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // -------------------------
            // Users table
            // -------------------------
            migrationBuilder.RenameColumn(name: "name", table: "Users", newName: "Name");
            migrationBuilder.RenameColumn(name: "email", table: "Users", newName: "Email");
            migrationBuilder.RenameColumn(name: "password_hash", table: "Users", newName: "PasswordHash");
            migrationBuilder.RenameColumn(name: "created_at", table: "Users", newName: "CreatedAt");
            migrationBuilder.RenameColumn(name: "user_id", table: "Users", newName: "UserId");

            // -------------------------
            // Roles table
            // -------------------------
            migrationBuilder.RenameColumn(name: "role_name", table: "Roles", newName: "RoleName");
            migrationBuilder.RenameColumn(name: "role_id", table: "Roles", newName: "RoleId");

            // -------------------------
            // Notifications table
            // -------------------------
            // 1️⃣ Drop check constraint first
            // Drop check constraint first
            migrationBuilder.DropCheckConstraint(
                name: "CK_Notifications_Status",
                table: "Notifications");

            // Rename columns
            migrationBuilder.RenameColumn(name: "status", table: "Notifications", newName: "Status");
            migrationBuilder.RenameColumn(name: "message", table: "Notifications", newName: "Message");
            migrationBuilder.RenameColumn(name: "user_id", table: "Notifications", newName: "UserId");
            migrationBuilder.RenameColumn(name: "created_at", table: "Notifications", newName: "CreatedAt");
            migrationBuilder.RenameColumn(name: "notification_id", table: "Notifications", newName: "NotificationId");

            // **Skip renaming the index** because SQL Server cannot find it.
            // If you need the index on UserId, recreate it explicitly:
            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId",
                table: "Notifications",
                column: "UserId");

            // Recreate check constraint
            migrationBuilder.AddCheckConstraint(
                name: "CK_Notifications_Status",
                table: "Notifications",
                sql: "[Status] = 'Archived' OR [Status] = 'Unread' OR [Status] = 'Read'");

            // -------------------------
            // Jobs_Status table
            // -------------------------
            migrationBuilder.RenameColumn(name: "status", table: "Jobs_Status", newName: "Status");
            migrationBuilder.RenameColumn(name: "reason", table: "Jobs_Status", newName: "Reason");
            migrationBuilder.RenameColumn(name: "changed_by", table: "Jobs_Status", newName: "ChangedBy");
            migrationBuilder.RenameColumn(name: "changed_at", table: "Jobs_Status", newName: "ChangedAt");
            migrationBuilder.RenameColumn(name: "status_id", table: "Jobs_Status", newName: "StatusId");

            migrationBuilder.CreateIndex(
    name: "IX_Jobs_Status_ChangedBy",
    table: "Jobs_Status",
    column: "ChangedBy");

            // -------------------------
            // Jobs_Skills table
            // -------------------------
            migrationBuilder.RenameColumn(name: "skill_type", table: "Jobs_Skills", newName: "SkillType");
            migrationBuilder.RenameColumn(name: "job_id", table: "Jobs_Skills", newName: "JobId");
            migrationBuilder.RenameColumn(name: "skill_id", table: "Jobs_Skills", newName: "SkillId");
            // Drop old index if it exists (optional, safe if you know the name)
            migrationBuilder.Sql(@"
IF EXISTS (SELECT name FROM sys.indexes 
           WHERE name = 'IX_Jobs_Skills_job_id' 
           AND object_id = OBJECT_ID('Jobs_Skills'))
BEGIN
    DROP INDEX [IX_Jobs_Skills_job_id] ON [Jobs_Skills]
END
");

            // Recreate the index with the desired name
            migrationBuilder.CreateIndex(
                name: "IX_Jobs_Skills_JobId",
                table: "Jobs_Skills",
                column: "JobId");

            // -------------------------
            // Jobs table
            // -------------------------
            migrationBuilder.RenameColumn(name: "scheduled", table: "Jobs", newName: "Scheduled");
            migrationBuilder.RenameColumn(name: "status_id", table: "Jobs", newName: "StatusId");
            migrationBuilder.RenameColumn(name: "job_title", table: "Jobs", newName: "JobTitle");
            migrationBuilder.RenameColumn(name: "job_description", table: "Jobs", newName: "JobDescription");
            migrationBuilder.RenameColumn(name: "created_by", table: "Jobs", newName: "CreatedBy");
            migrationBuilder.RenameColumn(name: "created_at", table: "Jobs", newName: "CreatedAt");
            migrationBuilder.RenameColumn(name: "job_id", table: "Jobs", newName: "JobId");

            // Drop old index if it exists
            migrationBuilder.Sql(@"
IF EXISTS (SELECT name FROM sys.indexes 
           WHERE name = 'IX_Jobs_status_id' 
           AND object_id = OBJECT_ID('Jobs'))
BEGIN
    DROP INDEX [IX_Jobs_status_id] ON [Jobs]
END
");

            // Recreate the index with the correct name
            migrationBuilder.CreateIndex(
                name: "IX_Jobs_StatusId",
                table: "Jobs",
                column: "StatusId");
            // Drop old index if it exists
            migrationBuilder.Sql(@"
IF EXISTS (SELECT name FROM sys.indexes 
           WHERE name = 'IX_Jobs_created_by' 
           AND object_id = OBJECT_ID('Jobs'))
BEGIN
    DROP INDEX [IX_Jobs_created_by] ON [Jobs]
END
");

            // Create the index with the new name
            migrationBuilder.CreateIndex(
                name: "IX_Jobs_CreatedBy",
                table: "Jobs",
                column: "CreatedBy");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // -------------------------
            // Users table
            // -------------------------
            migrationBuilder.RenameColumn(name: "Name", table: "Users", newName: "name");
            migrationBuilder.RenameColumn(name: "Email", table: "Users", newName: "email");
            migrationBuilder.RenameColumn(name: "PasswordHash", table: "Users", newName: "password_hash");
            migrationBuilder.RenameColumn(name: "CreatedAt", table: "Users", newName: "created_at");
            migrationBuilder.RenameColumn(name: "UserId", table: "Users", newName: "user_id");

            // -------------------------
            // Roles table
            // -------------------------
            migrationBuilder.RenameColumn(name: "RoleName", table: "Roles", newName: "role_name");
            migrationBuilder.RenameColumn(name: "RoleId", table: "Roles", newName: "role_id");

            // -------------------------
            // Notifications table
            // -------------------------
            // Drop check constraint on new column first
            // Drop check constraint first
            migrationBuilder.DropCheckConstraint(
                name: "CK_Notifications_Status",
                table: "Notifications");

            // Drop recreated index
            migrationBuilder.DropIndex(
                name: "IX_Notifications_UserId",
                table: "Notifications");

            // Rename columns back
            migrationBuilder.RenameColumn(name: "Status", table: "Notifications", newName: "status");
            migrationBuilder.RenameColumn(name: "Message", table: "Notifications", newName: "message");
            migrationBuilder.RenameColumn(name: "UserId", table: "Notifications", newName: "user_id");
            migrationBuilder.RenameColumn(name: "CreatedAt", table: "Notifications", newName: "created_at");
            migrationBuilder.RenameColumn(name: "NotificationId", table: "Notifications", newName: "notification_id");

            // Recreate original check constraint
            migrationBuilder.AddCheckConstraint(
                name: "CK_Notifications_Status",
                table: "Notifications",
                sql: "[status] = 'Archived' OR [status] = 'Unread' OR [status] = 'Read'");


            // -------------------------
            // Jobs_Status table
            // -------------------------
            migrationBuilder.RenameColumn(name: "Status", table: "Jobs_Status", newName: "status");
            migrationBuilder.RenameColumn(name: "Reason", table: "Jobs_Status", newName: "reason");
            migrationBuilder.RenameColumn(name: "ChangedBy", table: "Jobs_Status", newName: "changed_by");
            migrationBuilder.RenameColumn(name: "ChangedAt", table: "Jobs_Status", newName: "changed_at");
            migrationBuilder.RenameColumn(name: "StatusId", table: "Jobs_Status", newName: "status_id");

            migrationBuilder.DropIndex(
    name: "IX_Jobs_Status_ChangedBy",
    table: "Jobs_Status");

            // -------------------------
            // Jobs_Skills table
            // -------------------------
            migrationBuilder.RenameColumn(name: "SkillType", table: "Jobs_Skills", newName: "skill_type");
            migrationBuilder.RenameColumn(name: "JobId", table: "Jobs_Skills", newName: "job_id");
            migrationBuilder.RenameColumn(name: "SkillId", table: "Jobs_Skills", newName: "skill_id");
            migrationBuilder.DropIndex(
                name: "IX_Jobs_Skills_JobId",
                table: "Jobs_Skills");

            // Optionally recreate the old index
            migrationBuilder.CreateIndex(
                name: "IX_Jobs_Skills_job_id",
                table: "Jobs_Skills",
                column: "job_id");

            // -------------------------
            // Jobs table
            // -------------------------
            migrationBuilder.RenameColumn(name: "Scheduled", table: "Jobs", newName: "scheduled");
            migrationBuilder.RenameColumn(name: "StatusId", table: "Jobs", newName: "status_id");
            migrationBuilder.RenameColumn(name: "JobTitle", table: "Jobs", newName: "job_title");
            migrationBuilder.RenameColumn(name: "JobDescription", table: "Jobs", newName: "job_description");
            migrationBuilder.RenameColumn(name: "CreatedBy", table: "Jobs", newName: "created_by");
            migrationBuilder.RenameColumn(name: "CreatedAt", table: "Jobs", newName: "created_at");
            migrationBuilder.RenameColumn(name: "JobId", table: "Jobs", newName: "job_id");

            migrationBuilder.DropIndex(
                name: "IX_Jobs_StatusId",
                table: "Jobs");

            // Optionally recreate the old index
            migrationBuilder.CreateIndex(
                name: "IX_Jobs_status_id",
                table: "Jobs",
                column: "status_id");
            migrationBuilder.DropIndex(
                name: "IX_Jobs_CreatedBy",
                table: "Jobs");

            // Optionally recreate the old index
            migrationBuilder.CreateIndex(
                name: "IX_Jobs_created_by",
                table: "Jobs",
                column: "created_by");
        }
    }
}
