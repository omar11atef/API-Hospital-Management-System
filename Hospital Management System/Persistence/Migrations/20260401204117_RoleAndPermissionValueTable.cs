using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Hospital_Management_System.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RoleAndPermissionValueTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RefershTokens");

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Token = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExpiresON = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreateON = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReokedON = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => new { x.UserId, x.Id });
                    table.ForeignKey(
                        name: "FK_RefreshTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "IsDefault", "IsDeleted", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "92b75286-d8f8-4061-9995-e6e23ccdee94", "f51e5a91-bced-49c2-8b86-c2e170c0846c", false, false, "Admin", "ADMIN" },
                    { "9eaa03df-8e4f-4161-85de-0f6e5e30bfd4", "5ee6bc12-5cb0-4304-91e7-6a00744e042a", true, false, "Member", "MEMBER" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "9D51D5CD223E41CD823738A34DE01376", 0, "F12F8501A4E743889E7D2BE1979E43A7", "AdminValerio@Hosptial-Valerio-System.com", true, "ValerioSystem", "ValerioSystem", false, null, "ADMINVALERIO@HOSPTIAL-VALERIO-SYSTEM.COM", "ADMINVALERIO@HOSPTIAL-VALERIO-SYSTEM.COM", "AQAAAAIAAYagAAAAEK1Ypsch0DKBxOEVci5VRaYQ4YhD+EFAwsPWjseCM0CgYdAMmqWU0eCn5Cp5jKkYKA==", null, false, "601AB38C202F40C281B183149DBACC08", false, "AdminValerio@Hosptial-Valerio-System.com" });

            migrationBuilder.InsertData(
                table: "AspNetRoleClaims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "RoleId" },
                values: new object[,]
                {
                    { 1, "permissions", "Appointments:read", "92b75286-d8f8-4061-9995-e6e23ccdee94" },
                    { 2, "permissions", "Appointments:read-deleted", "92b75286-d8f8-4061-9995-e6e23ccdee94" },
                    { 3, "permissions", "Appointments:read-single", "92b75286-d8f8-4061-9995-e6e23ccdee94" },
                    { 4, "permissions", "Appointments:read-history", "92b75286-d8f8-4061-9995-e6e23ccdee94" },
                    { 5, "permissions", "Appointments:create", "92b75286-d8f8-4061-9995-e6e23ccdee94" },
                    { 6, "permissions", "Appointments:update", "92b75286-d8f8-4061-9995-e6e23ccdee94" },
                    { 7, "permissions", "Appointments:remove", "92b75286-d8f8-4061-9995-e6e23ccdee94" },
                    { 8, "permissions", "Appointments:toggle", "92b75286-d8f8-4061-9995-e6e23ccdee94" },
                    { 9, "permissions", "Appointments:cancel", "92b75286-d8f8-4061-9995-e6e23ccdee94" },
                    { 10, "permissions", "Author:login", "92b75286-d8f8-4061-9995-e6e23ccdee94" },
                    { 11, "permissions", "Author:register", "92b75286-d8f8-4061-9995-e6e23ccdee94" },
                    { 12, "permissions", "Author:confirm-email", "92b75286-d8f8-4061-9995-e6e23ccdee94" },
                    { 13, "permissions", "Author:resend-confirm-email", "92b75286-d8f8-4061-9995-e6e23ccdee94" },
                    { 14, "permissions", "Author:logout", "92b75286-d8f8-4061-9995-e6e23ccdee94" },
                    { 15, "permissions", "Department:read", "92b75286-d8f8-4061-9995-e6e23ccdee94" },
                    { 16, "permissions", "Department:read-single", "92b75286-d8f8-4061-9995-e6e23ccdee94" },
                    { 17, "permissions", "Department:create", "92b75286-d8f8-4061-9995-e6e23ccdee94" },
                    { 18, "permissions", "Department:update", "92b75286-d8f8-4061-9995-e6e23ccdee94" },
                    { 19, "permissions", "Doctor:read", "92b75286-d8f8-4061-9995-e6e23ccdee94" },
                    { 20, "permissions", "Doctor:read-exists", "92b75286-d8f8-4061-9995-e6e23ccdee94" },
                    { 21, "permissions", "Doctor:read-single", "92b75286-d8f8-4061-9995-e6e23ccdee94" },
                    { 22, "permissions", "Doctor:read-schedule", "92b75286-d8f8-4061-9995-e6e23ccdee94" },
                    { 23, "permissions", "Doctor:read", "92b75286-d8f8-4061-9995-e6e23ccdee94" },
                    { 24, "permissions", "Doctor:update", "92b75286-d8f8-4061-9995-e6e23ccdee94" },
                    { 25, "permissions", "Doctor:Remove", "92b75286-d8f8-4061-9995-e6e23ccdee94" },
                    { 26, "permissions", "Doctor:toggle", "92b75286-d8f8-4061-9995-e6e23ccdee94" },
                    { 27, "permissions", "Patient:read", "92b75286-d8f8-4061-9995-e6e23ccdee94" },
                    { 28, "permissions", "Patient:read-deleted", "92b75286-d8f8-4061-9995-e6e23ccdee94" },
                    { 29, "permissions", "Patient:read-not-deleted", "92b75286-d8f8-4061-9995-e6e23ccdee94" },
                    { 30, "permissions", "Patient:read-single", "92b75286-d8f8-4061-9995-e6e23ccdee94" },
                    { 31, "permissions", "Patient:read-appointments", "92b75286-d8f8-4061-9995-e6e23ccdee94" },
                    { 32, "permissions", "Patient:download-report", "92b75286-d8f8-4061-9995-e6e23ccdee94" },
                    { 33, "permissions", "Patient:create", "92b75286-d8f8-4061-9995-e6e23ccdee94" },
                    { 34, "permissions", "Patient:update", "92b75286-d8f8-4061-9995-e6e23ccdee94" },
                    { 35, "permissions", "Patient:remove", "92b75286-d8f8-4061-9995-e6e23ccdee94" },
                    { 36, "permissions", "Patient:toggle", "92b75286-d8f8-4061-9995-e6e23ccdee94" },
                    { 37, "permissions", "Patient:update-expenses", "92b75286-d8f8-4061-9995-e6e23ccdee94" },
                    { 38, "permissions", "Room:read", "92b75286-d8f8-4061-9995-e6e23ccdee94" },
                    { 39, "permissions", "Room:read-single", "92b75286-d8f8-4061-9995-e6e23ccdee94" },
                    { 40, "permissions", "Room:read-appointments", "92b75286-d8f8-4061-9995-e6e23ccdee94" },
                    { 41, "permissions", "Room:read-doctor", "92b75286-d8f8-4061-9995-e6e23ccdee94" },
                    { 42, "permissions", "Room:read-patient", "92b75286-d8f8-4061-9995-e6e23ccdee94" },
                    { 43, "permissions", "Room:create", "92b75286-d8f8-4061-9995-e6e23ccdee94" },
                    { 44, "permissions", "Room:update", "92b75286-d8f8-4061-9995-e6e23ccdee94" },
                    { 45, "permissions", "Room:remove", "92b75286-d8f8-4061-9995-e6e23ccdee94" },
                    { 46, "permissions", "Room:assign", "92b75286-d8f8-4061-9995-e6e23ccdee94" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "92b75286-d8f8-4061-9995-e6e23ccdee94", "9D51D5CD223E41CD823738A34DE01376" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 39);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 40);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 41);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 42);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 43);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 44);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 45);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 46);

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "9eaa03df-8e4f-4161-85de-0f6e5e30bfd4");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "92b75286-d8f8-4061-9995-e6e23ccdee94", "9D51D5CD223E41CD823738A34DE01376" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "92b75286-d8f8-4061-9995-e6e23ccdee94");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "9D51D5CD223E41CD823738A34DE01376");

            migrationBuilder.CreateTable(
                name: "RefershTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreateON = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresON = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReokedON = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Token = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefershTokens", x => new { x.UserId, x.Id });
                    table.ForeignKey(
                        name: "FK_RefershTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }
    }
}
