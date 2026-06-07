using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SamletInfo.Migrations
{
    /// <inheritdoc />
    public partial class AddTaskTemplateTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TaskTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskTemplates", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceTasks_RoomId",
                table: "ServiceTasks",
                column: "RoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceTasks_Rooms_RoomId",
                table: "ServiceTasks",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceTasks_Rooms_RoomId",
                table: "ServiceTasks");

            migrationBuilder.DropTable(
                name: "TaskTemplates");

            migrationBuilder.DropIndex(
                name: "IX_ServiceTasks_RoomId",
                table: "ServiceTasks");
        }
    }
}
