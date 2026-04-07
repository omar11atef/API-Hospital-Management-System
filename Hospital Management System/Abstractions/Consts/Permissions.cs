namespace Hospital_Management_System.Abstractions.Consts;

public static class Permissions
{
    public static string Type { get; } = "permissions";
    // Persmissions for Roles :
    public const string GetRoles = "Roles:read";
    public const string GetByIdRole = "Roles:read:single";
    public const string AddRole = "Roles:create";
    public const string UpdateRole = "Roles:Update";

    // Permissions for Users :
    public const string GetUsers = "Users:read";
    public const string AddUsers = "Users:create";
    public const string UpdateUsers = "Users:Update";
    public const string DeleteUsers = "Users:delete";

    // Persmissions for APPOINTMENTS :
    public const string GetAllAppointments = "Appointments:read";
    public const string GetDeletedAppointments = "Appointments:read-deleted";
    public const string GetByIdAppointments = "Appointments:read-single";
    public const string GetHistoryAppointments = "Appointments:read-history";
    public const string CreateAppointments = "Appointments:create";
    public const string UpdateAppointments = "Appointments:update";
    public const string DeleteAppointments = "Appointments:remove";
    public const string ToggleAppointments = "Appointments:toggle";
    public const string CancelAppointments = "Appointments:cancel";

    // Persmissions for Author :
    public const string Login = "Author:login";
    public const string Register = "Author:register";
    public const string ConfirmEmail = "Author:confirm-email";
    public const string ResendConfirm = "Author:resend-confirm-email";
    public const string Logout = "Author:logout";

    //Persmissions for Departments :
    public const string GetAllDepartments = "Department:read";
    public const string GetByIdDepartments = "Department:read-single";
    public const string CreateDepartments = "Department:create";
    public const string UpdateDepartments = "Department:update";

    // Permissions for Doctor :
    public const string GetDoctors = "Doctor:read";
    public const string GetExistsDoctors = "Doctor:read-exists";
    public const string GetByIdDoctors = "Doctor:read-single";
    public const string GetScheduleDoctors = "Doctor:read-schedule";
    public const string AddDcotors = "Doctor:read";
    public const string UpdateDcotors = "Doctor:update";
    public const string DeleteDcotors = "Doctor:Remove";
    public const string ToggleDoctor = "Doctor:toggle";

    // Permissions for Patients:
    public const string GetAllPatients = "Patient:read";
    public const string GetAllDeletedPatients = "Patient:read-deleted";
    public const string GetAllNotDeletedPatients = "Patient:read-not-deleted";
    public const string GetByIdPatients = "Patient:read-single";
    public const string GetAppointmentsPatients = "Patient:read-appointments";
    public const string DownloadReportPatientsPatients = "Patient:download-report";
    public const string CreatePatients = "Patient:create";
    public const string UpdatePatients = "Patient:update";
    public const string DeletePatients = "Patient:remove";
    public const string TogglePatients = "Patient:toggle";
    public const string UpdateExpensesPatients = "Patient:update-expenses";

    // Permissions for Rooms :
    public const string GetAllRooms = "Room:read";
    public const string GetByIdRooms = "Room:read-single";
    public const string GetAppointmentsRooms = "Room:read-appointments";
    public const string GetDoctorRoomsRooms = "Room:read-doctor";
    public const string GetPatientRoomsRooms = "Room:read-patient";
    public const string CreateRooms = "Room:create";
    public const string UpdateRooms = "Room:update";
    public const string DeleteRooms = "Room:remove";
    public const string AssignRooms = "Room:assign";

    // Get All Permissions :
    public static IList<string?> GetAllPermissions() =>
        typeof(Permissions).GetFields().Select(x=>x.GetValue(x) as string).ToList();
}
