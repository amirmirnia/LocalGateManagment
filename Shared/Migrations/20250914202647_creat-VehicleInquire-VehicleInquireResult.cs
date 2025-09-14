using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServicesGateManagment.Shared.Migrations
{
    /// <inheritdoc />
    public partial class creatVehicleInquireVehicleInquireResult : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GateArea",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GateArea", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VehicleInquireResult",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HasValidAccess = table.Column<bool>(type: "bit", nullable: false),
                    GateValidation = table.Column<bool>(type: "bit", nullable: false),
                    UnAuthorizedEntry = table.Column<bool>(type: "bit", nullable: false),
                    OverStayed = table.Column<bool>(type: "bit", nullable: false),
                    NotConfirmedVisitor = table.Column<bool>(type: "bit", nullable: false),
                    InBlackList = table.Column<bool>(type: "bit", nullable: false),
                    ArmAction = table.Column<int>(type: "int", nullable: false),
                    InquireId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleInquireResult", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Gate",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NeedValidAccess = table.Column<bool>(type: "bit", nullable: false),
                    CheckUnauthorizedEntry = table.Column<bool>(type: "bit", nullable: false),
                    Guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ArmActionOnSuccess = table.Column<int>(type: "int", nullable: false),
                    ArmActionOnFailed = table.Column<int>(type: "int", nullable: false),
                    InquireType = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    ObjectType = table.Column<int>(type: "int", nullable: false),
                    GateAreaId = table.Column<int>(type: "int", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Gate_GateArea_GateAreaId",
                        column: x => x.GateAreaId,
                        principalTable: "GateArea",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "VehicleInquire",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Plate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Class = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GateId = table.Column<int>(type: "int", nullable: false),
                    VehicleId = table.Column<int>(type: "int", nullable: true),
                    PropertyVehicleId = table.Column<int>(type: "int", nullable: true),
                    ResultId = table.Column<int>(type: "int", nullable: false),
                    GateValidation = table.Column<bool>(type: "bit", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    ReferenceId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PreviousInquireId = table.Column<int>(type: "int", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleInquire", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VehicleInquire_Gate_GateId",
                        column: x => x.GateId,
                        principalTable: "Gate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VehicleInquire_VehicleInquireResult_ResultId",
                        column: x => x.ResultId,
                        principalTable: "VehicleInquireResult",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VehicleInquireSnapshot",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlateImage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CameraSnapshot = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReferenceId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InquireId = table.Column<int>(type: "int", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleInquireSnapshot", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VehicleInquireSnapshot_VehicleInquire_InquireId",
                        column: x => x.InquireId,
                        principalTable: "VehicleInquire",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Gate_GateAreaId",
                table: "Gate",
                column: "GateAreaId");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleInquire_GateId",
                table: "VehicleInquire",
                column: "GateId");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleInquire_ResultId",
                table: "VehicleInquire",
                column: "ResultId");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleInquireSnapshot_InquireId",
                table: "VehicleInquireSnapshot",
                column: "InquireId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VehicleInquireSnapshot");

            migrationBuilder.DropTable(
                name: "VehicleInquire");

            migrationBuilder.DropTable(
                name: "Gate");

            migrationBuilder.DropTable(
                name: "VehicleInquireResult");

            migrationBuilder.DropTable(
                name: "GateArea");
        }
    }
}
