using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NuxibaEvaluation.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ccloglogin",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    User_id = table.Column<int>(type: "int", nullable: false),
                    Extension = table.Column<int>(type: "int", nullable: false),
                    TipoMov = table.Column<int>(type: "int", nullable: false),
                    fecha = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ccloglogin", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ccRIACat_Areas",
                columns: table => new
                {
                    IDArea = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AreaName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StatusArea = table.Column<int>(type: "int", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ccRIACat_Areas", x => x.IDArea);
                });

            migrationBuilder.CreateTable(
                name: "ccUsers",
                columns: table => new
                {
                    User_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Login = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nombres = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApellidoPaterno = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApellidoMaterno = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TipoUser_id = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: true),
                    fCreate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IDArea = table.Column<int>(type: "int", nullable: true),
                    LastLoginAttempt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ccUsers", x => x.User_id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ccloglogin_User_id_fecha",
                table: "ccloglogin",
                columns: new[] { "User_id", "fecha" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ccloglogin");

            migrationBuilder.DropTable(
                name: "ccRIACat_Areas");

            migrationBuilder.DropTable(
                name: "ccUsers");
        }
    }
}
