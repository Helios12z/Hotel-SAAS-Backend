using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hotel_SAAS_Backend.API.Migrations
{
    /// <inheritdoc />
    public partial class PendingModelChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasAdvancedReporting",
                table: "subscription_plans");

            migrationBuilder.DropColumn(
                name: "HasAnalytics",
                table: "subscription_plans");

            migrationBuilder.DropColumn(
                name: "HasApiAccess",
                table: "subscription_plans");

            migrationBuilder.DropColumn(
                name: "HasChannelManager",
                table: "subscription_plans");

            migrationBuilder.DropColumn(
                name: "HasCustomBranding",
                table: "subscription_plans");

            migrationBuilder.DropColumn(
                name: "HasDedicatedAccountManager",
                table: "subscription_plans");

            migrationBuilder.DropColumn(
                name: "HasMultiLanguage",
                table: "subscription_plans");

            migrationBuilder.DropColumn(
                name: "HasPrioritySupport",
                table: "subscription_plans");

            migrationBuilder.DropColumn(
                name: "HasRevenueManagement",
                table: "subscription_plans");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasAdvancedReporting",
                table: "subscription_plans",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasAnalytics",
                table: "subscription_plans",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasApiAccess",
                table: "subscription_plans",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasChannelManager",
                table: "subscription_plans",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasCustomBranding",
                table: "subscription_plans",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasDedicatedAccountManager",
                table: "subscription_plans",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasMultiLanguage",
                table: "subscription_plans",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasPrioritySupport",
                table: "subscription_plans",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasRevenueManagement",
                table: "subscription_plans",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
