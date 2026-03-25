using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hospital_Management_System.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAppointmentIdToPatientRoom : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PatientRooms_PatientVisits_RelatedVisitId",
                table: "PatientRooms");

            migrationBuilder.DropIndex(
                name: "IX_PatientRooms_RelatedVisitId",
                table: "PatientRooms");

            migrationBuilder.DropColumn(
                name: "RelatedVisitId",
                table: "PatientRooms");

            migrationBuilder.AddColumn<int>(
                name: "AppointmentId",
                table: "PatientRooms",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "PatientRooms",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_PatientRooms_AppointmentId",
                table: "PatientRooms",
                column: "AppointmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_PatientRooms_Appointments_AppointmentId",
                table: "PatientRooms",
                column: "AppointmentId",
                principalTable: "Appointments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PatientRooms_Appointments_AppointmentId",
                table: "PatientRooms");

            migrationBuilder.DropIndex(
                name: "IX_PatientRooms_AppointmentId",
                table: "PatientRooms");

            migrationBuilder.DropColumn(
                name: "AppointmentId",
                table: "PatientRooms");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "PatientRooms");

            migrationBuilder.AddColumn<int>(
                name: "RelatedVisitId",
                table: "PatientRooms",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PatientRooms_RelatedVisitId",
                table: "PatientRooms",
                column: "RelatedVisitId");

            migrationBuilder.AddForeignKey(
                name: "FK_PatientRooms_PatientVisits_RelatedVisitId",
                table: "PatientRooms",
                column: "RelatedVisitId",
                principalTable: "PatientVisits",
                principalColumn: "Id");
        }
    }
}
