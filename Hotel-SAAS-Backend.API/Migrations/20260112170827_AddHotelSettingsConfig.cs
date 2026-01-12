using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hotel_SAAS_Backend.API.Migrations
{
    /// <inheritdoc />
    public partial class AddHotelSettingsConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "CommissionRate",
                table: "hotels",
                type: "numeric(5,2)",
                precision: 5,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CheckOutTime",
                table: "hotels",
                type: "character varying(5)",
                maxLength: 5,
                nullable: false,
                defaultValue: "12:00",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CheckInTime",
                table: "hotels",
                type: "character varying(5)",
                maxLength: 5,
                nullable: false,
                defaultValue: "14:00",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "AllowExtraBed",
                table: "hotels",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "EnablePayAtHotel",
                table: "hotels",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "EnableStripePayment",
                table: "hotels",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "ExtraBedPrice",
                table: "hotels",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaxAdultsPerRoom",
                table: "hotels",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaxAdvanceBookingDays",
                table: "hotels",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaxChildrenPerRoom",
                table: "hotels",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaxGuestsPerRoom",
                table: "hotels",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaxNights",
                table: "hotels",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MinAdvanceBookingHours",
                table: "hotels",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MinNights",
                table: "hotels",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "ServiceFeeRate",
                table: "hotels",
                type: "numeric(5,4)",
                precision: 5,
                scale: 4,
                nullable: false,
                defaultValue: 0.05m);

            migrationBuilder.AddColumn<string>(
                name: "StripeAccountId",
                table: "hotels",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxRate",
                table: "hotels",
                type: "numeric(5,4)",
                precision: 5,
                scale: 4,
                nullable: false,
                defaultValue: 0.10m);

            migrationBuilder.CreateTable(
                name: "additional_charges",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BookingId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    IsPaid = table.Column<bool>(type: "boolean", nullable: false),
                    PaidAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PaymentMethod = table.Column<string>(type: "text", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_additional_charges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_additional_charges_bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "bookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "permissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Resource = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Action = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_permissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "role_permissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    PermissionId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_role_permissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_role_permissions_permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_hotel_permissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    HotelId = table.Column<Guid>(type: "uuid", nullable: false),
                    PermissionId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsGranted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_hotel_permissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_user_hotel_permissions_permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_hotel_permissions_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_additional_charges_BookingId",
                table: "additional_charges",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_permissions_Name",
                table: "permissions",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_permissions_Resource",
                table: "permissions",
                column: "Resource");

            migrationBuilder.CreateIndex(
                name: "IX_role_permissions_PermissionId",
                table: "role_permissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_role_permissions_Role_PermissionId",
                table: "role_permissions",
                columns: new[] { "Role", "PermissionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_hotel_permissions_PermissionId",
                table: "user_hotel_permissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_user_hotel_permissions_UserId_HotelId_PermissionId",
                table: "user_hotel_permissions",
                columns: new[] { "UserId", "HotelId", "PermissionId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "additional_charges");

            migrationBuilder.DropTable(
                name: "role_permissions");

            migrationBuilder.DropTable(
                name: "user_hotel_permissions");

            migrationBuilder.DropTable(
                name: "permissions");

            migrationBuilder.DropColumn(
                name: "AllowExtraBed",
                table: "hotels");

            migrationBuilder.DropColumn(
                name: "EnablePayAtHotel",
                table: "hotels");

            migrationBuilder.DropColumn(
                name: "EnableStripePayment",
                table: "hotels");

            migrationBuilder.DropColumn(
                name: "ExtraBedPrice",
                table: "hotels");

            migrationBuilder.DropColumn(
                name: "MaxAdultsPerRoom",
                table: "hotels");

            migrationBuilder.DropColumn(
                name: "MaxAdvanceBookingDays",
                table: "hotels");

            migrationBuilder.DropColumn(
                name: "MaxChildrenPerRoom",
                table: "hotels");

            migrationBuilder.DropColumn(
                name: "MaxGuestsPerRoom",
                table: "hotels");

            migrationBuilder.DropColumn(
                name: "MaxNights",
                table: "hotels");

            migrationBuilder.DropColumn(
                name: "MinAdvanceBookingHours",
                table: "hotels");

            migrationBuilder.DropColumn(
                name: "MinNights",
                table: "hotels");

            migrationBuilder.DropColumn(
                name: "ServiceFeeRate",
                table: "hotels");

            migrationBuilder.DropColumn(
                name: "StripeAccountId",
                table: "hotels");

            migrationBuilder.DropColumn(
                name: "TaxRate",
                table: "hotels");

            migrationBuilder.AlterColumn<decimal>(
                name: "CommissionRate",
                table: "hotels",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(5,2)",
                oldPrecision: 5,
                oldScale: 2,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CheckOutTime",
                table: "hotels",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(5)",
                oldMaxLength: 5,
                oldDefaultValue: "12:00");

            migrationBuilder.AlterColumn<string>(
                name: "CheckInTime",
                table: "hotels",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(5)",
                oldMaxLength: 5,
                oldDefaultValue: "14:00");
        }
    }
}
