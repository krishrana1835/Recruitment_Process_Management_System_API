using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecruitmentApi.Migrations
{
    public partial class RemoveUnusedIndexes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_HangFire_Job_StateName'
           AND object_id = OBJECT_ID('[HangFire].[Job]'))
    DROP INDEX [IX_HangFire_Job_StateName] ON [HangFire].[Job];

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_HangFire_Job_ExpireAt'
           AND object_id = OBJECT_ID('[HangFire].[Job]'))
    DROP INDEX [IX_HangFire_Job_ExpireAt] ON [HangFire].[Job];

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_HangFire_State_CreatedAt'
           AND object_id = OBJECT_ID('[HangFire].[State]'))
    DROP INDEX [IX_HangFire_State_CreatedAt] ON [HangFire].[State];

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_HangFire_Set_Score'
           AND object_id = OBJECT_ID('[HangFire].[Set]'))
    DROP INDEX [IX_HangFire_Set_Score] ON [HangFire].[Set];

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_HangFire_Set_ExpireAt'
           AND object_id = OBJECT_ID('[HangFire].[Set]'))
    DROP INDEX [IX_HangFire_Set_ExpireAt] ON [HangFire].[Set];

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_HangFire_List_ExpireAt'
           AND object_id = OBJECT_ID('[HangFire].[List]'))
    DROP INDEX [IX_HangFire_List_ExpireAt] ON [HangFire].[List];

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_HangFire_Hash_ExpireAt'
           AND object_id = OBJECT_ID('[HangFire].[Hash]'))
    DROP INDEX [IX_HangFire_Hash_ExpireAt] ON [HangFire].[Hash];

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_HangFire_AggregatedCounter_ExpireAt'
           AND object_id = OBJECT_ID('[HangFire].[AggregatedCounter]'))
    DROP INDEX [IX_HangFire_AggregatedCounter_ExpireAt] ON [HangFire].[AggregatedCounter];

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_HangFire_Server_LastHeartbeat'
           AND object_id = OBJECT_ID('[HangFire].[Server]'))
    DROP INDEX [IX_HangFire_Server_LastHeartbeat] ON [HangFire].[Server];
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // ❗ Do NOT recreate HangFire indexes from EF
            // HangFire manages its own schema
        }
    }
}
