using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecruitmentApi.Migrations
{
    public partial class RestoreHangfireIndexes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
CREATE NONCLUSTERED INDEX [IX_HangFire_Job_StateName]
ON [HangFire].[Job] ([StateName]);

CREATE NONCLUSTERED INDEX [IX_HangFire_Job_ExpireAt]
ON [HangFire].[Job] ([ExpireAt]);

CREATE NONCLUSTERED INDEX [IX_HangFire_List_ExpireAt]
ON [HangFire].[List] ([ExpireAt]);

CREATE NONCLUSTERED INDEX [IX_HangFire_Set_ExpireAt]
ON [HangFire].[Set] ([ExpireAt]);

CREATE NONCLUSTERED INDEX [IX_HangFire_Set_Score]
ON [HangFire].[Set] ([Score]);

CREATE NONCLUSTERED INDEX [IX_HangFire_Hash_ExpireAt]
ON [HangFire].[Hash] ([ExpireAt]);

CREATE NONCLUSTERED INDEX [IX_HangFire_State_CreatedAt]
ON [HangFire].[State] ([CreatedAt]);

CREATE NONCLUSTERED INDEX [IX_HangFire_AggregatedCounter_ExpireAt]
ON [HangFire].[AggregatedCounter] ([ExpireAt]);

CREATE NONCLUSTERED INDEX [IX_HangFire_Server_LastHeartbeat]
ON [HangFire].[Server] ([LastHeartbeat]);
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // NEVER drop Hangfire indexes again
        }
    }
}
