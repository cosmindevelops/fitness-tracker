using Microsoft.EntityFrameworkCore.Migrations;

namespace GymTracker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedWorkoutTemplates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WorkoutTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DurationWeeks = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkoutTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TemplateWeeks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WorkoutTemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WeekNumber = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TemplateWeeks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TemplateWeeks_WorkoutTemplates_WorkoutTemplateId",
                        column: x => x.WorkoutTemplateId,
                        principalTable: "WorkoutTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserWorkoutTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WorkoutTemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserWorkoutTemplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserWorkoutTemplates_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserWorkoutTemplates_WorkoutTemplates_WorkoutTemplateId",
                        column: x => x.WorkoutTemplateId,
                        principalTable: "WorkoutTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TemplateWorkouts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TemplateWeekId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TemplateWorkouts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TemplateWorkouts_TemplateWeeks_TemplateWeekId",
                        column: x => x.TemplateWeekId,
                        principalTable: "TemplateWeeks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TemplateExercises",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TemplateWorkoutId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExerciseName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastSetIntensity = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    WarmupSets = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    WorkingSets = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Reps = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Rpe = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Rest = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Substitution1 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Substitution2 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TemplateExercises", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TemplateExercises_TemplateWorkouts_TemplateWorkoutId",
                        column: x => x.TemplateWorkoutId,
                        principalTable: "TemplateWorkouts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserExerciseProgresses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserWorkoutTemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TemplateExerciseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Set1Reps = table.Column<int>(type: "int", nullable: true),
                    Set2Reps = table.Column<int>(type: "int", nullable: true),
                    Set3Reps = table.Column<int>(type: "int", nullable: true),
                    Set4Reps = table.Column<int>(type: "int", nullable: true),
                    WorkoutCompleted = table.Column<bool>(type: "bit", nullable: false),
                    CompletionDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserExerciseProgresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserExerciseProgresses_TemplateExercises_TemplateExerciseId",
                        column: x => x.TemplateExerciseId,
                        principalTable: "TemplateExercises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserExerciseProgresses_UserWorkoutTemplates_UserWorkoutTemplateId",
                        column: x => x.UserWorkoutTemplateId,
                        principalTable: "UserWorkoutTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TemplateExercises_TemplateWorkoutId",
                table: "TemplateExercises",
                column: "TemplateWorkoutId");

            migrationBuilder.CreateIndex(
                name: "IX_TemplateWeeks_WorkoutTemplateId",
                table: "TemplateWeeks",
                column: "WorkoutTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_TemplateWorkouts_TemplateWeekId",
                table: "TemplateWorkouts",
                column: "TemplateWeekId");

            migrationBuilder.CreateIndex(
                name: "IX_UserExerciseProgresses_TemplateExerciseId",
                table: "UserExerciseProgresses",
                column: "TemplateExerciseId");

            migrationBuilder.CreateIndex(
                name: "IX_UserExerciseProgresses_UserWorkoutTemplateId",
                table: "UserExerciseProgresses",
                column: "UserWorkoutTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_UserWorkoutTemplates_UserId",
                table: "UserWorkoutTemplates",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserWorkoutTemplates_WorkoutTemplateId",
                table: "UserWorkoutTemplates",
                column: "WorkoutTemplateId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserExerciseProgresses");

            migrationBuilder.DropTable(
                name: "TemplateExercises");

            migrationBuilder.DropTable(
                name: "UserWorkoutTemplates");

            migrationBuilder.DropTable(
                name: "TemplateWorkouts");

            migrationBuilder.DropTable(
                name: "TemplateWeeks");

            migrationBuilder.DropTable(
                name: "WorkoutTemplates");
        }
    }
}