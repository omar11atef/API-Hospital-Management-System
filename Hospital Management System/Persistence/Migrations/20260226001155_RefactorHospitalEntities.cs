using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hospital_Management_System.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RefactorHospitalEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. حذف العلاقات والمؤشرات القديمة
            migrationBuilder.DropForeignKey(
                name: "FK_Patients_Doctors_DoctorId",
                table: "Patients");

            migrationBuilder.DropForeignKey(
                name: "FK_PatientVisits_Patients_PatientId",
                table: "PatientVisits");

            migrationBuilder.DropIndex(
                name: "IX_Patients_DoctorId",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "DoctorId",
                table: "Patients");

            // 2. إنشاء جدول الأقسام أولاً (قبل إضافة الأعمدة التي تعتمد عليه)
            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Location = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.Id);
                });

            // 3. 🌟 إدخال القسم الافتراضي (General Department) لتجنب خطأ الـ Foreign Key
            migrationBuilder.InsertData(
                table: "Departments",
                columns: new[] { "Id", "Name", "Location", "PhoneNumber", "CreatedBy", "CreatedAt", "ModifiedBy", "ModifiedAt" },
                values: new object[] { 1, "General Department", "Main Building", "0000000000", "System", DateTime.UtcNow, "System", DateTime.UtcNow }
            );

            // 4. إضافة أعمدة القسم مع القيمة الافتراضية 1 (وليس 0)
            migrationBuilder.AddColumn<int>(
                name: "DepartmentId",
                table: "Patients",
                type: "int",
                nullable: false,
                defaultValue: 1); // 🌟 تم التعديل إلى 1

            migrationBuilder.AddColumn<int>(
                name: "DepartmentId",
                table: "Doctors",
                type: "int",
                nullable: false,
                defaultValue: 1); // 🌟 تم التعديل إلى 1

            // 5. إنشاء جدول الربط (Many-to-Many) بأسماء مفردة صحيحة
            migrationBuilder.CreateTable(
                name: "DoctorPatient",
                columns: table => new
                {
                    DoctorId = table.Column<int>(type: "int", nullable: false),
                    PatientId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DoctorPatient", x => new { x.DoctorId, x.PatientId });
                    table.ForeignKey(
                        name: "FK_DoctorPatient_Doctors_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "Doctors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict); // كما اخترت أنت
                    table.ForeignKey(
                        name: "FK_DoctorPatient_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict); // كما اخترت أنت
                });

            // 6. الفهارس (Indexes)
            migrationBuilder.CreateIndex(
                name: "IX_Patients_DepartmentId",
                table: "Patients",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Doctors_DepartmentId",
                table: "Doctors",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_Name",
                table: "Departments",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_DoctorPatient_PatientId",
                table: "DoctorPatient",
                column: "PatientId");

            // 7. القيود (Check Constraints)
            migrationBuilder.AddCheckConstraint(
                name: "CK_Doctor_NationalId_Numeric",
                table: "Doctors",
                sql: "[NationalId] NOT LIKE '%[^0-9]%'");

            

            // 8. العلاقات (Foreign Keys)
            migrationBuilder.AddForeignKey(
                name: "FK_Doctors_Departments_DepartmentId",
                table: "Doctors",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Patients_Departments_DepartmentId",
                table: "Patients",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            // العلاقة التي قمت بتعديلها للـ PatientVisits
            migrationBuilder.AddForeignKey(
                name: "FK_PatientVisits_Patients_PatientId",
                table: "PatientVisits",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Doctors_Departments_DepartmentId",
                table: "Doctors");

            migrationBuilder.DropForeignKey(
                name: "FK_Patients_Departments_DepartmentId",
                table: "Patients");

            migrationBuilder.DropForeignKey(
                name: "FK_PatientVisits_Patients_PatientId",
                table: "PatientVisits");

            migrationBuilder.DropTable(
                name: "Departments");

            migrationBuilder.DropTable(
                name: "DoctorPatient");

            migrationBuilder.DropIndex(
                name: "IX_Patients_DepartmentId",
                table: "Patients");

            migrationBuilder.DropIndex(
                name: "IX_Doctors_DepartmentId",
                table: "Doctors");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Doctor_NationalId_Numeric",
                table: "Doctors");

      
            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "Doctors");

            migrationBuilder.AddColumn<int>(
                name: "DoctorId",
                table: "Patients",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Patients_DoctorId",
                table: "Patients",
                column: "DoctorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Patients_Doctors_DoctorId",
                table: "Patients",
                column: "DoctorId",
                principalTable: "Doctors",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PatientVisits_Patients_PatientId",
                table: "PatientVisits",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}