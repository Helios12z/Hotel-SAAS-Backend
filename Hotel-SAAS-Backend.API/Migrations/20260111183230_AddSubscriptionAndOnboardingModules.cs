using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hotel_SAAS_Backend.API.Migrations
{
    /// <inheritdoc />
    public partial class AddSubscriptionAndOnboardingModules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "subscription_plans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    PlanType = table.Column<int>(type: "integer", nullable: false),
                    MonthlyPrice = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    QuarterlyPrice = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    YearlyPrice = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    MaxHotels = table.Column<int>(type: "integer", nullable: false),
                    MaxRoomsPerHotel = table.Column<int>(type: "integer", nullable: false),
                    MaxUsersPerHotel = table.Column<int>(type: "integer", nullable: false),
                    CommissionRate = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    HasAnalytics = table.Column<bool>(type: "boolean", nullable: false),
                    HasAdvancedReporting = table.Column<bool>(type: "boolean", nullable: false),
                    HasApiAccess = table.Column<bool>(type: "boolean", nullable: false),
                    HasPrioritySupport = table.Column<bool>(type: "boolean", nullable: false),
                    HasChannelManager = table.Column<bool>(type: "boolean", nullable: false),
                    HasRevenueManagement = table.Column<bool>(type: "boolean", nullable: false),
                    HasMultiLanguage = table.Column<bool>(type: "boolean", nullable: false),
                    HasCustomBranding = table.Column<bool>(type: "boolean", nullable: false),
                    HasDedicatedAccountManager = table.Column<bool>(type: "boolean", nullable: false),
                    TrialDays = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsPopular = table.Column<bool>(type: "boolean", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_subscription_plans", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "hotel_onboardings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ApplicantId = table.Column<Guid>(type: "uuid", nullable: false),
                    ExistingBrandId = table.Column<Guid>(type: "uuid", nullable: true),
                    BrandName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    BrandDescription = table.Column<string>(type: "text", nullable: true),
                    BrandLogoUrl = table.Column<string>(type: "text", nullable: true),
                    BrandWebsite = table.Column<string>(type: "text", nullable: true),
                    HotelName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    HotelDescription = table.Column<string>(type: "text", nullable: true),
                    Address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    City = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    State = table.Column<string>(type: "text", nullable: true),
                    Country = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PostalCode = table.Column<string>(type: "text", nullable: true),
                    Latitude = table.Column<double>(type: "double precision", nullable: true),
                    Longitude = table.Column<double>(type: "double precision", nullable: true),
                    StarRating = table.Column<int>(type: "integer", nullable: false),
                    TotalRooms = table.Column<int>(type: "integer", nullable: false),
                    NumberOfFloors = table.Column<int>(type: "integer", nullable: true),
                    ContactName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ContactEmail = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ContactPhone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ContactPosition = table.Column<string>(type: "text", nullable: true),
                    LegalBusinessName = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    TaxId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    BusinessRegistrationNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    BankName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    BankAccountName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    BankAccountNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    BankRoutingNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    BankSwiftCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    SelectedPlanId = table.Column<Guid>(type: "uuid", nullable: true),
                    SelectedBillingCycle = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ReviewedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ReviewedById = table.Column<Guid>(type: "uuid", nullable: true),
                    ReviewNotes = table.Column<string>(type: "text", nullable: true),
                    RejectionReason = table.Column<string>(type: "text", nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ApprovedById = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedBrandId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedHotelId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedSubscriptionId = table.Column<Guid>(type: "uuid", nullable: true),
                    AcceptedTerms = table.Column<bool>(type: "boolean", nullable: false),
                    AcceptedTermsAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IpAddress = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_hotel_onboardings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_hotel_onboardings_brands_ExistingBrandId",
                        column: x => x.ExistingBrandId,
                        principalTable: "brands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_hotel_onboardings_subscription_plans_SelectedPlanId",
                        column: x => x.SelectedPlanId,
                        principalTable: "subscription_plans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_hotel_onboardings_users_ApplicantId",
                        column: x => x.ApplicantId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_hotel_onboardings_users_ApprovedById",
                        column: x => x.ApprovedById,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_hotel_onboardings_users_ReviewedById",
                        column: x => x.ReviewedById,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "subscriptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BrandId = table.Column<Guid>(type: "uuid", nullable: false),
                    PlanId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    BillingCycle = table.Column<int>(type: "integer", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TrialEndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CancelledAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CancellationReason = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Price = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    DiscountPercentage = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    AutoRenew = table.Column<bool>(type: "boolean", nullable: false),
                    NextBillingDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_subscriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_subscriptions_brands_BrandId",
                        column: x => x.BrandId,
                        principalTable: "brands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_subscriptions_subscription_plans_PlanId",
                        column: x => x.PlanId,
                        principalTable: "subscription_plans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "onboarding_documents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OnboardingId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    FileName = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    FileUrl = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    FileType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    FileSize = table.Column<long>(type: "bigint", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ReviewedById = table.Column<Guid>(type: "uuid", nullable: true),
                    ReviewedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ReviewNotes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    RejectionReason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ExpiryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_onboarding_documents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_onboarding_documents_hotel_onboardings_OnboardingId",
                        column: x => x.OnboardingId,
                        principalTable: "hotel_onboardings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_onboarding_documents_users_ReviewedById",
                        column: x => x.ReviewedById,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "subscription_invoices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SubscriptionId = table.Column<Guid>(type: "uuid", nullable: false),
                    InvoiceNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PeriodStart = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PeriodEnd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Subtotal = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    TaxAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    PaidAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PaymentMethod = table.Column<int>(type: "integer", nullable: true),
                    TransactionId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    DueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    InvoicePdfUrl = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_subscription_invoices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_subscription_invoices_subscriptions_SubscriptionId",
                        column: x => x.SubscriptionId,
                        principalTable: "subscriptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_hotel_onboardings_ApplicantId",
                table: "hotel_onboardings",
                column: "ApplicantId");

            migrationBuilder.CreateIndex(
                name: "IX_hotel_onboardings_ApprovedById",
                table: "hotel_onboardings",
                column: "ApprovedById");

            migrationBuilder.CreateIndex(
                name: "IX_hotel_onboardings_City",
                table: "hotel_onboardings",
                column: "City");

            migrationBuilder.CreateIndex(
                name: "IX_hotel_onboardings_Country",
                table: "hotel_onboardings",
                column: "Country");

            migrationBuilder.CreateIndex(
                name: "IX_hotel_onboardings_ExistingBrandId",
                table: "hotel_onboardings",
                column: "ExistingBrandId");

            migrationBuilder.CreateIndex(
                name: "IX_hotel_onboardings_ReviewedById",
                table: "hotel_onboardings",
                column: "ReviewedById");

            migrationBuilder.CreateIndex(
                name: "IX_hotel_onboardings_SelectedPlanId",
                table: "hotel_onboardings",
                column: "SelectedPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_hotel_onboardings_Status",
                table: "hotel_onboardings",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_hotel_onboardings_SubmittedAt",
                table: "hotel_onboardings",
                column: "SubmittedAt");

            migrationBuilder.CreateIndex(
                name: "IX_onboarding_documents_OnboardingId",
                table: "onboarding_documents",
                column: "OnboardingId");

            migrationBuilder.CreateIndex(
                name: "IX_onboarding_documents_ReviewedById",
                table: "onboarding_documents",
                column: "ReviewedById");

            migrationBuilder.CreateIndex(
                name: "IX_onboarding_documents_Status",
                table: "onboarding_documents",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_onboarding_documents_Type",
                table: "onboarding_documents",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_subscription_invoices_DueDate",
                table: "subscription_invoices",
                column: "DueDate");

            migrationBuilder.CreateIndex(
                name: "IX_subscription_invoices_InvoiceNumber",
                table: "subscription_invoices",
                column: "InvoiceNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_subscription_invoices_Status",
                table: "subscription_invoices",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_subscription_invoices_SubscriptionId",
                table: "subscription_invoices",
                column: "SubscriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_subscription_plans_IsActive",
                table: "subscription_plans",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_subscription_plans_PlanType",
                table: "subscription_plans",
                column: "PlanType");

            migrationBuilder.CreateIndex(
                name: "IX_subscriptions_BrandId",
                table: "subscriptions",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_subscriptions_EndDate",
                table: "subscriptions",
                column: "EndDate");

            migrationBuilder.CreateIndex(
                name: "IX_subscriptions_PlanId",
                table: "subscriptions",
                column: "PlanId");

            migrationBuilder.CreateIndex(
                name: "IX_subscriptions_Status",
                table: "subscriptions",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "onboarding_documents");

            migrationBuilder.DropTable(
                name: "subscription_invoices");

            migrationBuilder.DropTable(
                name: "hotel_onboardings");

            migrationBuilder.DropTable(
                name: "subscriptions");

            migrationBuilder.DropTable(
                name: "subscription_plans");
        }
    }
}
