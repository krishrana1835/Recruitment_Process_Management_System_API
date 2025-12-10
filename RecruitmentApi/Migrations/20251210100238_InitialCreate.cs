using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecruitmentApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Candidates",
                columns: table => new
                {
                    candidate_id = table.Column<string>(type: "char(8)", unicode: false, fixedLength: true, maxLength: 8, nullable: false),
                    full_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    resume_path = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    password = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Candidates", x => x.candidate_id);
                });

            migrationBuilder.CreateTable(
                name: "Interview_Type",
                columns: table => new
                {
                    interview_type_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    interview_round_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    process_descreption = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Interview_Type", x => x.interview_type_id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    role_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    role_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.role_id);
                });

            migrationBuilder.CreateTable(
                name: "Skills",
                columns: table => new
                {
                    skill_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    skill_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Skills", x => x.skill_id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    user_id = table.Column<string>(type: "char(8)", unicode: false, fixedLength: true, maxLength: 8, nullable: false),
                    name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    password_hash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.user_id);
                });

            migrationBuilder.CreateTable(
                name: "Candidate_Documents",
                columns: table => new
                {
                    document_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    document_type = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    file_path = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    verification_status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Pending"),
                    uploaded_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    candidate_id = table.Column<string>(type: "char(8)", unicode: false, fixedLength: true, maxLength: 8, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Candidate_Documents", x => x.document_id);
                    table.ForeignKey(
                        name: "FK_Candidate_Documents_Candidates",
                        column: x => x.candidate_id,
                        principalTable: "Candidates",
                        principalColumn: "candidate_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Candidate_Skills",
                columns: table => new
                {
                    candidate_skill_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    years_experience = table.Column<int>(type: "int", nullable: false),
                    skill_id = table.Column<int>(type: "int", nullable: false),
                    candidate_id = table.Column<string>(type: "char(8)", unicode: false, fixedLength: true, maxLength: 8, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Candidate_Skills", x => x.candidate_skill_id);
                    table.ForeignKey(
                        name: "FK_Candidate_Skills_Candidates",
                        column: x => x.candidate_id,
                        principalTable: "Candidates",
                        principalColumn: "candidate_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Candidate_Skills_Skills",
                        column: x => x.skill_id,
                        principalTable: "Skills",
                        principalColumn: "skill_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Jobs_Status",
                columns: table => new
                {
                    status_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    changed_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    changed_by = table.Column<string>(type: "char(8)", unicode: false, fixedLength: true, maxLength: 8, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jobs_Status", x => x.status_id);
                    table.ForeignKey(
                        name: "FK_Jobs_Status_Users",
                        column: x => x.changed_by,
                        principalTable: "Users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    notification_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    message = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Unread"),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    user_id = table.Column<string>(type: "char(8)", unicode: false, fixedLength: true, maxLength: 8, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.notification_id);
                    table.ForeignKey(
                        name: "FK_Notifications_Users",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "Users_Roles",
                columns: table => new
                {
                    role_id = table.Column<int>(type: "int", nullable: false),
                    user_id = table.Column<string>(type: "char(8)", unicode: false, fixedLength: true, maxLength: 8, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users_Roles", x => new { x.role_id, x.user_id });
                    table.ForeignKey(
                        name: "FK_Users_Roles_Roles",
                        column: x => x.role_id,
                        principalTable: "Roles",
                        principalColumn: "role_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Users_Roles_Users",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Jobs",
                columns: table => new
                {
                    job_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    job_title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    job_description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    created_by = table.Column<string>(type: "char(8)", unicode: false, fixedLength: true, maxLength: 8, nullable: false),
                    status_id = table.Column<int>(type: "int", nullable: false),
                    scheduled = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Pending")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jobs", x => x.job_id);
                    table.ForeignKey(
                        name: "FK_Jobs_Jobs_Status",
                        column: x => x.status_id,
                        principalTable: "Jobs_Status",
                        principalColumn: "status_id");
                    table.ForeignKey(
                        name: "FK_Jobs_Users",
                        column: x => x.created_by,
                        principalTable: "Users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "Candidate_Reviews",
                columns: table => new
                {
                    review_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    comments = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    reviewed_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    candidate_id = table.Column<string>(type: "char(8)", unicode: false, fixedLength: true, maxLength: 8, nullable: false),
                    job_id = table.Column<int>(type: "int", nullable: false),
                    user_id = table.Column<string>(type: "char(8)", unicode: false, fixedLength: true, maxLength: 8, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Candidate_Reviews", x => x.review_id);
                    table.ForeignKey(
                        name: "FK_Candidate_Reviews_Candidates",
                        column: x => x.candidate_id,
                        principalTable: "Candidates",
                        principalColumn: "candidate_id");
                    table.ForeignKey(
                        name: "FK_Candidate_Reviews_Jobs",
                        column: x => x.job_id,
                        principalTable: "Jobs",
                        principalColumn: "job_id");
                    table.ForeignKey(
                        name: "FK_Candidate_Reviews_Users",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "Candidate_Status_History",
                columns: table => new
                {
                    candidate_status_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    changed_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    candidate_id = table.Column<string>(type: "char(8)", unicode: false, fixedLength: true, maxLength: 8, nullable: false),
                    job_id = table.Column<int>(type: "int", nullable: false),
                    changed_by = table.Column<string>(type: "char(8)", unicode: false, fixedLength: true, maxLength: 8, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Candidate_Status_History", x => x.candidate_status_id);
                    table.ForeignKey(
                        name: "FK_Candidate_Status_History_Candidates",
                        column: x => x.candidate_id,
                        principalTable: "Candidates",
                        principalColumn: "candidate_id");
                    table.ForeignKey(
                        name: "FK_Candidate_Status_History_Jobs",
                        column: x => x.job_id,
                        principalTable: "Jobs",
                        principalColumn: "job_id");
                    table.ForeignKey(
                        name: "FK_Candidate_Status_History_Users",
                        column: x => x.changed_by,
                        principalTable: "Users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "Employee_Records",
                columns: table => new
                {
                    employee_id = table.Column<string>(type: "char(8)", unicode: false, fixedLength: true, maxLength: 8, nullable: false),
                    joining_date = table.Column<DateOnly>(type: "date", nullable: false),
                    offer_letter_path = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    candidate_id = table.Column<string>(type: "char(8)", unicode: false, fixedLength: true, maxLength: 8, nullable: false),
                    job_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employee_Records", x => x.employee_id);
                    table.ForeignKey(
                        name: "FK_Employee_Records_Candidates",
                        column: x => x.candidate_id,
                        principalTable: "Candidates",
                        principalColumn: "candidate_id");
                    table.ForeignKey(
                        name: "FK_Employee_Records_Jobs",
                        column: x => x.job_id,
                        principalTable: "Jobs",
                        principalColumn: "job_id");
                });

            migrationBuilder.CreateTable(
                name: "Interviews",
                columns: table => new
                {
                    interview_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    round_number = table.Column<int>(type: "int", nullable: false),
                    location_or_link = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    candidate_id = table.Column<string>(type: "char(8)", unicode: false, fixedLength: true, maxLength: 8, nullable: false),
                    job_id = table.Column<int>(type: "int", nullable: false),
                    scheduled_by = table.Column<string>(type: "char(8)", unicode: false, fixedLength: true, maxLength: 8, nullable: false),
                    interview_type_id = table.Column<int>(type: "int", nullable: false),
                    mode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    start_time = table.Column<DateTime>(type: "datetime2", nullable: false),
                    end_time = table.Column<DateTime>(type: "datetime2", nullable: false),
                    status = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Interviews", x => x.interview_id);
                    table.ForeignKey(
                        name: "FK_Interviews_Candidates",
                        column: x => x.candidate_id,
                        principalTable: "Candidates",
                        principalColumn: "candidate_id");
                    table.ForeignKey(
                        name: "FK_Interviews_Interview_Type",
                        column: x => x.interview_type_id,
                        principalTable: "Interview_Type",
                        principalColumn: "interview_type_id");
                    table.ForeignKey(
                        name: "FK_Interviews_Jobs",
                        column: x => x.job_id,
                        principalTable: "Jobs",
                        principalColumn: "job_id");
                    table.ForeignKey(
                        name: "FK_Interviews_Users",
                        column: x => x.scheduled_by,
                        principalTable: "Users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "Jobs_Skills",
                columns: table => new
                {
                    skill_id = table.Column<int>(type: "int", nullable: false),
                    job_id = table.Column<int>(type: "int", nullable: false),
                    skill_type = table.Column<string>(type: "char(1)", unicode: false, fixedLength: true, maxLength: 1, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jobs_Skills", x => new { x.skill_id, x.job_id });
                    table.ForeignKey(
                        name: "FK_Jobs_Skills_Jobs",
                        column: x => x.job_id,
                        principalTable: "Jobs",
                        principalColumn: "job_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Jobs_Skills_Skills",
                        column: x => x.skill_id,
                        principalTable: "Skills",
                        principalColumn: "skill_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HR_Review",
                columns: table => new
                {
                    review_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    communication_rating = table.Column<int>(type: "int", nullable: false),
                    teamwork_rating = table.Column<int>(type: "int", nullable: false),
                    adaptability_rating = table.Column<int>(type: "int", nullable: false),
                    leadership_rating = table.Column<int>(type: "int", nullable: false),
                    strengths = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    areas_for_improvement = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    training_recommendations = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    career_path_notes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    overall_rating = table.Column<int>(type: "int", nullable: false),
                    interview_id = table.Column<int>(type: "int", nullable: false),
                    user_id = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    interview_id1 = table.Column<int>(type: "int", nullable: false),
                    user_id1 = table.Column<string>(type: "char(8)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HR_Review", x => x.review_id);
                    table.ForeignKey(
                        name: "FK_HR_Review_Interviews_interview_id1",
                        column: x => x.interview_id1,
                        principalTable: "Interviews",
                        principalColumn: "interview_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HR_Review_Users_user_id1",
                        column: x => x.user_id1,
                        principalTable: "Users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Interview_Feedback",
                columns: table => new
                {
                    feedback_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    concept_rating = table.Column<int>(type: "int", nullable: false),
                    technical_rating = table.Column<int>(type: "int", nullable: false),
                    comments = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    feedback_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    interview_id = table.Column<int>(type: "int", nullable: false),
                    user_id = table.Column<string>(type: "char(8)", unicode: false, fixedLength: true, maxLength: 8, nullable: false),
                    candidate_skill_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Interview_Feedback", x => x.feedback_id);
                    table.ForeignKey(
                        name: "FK_Interview_Feedback_CandidateSkills",
                        column: x => x.candidate_skill_id,
                        principalTable: "Candidate_Skills",
                        principalColumn: "candidate_skill_id");
                    table.ForeignKey(
                        name: "FK_Interview_Feedback_Interviews",
                        column: x => x.interview_id,
                        principalTable: "Interviews",
                        principalColumn: "interview_id");
                    table.ForeignKey(
                        name: "FK_Interview_Feedback_Users",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "Interview_Panel",
                columns: table => new
                {
                    user_id = table.Column<string>(type: "char(8)", unicode: false, fixedLength: true, maxLength: 8, nullable: false),
                    interview_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Interview_Panel", x => new { x.user_id, x.interview_id });
                    table.ForeignKey(
                        name: "FK_Interview_Panel_Interviews",
                        column: x => x.interview_id,
                        principalTable: "Interviews",
                        principalColumn: "interview_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Interview_Panel_Users",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Candidate_Documents_candidate_id",
                table: "Candidate_Documents",
                column: "candidate_id");

            migrationBuilder.CreateIndex(
                name: "IX_Candidate_Reviews_candidate_id",
                table: "Candidate_Reviews",
                column: "candidate_id");

            migrationBuilder.CreateIndex(
                name: "IX_Candidate_Reviews_job_id",
                table: "Candidate_Reviews",
                column: "job_id");

            migrationBuilder.CreateIndex(
                name: "IX_Candidate_Reviews_user_id",
                table: "Candidate_Reviews",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Candidate_Skills_candidate_id",
                table: "Candidate_Skills",
                column: "candidate_id");

            migrationBuilder.CreateIndex(
                name: "IX_Candidate_Skills_skill_id",
                table: "Candidate_Skills",
                column: "skill_id");

            migrationBuilder.CreateIndex(
                name: "IX_Candidate_Status_History_candidate_id",
                table: "Candidate_Status_History",
                column: "candidate_id");

            migrationBuilder.CreateIndex(
                name: "IX_Candidate_Status_History_changed_by",
                table: "Candidate_Status_History",
                column: "changed_by");

            migrationBuilder.CreateIndex(
                name: "IX_Candidate_Status_History_job_id",
                table: "Candidate_Status_History",
                column: "job_id");

            migrationBuilder.CreateIndex(
                name: "UQ_Candidates_Email",
                table: "Candidates",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employee_Records_job_id",
                table: "Employee_Records",
                column: "job_id");

            migrationBuilder.CreateIndex(
                name: "UQ_Employee_Records_Candidate",
                table: "Employee_Records",
                column: "candidate_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HR_Review_interview_id1",
                table: "HR_Review",
                column: "interview_id1");

            migrationBuilder.CreateIndex(
                name: "IX_HR_Review_user_id1",
                table: "HR_Review",
                column: "user_id1");

            migrationBuilder.CreateIndex(
                name: "IX_Interview_Feedback_candidate_skill_id",
                table: "Interview_Feedback",
                column: "candidate_skill_id");

            migrationBuilder.CreateIndex(
                name: "IX_Interview_Feedback_interview_id",
                table: "Interview_Feedback",
                column: "interview_id");

            migrationBuilder.CreateIndex(
                name: "IX_Interview_Feedback_user_id",
                table: "Interview_Feedback",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Interview_Panel_interview_id",
                table: "Interview_Panel",
                column: "interview_id");

            migrationBuilder.CreateIndex(
                name: "IX_Interviews_candidate_id",
                table: "Interviews",
                column: "candidate_id");

            migrationBuilder.CreateIndex(
                name: "IX_Interviews_interview_type_id",
                table: "Interviews",
                column: "interview_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_Interviews_job_id",
                table: "Interviews",
                column: "job_id");

            migrationBuilder.CreateIndex(
                name: "IX_Interviews_scheduled_by",
                table: "Interviews",
                column: "scheduled_by");

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_created_by",
                table: "Jobs",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_status_id",
                table: "Jobs",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_Skills_job_id",
                table: "Jobs_Skills",
                column: "job_id");

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_Status_changed_by",
                table: "Jobs_Status",
                column: "changed_by");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_user_id",
                table: "Notifications",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "UQ_Skills_Name",
                table: "Skills",
                column: "skill_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_Users_Email",
                table: "Users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Roles_user_id",
                table: "Users_Roles",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Candidate_Documents");

            migrationBuilder.DropTable(
                name: "Candidate_Reviews");

            migrationBuilder.DropTable(
                name: "Candidate_Status_History");

            migrationBuilder.DropTable(
                name: "Employee_Records");

            migrationBuilder.DropTable(
                name: "HR_Review");

            migrationBuilder.DropTable(
                name: "Interview_Feedback");

            migrationBuilder.DropTable(
                name: "Interview_Panel");

            migrationBuilder.DropTable(
                name: "Jobs_Skills");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "Users_Roles");

            migrationBuilder.DropTable(
                name: "Candidate_Skills");

            migrationBuilder.DropTable(
                name: "Interviews");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Skills");

            migrationBuilder.DropTable(
                name: "Candidates");

            migrationBuilder.DropTable(
                name: "Interview_Type");

            migrationBuilder.DropTable(
                name: "Jobs");

            migrationBuilder.DropTable(
                name: "Jobs_Status");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
