using System;

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DarkPatterns.OneTimePassword.Migrations
{
	/// <inheritdoc />
	public partial class AppConfigurations : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<DateTimeOffset>(
				name: "ExpirationTime",
				table: "DeliveredPasswords",
				type: "timestamp with time zone",
				nullable: false,
				defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

			migrationBuilder.AddColumn<int>(
				name: "RemainingCount",
				table: "DeliveredPasswords",
				type: "integer",
				nullable: false,
				defaultValue: 0);

			migrationBuilder.CreateTable(
				name: "Applications",
				columns: table => new
				{
					ApplicationId = table.Column<Guid>(type: "uuid", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Applications", x => x.ApplicationId);
				});

			migrationBuilder.CreateTable(
				name: "Configurations",
				columns: table => new
				{
					ConfigurationId = table.Column<Guid>(type: "uuid", nullable: false),
					MaxAttemptCount = table.Column<int>(type: "integer", nullable: false),
					ExpirationWindow = table.Column<TimeSpan>(type: "interval", nullable: false),
					IsSliding = table.Column<bool>(type: "boolean", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Configurations", x => x.ConfigurationId);
				});

			migrationBuilder.CreateTable(
				name: "ConfiguredApplications",
				columns: table => new
				{
					ApplicationId = table.Column<Guid>(type: "uuid", nullable: false),
					ConfigurationId = table.Column<Guid>(type: "uuid", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_ConfiguredApplications", x => new { x.ConfigurationId, x.ApplicationId });
					table.ForeignKey(
						name: "FK_ConfiguredApplications_Applications_ApplicationId",
						column: x => x.ApplicationId,
						principalTable: "Applications",
						principalColumn: "ApplicationId",
						onDelete: ReferentialAction.Cascade);
					table.ForeignKey(
						name: "FK_ConfiguredApplications_Configurations_ConfigurationId",
						column: x => x.ConfigurationId,
						principalTable: "Configurations",
						principalColumn: "ConfigurationId",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateIndex(
				name: "IX_ConfiguredApplications_ApplicationId",
				table: "ConfiguredApplications",
				column: "ApplicationId");

			migrationBuilder.AddForeignKey(
				name: "FK_DeliveredPasswords_Applications_ApplicationId",
				table: "DeliveredPasswords",
				column: "ApplicationId",
				principalTable: "Applications",
				principalColumn: "ApplicationId",
				onDelete: ReferentialAction.Cascade);
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropForeignKey(
				name: "FK_DeliveredPasswords_Applications_ApplicationId",
				table: "DeliveredPasswords");

			migrationBuilder.DropTable(
				name: "ConfiguredApplications");

			migrationBuilder.DropTable(
				name: "Applications");

			migrationBuilder.DropTable(
				name: "Configurations");

			migrationBuilder.DropColumn(
				name: "ExpirationTime",
				table: "DeliveredPasswords");

			migrationBuilder.DropColumn(
				name: "RemainingCount",
				table: "DeliveredPasswords");
		}
	}
}
