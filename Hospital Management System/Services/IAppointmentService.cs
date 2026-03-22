namespace Hospital_Management_System.Services;

public interface IAppointmentService
{
    Task<Result<IEnumerable<Appointment>>> GetAllAppointmentsAsync(CancellationToken cancellationToken);
    Task<Result<AppointmentResponse>> GetAppointmentByIdAsync(int appointmentId,CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<AppointmentResponse>>> GetDeletedAppointmentsAsync(CancellationToken cancellationToken = default);

    Task<Result<DoctorAppointmentsResponse>> GetAppointmentsByDoctorAsync(int doctorId, CancellationToken cancellationToken = default);
    Task<Result<PatientAppointmentsResponse>> GetAppointmentsByPatientAsync(int patientId, CancellationToken cancellationToken = default);
    Task<Result<AppointmentResponse>> CreateAppointmentAsync(int doctorId, int patientId, AppointmentRequest request, CancellationToken cancellationToken = default);
    Task<Result<AppointmentResponse>> UpdateAppointmentAsync(int appointmentId,AppointmentRequest request,CancellationToken cancellationToken = default);
    Task<Result> DeleteAppointmentAsync(int appointmentId,CancellationToken cancellationToken = default);
    Task<Result> ToggleAppointmentStatusAsync(int id,CancellationToken cancellationToken = default);
}
