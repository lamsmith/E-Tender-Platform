using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RFQService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RFQService : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RFQs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ContractTitle = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CompanyName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ScopeOfSupply = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: false),
                    PaymentTerms = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: false),
                    DeliveryTerms = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: false),
                    OtherInformation = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: true),
                    Deadline = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    Visibility = table.Column<string>(type: "text", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RFQs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RFQRecipients",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RFQId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RFQRecipients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RFQRecipients_RFQs_RFQId",
                        column: x => x.RFQId,
                        principalTable: "RFQs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RFQRecipients_Email",
                table: "RFQRecipients",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_RFQRecipients_RFQId",
                table: "RFQRecipients",
                column: "RFQId");

            migrationBuilder.CreateIndex(
                name: "IX_RFQRecipients_UserId",
                table: "RFQRecipients",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RFQRecipients");

            migrationBuilder.DropTable(
                name: "RFQs");
        }
    }
}
