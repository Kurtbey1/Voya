using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Voya.Migrations
{
    /// <inheritdoc />
    public partial class InitialManualSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Hotels",
                columns: table => new
                {
                    Hotel_ID = table.Column<long>(type: "bigint", nullable: false),
                    Hotel_Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    StarRating = table.Column<int>(type: "int", nullable: false),
                    BasePricePerNight = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    MainImageUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hotels", x => x.Hotel_ID);
                    table.CheckConstraint("CHK_Hotel_StarRating", "[StarRating] >= 1 AND [StarRating] <= 5");
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    User_ID = table.Column<long>(type: "bigint", nullable: false),
                    User_Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Nationality = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Password_Hash = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Role = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Customer"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.User_ID);
                    table.CheckConstraint("CK_User_Email", "Email LIKE '%_@__%.__%'");
                });

            migrationBuilder.CreateTable(
                name: "Bookings",
                columns: table => new
                {
                    Booking_ID = table.Column<long>(type: "bigint", nullable: false),
                    Booking_Date = table.Column<DateTime>(type: "DATETIME2", nullable: false),
                    Booking_State = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Adults_Number = table.Column<int>(type: "int", nullable: false),
                    Children_Number = table.Column<int>(type: "int", nullable: false),
                    User_ID = table.Column<long>(type: "bigint", nullable: false),
                    Hotel_ID = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookings", x => x.Booking_ID);
                    table.CheckConstraint("CHK_Bookings_Adults_Positive", "[Adults_Number] >= 1");
                    table.CheckConstraint("CHK_Bookings_Children_NonNegative", "[Children_Number] >= 0");
                    table.ForeignKey(
                        name: "FK_Bookings_Hotels_Hotel_ID",
                        column: x => x.Hotel_ID,
                        principalTable: "Hotels",
                        principalColumn: "Hotel_ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Bookings_Users_User_ID",
                        column: x => x.User_ID,
                        principalTable: "Users",
                        principalColumn: "User_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Transaction_ID = table.Column<long>(type: "bigint", nullable: false),
                    Transaction_Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Transaction_Date = table.Column<DateTime>(type: "DATETIME2", nullable: false),
                    Provider_Ref = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Transaction_Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Booking_ID = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Transaction_ID);
                    table.ForeignKey(
                        name: "FK_Transactions_Bookings_Booking_ID",
                        column: x => x.Booking_ID,
                        principalTable: "Bookings",
                        principalColumn: "Booking_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Payment_ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Transaction_ID = table.Column<long>(type: "bigint", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Payment_Method = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: ""),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Pending"),
                    Stripe_PaymentIntent_Id = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Created_At = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Payment_ID);
                    table.CheckConstraint("CHK_Payments_Amount_Positive", "[Amount] > 0");
                    table.ForeignKey(
                        name: "FK_Payments_Transactions_Transaction_ID",
                        column: x => x.Transaction_ID,
                        principalTable: "Transactions",
                        principalColumn: "Transaction_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_Hotel_ID",
                table: "Bookings",
                column: "Hotel_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_User_ID",
                table: "Bookings",
                column: "User_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_Transaction_ID",
                table: "Payments",
                column: "Transaction_ID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_Booking_ID",
                table: "Transactions",
                column: "Booking_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "Bookings");

            migrationBuilder.DropTable(
                name: "Hotels");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
