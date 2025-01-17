﻿using System;

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DarkPatterns.OneTimePassword.Migrations
{
	/// <inheritdoc />
	public partial class Initial : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable(
				name: "DeliveredPasswords",
				columns: table => new
				{
					ApplicationId = table.Column<Guid>(type: "uuid", nullable: false),
					MediumCode = table.Column<string>(type: "text", nullable: false),
					DeliveryTarget = table.Column<string>(type: "text", nullable: false),
					PasswordHash = table.Column<byte[]>(type: "bytea", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_DeliveredPasswords", x => new { x.ApplicationId, x.MediumCode, x.DeliveryTarget });
				});
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				name: "DeliveredPasswords");
		}
	}
}
